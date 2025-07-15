namespace NetahsilatDIA.UI
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.btnPayment = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menü1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menü2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAccount = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.flwPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cariHesapYönetimiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cariHesapEkstreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPayment
            // 
            this.btnPayment.ContextMenuStrip = this.contextMenuStrip1;
            this.btnPayment.Location = new System.Drawing.Point(13, 13);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(125, 50);
            this.btnPayment.TabIndex = 0;
            this.btnPayment.Text = "Ödemeler";
            this.btnPayment.UseVisualStyleBackColor = true;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menü1ToolStripMenuItem,
            this.menü2ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(274, 48);
            // 
            // menü1ToolStripMenuItem
            // 
            this.menü1ToolStripMenuItem.Name = "menü1ToolStripMenuItem";
            this.menü1ToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.menü1ToolStripMenuItem.Text = "PosRapor Güncelleme Ekranı";
            this.menü1ToolStripMenuItem.Click += new System.EventHandler(this.menü1ToolStripMenuItem_Click);
            // 
            // menü2ToolStripMenuItem
            // 
            this.menü2ToolStripMenuItem.Name = "menü2ToolStripMenuItem";
            this.menü2ToolStripMenuItem.Size = new System.Drawing.Size(273, 22);
            this.menü2ToolStripMenuItem.Text = "Tarih Aralığına Göre Banka Hareketleri";
            this.menü2ToolStripMenuItem.Click += new System.EventHandler(this.menü2ToolStripMenuItem_Click);
            // 
            // btnAccount
            // 
            this.btnAccount.Location = new System.Drawing.Point(154, 12);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(125, 50);
            this.btnAccount.TabIndex = 0;
            this.btnAccount.Text = "Cari Hesaplar";
            this.btnAccount.UseVisualStyleBackColor = true;
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(298, 12);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(125, 50);
            this.btnSettings.TabIndex = 0;
            this.btnSettings.Text = "Ayarlar";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // flwPanel
            // 
            this.flwPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flwPanel.Location = new System.Drawing.Point(13, 69);
            this.flwPanel.Name = "flwPanel";
            this.flwPanel.Size = new System.Drawing.Size(410, 160);
            this.flwPanel.TabIndex = 1;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cariHesapYönetimiToolStripMenuItem,
            this.cariHesapEkstreToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(182, 48);
            // 
            // cariHesapYönetimiToolStripMenuItem
            // 
            this.cariHesapYönetimiToolStripMenuItem.Name = "cariHesapYönetimiToolStripMenuItem";
            this.cariHesapYönetimiToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.cariHesapYönetimiToolStripMenuItem.Text = "Cari Hesap Yönetimi";
            this.cariHesapYönetimiToolStripMenuItem.Click += new System.EventHandler(this.cariHesapYönetimiToolStripMenuItem_Click);
            // 
            // cariHesapEkstreToolStripMenuItem
            // 
            this.cariHesapEkstreToolStripMenuItem.Name = "cariHesapEkstreToolStripMenuItem";
            this.cariHesapEkstreToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.cariHesapEkstreToolStripMenuItem.Text = "Cari Hesap Ekstre";
            this.cariHesapEkstreToolStripMenuItem.Click += new System.EventHandler(this.cariHesapEkstreToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 241);
            this.Controls.Add(this.flwPanel);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnAccount);
            this.Controls.Add(this.btnPayment);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Finrota Netahsilat | DİA Erp Entegrasyon Uygulaması";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPayment;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.FlowLayoutPanel flwPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menü2ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem cariHesapYönetimiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menü1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cariHesapEkstreToolStripMenuItem;
    }
}