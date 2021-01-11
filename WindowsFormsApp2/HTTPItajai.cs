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
    public partial class frHTTPItajai : Form
    {
        static readonly HttpClient client = new HttpClient();

        public frHTTPItajai()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebRequest_NFSe_Itajai buscaItajai = new HttpWebRequest_NFSe_Itajai();
            buscaItajai.CPF_CNPJ = edCPF.Text;
            buscaItajai.Senha = edSenha.Text;
            buscaItajai.dataInicial = dpDataInicial.Value.ToString("dd/MM/yyyy");
            buscaItajai.dataFinal = dpDataFinal.Value.ToString("dd/MM/yyyy");
            buscaItajai.DiretorioDestinoDownload = edDestinoDownload.Text;

            buscaItajai.baixarRPS();


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }


}
