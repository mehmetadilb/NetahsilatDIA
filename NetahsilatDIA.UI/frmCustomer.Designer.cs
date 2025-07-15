namespace NetahsilatDIA.UI
{
    partial class frmCustomer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.cboxTransferFailedAccounts = new System.Windows.Forms.CheckBox();
            this.btnSendAccount = new System.Windows.Forms.Button();
            this.grpLogoCustomer = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.flwCustomerMapping = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFirmNo = new System.Windows.Forms.TextBox();
            this.txtCustomerPrefix = new System.Windows.Forms.TextBox();
            this.txtPaymentPlanId = new System.Windows.Forms.TextBox();
            this.txtCurrentAccountTypeId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAccountCode = new System.Windows.Forms.TextBox();
            this.labelAccountCode = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpLogoCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboxTransferFailedAccounts
            // 
            this.cboxTransferFailedAccounts.AutoSize = true;
            this.cboxTransferFailedAccounts.Font = new System.Drawing.Font("Calibri", 10.2F, System.Drawing.FontStyle.Bold);
            this.cboxTransferFailedAccounts.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.cboxTransferFailedAccounts.Location = new System.Drawing.Point(84, 103);
            this.cboxTransferFailedAccounts.Name = "cboxTransferFailedAccounts";
            this.cboxTransferFailedAccounts.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cboxTransferFailedAccounts.Size = new System.Drawing.Size(370, 21);
            this.cboxTransferFailedAccounts.TabIndex = 115;
            this.cboxTransferFailedAccounts.Text = "Daha önce aktarımı yapılamayan hesaplar tekrar denensin";
            this.cboxTransferFailedAccounts.UseVisualStyleBackColor = true;
            // 
            // btnSendAccount
            // 
            this.btnSendAccount.Location = new System.Drawing.Point(14, 59);
            this.btnSendAccount.Name = "btnSendAccount";
            this.btnSendAccount.Size = new System.Drawing.Size(494, 38);
            this.btnSendAccount.TabIndex = 116;
            this.btnSendAccount.Text = "Cari Hesapları Aktar";
            this.btnSendAccount.UseVisualStyleBackColor = true;
            this.btnSendAccount.Click += new System.EventHandler(this.btnSendAccount_Click);
            // 
            // grpLogoCustomer
            // 
            this.grpLogoCustomer.BackColor = System.Drawing.Color.Transparent;
            this.grpLogoCustomer.Controls.Add(this.btnAdd);
            this.grpLogoCustomer.Controls.Add(this.flwCustomerMapping);
            this.grpLogoCustomer.Controls.Add(this.txtFirmNo);
            this.grpLogoCustomer.Controls.Add(this.txtCustomerPrefix);
            this.grpLogoCustomer.Controls.Add(this.txtPaymentPlanId);
            this.grpLogoCustomer.Controls.Add(this.txtCurrentAccountTypeId);
            this.grpLogoCustomer.Controls.Add(this.label1);
            this.grpLogoCustomer.Controls.Add(this.label10);
            this.grpLogoCustomer.Controls.Add(this.label11);
            this.grpLogoCustomer.Controls.Add(this.label12);
            this.grpLogoCustomer.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            this.grpLogoCustomer.ForeColor = System.Drawing.Color.SlateGray;
            this.grpLogoCustomer.Location = new System.Drawing.Point(4, 142);
            this.grpLogoCustomer.Name = "grpLogoCustomer";
            this.grpLogoCustomer.Size = new System.Drawing.Size(510, 270);
            this.grpLogoCustomer.TabIndex = 113;
            this.grpLogoCustomer.TabStop = false;
            this.grpLogoCustomer.Text = "Cari Ön Ek ve Ödeme Seti Id Bilgileri";
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btnAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdd.Location = new System.Drawing.Point(429, 66);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 113;
            this.btnAdd.Text = "Ekle";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // flwCustomerMapping
            // 
            this.flwCustomerMapping.AllowDrop = true;
            this.flwCustomerMapping.AutoScroll = true;
            this.flwCustomerMapping.BackColor = System.Drawing.Color.Transparent;
            this.flwCustomerMapping.Font = new System.Drawing.Font("Calibri", 9F);
            this.flwCustomerMapping.Location = new System.Drawing.Point(7, 109);
            this.flwCustomerMapping.Name = "flwCustomerMapping";
            this.flwCustomerMapping.Size = new System.Drawing.Size(498, 153);
            this.flwCustomerMapping.TabIndex = 101;
            // 
            // txtFirmNo
            // 
            this.txtFirmNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFirmNo.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtFirmNo.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtFirmNo.Location = new System.Drawing.Point(7, 66);
            this.txtFirmNo.Name = "txtFirmNo";
            this.txtFirmNo.Size = new System.Drawing.Size(59, 27);
            this.txtFirmNo.TabIndex = 1;
            this.txtFirmNo.Tag = "COMPANY";
            // 
            // txtCustomerPrefix
            // 
            this.txtCustomerPrefix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustomerPrefix.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtCustomerPrefix.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtCustomerPrefix.Location = new System.Drawing.Point(80, 65);
            this.txtCustomerPrefix.Name = "txtCustomerPrefix";
            this.txtCustomerPrefix.Size = new System.Drawing.Size(111, 27);
            this.txtCustomerPrefix.TabIndex = 2;
            this.txtCustomerPrefix.Tag = "Prefix";
            // 
            // txtPaymentPlanId
            // 
            this.txtPaymentPlanId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPaymentPlanId.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtPaymentPlanId.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtPaymentPlanId.Location = new System.Drawing.Point(209, 66);
            this.txtPaymentPlanId.Name = "txtPaymentPlanId";
            this.txtPaymentPlanId.Size = new System.Drawing.Size(94, 27);
            this.txtPaymentPlanId.TabIndex = 3;
            this.txtPaymentPlanId.Tag = "PAYMENTSET";
            // 
            // txtCurrentAccountTypeId
            // 
            this.txtCurrentAccountTypeId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurrentAccountTypeId.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtCurrentAccountTypeId.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtCurrentAccountTypeId.Location = new System.Drawing.Point(317, 66);
            this.txtCurrentAccountTypeId.Name = "txtCurrentAccountTypeId";
            this.txtCurrentAccountTypeId.Size = new System.Drawing.Size(94, 27);
            this.txtCurrentAccountTypeId.TabIndex = 4;
            this.txtCurrentAccountTypeId.Tag = "CURRENT_ACCOUNT_TYPE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label1.Location = new System.Drawing.Point(4, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 14);
            this.label1.TabIndex = 97;
            this.label1.Text = "Firma No";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label10.Location = new System.Drawing.Point(77, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(114, 14);
            this.label10.TabIndex = 98;
            this.label10.Text = "Cari Hesap Kod Ön Ek";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label11.Location = new System.Drawing.Point(206, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 14);
            this.label11.TabIndex = 99;
            this.label11.Text = "Ödeme Seti Id";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label12.Location = new System.Drawing.Point(314, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 14);
            this.label12.TabIndex = 100;
            this.label12.Text = "Cari Hesap Tipi Id";
            // 
            // txtAccountCode
            // 
            this.txtAccountCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAccountCode.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtAccountCode.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtAccountCode.Location = new System.Drawing.Point(133, 14);
            this.txtAccountCode.Name = "txtAccountCode";
            this.txtAccountCode.Size = new System.Drawing.Size(375, 27);
            this.txtAccountCode.TabIndex = 5;
            this.txtAccountCode.Tag = "ACCOUNTCODE";
            // 
            // labelAccountCode
            // 
            this.labelAccountCode.AutoSize = true;
            this.labelAccountCode.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.labelAccountCode.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.labelAccountCode.Location = new System.Drawing.Point(7, 16);
            this.labelAccountCode.Name = "labelAccountCode";
            this.labelAccountCode.Size = new System.Drawing.Size(120, 19);
            this.labelAccountCode.TabIndex = 101;
            this.labelAccountCode.Text = "Cari Hesap Kodu";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(378, 419);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 33);
            this.btnSave.TabIndex = 117;
            this.btnSave.Text = "Kaydet";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 463);
            this.Controls.Add(this.labelAccountCode);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnSendAccount);
            this.Controls.Add(this.cboxTransferFailedAccounts);
            this.Controls.Add(this.grpLogoCustomer);
            this.Controls.Add(this.txtAccountCode);
            this.Name = "frmCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cari Hesap Kod-Ödeme Seti Eşleştirmesi";
            this.Load += new System.EventHandler(this.frmCustomer_Load);
            this.grpLogoCustomer.ResumeLayout(false);
            this.grpLogoCustomer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cboxTransferFailedAccounts;
        private System.Windows.Forms.Button btnSendAccount;
        private System.Windows.Forms.GroupBox grpLogoCustomer;
        private System.Windows.Forms.FlowLayoutPanel flwCustomerMapping;
        private System.Windows.Forms.TextBox txtFirmNo;
        private System.Windows.Forms.TextBox txtCustomerPrefix;
        private System.Windows.Forms.TextBox txtPaymentPlanId;
        private System.Windows.Forms.TextBox txtCurrentAccountTypeId;
        private System.Windows.Forms.TextBox txtAccountCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelAccountCode;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
    }
}