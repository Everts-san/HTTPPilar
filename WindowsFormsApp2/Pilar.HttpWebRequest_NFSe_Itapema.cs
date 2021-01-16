using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using TuesPechkin;
using WindowsFormsApp2;

public class HttpWebRequest_NFSe_Itapema : HttpWebRequestBase
{
	public string Login { get; set; }
	public string Senha { get; set; }

	public DateTimePicker dataInicial { get; set; }
	public DateTimePicker dataFinal { get; set; }

    /// <summary>
    /// Cache das imagens utilizado na hora do download das notas.
    /// Utilizado para fazer menos requisições no site, evitando que ele nos corte ou fique lento.
    /// O cache é composto de um par "Link"/"base64 da imagem".
    /// </summary>
    private Dictionary<string, string> cacheImagens { get; set; }
	
    public HttpWebRequest_NFSe_Itapema() : base()
	{        
		this.Login = "";
		this.Senha = "";
		this.dataInicial = null;
		this.dataFinal = null;
        this.cacheImagens = new Dictionary<string, string>();
	}
    
    /// <summary>
    /// Rotina para retornar uma representação string de um Hash. Pega do site da Microsoft (MSDN).
    /// </summary>
    /// <param name="hashAlgorithm">Algoritmo do hash a ser gerado</param>
    /// <param name="input">String a ser gerada um hash.</param>
    /// <returns></returns>
    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    /// <summary>
    /// Rotina para baixar as notas em PDF do site de Itapema.
    /// </summary>
    /// <exception  cref="EHttpWebRequestPilarException">Caso os parâmetros "Login", "Senha", "dataInicial", "dataFinal" ou "DiretorioDestinoDownload" não estejam informados.</exception>
    public void baixarNotasPDF()
    {
        if (this.Login == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itapema.BaixarRPS: é necessário informar o CNPJ!", "");
        }
        if (this.Senha == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itapema.BaixarRPS: é necessário informar a senha!", "");
        }
        if (this.dataInicial == null)
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itapema.BaixarRPS: é necessário informar a data inicial da busca!", "");
        }
        if (this.dataFinal == null)
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itapema.BaixarRPS: é necessário informar a data final da busca!", "");
        }
        if (this.DiretorioDestinoDownload == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itapema.BaixarRPS: é necessário informar a pasta destino do download das notas!", "");
        }

        #region Acesso a página inicial da NFSe de Itajaí        
        this.Request("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/index.php?cidade=itapema");
        string html = this.ResponseDataText;

        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        #endregion

        Random l = new Random();

        #region Login
        StreamWriter sw = new StreamWriter(this.RequestDataStream);

        MD5 hashSenha = MD5.Create("MD5");

        //Faz o hash MD5 da senha e preenchimento dos parâmetros l.x e l.y com parâmetros aleatórios.
        //Esses dois parâmetros são o posicionamento do mouse no botão de login.
        sw.Write("login_itbi="+this.Login + "&senha_itbi_digite="+ this.Senha + "&senha_itbi="+HttpWebRequest_NFSe_Itapema.GetHash(hashSenha,this.Senha)+"&l.x="+l.Next(5,40).ToString()+"&l.y="+ l.Next(5, 20));
        sw.Flush();

        this.Request("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/index.php?cidade=itapema");
        html = this.ResponseDataText;
        #endregion

        //TODO: Verificar login
        //      No momento não é um problema pois se estiver incorreto ele não vai baixar nada, mas tem que informar o usuário.

        #region Acesso a página de listagem das NFSe
        this.Request("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/painel.php?pg=relatorio");
        html = this.ResponseDataText;
        #endregion

        #region Filtro das NFSe
        doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        string refererPainel = "https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/painel.php?tp_doc=1&nr_nferps_ini=&nr_nferps_fim=&st_rps=1&dt_inicial="+ HttpUtility.UrlEncode(this.dataInicial.Value.ToString("dd/MM/yyyy")) +"&dt_final="+ HttpUtility.UrlEncode(this.dataFinal.Value.ToString("dd/MM/yyyy")) + "&vl_inicial=&vl_final=&cd_atividade=&nr_doc=&ordem=ASC&l.x=58&l.y=16&consulta=1&pg=relatorio";
        this.Request(refererPainel);
        
        #endregion

        #region Download do PDF das notas se encontradas
        int contadorNotas = 0;

        while (true)
        {
            html = this.ResponseDataText;
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            //Pega todos os links da página e faz a primeira iteração procurando os links de emissão das notas.
            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode nodoDown in nodoTabelaDownload)
            {
                string linkDown = nodoDown.GetAttributeValue("href", "");
                string onClick = nodoDown.GetAttributeValue("onclick", "");

                //Os links de emissão da nota são javascript que apontam para a página "print_nota.php"
                if ((!linkDown.Contains("javascript")) && (!onClick.Contains("print_nota.php")))
                {
                    continue;
                }

                if (linkDown != "")
                {
                    string linkDownload = onClick.Substring(13, onClick.Length - 13);
                    linkDownload = linkDownload.Substring(0, linkDownload.IndexOf("'"));
                    linkDownload = "https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload;
                    if (this.Headers.ContainsKey(System.Net.HttpRequestHeader.Referer))
                    {
                        this.Headers.Remove(System.Net.HttpRequestHeader.Referer);
                    }
                    this.Headers.Add(System.Net.HttpRequestHeader.Referer, refererPainel);
                    
                    this.imprimePaginaParaPDF(linkDownload, contadorNotas);
                    contadorNotas++;

                    //Sleep para o site não cortar a gente
                    Random r = new Random();
                    Thread.Sleep(r.Next(1000, 2000));
                }
            }

            //Após ter emitido todas as notas, deve ver se há um link "Próximo".
            //Se houver chama ele para prosseguir o download das notas e seta a variável para continuar, senão
            //aborta o processo.
            bool continuar = false;
            foreach (HtmlNode nodoProximo in nodoTabelaDownload)
            {
                string linkProximo = HttpUtility.HtmlDecode(nodoProximo.GetAttributeValue("href", ""));
              
                if ((!linkProximo.Contains("pageNum_documento"))||(!HttpUtility.HtmlDecode(nodoProximo.InnerText).Contains("Próximo")))
                {
                    continue;
                }

                if (linkProximo != "")
                {
                    continuar = true;

                    refererPainel = "https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/painel.php" + linkProximo;
                    this.Request(refererPainel);

                    //Sleep para o site não cortar a gente
                    Random r = new Random();
                    Thread.Sleep(r.Next(1000, 2000));

                    break;
                }
            }
            if (!continuar)
            {
                break;
            }
        }
        #endregion
    }


    /// <summary>
    /// Faz a impressão de uma página HTML de nota de Itapema para PDF e salva ela em um arquivo no diretório "DiretorioDestinoDownload".
    /// Essa rotina faz também as buscas das imagens do HTML para mostrar a página no PDF como ela é exibida para o usuário.
    /// </summary>
    /// <param name="URL">Link da página da nota.</param>
    /// <param name="contador">Contador de qual nota está atualmente. Será inserido no nome do arquivo.</param>
    public void imprimePaginaParaPDF(string URL, int contador)
    {
        this.Request(URL);

        #region Busca das imagens para inserção no HTML retornado.
        string html = this.ResponseDataText;
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNodeCollection imagens = doc.DocumentNode.SelectNodes("//img[@src]");
        foreach(HtmlNode imagem in imagens)
        {
            string linkDownload = imagem.GetAttributeValue("src", "");
            if (linkDownload != "")
            {
                string imagemb64 = "";
                //Se a imagem já foi baixada vai estar em cache. Senão faz o download dela e transforma ela em base64.
                if (this.cacheImagens.ContainsKey("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload))
                {
                    this.cacheImagens.TryGetValue("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload,out imagemb64);                    
                }
                else
                {
                    this.Request("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload);
                    this.ResponseDataStream.Seek(0, 0);

                    imagemb64 = Convert.ToBase64String(this.ResponseDataStream.ToArray());

                    this.cacheImagens.Add("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload, imagemb64);

                }

                //Insere o base64 da imagem no HTML.
                imagem.SetAttributeValue("src", "data:image/png;base64," + imagemb64);
            }
        }
        #endregion

        //Atualiza o HTML na variável html.
        StringWriter tx = new StringWriter();
        doc.Save(tx);
        html = tx.ToString();

        #region Impressao do HTML para PDF usando o componente TuesPeckin (https://github.com/tuespetre/TuesPechkin)
        var document = new HtmlToPdfDocument
                            {
                                GlobalSettings =
                        {
                            ProduceOutline = true,
                            DocumentTitle = "Nota Itapema",
                            PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
                            Margins =
                            {
                                All = 1.375,
                                Unit = Unit.Centimeters
                            }
                        },
                                Objects = {
                            new ObjectSettings { HtmlText = html }
                        }
                            };

        IConverter converter = new StandardConverter(new PdfToolset(new Win32EmbeddedDeployment(new TempFolderDeployment())));

        byte[] result = converter.Convert(document);
        #endregion

        //Salvando o arquivo obtido.
        Directory.CreateDirectory(this.DiretorioDestinoDownload);
        FileStream fs = new FileStream(Path.Combine(this.DiretorioDestinoDownload,DateTime.Now.ToString("hhmmss_ddmmyyyy") +"_"+ contador.ToString()+".pdf"), FileMode.Create, FileAccess.ReadWrite);
        fs.Write(result,0,result.Length);
        fs.Close();
    }
}
