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
            string dataIni = "01/01/2021";
            string dataFim = "01/08/2021";
            sw.Write("ACTION_FORM_SUBMETIDO="+ formAction+"&NAME_SQL_ANALITICO_CRIPT=" +SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT="+ SQLSinteticoCript + "&dtInicial="+ HttpUtility.UrlEncode(dataIni) +"&dtFinal="+ HttpUtility.UrlEncode(dataFim)+"&lrp_numero="+"&lrs_numero="+"0"+"&NAME_BOTAO_CLICADO=Pesquisar");



              //  "cpf=" + HttpUtility.UrlEncode("75.289.595/0001-46") + "&senha=" + HttpUtility.UrlEncode("346346") + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);

            httpteste.Request("https://nfse.itajai.sc.gov.br/jsp/nfse/emitido/lote/listagem.jsp");
            html = httpteste.ResponseDataText;

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectSingleNode("//form[@id='form_principal']").SelectSingleNode("//table[@class='tableDados']").SelectNodes("//a");

        }
    }
}
