using System;
using System.IO;
using System.Net;
using System.Text;

public class HttpWebRequestBase
{
    /// <summary>
    /// Dicionário com os Headers padrões personalizados.
    /// </summary>
    public System.Collections.Generic.Dictionary<HttpRequestHeader, string> Headers { get; }
    
    /// <summary>
    /// Stream com os dados a enviar na requisiçaõ
    /// </summary>
    public MemoryStream RequestDataStream { get; }

    /// <summary>
    /// Stream com os dados recebidos da última requisição
    /// </summary>
    public MemoryStream ResponseDataStream { get; }

    /// <summary>
    /// Último retorno textual da requisição
    /// </summary>
    public string ResponseDataText { get; private set;  }

    /// <summary>
    /// Status code da última requisição
    /// </summary>
    public HttpStatusCode ResponseStatusCode { get; private set; }

    /// <summary>
    /// Cookies das requisições. É persistido entre requisições efetuadas em uma mesma instância de um HttpWebRequestBase.
    /// </summary>
    private CookieContainer Cookies { get; set; }

    /// <summary>
    /// ResponseHeaders da última requisição
    /// </summary>
    private WebHeaderCollection responseHeaders { get; set; }

    /// <summary>
    /// Diretório destino de eventuais downloads efetuados com a instância atual do HttpWebRequest
    /// </summary>
    public string DiretorioDestinoDownload { get; set; }

    public HttpWebRequestBase()
	{
        this.Headers = new System.Collections.Generic.Dictionary<HttpRequestHeader, string>();
        this.ResponseDataStream = new MemoryStream();
        this.RequestDataStream = new MemoryStream();
        this.ResponseDataText = "";
        this.Cookies = new CookieContainer();
        this.responseHeaders = new WebHeaderCollection();
        this.DiretorioDestinoDownload = "";
        this.revertHeadersToDefault();
    }

    public void Request(string URL)
    {
        this.ResponseDataStream.SetLength(0);

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
        //Os cookies devem ser atribuídos a cada nova requisição.
        //Do contrário, cookies devolvidos pelo servidor na última requisição não são guardados, 
        //podendo gerar problemas como não salvar sessões e logins.
        req.CookieContainer = this.Cookies;

        internalBeforeRequest(req);

        HttpWebResponse response = (HttpWebResponse)req.GetResponse();

        internalAfterRequest(response);

        Stream receiveStream = response.GetResponseStream();

        receiveStream.CopyTo(ResponseDataStream);
        ResponseDataStream.Seek(0, 0);

        StreamReader readStream = new StreamReader(ResponseDataStream, Encoding.ASCII);

        this.ResponseDataText = readStream.ReadToEnd();
        readStream = null;

        ResponseDataStream.Seek(0, 0);

        this.ResponseStatusCode = response.StatusCode;

        //TODO: Ver
        /*foreach(Cookie c in response.Cookies)
        {
            Cookies.Add(c);
        }*/

        this.responseHeaders.Clear();
        //Salvar os últimos request headers
        foreach(string h in response.Headers)
        {
            this.responseHeaders.Add(h, response.Headers[h]);
        }

        //Liberar os recursos
        response.Close();

        this.RequestDataStream.SetLength(0);
    }

    protected void internalBeforeRequest(HttpWebRequest requisicao)
    {
        if (this.Headers.Count > 0)
        {
            foreach (HttpRequestHeader key in this.Headers.Keys)
            {

                string valorHeader = "";
                this.Headers.TryGetValue(key, out valorHeader);

                if (key == HttpRequestHeader.UserAgent)
                {
                    requisicao.UserAgent = valorHeader;
                    continue;
                }
                else
                if (key == HttpRequestHeader.Accept)
                {
                    requisicao.Accept = valorHeader;
                    continue;
                }else
                if (key == HttpRequestHeader.Referer)
                {
                    requisicao.Referer = valorHeader;
                    continue;
                }

                requisicao.Headers.Set(key, valorHeader);
            }
        }

        requisicao.KeepAlive = true;

        //Caso o RequestDataStream tenha conteúdo, trata-se de uma requisição POST.
        if (this.RequestDataStream.Length > 0)
        {
            requisicao.Method = "POST";
            requisicao.AllowWriteStreamBuffering = true;
            requisicao.ContentLength = this.RequestDataStream.Length;
            //TODO: ver pois podem haver requisições que mandem binário, aí o ContentType muda.
            requisicao.ContentType = "application/x-www-form-urlencoded";

            this.RequestDataStream.WriteTo(requisicao.GetRequestStream());
            this.RequestDataStream.Flush();
        }
    }

    protected void internalAfterRequest(HttpWebResponse resposta)
    {
    }

    protected void revertHeadersToDefault()
    {
        this.Headers.Clear();
        this.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0(Windows NT 10.0; Win64; x64; rv: 84.0) Gecko / 20100101 Firefox/84.0");
        //Header do navegador, mas comentado aqui pois senão o conteúdo vem incorreto (compactado)
        //this.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
        this.Headers.Add(HttpRequestHeader.AcceptLanguage, "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
        this.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
    }

    public void Download(string URL, string arquivoDestino)
    {
        string internalNomeArquivo = arquivoDestino;
        
        this.Request(URL);

        if (arquivoDestino == "")
        {
            #region Tratamento para casos onde o servidor devolve o nome do arquivo.
            if (this.responseHeaders.Get("Content-Disposition")!= null)
            {
                if(this.responseHeaders.Get("Content-Disposition").Contains("filename=\"")){
                    arquivoDestino = this.responseHeaders.Get("Content-Disposition");
                    arquivoDestino = arquivoDestino.Replace(arquivoDestino.Substring(0, this.responseHeaders.Get("Content-Disposition").IndexOf("filename=\"") + 10),"");

                    Directory.CreateDirectory(this.DiretorioDestinoDownload);
                    
                    arquivoDestino = Path.Combine(this.DiretorioDestinoDownload,arquivoDestino.Substring(0, arquivoDestino.Length - 1));
                }
            }
            #endregion
        }
        else
        {
            Directory.CreateDirectory(this.DiretorioDestinoDownload);
            arquivoDestino = Path.Combine(this.DiretorioDestinoDownload, arquivoDestino);
        }

        //TODO: Verificar casos onde há necessidade de permissão de acesso na pasta.
        FileStream fs = new FileStream(arquivoDestino, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        this.ResponseDataStream.Seek(0, 0);
        this.ResponseDataStream.CopyTo(fs);
        this.ResponseDataStream.Flush();
        fs.Close();
    }

}
