using System;
using System.IO;
using System.Net;
using System.Text;

public class HttpWebRequestBase
{
    public System.Collections.Generic.Dictionary<HttpRequestHeader, string> Headers { get; }
    
    public MemoryStream RequestDataStream { get; }

    public MemoryStream ResponseDataStream { get; }

    public string ResponseDataText { get; private set;  }
    public HttpStatusCode ResponseStatusCode { get; private set; }

    private CookieContainer sessao { get; set; }
    private WebHeaderCollection responseHeaders { get; set; }

	public HttpWebRequestBase()
	{
        this.Headers = new System.Collections.Generic.Dictionary<HttpRequestHeader, string>();
        this.ResponseDataStream = new MemoryStream();
        this.RequestDataStream = new MemoryStream();
        this.ResponseDataText = "";
        this.sessao = new CookieContainer();
        this.responseHeaders = new WebHeaderCollection();
        this.revertHeadersToDefault();
    }

    public void Request(string URL)
    {
        this.ResponseDataStream.SetLength(0);

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
        req.CookieContainer = this.sessao;
        internalBeforeRequest(req);


        HttpWebResponse response = (HttpWebResponse)req.GetResponse();

        internalAfterRequest(response);

        // Get the stream associated with the response.
        Stream receiveStream = response.GetResponseStream();

        receiveStream.CopyTo(ResponseDataStream);
        ResponseDataStream.Seek(0, 0);


        // Pipes the stream to a higher level stream reader with the required encoding format.
        StreamReader readStream = new StreamReader(ResponseDataStream, Encoding.ASCII);

        this.ResponseDataText = readStream.ReadToEnd();
        readStream = null;

        ResponseDataStream.Seek(0, 0);

        this.ResponseStatusCode = response.StatusCode;

        foreach(Cookie c in response.Cookies)
        {
            sessao.Add(c);
        }

        this.responseHeaders.Clear();


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
  //    /*  GET / index.jsp HTTP / 1.1
//Host: nfse.itajai.sc.gov.br
//User - Agent: Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 84.0) Gecko / 20100101 Firefox / 84.0
//Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8
//Accept-Language: 
//Accept-Encoding: gzip, deflate, br
//Connection: keep-alive
//Cookie: JSESSIONID=qQ-p9KuD7EsGXyHvQhnNFkcq.undefined
//Upgrade-Insecure-Requests: 1

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
                }

                requisicao.Headers.Set(key, valorHeader);
            }
        }

        requisicao.KeepAlive = true;

        if (this.RequestDataStream.Length > 0)
        {
            requisicao.Method = "POST";
            requisicao.AllowWriteStreamBuffering = true;
            requisicao.ContentLength = this.RequestDataStream.Length;
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
            if(this.responseHeaders.Get("Content-Disposition")!= null)
            {
                if(this.responseHeaders.Get("Content-Disposition").Contains("filename=\"")){
                    arquivoDestino = this.responseHeaders.Get("Content-Disposition");
                    arquivoDestino = arquivoDestino.Replace(arquivoDestino.Substring(0, this.responseHeaders.Get("Content-Disposition").IndexOf("filename=\"") + 10),"");
                        
                    arquivoDestino = "C:\\sci\\"+arquivoDestino.Substring(0, arquivoDestino.Length - 1);
                }
            }
        }

        FileStream fs = new FileStream(arquivoDestino, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        this.ResponseDataStream.Seek(0, 0);
        this.ResponseDataStream.CopyTo(fs);
        this.ResponseDataStream.Flush();
        fs.Close();
    }

	/*-HTTPWebRequestBase.Headers : Dictionary
         Dicionário com os headers padrões
      -HTTPWebRequestBase.StreamRequestData : Stream
         StreamData a ser enviado
      -HTTPWebRequestBase.StreamResponse : Stream
         Resposta recebida(stream)
      -HTTPWebRequestBase.internalBeforeRequest()
         Faz um processo padrão antes de buscar a requisição, ex: setar viewstate de páginas e setar os headers atuais
      -HTTPWebRequestBase.Request(string URL)
         Faz uma requisicao com a URL passada e os headers e streamData enviados
      -HTTPWebRequestBase.internalAfterRequest()
         Faz um processo padrão depois buscar a requisição, ex: buscar viewstate de páginas e converter o conteúdo do stream da página de ele não for binário ou zip
      -HTTPWebRequestBase.RevertHeadersToDefault();
         Reverte os headers para os padrões da página
      -HTTPWebRequestBase.Download(string URL, string arquivoDestino)
         Faz o download de uma URL para o arquivo destino.Cria o arquivo se não existe, sobrescreve se existe*/
}
