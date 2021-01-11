namespace WindowsFormsApp2
{
    partial class frHTTPItajai
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.edSenha = new System.Windows.Forms.TextBox();
            this.btBuscar = new System.Windows.Forms.Button();
            this.lbCPFCNPJ = new System.Windows.Forms.Label();
            this.lbSenha = new System.Windows.Forms.Label();
            this.dpDataInicial = new System.Windows.Forms.DateTimePicker();
            this.edCPF = new System.Windows.Forms.MaskedTextBox();
            this.lbDataInicial = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dpDataFinal = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.edDestinoDownload = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // edSenha
            // 
            this.edSenha.Location = new System.Drawing.Point(146, 31);
            this.edSenha.Name = "edSenha";
            this.edSenha.Size = new System.Drawing.Size(159, 20);
            this.edSenha.TabIndex = 1;
            // 
            // btBuscar
            // 
            this.btBuscar.Location = new System.Drawing.Point(403, 145);
            this.btBuscar.Name = "btBuscar";
            this.btBuscar.Size = new System.Drawing.Size(101, 23);
            this.btBuscar.TabIndex = 2;
            this.btBuscar.Text = "Buscar XMLs";
            this.btBuscar.UseVisualStyleBackColor = true;
            this.btBuscar.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbCPFCNPJ
            // 
            this.lbCPFCNPJ.AutoSize = true;
            this.lbCPFCNPJ.Location = new System.Drawing.Point(106, 9);
            this.lbCPFCNPJ.Name = "lbCPFCNPJ";
            this.lbCPFCNPJ.Size = new System.Drawing.Size(37, 13);
            this.lbCPFCNPJ.TabIndex = 3;
            this.lbCPFCNPJ.Text = "CNPJ:";
            this.lbCPFCNPJ.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbSenha
            // 
            this.lbSenha.AutoSize = true;
            this.lbSenha.Location = new System.Drawing.Point(102, 34);
            this.lbSenha.Name = "lbSenha";
            this.lbSenha.Size = new System.Drawing.Size(41, 13);
            this.lbSenha.TabIndex = 4;
            this.lbSenha.Text = "Senha:";
            // 
            // dpDataInicial
            // 
            this.dpDataInicial.Location = new System.Drawing.Point(146, 57);
            this.dpDataInicial.Name = "dpDataInicial";
            this.dpDataInicial.Size = new System.Drawing.Size(197, 20);
            this.dpDataInicial.TabIndex = 5;
            // 
            // edCPF
            // 
            this.edCPF.Location = new System.Drawing.Point(146, 6);
            this.edCPF.Mask = "99.999.999/9999-99";
            this.edCPF.Name = "edCPF";
            this.edCPF.Size = new System.Drawing.Size(100, 20);
            this.edCPF.TabIndex = 6;
            this.edCPF.ValidatingType = typeof(System.DateTime);
            // 
            // lbDataInicial
            // 
            this.lbDataInicial.AutoSize = true;
            this.lbDataInicial.Location = new System.Drawing.Point(81, 63);
            this.lbDataInicial.Name = "lbDataInicial";
            this.lbDataInicial.Size = new System.Drawing.Size(62, 13);
            this.lbDataInicial.TabIndex = 7;
            this.lbDataInicial.Text = "Data inicial:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data final:";
            // 
            // dpDataFinal
            // 
            this.dpDataFinal.Location = new System.Drawing.Point(146, 83);
            this.dpDataFinal.Name = "dpDataFinal";
            this.dpDataFinal.Size = new System.Drawing.Size(197, 20);
            this.dpDataFinal.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Diretório a salvar os XMLs:";
            // 
            // edDestinoDownload
            // 
            this.edDestinoDownload.Location = new System.Drawing.Point(146, 109);
            this.edDestinoDownload.Name = "edDestinoDownload";
            this.edDestinoDownload.Size = new System.Drawing.Size(359, 20);
            this.edDestinoDownload.TabIndex = 11;
            this.edDestinoDownload.Text = "c:\\pilar\\";
            // 
            // frHTTPItajai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 180);
            this.Controls.Add(this.edDestinoDownload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dpDataFinal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbDataInicial);
            this.Controls.Add(this.edCPF);
            this.Controls.Add(this.dpDataInicial);
            this.Controls.Add(this.lbSenha);
            this.Controls.Add(this.lbCPFCNPJ);
            this.Controls.Add(this.btBuscar);
            this.Controls.Add(this.edSenha);
            this.Name = "frHTTPItajai";
            this.Text = "HTTP Itajaí";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox edSenha;
        private System.Windows.Forms.Button btBuscar;
        private System.Windows.Forms.Label lbCPFCNPJ;
        private System.Windows.Forms.Label lbSenha;
        private System.Windows.Forms.DateTimePicker dpDataInicial;
        private System.Windows.Forms.MaskedTextBox edCPF;
        private System.Windows.Forms.Label lbDataInicial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dpDataFinal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edDestinoDownload;
    }
}

