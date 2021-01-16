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

public class HttpWebRequest_NFSe_Itapema : HttpWebRequestBase
{
	public string Login { get; set; }
	public string Senha { get; set; }

	public DateTimePicker dataInicial { get; set; }
	public DateTimePicker dataFinal { get; set; }

    private Dictionary<string, string> cacheImagens { get; set; }
	

    private const string C_NFSE_ITAJAI = "https://nfse.itajai.sc.gov.br/";

    public HttpWebRequest_NFSe_Itapema() : base()
	{        
		this.Login = "";
		this.Senha = "";
		this.dataInicial = null;
		this.dataFinal = null;
        this.cacheImagens = new Dictionary<string, string>();
	}

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

    public void baixarNotasPDF()
    {
        if (this.Login == "")
        {
            throw new Exception("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar o CNPJ!");
        }
        if (this.Senha == "")
        {
            throw new Exception("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a senha!");
        }
        if (this.dataInicial == null)
        {
            throw new Exception("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a data inicial da busca!");
        }
        if (this.dataFinal == null)
        {
            throw new Exception("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a data final da busca!");
        }
        if (this.DiretorioDestinoDownload == "")
        {
            throw new Exception("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a pasta destino do download das notas!");
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

        //sw.Write("login_itbi=159397&senha_itbi_digite=Escrita346&senha_itbi=5bf6e52e438dc1b0e9d2f74c91bba20d&l.x=" + l.Next(5, 40).ToString() + "&l.y=" + l.Next(5, 20));
        sw.Write("login_itbi="+this.Login + "&senha_itbi_digite="+ this.Senha + "&senha_itbi="+HttpWebRequest_NFSe_Itapema.GetHash(hashSenha,this.Senha)+"&l.x="+l.Next(5,40).ToString()+"&l.y="+ l.Next(5, 20));
        sw.Flush();

        this.Request("https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/index.php?cidade=itapema");
        html = this.ResponseDataText;
        #endregion

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

        bool primeira = false;
        while (true)
        {
            html = this.ResponseDataText;
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode nodoDown in nodoTabelaDownload)
            {
                /*if (!primeira)
                {
                    primeira = true;
                    break;
                }*/
                string linkDown = nodoDown.GetAttributeValue("href", "");
                string onClick = nodoDown.GetAttributeValue("onclick", "");

                if ((!linkDown.Contains("javascript")) && (!onClick.Contains("print_nota.php")))
                {
                    continue;
                }

                if (linkDown != "")
                {
                    string linkDownload = onClick.Substring(13, onClick.Length - 13);
                    linkDownload = linkDownload.Substring(0, linkDownload.IndexOf("'"));
                    linkDownload = "https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/" + linkDownload;
                    //https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/print_nota.php?nrnota=0002522&idnota=2191574 
                    if (this.Headers.ContainsKey(System.Net.HttpRequestHeader.Referer))
                    {
                        this.Headers.Remove(System.Net.HttpRequestHeader.Referer);
                    }
                    this.Headers.Add(System.Net.HttpRequestHeader.Referer, refererPainel);
                    
                    this.imprimePaginaParaPDF(linkDownload, contadorNotas);
                    contadorNotas++;

                    Random r = new Random();
                    Thread.Sleep(r.Next(1000, 2000));
                }
            }

            bool continuar = false;
            foreach (HtmlNode nodoProximo in nodoTabelaDownload)
            {
                string linkProximo = HttpUtility.HtmlDecode(nodoProximo.GetAttributeValue("href", ""));
                ;

                if ((!linkProximo.Contains("pageNum_documento"))||(!HttpUtility.HtmlDecode(nodoProximo.InnerText).Contains("Próximo")))
                {
                    continue;
                }

                if (linkProximo != "")
                {
                    continuar = true;

                    refererPainel = "https://itapema-sc.prefeituramoderna.com.br/meuiss_new/nfe/painel.php" + linkProximo;
                    this.Request(refererPainel);

                    

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

    public void imprimePaginaParaPDF(string URL, int contador)
    {
        this.Request(URL);

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
                imagem.SetAttributeValue("src", "data:image/png;base64," + imagemb64);


            }
        }

        StringWriter tx = new StringWriter();
        doc.Save(tx);
        html = tx.ToString();

        var document = new HtmlToPdfDocument
                            {
                                GlobalSettings =
                        {
                            ProduceOutline = true,
                            DocumentTitle = "Pretty Websites",
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
        
        Directory.CreateDirectory(this.DiretorioDestinoDownload);

        FileStream fs = new FileStream(Path.Combine(this.DiretorioDestinoDownload,DateTime.Now.ToString("hhmmss_ddmmyyyy") +"_"+ contador.ToString()+".pdf"), FileMode.Create, FileAccess.ReadWrite);
        fs.Write(result,0,result.Length);
        fs.Close();

        //PdfDocument pdf = PdfGenerator.GeneratePdf(html,PageSize.A4,20,d,null,null);
        //pdf.Save("c:\\pilar\\document.pdf");

    }
}
