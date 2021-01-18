using HtmlAgilityPack;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using WindowsFormsApp2;

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

    /// <summary>
    /// Rotina interna para login na página do site de NFSe de Itajaí
    /// </summary>
    /// <exception  cref="EHttpWebRequestPilarException">Caso os dados de login estejam incorretos. 
    /// Nesse caso, retornará o HTML da página para análise na exceção.
    /// Também irá retornar essa exceção caso os parâmetros "CPF_CNPJ", "Senha", "dataInicial", "dataFinal" ou "DiretorioDestinoDownload" não estejam informados.</exception>
    private void loginSiteNFSeItajai()
    {
        #region Validação dos parâmetros necessários
        if (this.CPF_CNPJ == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar o CNPJ!","");
        }
        if (this.Senha == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a senha!","");
        }
        if (this.dataInicial == null)
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a data inicial da busca!", "");
        }
        if (this.dataFinal == null)
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a data final da busca!", "");
        }
        if (this.DiretorioDestinoDownload == "")
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.BaixarRPS: é necessário informar a pasta destino do download das notas!", "");
        }
        #endregion

        #region Acesso a página inicial da NFSe de Itajaí        
        this.Request(C_NFSE_ITAJAI);
        string html = this.ResponseDataText;

        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        HtmlNode nodoCaptcha = doc.DocumentNode.SelectSingleNode("//input[@name='captcha']");
        HtmlNode nodoTentativa = doc.DocumentNode.SelectSingleNode("//input[@name='tentativa']");
        HtmlNode nodoExecutar = doc.DocumentNode.SelectSingleNode("//input[@name='executar']");

        if ((nodoCaptcha == null) || (nodoTentativa == null) || (nodoExecutar == null))
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.baixarRPS: página de login retornada pelo site inválida!", html);
        }

        string captcha = nodoCaptcha.GetAttributeValue("value", "");
        string tentativa = nodoTentativa.GetAttributeValue("value", "1");
        string executar = nodoExecutar.GetAttributeValue("value", "entrarNoSistema");
        #endregion

        #region Login
        StreamWriter sw = new StreamWriter(this.RequestDataStream);
        sw.Write("cpf=" + HttpUtility.UrlEncode(this.CPF_CNPJ) + "&senha=" + HttpUtility.UrlEncode(this.Senha) + "&exec=Entrar&executar=" + executar + "&captcha=" + HttpUtility.UrlEncode(captcha) + "&tentativa=" + tentativa);
        sw.Flush();

        this.Request(C_NFSE_ITAJAI + "controlador.jsp");
        html = this.ResponseDataText;
        #endregion
    }

    /// <summary>
    /// Rotina para baixar os RPS do site de Itajaí. Salvará eles no diretório setado em "DiretorioDestinoDownload".
    /// </summary>
    /// <exception  cref="EHttpWebRequestPilarException">Caso os dados de login estejam incorretos. 
    /// Nesse caso, retornará o HTML da página para análise na exceção.
    /// Verificar também <see cref="loginSiteNFSeItajai">"loginSiteNFSeItajai"</see></exception>
    public void BaixarRPS()
	{
        this.loginSiteNFSeItajai();

        #region Acesso a página de listagem das notas
        this.Request(C_NFSE_ITAJAI+"jsp/nfse/emitido/lote/listagem.jsp");
        string html = this.ResponseDataText;
        #endregion

        #region Filtro das notas
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        //Variáveis que vem na página e precisamos informar nas requisições
        HtmlNode nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");        
        HtmlNode nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");        
        HtmlNode nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");

        //TODO: Aprimorar tratamento de login. Pode ser verificado após a requisição ao controlador.jsp
        if ((nodoFormAction == null) || (nodoSQLAnaliticoCript == null) || (nodoSQLSinteticoCript == null))
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.baixarRPS: não foi possível efetuar o login no site, verifique os dados de login informados!", html);
        }

        string formAction = nodoFormAction.GetAttributeValue("value", "");
        string SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", ""); 
        string SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");

        StreamWriter sw = new StreamWriter(this.RequestDataStream);
        string dataIni = this.dataInicial.Value.ToString("dd/MM/yyyy");
        string dataFim = this.dataFinal.Value.ToString("dd/MM/yyyy");
        sw.Write("ACTION_FORM_SUBMETIDO=" + formAction + "&NAME_SQL_ANALITICO_CRIPT=" + SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT=" + SQLSinteticoCript + "&dtInicial=" + HttpUtility.UrlEncode(dataIni) + "&dtFinal=" + HttpUtility.UrlEncode(dataFim) + "&lrp_numero=" + "&lrs_numero=" + "0" + "&NAME_BOTAO_CLICADO=Pesquisar");
        sw.Flush();

        this.Request(C_NFSE_ITAJAI+"jsp/nfse/emitido/lote/listagem.jsp");

        #endregion

        #region Download das notas se encontradas, se não encontrada nenhuma vai passar reto aqui.

        while (true)
        {
            html = this.ResponseDataText;
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");

            foreach (HtmlNode nodoDown in nodoTabelaDownload)
            {
                string linkDown = nodoDown.GetAttributeValue("href", "");

                //Os links para downloads dos XMLs vão conter "DownloadFile" no href;
                if (!linkDown.Contains("DownloadFile"))
                {
                    continue;
                }

                if (linkDown != "")
                {
                    this.Download(C_NFSE_ITAJAI.Substring(0, C_NFSE_ITAJAI.Length - 1) + linkDown, "");

                    //Sleep para a página não "cortar" a gente
                    Random r = new Random();
                    Thread.Sleep(r.Next(1000, 2000));
                }
            }

            //Se tiver mais páginas, avançar para a próxima para prosseguir o download
            bool continuar = false;
            HtmlNodeCollection nodoBotoesNavegacao = doc.DocumentNode.SelectNodes("//input[@name='NAME_BOTAO_CLICADO']");
            foreach (HtmlNode botao in nodoBotoesNavegacao)
            {
                string nomeBotao = botao.GetAttributeValue("value", "");
                bool disabled = botao.Attributes.Contains("disabled");
                if ((!nomeBotao.Equals("Próxima")) || (disabled))
                {
                    continue;
                }

                continuar = true;

                //Variáveis que vem na página e precisamos informar nas requisições
                nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");
                nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");
                nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
                HtmlNode nodo_NAME_PAGINA_ATUAL = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_PAGINA_ATUAL']");

                formAction = nodoFormAction.GetAttributeValue("value", "");
                SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
                SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");
                string NAME_PAGINA_ATUAL = nodo_NAME_PAGINA_ATUAL.GetAttributeValue("value", "");

                //NAME_QTD_POR_PAGINA = 25          
                string request = "ACTION_FORM_SUBMETIDO=" + formAction + "&NAME_SQL_ANALITICO_CRIPT=" + SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT=" + SQLSinteticoCript + "&dtInicial=" + HttpUtility.UrlEncode(dataIni) + "&dtFinal=" + HttpUtility.UrlEncode(dataFim) + "&lrp_numero=&lrs_numero=0&NAME_PAGINA_ATUAL="+ NAME_PAGINA_ATUAL + "&NAME_BOTAO_CLICADO=Pr%F3xima&NAME_QTD_POR_PAGINA=25"; 

                sw = new StreamWriter(this.RequestDataStream);
                sw.Write(request);
                sw.Flush();

                this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/lote/listagem.jsp");
            }

            if (!continuar)
            {
                break;
            }
        }
        #endregion
    }

    /// <summary>
    /// Rotina para baixar os PDFs de NFSe do site de Itajaí. Salvará eles no diretório setado em "DiretorioDestinoDownload".
    /// </summary>
    /// <exception  cref="EHttpWebRequestPilarException">Caso os dados de login estejam incorretos. 
    /// Nesse caso, retornará o HTML da página para análise na exceção.
    /// Verificar também <see cref="loginSiteNFSeItajai">"loginSiteNFSeItajai"</see></exception>
    public void baixarNotasPDF()
    {
        this.loginSiteNFSeItajai();

        #region Acesso a página de listagem das NFSe
        this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/notas/emissao.jsp");
        string html = this.ResponseDataText;
        #endregion

        #region Filtro das NFSe
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        //Variáveis que vem na página e precisamos informar nas requisições
        HtmlNode nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");        
        HtmlNode nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");        
        HtmlNode nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
        
        //TODO: Aprimorar tratamento de login. Pode ser verificado após a requisição ao controlador.jsp
        if ((nodoFormAction == null) || (nodoSQLAnaliticoCript == null) || (nodoSQLSinteticoCript == null))
        {
            throw new EHttpWebRequestPilarException("HttpWebRequest_NFSe_Itajai.baixarRPS: não foi possível efetuar o login no site, verifique os dados de login informados!", html);
        }

        string formAction = nodoFormAction.GetAttributeValue("value", "");
        string SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
        string SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");

        
        string dataIni = this.dataInicial.Value.ToString("dd/MM/yyyy");
        string dataFim = this.dataFinal.Value.ToString("dd/MM/yyyy");
        string anoIni = this.dataInicial.Value.ToString("yyyy");
        string mesIni = int.Parse(this.dataInicial.Value.ToString("MM")).ToString();
        string anoFim = this.dataInicial.Value.ToString("yyyy");
        string mesFim = int.Parse(this.dataInicial.Value.ToString("MM")).ToString();

        StreamWriter sw = new StreamWriter(this.RequestDataStream);
        sw.Write("ACTION_FORM_SUBMETIDO=" + formAction + "&NAME_SQL_ANALITICO_CRIPT=" + SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT=" + SQLSinteticoCript + "&realizar_consulta=nfse_por_periodo&dtInicial=" + HttpUtility.UrlEncode(dataIni) + "&dtFinal=" + HttpUtility.UrlEncode(dataFim) + "&nfp_numero=&mes_inicio="+ mesIni + "&ano_inicio="+anoIni+"&mes_final="+mesFim+"&ano_final="+anoFim+"&exibir=exibir_tela&tipo_rps=0&atv_codigo=0&status=todos&exibir_cce=&cst_codigo=0&local_prestacao= todos&manutencao=&cpff=&razaosocial=&NAME_TIPO_RELATORIO=0&NAME_BOTAO_CLICADO=Pesquisar&parans_assinar_documento=");
        sw.Flush();

        this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/notas/emissao.jsp");

        #endregion

        #region Download do PDF das notas se encontradas

        while (true)
        {
            html = this.ResponseDataText;
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection nodoTabelaDownload = doc.DocumentNode.SelectNodes("//a[@href]");

            //Itera os documentos para download. Se não encontrar nenhum, passa reto por aqui.
            int contadorNotas = 0;
            foreach (HtmlNode nodoDown in nodoTabelaDownload)
            {
                string linkDown = nodoDown.GetAttributeValue("href", "");
                string atributoVisualizar = nodoDown.GetAttributeValue("title", "");

                //Os links para downloads dos PDFs vão conter "NFES" no href;
                if (!linkDown.Contains("NFES"))
                {
                    continue;
                }
                if(atributoVisualizar != "Visualizar nota")
                {
                    continue;
                }

                if (linkDown != "")
                {
                    this.Download(C_NFSE_ITAJAI.Substring(0, C_NFSE_ITAJAI.Length - 1) + linkDown, DateTime.Now.ToString("hhmmss_ddmmyyyy") + contadorNotas.ToString() + ".pdf");
                    contadorNotas++;

                    //Sleep para a página não "cortar" a gente
                    Random r = new Random();
                    Thread.Sleep(r.Next(1000, 2000));
                }
            }

            //Se tiver mais páginas, avançar para a próxima para prosseguir o download
            bool continuar = false;
            HtmlNodeCollection nodoBotoesNavegacao = doc.DocumentNode.SelectNodes("//input[@name='NAME_BOTAO_CLICADO']");
            foreach(HtmlNode botao in nodoBotoesNavegacao)
            {
                string nomeBotao = botao.GetAttributeValue("value", "");
                bool disabled = botao.Attributes.Contains("disabled");
                if ((!nomeBotao.Equals("Próxima")) || (disabled))
                {
                    continue;
                }

                continuar = true;

                //Variáveis que vem na página e precisamos informar nas requisições
                nodoFormAction = doc.DocumentNode.SelectSingleNode("//input[@name='ACTION_FORM_SUBMETIDO']");
                nodoSQLAnaliticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_ANALITICO_CRIPT']");
                nodoSQLSinteticoCript = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_SQL_SINTETICO_CRIPT']");
                HtmlNode nodo_nfp_ids = doc.DocumentNode.SelectSingleNode("//input[@id='nfp_ids']");
                HtmlNode nodo_NAME_PAGINA_ATUAL = doc.DocumentNode.SelectSingleNode("//input[@name='NAME_PAGINA_ATUAL']");

                formAction = nodoFormAction.GetAttributeValue("value", "");
                SQLAnaliticoCript = nodoSQLAnaliticoCript.GetAttributeValue("value", "");
                SQLSinteticoCript = nodoSQLSinteticoCript.GetAttributeValue("value", "");
                string nfp_ids = nodo_nfp_ids.GetAttributeValue("value", "");
                string NAME_PAGINA_ATUAL = nodo_NAME_PAGINA_ATUAL.GetAttributeValue("value", "");

                //NAME_QTD_POR_PAGINA = 25          
                string request = "ACTION_FORM_SUBMETIDO="+ formAction + "&NAME_SQL_ANALITICO_CRIPT="+ SQLAnaliticoCript + "&NAME_SQL_SINTETICO_CRIPT="+ SQLSinteticoCript + "&realizar_consulta=nfse_por_periodo&dtInicial="+ HttpUtility.UrlEncode(dataIni) + "&dtFinal="+ HttpUtility.UrlEncode(dataFim) + "&nfp_numero=&mes_inicio="+ mesIni + "&ano_inicio="+ anoIni + "&mes_final="+ mesFim + "&ano_final="+ anoFim + "&exibir=exibir_tela&tipo_rps=0&atv_codigo=0&status=todos&exibir_cce=&cst_codigo=0&local_prestacao=todos&manutencao=&cpff=&razaosocial=&NAME_TIPO_RELATORIO=0&nfp_ids="+ nfp_ids + "&NAME_PAGINA_ATUAL=" + NAME_PAGINA_ATUAL+"&NAME_BOTAO_CLICADO=Pr%F3xima&NAME_QTD_POR_PAGINA=25&parans_assinar_documento=";

                sw = new StreamWriter(this.RequestDataStream);
                sw.Write(request);
                sw.Flush();

                this.Request(C_NFSE_ITAJAI + "jsp/nfse/emitido/notas/emissao.jsp");
            }

            if (!continuar)
            {
                break;
            }
        }
        #endregion
    }
}
