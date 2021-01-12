using HtmlAgilityPack;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows.Forms;

public class HttpWebRequest_NFSe_Itajai : HttpWebRequestBase
{
	public string CPF_CNPJ { get; set; }
	public string Senha { get; set; }

	public DateTimePicker dataInicial { get; set; }
	public DateTimePicker dataFinal { get; set; }

	

    private const string C_NFSE_ITAJAI = "https://nfse.itajai.sc.gov.br/";

    public HttpWebRequest_NFSe_Itajai() : base()
	{        
		this.CPF_CNPJ = "";
		this.Senha = "";
		this.dataInicial = null;
		this.dataFinal = null;	
	}

	public void baixarRPS()
	{
        if (this.CPF_CNPJ == "")
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
        this.Request(C_NFSE_ITAJAI);
        string html = this.ResponseDataText;

        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNode nodoCaptcha = doc.DocumentNode.SelectSingleNode("//input[@name='captcha']");
        string captcha = nodoCaptcha.GetAttributeValue("value", "");

        HtmlNode nodoTentativa = doc.DocumentNode.SelectSingleNode("//input[@name='tentativa']");
        string tentativa = nodoTentativa.GetAttributeValue("value", "1");

        HtmlNode nodoExecutar = doc.DocumentNode.SelectSingleNode("//input[@name='executar']");
        string executar = nodoExecutar.GetAttributeValue("value", "entrarNoSistema");
        #endregion

        #region Login
        StreamWriter sw = new StreamWriter(this.RequestDataStream);
        sw.Write("cpf=" + HttpUtility.UrlEncode(this.CPF_CNPJ) + "&senha=" + HttpUtility.UrlEncode(this.Senha) + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);
        sw.Flush();

        this.Request(C_NFSE_ITAJAI+"controlador.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Acesso a página de listagem das notas
        this.Request(C_NFSE_ITAJAI+"jsp/nfse/emitido/lote/listagem.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Filtro das notas
        doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNode nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");
        string formAction = nodoFormAction.GetAttributeValue("value", "");
        HtmlNode nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");
        string SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
        HtmlNode nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
        string SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");

        sw = new StreamWriter(this.RequestDataStream);
        string dataIni = this.dataInicial.Value.ToString("dd/MM/yyyy");
        string dataFim = this.dataFinal.Value.ToString("dd/MM/yyyy");
        sw.Write("ACTION_FORM_SUBMETIDO=" + formAction + "&NAME_SQL_ANALITICO_CRIPT=" + SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT=" + SQLSinteticoCript + "&dtInicial=" + HttpUtility.UrlEncode(dataIni) + "&dtFinal=" + HttpUtility.UrlEncode(dataFim) + "&lrp_numero=" + "&lrs_numero=" + "0" + "&NAME_BOTAO_CLICADO=Pesquisar");
        sw.Flush();


        this.Request(C_NFSE_ITAJAI+"jsp/nfse/emitido/lote/listagem.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Download das notas se encontradas
        doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");

        foreach (HtmlNode nodoDown in nodoTabelaDownload)
        {
            string linkDown = nodoDown.GetAttributeValue("href", "");

            if (!linkDown.Contains("DownloadFile"))
            {
                continue;
            }

            if (linkDown != "")
            {
                this.Download(C_NFSE_ITAJAI.Substring(0, C_NFSE_ITAJAI.Length-1) + linkDown, "");
                Random r = new Random();
                Thread.Sleep(r.Next(1000, 2000));
            }

        }
        #endregion
    }

    public void baixarNotasPDF()
    {
        if (this.CPF_CNPJ == "")
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
        this.Request(C_NFSE_ITAJAI);
        string html = this.ResponseDataText;

        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNode nodoCaptcha = doc.DocumentNode.SelectSingleNode("//input[@name='captcha']");
        string captcha = nodoCaptcha.GetAttributeValue("value", "");

        HtmlNode nodoTentativa = doc.DocumentNode.SelectSingleNode("//input[@name='tentativa']");
        string tentativa = nodoTentativa.GetAttributeValue("value", "1");

        HtmlNode nodoExecutar = doc.DocumentNode.SelectSingleNode("//input[@name='executar']");
        string executar = nodoExecutar.GetAttributeValue("value", "entrarNoSistema");
        #endregion

        #region Login
        StreamWriter sw = new StreamWriter(this.RequestDataStream);
        sw.Write("cpf=" + HttpUtility.UrlEncode(this.CPF_CNPJ) + "&senha=" + HttpUtility.UrlEncode(this.Senha) + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);
        sw.Flush();

        this.Request(C_NFSE_ITAJAI + "controlador.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Acesso a página de listagem das NFSe
        this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/notas/emissao.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Filtro das NFSe
        doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNode nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");
        string formAction = nodoFormAction.GetAttributeValue("value", "");
        HtmlNode nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");
        string SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
        HtmlNode nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
        string SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");

        sw = new StreamWriter(this.RequestDataStream);
        string dataIni = this.dataInicial.Value.ToString("dd/MM/yyyy");
        string dataFim = this.dataFinal.Value.ToString("dd/MM/yyyy");
        string anoIni = this.dataInicial.Value.ToString("yyyy");
        string mesIni = int.Parse(this.dataInicial.Value.ToString("MM")).ToString();
        string anoFim = this.dataInicial.Value.ToString("yyyy");
        string mesFim = int.Parse(this.dataInicial.Value.ToString("MM")).ToString();
                
        sw.Write("ACTION_FORM_SUBMETIDO=" + formAction + "&NAME_SQL_ANALITICO_CRIPT=" + SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT=" + SQLSinteticoCript + "&realizar_consulta=nfse_por_periodo&dtInicial=" + HttpUtility.UrlEncode(dataIni) + "&dtFinal=" + HttpUtility.UrlEncode(dataFim) + "&nfp_numero=&mes_inicio="+ mesIni + "&ano_inicio="+anoIni+"&mes_final="+mesFim+"&ano_final="+anoFim+"&exibir=exibir_tela&tipo_rps=0&atv_codigo=0&status=todos&exibir_cce=&cst_codigo=0&local_prestacao= todos&manutencao=&cpff=&razaosocial=&NAME_TIPO_RELATORIO=0&NAME_BOTAO_CLICADO=Pesquisar&parans_assinar_documento=");
        sw.Flush();


        this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/notas/emissao.jsp");
        html = this.ResponseDataText;
        #endregion

        #region Download do PDF das notas se encontradas
        doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");

        int contadorNotas = 0;
        foreach (HtmlNode nodoDown in nodoTabelaDownload)
        {
            string linkDown = nodoDown.GetAttributeValue("href", "");

            if (!linkDown.Contains("NFES"))
            {
                continue;
            }

            if (linkDown != "")
            {
                this.Download(C_NFSE_ITAJAI.Substring(0, C_NFSE_ITAJAI.Length - 1) + linkDown, DateTime.Now.ToString("hhmmss_ddmmyyyy")+contadorNotas.ToString()+".pdf");
                contadorNotas++;

                Random r = new Random();
                Thread.Sleep(r.Next(1000, 2000));
            }

        }
        #endregion
    }
}
