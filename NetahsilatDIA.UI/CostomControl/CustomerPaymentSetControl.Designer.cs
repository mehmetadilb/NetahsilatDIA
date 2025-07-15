namespace NetahsilatDIA.UI.CostomControl
{
    partial class CustomerPaymentSetControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtPaymentSetId = new System.Windows.Forms.TextBox();
            this.txtCustomerPrefix = new System.Windows.Forms.TextBox();
            this.txtFirmNo = new System.Windows.Forms.TextBox();
            this.txtCurrentAccountTypeId = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.DarkRed;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(423, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 21);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Sil";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtPaymentSetId
            // 
            this.txtPaymentSetId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPaymentSetId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtPaymentSetId.Location = new System.Drawing.Point(199, 8);
            this.txtPaymentSetId.Name = "txtPaymentSetId";
            this.txtPaymentSetId.Size = new System.Drawing.Size(94, 16);
            this.txtPaymentSetId.TabIndex = 2;
            // 
            // txtCustomerPrefix
            // 
            this.txtCustomerPrefix.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCustomerPrefix.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtCustomerPrefix.Location = new System.Drawing.Point(70, 8);
            this.txtCustomerPrefix.Name = "txtCustomerPrefix";
            this.txtCustomerPrefix.Size = new System.Drawing.Size(111, 16);
            this.txtCustomerPrefix.TabIndex = 1;
            // 
            // txtFirmNo
            // 
            this.txtFirmNo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFirmNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtFirmNo.Location = new System.Drawing.Point(0, 8);
            this.txtFirmNo.Name = "txtFirmNo";
            this.txtFirmNo.Size = new System.Drawing.Size(53, 16);
            this.txtFirmNo.TabIndex = 0;
            // 
            // txtCurrentAccountTypeId
            // 
            this.txtCurrentAccountTypeId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCurrentAccountTypeId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtCurrentAccountTypeId.Location = new System.Drawing.Point(310, 8);
            this.txtCurrentAccountTypeId.Name = "txtCurrentAccountTypeId";
            this.txtCurrentAccountTypeId.Size = new System.Drawing.Size(94, 16);
            this.txtCurrentAccountTypeId.TabIndex = 3;
            // 
            // CustomerPaymentSetControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.txtCurrentAccountTypeId);
            this.Controls.Add(this.txtFirmNo);
            this.Controls.Add(this.txtCustomerPrefix);
            this.Controls.Add(this.txtPaymentSetId);
            this.Controls.Add(this.btnDelete);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CustomerPaymentSetControl";
            this.Size = new System.Drawing.Size(501, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtPaymentSetId;
        private System.Windows.Forms.TextBox txtCustomerPrefix;
        private System.Windows.Forms.TextBox txtFirmNo;
        private System.Windows.Forms.TextBox txtCurrentAccountTypeId;
    }
}
