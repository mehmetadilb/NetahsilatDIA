namespace NetahsilatDIA.UI
{
    partial class frmBankPaymentDetail
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
            this.cbGetNonIntegrated = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtOrderReference = new System.Windows.Forms.TextBox();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbGetNonIntegrated
            // 
            this.cbGetNonIntegrated.AutoSize = true;
            this.cbGetNonIntegrated.Checked = true;
            this.cbGetNonIntegrated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGetNonIntegrated.Location = new System.Drawing.Point(303, 27);
            this.cbGetNonIntegrated.Name = "cbGetNonIntegrated";
            this.cbGetNonIntegrated.Size = new System.Drawing.Size(15, 14);
            this.cbGetNonIntegrated.TabIndex = 114;
            this.cbGetNonIntegrated.UseVisualStyleBackColor = true;
            this.cbGetNonIntegrated.CheckedChanged += new System.EventHandler(this.cbGetNonIntegrated_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label1.Location = new System.Drawing.Point(32, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 14);
            this.label1.TabIndex = 111;
            this.label1.Text = "Entegrasyonu Yapılmamış Ödeme Planlarını Aktar";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label14.Location = new System.Drawing.Point(32, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(195, 14);
            this.label14.TabIndex = 112;
            this.label14.Text = "Ödeme Netahsilat Referans Numarası";
            // 
            // txtOrderReference
            // 
            this.txtOrderReference.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOrderReference.Enabled = false;
            this.txtOrderReference.Font = new System.Drawing.Font("Calibri", 9F);
            this.txtOrderReference.Location = new System.Drawing.Point(303, 74);
            this.txtOrderReference.Name = "txtOrderReference";
            this.txtOrderReference.Size = new System.Drawing.Size(288, 22);
            this.txtOrderReference.TabIndex = 90;
            this.txtOrderReference.Tag = "WEB_SERVICE_UID";
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(268, 136);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(154, 23);
            this.btnTransfer.TabIndex = 115;
            this.btnTransfer.Text = "Ödemeleri Aktar";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // frmBankPaymentDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 181);
            this.Controls.Add(this.txtOrderReference);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.cbGetNonIntegrated);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label14);
            this.Name = "frmBankPaymentDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Banka Geri Ödeme Hareketleri Aktarım Ekranı";
            this.Load += new System.EventHandler(this.frmBankPaymentDetail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbGetNonIntegrated;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtOrderReference;
        private System.Windows.Forms.Button btnTransfer;
    }
}