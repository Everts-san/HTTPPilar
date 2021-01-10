using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
           /* CookieContainer cookies = new CookieContainer();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://nfse.itajai.sc.gov.br/");
            req.CookieContainer = cookies;

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string html = readStream.ReadToEnd();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode nodoCaptcha = doc.DocumentNode.SelectSingleNode("//input[@name='captcha']");
            string captcha = nodoCaptcha.GetAttributeValue("value", "");

            HtmlNode nodoTentativa = doc.DocumentNode.SelectSingleNode("//input[@name='tentativa']");
            string tentativa = nodoTentativa.GetAttributeValue("value", "1");

            HtmlNode nodoExecutar = doc.DocumentNode.SelectSingleNode("//input[@name='executar']");
            string executar = nodoExecutar.GetAttributeValue("value", "entrarNoSistema");

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            //75.289.595 % 2F0001 - 46
            //346346
            sw.Write("cpf=" + HttpUtility.UrlEncode("75.289.595/0001-46") + "&senha=" + HttpUtility.UrlEncode("346346") + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);

            ////////////////////

            req = (HttpWebRequest)WebRequest.Create("https://nfse.itajai.sc.gov.br/controlador.jsp");
            req.CookieContainer = cookies;

            ms.Seek(0, 0);
            req.Method = "POST";
            ms.CopyTo(req.GetRequestStream());
            response = (HttpWebResponse)req.GetResponse();

            receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            readStream = new StreamReader(receiveStream, Encoding.UTF8);

            html = readStream.ReadToEnd();

            ///////////////////////////////
            ///

            req = (HttpWebRequest)WebRequest.Create("https://nfse.itajai.sc.gov.br/jsp/nfse/emitido/lote/listagem.jsp");
            req.CookieContainer = cookies;
            req.Referer = "https://nfse.itajai.sc.gov.br/controlador.jsp";
            response = (HttpWebResponse)req.GetResponse();

            receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            readStream = new StreamReader(receiveStream, Encoding.UTF8);

            html = readStream.ReadToEnd();

            */








            HttpWebRequestBase httpteste = new HttpWebRequestBase();
            httpteste.Request("https://nfse.itajai.sc.gov.br/");
            string html = httpteste.ResponseDataText;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode nodoCaptcha = doc.DocumentNode.SelectSingleNode("//input[@name='captcha']");
            string captcha = nodoCaptcha.GetAttributeValue("value", "");

            HtmlNode nodoTentativa = doc.DocumentNode.SelectSingleNode("//input[@name='tentativa']");
            string tentativa = nodoTentativa.GetAttributeValue("value", "1");

            HtmlNode nodoExecutar = doc.DocumentNode.SelectSingleNode("//input[@name='executar']");
            string executar = nodoExecutar.GetAttributeValue("value", "entrarNoSistema");


            ///////////////////////////////////////////////////

            
            StreamWriter sw = new StreamWriter(httpteste.RequestDataStream);
            //75.289.595 % 2F0001 - 46
            //346346
            sw.Write("cpf=" + HttpUtility.UrlEncode("75.289.595/0001-46") + "&senha=" + HttpUtility.UrlEncode("346346") + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);
            sw.Flush();
            
            httpteste.Request("https://nfse.itajai.sc.gov.br/controlador.jsp");
            html = httpteste.ResponseDataText;

            ///////////////////////////////////////////////////
            httpteste.Request("https://nfse.itajai.sc.gov.br/jsp/nfse/emitido/lote/listagem.jsp");
            html = httpteste.ResponseDataText;

            ///////////////////////////////////////////////////

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");
            string formAction = nodoFormAction.GetAttributeValue("value", "");
            HtmlNode nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");
            string SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
            HtmlNode nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
            string SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");



            sw = new StreamWriter(httpteste.RequestDataStream);
            //75.289.595 % 2F0001 - 46
            //346346
            //ACTION_FORM_SUBMETIDO=ACTION_FORM_SUBMETIDO&NAME_SQL_ANALITICO_CRIPT=&NAME_SQL_SINTETICO_CRIPT=&dtInicial=01%2F01%2F2021&dtFinal=08%2F01%2F2021&lrp_numero=&lrs_numero=0&NAME_BOTAO_CLICADO=Pesquisar
            string dataIni = "01/12/2020";
            string dataFim = "01/08/2021";
            sw.Write("ACTION_FORM_SUBMETIDO="+ formAction+"&NAME_SQL_ANALITICO_CRIPT=" +SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT="+ SQLSinteticoCript + "&dtInicial="+ HttpUtility.UrlEncode(dataIni) +"&dtFinal="+ HttpUtility.UrlEncode(dataFim)+"&lrp_numero="+"&lrs_numero="+"0"+"&NAME_BOTAO_CLICADO=Pesquisar");
            sw.Flush();


            //  "cpf=" + HttpUtility.UrlEncode("75.289.595/0001-46") + "&senha=" + HttpUtility.UrlEncode("346346") + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);

            httpteste.Request("https://nfse.itajai.sc.gov.br/jsp/nfse/emitido/lote/listagem.jsp");
            html = httpteste.ResponseDataText;

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            //HtmlNode nodoTableDados = doc.DocumentNode.SelectSingleNode("/table[@class='tableDados']/a[@href]");

            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");

            foreach(HtmlNode nodoDown in nodoTabelaDownload)
            {
                string linkDown = nodoDown.GetAttributeValue("href", "");

                if (!linkDown.Contains("DownloadFile"))
                {
                    continue;
                }

                if (linkDown != "")
                {
                    httpteste.Download("https://nfse.itajai.sc.gov.br" + linkDown,"");

                }

            }

        }
    }


}
