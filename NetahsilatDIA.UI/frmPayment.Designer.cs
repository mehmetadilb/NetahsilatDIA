namespace NetahsilatDIA.UI
{
    partial class frmPayment
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
            this.dtpAllLastDate = new System.Windows.Forms.DateTimePicker();
            this.dtpAllFirstDate = new System.Windows.Forms.DateTimePicker();
            this.cmbOrderType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReferenceNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTransferType = new System.Windows.Forms.Label();
            this.btnGetPayment = new System.Windows.Forms.Button();
            this.btnGetReversal = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpAllLastDate);
            this.groupBox1.Controls.Add(this.dtpAllFirstDate);
            this.groupBox1.Controls.Add(this.cmbOrderType);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(516, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ödemeler/İadeler";
            // 
            // dtpAllLastDate
            // 
            this.dtpAllLastDate.CustomFormat = "dd:MM:yyyy ";
            this.dtpAllLastDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAllLastDate.Location = new System.Drawing.Point(214, 78);
            this.dtpAllLastDate.Name = "dtpAllLastDate";
            this.dtpAllLastDate.Size = new System.Drawing.Size(231, 20);
            this.dtpAllLastDate.TabIndex = 31;
            // 
            // dtpAllFirstDate
            // 
            this.dtpAllFirstDate.CustomFormat = "dd:MM:yyyy";
            this.dtpAllFirstDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAllFirstDate.Location = new System.Drawing.Point(214, 52);
            this.dtpAllFirstDate.Name = "dtpAllFirstDate";
            this.dtpAllFirstDate.Size = new System.Drawing.Size(231, 20);
            this.dtpAllFirstDate.TabIndex = 31;
            // 
            // cmbOrderType
            // 
            this.cmbOrderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrderType.FormattingEnabled = true;
            this.cmbOrderType.Items.AddRange(new object[] {
            "Tüm Hareketler",
            "Entegrasyonu Yapılmamış Hareketler",
            "Entegrasyonu Yapılmış Hareketler"});
            this.cmbOrderType.Location = new System.Drawing.Point(214, 25);
            this.cmbOrderType.Name = "cmbOrderType";
            this.cmbOrderType.Size = new System.Drawing.Size(231, 21);
            this.cmbOrderType.TabIndex = 30;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(66, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Bitiş Tarihi                            :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Başlangıç Tarihi                   :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Entegraasyon Durumu         :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtReferenceNumber
            // 
            this.txtReferenceNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtReferenceNumber.Location = new System.Drawing.Point(226, 150);
            this.txtReferenceNumber.Name = "txtReferenceNumber";
            this.txtReferenceNumber.Size = new System.Drawing.Size(231, 20);
            this.txtReferenceNumber.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 8F);
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(9, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(519, 14);
            this.label2.TabIndex = 28;
            this.label2.Text = "Bilgi : Referans numarası ile arama yapılacağı zaman tarih veya entegrasyon durum" +
    "u girmeye gerek yoktur.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial", 8F);
            this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label4.Location = new System.Drawing.Point(53, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(439, 14);
            this.label4.TabIndex = 27;
            this.label4.Text = "Bilgi : Belirteceğiniz tarih aralığındaki ödemeler veya iadeler direkt olarak LOG" +
    "O \' ya aktarılır";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(78, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Referans Numarası              :";
            // 
            // lblTransferType
            // 
            this.lblTransferType.AutoSize = true;
            this.lblTransferType.BackColor = System.Drawing.Color.Transparent;
            this.lblTransferType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransferType.ForeColor = System.Drawing.Color.DimGray;
            this.lblTransferType.Location = new System.Drawing.Point(9, 512);
            this.lblTransferType.Name = "lblTransferType";
            this.lblTransferType.Size = new System.Drawing.Size(103, 16);
            this.lblTransferType.TabIndex = 30;
            this.lblTransferType.Text = "Aktarım Modu: ";
            // 
            // btnGetPayment
            // 
            this.btnGetPayment.Location = new System.Drawing.Point(85, 249);
            this.btnGetPayment.Name = "btnGetPayment";
            this.btnGetPayment.Size = new System.Drawing.Size(112, 23);
            this.btnGetPayment.TabIndex = 31;
            this.btnGetPayment.Text = "Ödemeleri Getir";
            this.btnGetPayment.UseVisualStyleBackColor = true;
            this.btnGetPayment.Click += new System.EventHandler(this.btnGetPayment_Click);
            // 
            // btnGetReversal
            // 
            this.btnGetReversal.Location = new System.Drawing.Point(206, 249);
            this.btnGetReversal.Name = "btnGetReversal";
            this.btnGetReversal.Size = new System.Drawing.Size(112, 23);
            this.btnGetReversal.TabIndex = 31;
            this.btnGetReversal.Text = "İadeleri Getir";
            this.btnGetReversal.UseVisualStyleBackColor = true;
            this.btnGetReversal.Click += new System.EventHandler(this.btnGetReversal_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(327, 249);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 23);
            this.btnClose.TabIndex = 31;
            this.btnClose.Text = "İptal";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 320);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnGetReversal);
            this.Controls.Add(this.btnGetPayment);
            this.Controls.Add(this.lblTransferType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtReferenceNumber);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ödemeler/İadeler";
            this.Load += new System.EventHandler(this.frmPayment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReferenceNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpAllLastDate;
        private System.Windows.Forms.DateTimePicker dtpAllFirstDate;
        private System.Windows.Forms.ComboBox cmbOrderType;
        private System.Windows.Forms.Label lblTransferType;
        private System.Windows.Forms.Button btnGetPayment;
        private System.Windows.Forms.Button btnGetReversal;
        private System.Windows.Forms.Button btnClose;
    }
}