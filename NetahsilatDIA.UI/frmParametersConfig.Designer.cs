namespace NetahsilatDIA.UI
{
    partial class frmParametersConfig
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtWebServicePwd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtWebServiceUid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVendorService = new System.Windows.Forms.TextBox();
            this.txtAccountService = new System.Windows.Forms.TextBox();
            this.txtDiaBaseUrl = new System.Windows.Forms.TextBox();
            this.txtErpService = new System.Windows.Forms.TextBox();
            this.dgvFirms = new System.Windows.Forms.DataGridView();
            this.btnPayBackPlan = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTmrInterval = new System.Windows.Forms.Label();
            this.numTmrInterval = new System.Windows.Forms.NumericUpDown();
            this.lblSetReversal = new System.Windows.Forms.Label();
            this.chkSetReversal = new System.Windows.Forms.CheckBox();
            this.lblIsTransferRepaymentPlan = new System.Windows.Forms.Label();
            this.chkIsTransferRepaymentPlan = new System.Windows.Forms.CheckBox();
            this.lblAddCommissionLine = new System.Windows.Forms.Label();
            this.chkAddCommissionLine = new System.Windows.Forms.CheckBox();
            this.lblSendEmail = new System.Windows.Forms.Label();
            this.chkSendEmail = new System.Windows.Forms.CheckBox();
            this.lblSendTransferTrans = new System.Windows.Forms.Label();
            this.chkSendTransferTrans = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.txtNonCustomerBagAccount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbNonCustomerPaymentType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbCustomerTransferType = new System.Windows.Forms.ComboBox();
            this.lblCustomerTransferType = new System.Windows.Forms.Label();
            this.lblAccountErpCodeField = new System.Windows.Forms.Label();
            this.cmbAccountErpCodeField = new System.Windows.Forms.ComboBox();
            this.lblReceiptTypeToBeTransferred = new System.Windows.Forms.Label();
            this.cmbReceiptTypeToBeTransferred = new System.Windows.Forms.ComboBox();
            this.lblOutOfCommission = new System.Windows.Forms.Label();
            this.cmbOutOfCommission = new System.Windows.Forms.ComboBox();
            this.lblPublicDescriptionFormat = new System.Windows.Forms.Label();
            this.lblLineDescriptionFormat = new System.Windows.Forms.Label();
            this.txtPublicDescriptionFormat = new System.Windows.Forms.TextBox();
            this.txtLineDescriptionFormat = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFirms)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTmrInterval)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtWebServicePwd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtWebServiceUid);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtVendorService);
            this.groupBox1.Controls.Add(this.txtAccountService);
            this.groupBox1.Controls.Add(this.txtDiaBaseUrl);
            this.groupBox1.Controls.Add(this.txtErpService);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 183);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Servis Adresleri";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Web Servis Şifre:";
            // 
            // txtWebServicePwd
            // 
            this.txtWebServicePwd.Location = new System.Drawing.Point(138, 151);
            this.txtWebServicePwd.Name = "txtWebServicePwd";
            this.txtWebServicePwd.PasswordChar = '*';
            this.txtWebServicePwd.Size = new System.Drawing.Size(266, 20);
            this.txtWebServicePwd.TabIndex = 2;
            this.txtWebServicePwd.Tag = "WEB_SERVICE_PWD";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Web Servis K. Adı:";
            // 
            // txtWebServiceUid
            // 
            this.txtWebServiceUid.Location = new System.Drawing.Point(137, 125);
            this.txtWebServiceUid.Name = "txtWebServiceUid";
            this.txtWebServiceUid.Size = new System.Drawing.Size(266, 20);
            this.txtWebServiceUid.TabIndex = 2;
            this.txtWebServiceUid.Tag = "WEB_SERVICE_UID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Cari Kart Servis Adresi";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "DİA Servis Adresi";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Cari Ekstre Servis Adresi";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ödeme Servis Adresi";
            // 
            // txtVendorService
            // 
            this.txtVendorService.Location = new System.Drawing.Point(137, 72);
            this.txtVendorService.Name = "txtVendorService";
            this.txtVendorService.Size = new System.Drawing.Size(266, 20);
            this.txtVendorService.TabIndex = 0;
            this.txtVendorService.Tag = "VENDOR_SERVICE";
            // 
            // txtAccountService
            // 
            this.txtAccountService.Location = new System.Drawing.Point(137, 46);
            this.txtAccountService.Name = "txtAccountService";
            this.txtAccountService.Size = new System.Drawing.Size(266, 20);
            this.txtAccountService.TabIndex = 0;
            this.txtAccountService.Tag = "ACCOUNT_SERVICE";
            // 
            // txtDiaBaseUrl
            // 
            this.txtDiaBaseUrl.Location = new System.Drawing.Point(137, 98);
            this.txtDiaBaseUrl.Name = "txtDiaBaseUrl";
            this.txtDiaBaseUrl.Size = new System.Drawing.Size(266, 20);
            this.txtDiaBaseUrl.TabIndex = 0;
            this.txtDiaBaseUrl.Tag = "DiaBaseUrl";
            // 
            // txtErpService
            // 
            this.txtErpService.Location = new System.Drawing.Point(137, 20);
            this.txtErpService.Name = "txtErpService";
            this.txtErpService.Size = new System.Drawing.Size(266, 20);
            this.txtErpService.TabIndex = 0;
            this.txtErpService.Tag = "ERP_SERVICE";
            // 
            // dgvFirms
            // 
            this.dgvFirms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFirms.Location = new System.Drawing.Point(12, 201);
            this.dgvFirms.Name = "dgvFirms";
            this.dgvFirms.Size = new System.Drawing.Size(409, 237);
            this.dgvFirms.TabIndex = 1;
            // 
            // btnPayBackPlan
            // 
            this.btnPayBackPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPayBackPlan.Location = new System.Drawing.Point(1021, 570);
            this.btnPayBackPlan.Name = "btnPayBackPlan";
            this.btnPayBackPlan.Size = new System.Drawing.Size(141, 23);
            this.btnPayBackPlan.TabIndex = 2;
            this.btnPayBackPlan.Text = "Geri Ödeme Planı";
            this.btnPayBackPlan.UseVisualStyleBackColor = true;
            this.btnPayBackPlan.Click += new System.EventHandler(this.btnPayBackPlan_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(1168, 570);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Kaydet";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(427, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(897, 552);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global Ayarlar";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 523F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 523F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 523F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(885, 523);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.AutoScroll = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lblTmrInterval, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.numTmrInterval, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSetReversal, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkSetReversal, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblIsTransferRepaymentPlan, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkIsTransferRepaymentPlan, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblAddCommissionLine, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.chkAddCommissionLine, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblSendEmail, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.chkSendEmail, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.lblSendTransferTrans, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.chkSendTransferTrans, 1, 5);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 9;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(436, 517);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblTmrInterval
            // 
            this.lblTmrInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTmrInterval.AutoSize = true;
            this.lblTmrInterval.Location = new System.Drawing.Point(3, 8);
            this.lblTmrInterval.Name = "lblTmrInterval";
            this.lblTmrInterval.Size = new System.Drawing.Size(73, 13);
            this.lblTmrInterval.TabIndex = 32;
            this.lblTmrInterval.Text = "Çalışma Sıklığı";
            // 
            // numTmrInterval
            // 
            this.numTmrInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numTmrInterval.Location = new System.Drawing.Point(221, 5);
            this.numTmrInterval.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.numTmrInterval.Name = "numTmrInterval";
            this.numTmrInterval.Size = new System.Drawing.Size(120, 20);
            this.numTmrInterval.TabIndex = 33;
            this.numTmrInterval.Tag = "TMR_INTERVAL";
            // 
            // lblSetReversal
            // 
            this.lblSetReversal.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSetReversal.AutoSize = true;
            this.lblSetReversal.Location = new System.Drawing.Point(3, 38);
            this.lblSetReversal.Name = "lblSetReversal";
            this.lblSetReversal.Size = new System.Drawing.Size(28, 13);
            this.lblSetReversal.TabIndex = 44;
            this.lblSetReversal.Text = "İade";
            // 
            // chkSetReversal
            // 
            this.chkSetReversal.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkSetReversal.AutoSize = true;
            this.chkSetReversal.Location = new System.Drawing.Point(221, 38);
            this.chkSetReversal.Name = "chkSetReversal";
            this.chkSetReversal.Size = new System.Drawing.Size(15, 14);
            this.chkSetReversal.TabIndex = 45;
            this.chkSetReversal.Tag = "SET_REVERSAL";
            this.chkSetReversal.UseVisualStyleBackColor = true;
            // 
            // lblIsTransferRepaymentPlan
            // 
            this.lblIsTransferRepaymentPlan.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblIsTransferRepaymentPlan.AutoSize = true;
            this.lblIsTransferRepaymentPlan.Location = new System.Drawing.Point(3, 68);
            this.lblIsTransferRepaymentPlan.Name = "lblIsTransferRepaymentPlan";
            this.lblIsTransferRepaymentPlan.Size = new System.Drawing.Size(159, 13);
            this.lblIsTransferRepaymentPlan.TabIndex = 46;
            this.lblIsTransferRepaymentPlan.Text = "Ödeme Planı Transfer Edilsin Mi:";
            // 
            // chkIsTransferRepaymentPlan
            // 
            this.chkIsTransferRepaymentPlan.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkIsTransferRepaymentPlan.AutoSize = true;
            this.chkIsTransferRepaymentPlan.Location = new System.Drawing.Point(221, 68);
            this.chkIsTransferRepaymentPlan.Name = "chkIsTransferRepaymentPlan";
            this.chkIsTransferRepaymentPlan.Size = new System.Drawing.Size(15, 14);
            this.chkIsTransferRepaymentPlan.TabIndex = 47;
            this.chkIsTransferRepaymentPlan.Tag = "IS_TRANSFER_REPAYMENTPLAN";
            this.chkIsTransferRepaymentPlan.UseVisualStyleBackColor = true;
            // 
            // lblAddCommissionLine
            // 
            this.lblAddCommissionLine.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAddCommissionLine.AutoSize = true;
            this.lblAddCommissionLine.Location = new System.Drawing.Point(3, 98);
            this.lblAddCommissionLine.Name = "lblAddCommissionLine";
            this.lblAddCommissionLine.Size = new System.Drawing.Size(105, 13);
            this.lblAddCommissionLine.TabIndex = 48;
            this.lblAddCommissionLine.Text = "Komisyon Satırı Ekle:";
            // 
            // chkAddCommissionLine
            // 
            this.chkAddCommissionLine.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAddCommissionLine.AutoSize = true;
            this.chkAddCommissionLine.Location = new System.Drawing.Point(221, 98);
            this.chkAddCommissionLine.Name = "chkAddCommissionLine";
            this.chkAddCommissionLine.Size = new System.Drawing.Size(15, 14);
            this.chkAddCommissionLine.TabIndex = 49;
            this.chkAddCommissionLine.Tag = "ADD_COMMISSION_LINE";
            this.chkAddCommissionLine.UseVisualStyleBackColor = true;
            // 
            // lblSendEmail
            // 
            this.lblSendEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSendEmail.AutoSize = true;
            this.lblSendEmail.Location = new System.Drawing.Point(3, 128);
            this.lblSendEmail.Name = "lblSendEmail";
            this.lblSendEmail.Size = new System.Drawing.Size(169, 13);
            this.lblSendEmail.TabIndex = 60;
            this.lblSendEmail.Text = "Cari Aktarımda Mail Gönderilsin Mi:";
            // 
            // chkSendEmail
            // 
            this.chkSendEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkSendEmail.AutoSize = true;
            this.chkSendEmail.Location = new System.Drawing.Point(221, 128);
            this.chkSendEmail.Name = "chkSendEmail";
            this.chkSendEmail.Size = new System.Drawing.Size(15, 14);
            this.chkSendEmail.TabIndex = 61;
            this.chkSendEmail.Tag = "SEND_EMAIL";
            this.chkSendEmail.UseVisualStyleBackColor = true;
            // 
            // lblSendTransferTrans
            // 
            this.lblSendTransferTrans.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSendTransferTrans.AutoSize = true;
            this.lblSendTransferTrans.Location = new System.Drawing.Point(3, 158);
            this.lblSendTransferTrans.Name = "lblSendTransferTrans";
            this.lblSendTransferTrans.Size = new System.Drawing.Size(116, 13);
            this.lblSendTransferTrans.TabIndex = 58;
            this.lblSendTransferTrans.Text = "Transfer İşlemi Gönder:";
            // 
            // chkSendTransferTrans
            // 
            this.chkSendTransferTrans.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkSendTransferTrans.AutoSize = true;
            this.chkSendTransferTrans.Location = new System.Drawing.Point(221, 158);
            this.chkSendTransferTrans.Name = "chkSendTransferTrans";
            this.chkSendTransferTrans.Size = new System.Drawing.Size(15, 14);
            this.chkSendTransferTrans.TabIndex = 59;
            this.chkSendTransferTrans.Tag = "SEND_TRANSFER_TRANS";
            this.chkSendTransferTrans.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoScroll = true;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.txtNonCustomerBagAccount, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.cmbNonCustomerPaymentType, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.cmbCustomerTransferType, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblCustomerTransferType, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblAccountErpCodeField, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmbAccountErpCodeField, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblReceiptTypeToBeTransferred, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.cmbReceiptTypeToBeTransferred, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.lblOutOfCommission, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.cmbOutOfCommission, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.lblPublicDescriptionFormat, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.lblLineDescriptionFormat, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.txtPublicDescriptionFormat, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this.txtLineDescriptionFormat, 1, 7);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(445, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 12;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(437, 517);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // txtNonCustomerBagAccount
            // 
            this.txtNonCustomerBagAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNonCustomerBagAccount.Location = new System.Drawing.Point(221, 155);
            this.txtNonCustomerBagAccount.Name = "txtNonCustomerBagAccount";
            this.txtNonCustomerBagAccount.Size = new System.Drawing.Size(213, 20);
            this.txtNonCustomerBagAccount.TabIndex = 69;
            this.txtNonCustomerBagAccount.Tag = "NONCUSTOMER_BAGACCOUNT";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 13);
            this.label8.TabIndex = 68;
            this.label8.Text = "Müşteri Olmayan Bağ. Hesap:";
            // 
            // cmbNonCustomerPaymentType
            // 
            this.cmbNonCustomerPaymentType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbNonCustomerPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNonCustomerPaymentType.FormattingEnabled = true;
            this.cmbNonCustomerPaymentType.Location = new System.Drawing.Point(221, 124);
            this.cmbNonCustomerPaymentType.Name = "cmbNonCustomerPaymentType";
            this.cmbNonCustomerPaymentType.Size = new System.Drawing.Size(213, 21);
            this.cmbNonCustomerPaymentType.TabIndex = 66;
            this.cmbNonCustomerPaymentType.Tag = "NONCUSTOMER_PAYMENT_TYPE";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 65;
            this.label7.Text = "Üyeliksiz İşlemlerde";
            // 
            // cmbCustomerTransferType
            // 
            this.cmbCustomerTransferType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCustomerTransferType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomerTransferType.FormattingEnabled = true;
            this.cmbCustomerTransferType.Location = new System.Drawing.Point(221, 34);
            this.cmbCustomerTransferType.Name = "cmbCustomerTransferType";
            this.cmbCustomerTransferType.Size = new System.Drawing.Size(213, 21);
            this.cmbCustomerTransferType.TabIndex = 64;
            this.cmbCustomerTransferType.Tag = "CUSTOMER_TRANSFER_TYPE";
            // 
            // lblCustomerTransferType
            // 
            this.lblCustomerTransferType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomerTransferType.AutoSize = true;
            this.lblCustomerTransferType.Location = new System.Drawing.Point(3, 38);
            this.lblCustomerTransferType.Name = "lblCustomerTransferType";
            this.lblCustomerTransferType.Size = new System.Drawing.Size(90, 13);
            this.lblCustomerTransferType.TabIndex = 52;
            this.lblCustomerTransferType.Text = "Cari Transfer Tipi:";
            // 
            // lblAccountErpCodeField
            // 
            this.lblAccountErpCodeField.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAccountErpCodeField.AutoSize = true;
            this.lblAccountErpCodeField.Location = new System.Drawing.Point(3, 8);
            this.lblAccountErpCodeField.Name = "lblAccountErpCodeField";
            this.lblAccountErpCodeField.Size = new System.Drawing.Size(120, 13);
            this.lblAccountErpCodeField.TabIndex = 50;
            this.lblAccountErpCodeField.Text = "Hesap ERP Kodu Alanı:";
            // 
            // cmbAccountErpCodeField
            // 
            this.cmbAccountErpCodeField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccountErpCodeField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccountErpCodeField.FormattingEnabled = true;
            this.cmbAccountErpCodeField.Location = new System.Drawing.Point(221, 4);
            this.cmbAccountErpCodeField.Name = "cmbAccountErpCodeField";
            this.cmbAccountErpCodeField.Size = new System.Drawing.Size(213, 21);
            this.cmbAccountErpCodeField.TabIndex = 64;
            this.cmbAccountErpCodeField.Tag = "ACCOUNT_ERPCODE_FIELD";
            // 
            // lblReceiptTypeToBeTransferred
            // 
            this.lblReceiptTypeToBeTransferred.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblReceiptTypeToBeTransferred.AutoSize = true;
            this.lblReceiptTypeToBeTransferred.Location = new System.Drawing.Point(3, 68);
            this.lblReceiptTypeToBeTransferred.Name = "lblReceiptTypeToBeTransferred";
            this.lblReceiptTypeToBeTransferred.Size = new System.Drawing.Size(104, 13);
            this.lblReceiptTypeToBeTransferred.TabIndex = 62;
            this.lblReceiptTypeToBeTransferred.Text = "Aktarılacak Fiş Türü:";
            // 
            // cmbReceiptTypeToBeTransferred
            // 
            this.cmbReceiptTypeToBeTransferred.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbReceiptTypeToBeTransferred.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReceiptTypeToBeTransferred.FormattingEnabled = true;
            this.cmbReceiptTypeToBeTransferred.Location = new System.Drawing.Point(221, 64);
            this.cmbReceiptTypeToBeTransferred.Name = "cmbReceiptTypeToBeTransferred";
            this.cmbReceiptTypeToBeTransferred.Size = new System.Drawing.Size(213, 21);
            this.cmbReceiptTypeToBeTransferred.TabIndex = 63;
            this.cmbReceiptTypeToBeTransferred.Tag = "RECEIPT_TYPE_TO_BE_TRANSFERRED";
            // 
            // lblOutOfCommission
            // 
            this.lblOutOfCommission.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOutOfCommission.AutoSize = true;
            this.lblOutOfCommission.Location = new System.Drawing.Point(3, 98);
            this.lblOutOfCommission.Name = "lblOutOfCommission";
            this.lblOutOfCommission.Size = new System.Drawing.Size(75, 13);
            this.lblOutOfCommission.TabIndex = 62;
            this.lblOutOfCommission.Text = "Komisyon Dışı:";
            // 
            // cmbOutOfCommission
            // 
            this.cmbOutOfCommission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOutOfCommission.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutOfCommission.FormattingEnabled = true;
            this.cmbOutOfCommission.Location = new System.Drawing.Point(221, 94);
            this.cmbOutOfCommission.Name = "cmbOutOfCommission";
            this.cmbOutOfCommission.Size = new System.Drawing.Size(213, 21);
            this.cmbOutOfCommission.TabIndex = 66;
            this.cmbOutOfCommission.Tag = "OUT_OF_COMMISSION";
            // 
            // lblPublicDescriptionFormat
            // 
            this.lblPublicDescriptionFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPublicDescriptionFormat.AutoSize = true;
            this.lblPublicDescriptionFormat.Location = new System.Drawing.Point(3, 188);
            this.lblPublicDescriptionFormat.Name = "lblPublicDescriptionFormat";
            this.lblPublicDescriptionFormat.Size = new System.Drawing.Size(121, 13);
            this.lblPublicDescriptionFormat.TabIndex = 56;
            this.lblPublicDescriptionFormat.Text = "Genel Açıklama Formatı:";
            // 
            // lblLineDescriptionFormat
            // 
            this.lblLineDescriptionFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLineDescriptionFormat.AutoSize = true;
            this.lblLineDescriptionFormat.Location = new System.Drawing.Point(3, 218);
            this.lblLineDescriptionFormat.Name = "lblLineDescriptionFormat";
            this.lblLineDescriptionFormat.Size = new System.Drawing.Size(114, 13);
            this.lblLineDescriptionFormat.TabIndex = 58;
            this.lblLineDescriptionFormat.Text = "Satır Açıklama Formatı:";
            // 
            // txtPublicDescriptionFormat
            // 
            this.txtPublicDescriptionFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPublicDescriptionFormat.Location = new System.Drawing.Point(221, 185);
            this.txtPublicDescriptionFormat.Name = "txtPublicDescriptionFormat";
            this.txtPublicDescriptionFormat.Size = new System.Drawing.Size(213, 20);
            this.txtPublicDescriptionFormat.TabIndex = 57;
            this.txtPublicDescriptionFormat.Tag = "PUBLIC_DESCRIPTION_FORMAT";
            // 
            // txtLineDescriptionFormat
            // 
            this.txtLineDescriptionFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLineDescriptionFormat.Location = new System.Drawing.Point(221, 215);
            this.txtLineDescriptionFormat.Name = "txtLineDescriptionFormat";
            this.txtLineDescriptionFormat.Size = new System.Drawing.Size(213, 20);
            this.txtLineDescriptionFormat.TabIndex = 59;
            this.txtLineDescriptionFormat.Tag = "LINE_DESCRIPTION_FORMAT";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(1249, 570);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmParametersConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 605);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnPayBackPlan);
            this.Controls.Add(this.dgvFirms);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(1035, 566);
            this.Name = "frmParametersConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "   Genel Çalışma Parametreleri";
            this.Load += new System.EventHandler(this.frmParametersConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFirms)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTmrInterval)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVendorService;
        private System.Windows.Forms.TextBox txtAccountService;
        private System.Windows.Forms.TextBox txtDiaBaseUrl;
        private System.Windows.Forms.TextBox txtErpService;
        private System.Windows.Forms.DataGridView dgvFirms;
        private System.Windows.Forms.Button btnPayBackPlan;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblTmrInterval;
        private System.Windows.Forms.NumericUpDown numTmrInterval;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblSetReversal;
        private System.Windows.Forms.CheckBox chkSetReversal;
        private System.Windows.Forms.Label lblIsTransferRepaymentPlan;
        private System.Windows.Forms.CheckBox chkIsTransferRepaymentPlan;
        private System.Windows.Forms.Label lblAddCommissionLine;
        private System.Windows.Forms.CheckBox chkAddCommissionLine;
        private System.Windows.Forms.Label lblAccountErpCodeField;
        private System.Windows.Forms.Label lblCustomerTransferType;
        private System.Windows.Forms.Label lblPublicDescriptionFormat;
        private System.Windows.Forms.Label lblLineDescriptionFormat;
        private System.Windows.Forms.Label lblSendEmail;
        private System.Windows.Forms.CheckBox chkSendEmail;
        private System.Windows.Forms.Label lblOutOfCommission;
        private System.Windows.Forms.Label lblReceiptTypeToBeTransferred;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbAccountErpCodeField;
        private System.Windows.Forms.ComboBox cmbOutOfCommission;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtWebServicePwd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtWebServiceUid;
        private System.Windows.Forms.ComboBox cmbNonCustomerPaymentType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbCustomerTransferType;
        private System.Windows.Forms.ComboBox cmbReceiptTypeToBeTransferred;
        private System.Windows.Forms.TextBox txtPublicDescriptionFormat;
        private System.Windows.Forms.TextBox txtLineDescriptionFormat;
        private System.Windows.Forms.TextBox txtNonCustomerBagAccount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblSendTransferTrans;
        private System.Windows.Forms.CheckBox chkSendTransferTrans;
    }
}

