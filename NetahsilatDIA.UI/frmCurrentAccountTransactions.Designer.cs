using System.Drawing;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    partial class frmCurrentAccountTransactions
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblAccountCode;
        private TextBox txtAccountCode;
        private CheckBox chkAllTransactions;
        private Button btnSendAccountTrans;

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
            this.lblAccountCode = new System.Windows.Forms.Label();
            this.txtAccountCode = new System.Windows.Forms.TextBox();
            this.chkAllTransactions = new System.Windows.Forms.CheckBox();
            this.btnSendAccountTrans = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAccountCode
            // 
            this.lblAccountCode.AutoSize = true;
            this.lblAccountCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAccountCode.Location = new System.Drawing.Point(20, 25);
            this.lblAccountCode.Name = "lblAccountCode";
            this.lblAccountCode.Size = new System.Drawing.Size(108, 19);
            this.lblAccountCode.TabIndex = 0;
            this.lblAccountCode.Text = "Cari Hesap Kodu:";
            // 
            // txtAccountCode
            // 
            this.txtAccountCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAccountCode.Location = new System.Drawing.Point(160, 20);
            this.txtAccountCode.Name = "txtAccountCode";
            this.txtAccountCode.Size = new System.Drawing.Size(220, 25);
            this.txtAccountCode.TabIndex = 1;
            // 
            // chkAllTransactions
            // 
            this.chkAllTransactions.AutoSize = true;
            this.chkAllTransactions.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkAllTransactions.Location = new System.Drawing.Point(160, 60);
            this.chkAllTransactions.Name = "chkAllTransactions";
            this.chkAllTransactions.Size = new System.Drawing.Size(164, 23);
            this.chkAllTransactions.TabIndex = 2;
            this.chkAllTransactions.Text = "Bütün Hareketleri Gönder";
            this.chkAllTransactions.UseVisualStyleBackColor = true;
            // 
            // btnSendAccountTrans
            // 
            this.btnSendAccountTrans.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnSendAccountTrans.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendAccountTrans.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSendAccountTrans.ForeColor = System.Drawing.Color.White;
            this.btnSendAccountTrans.Location = new System.Drawing.Point(160, 100);
            this.btnSendAccountTrans.Name = "btnSendAccountTrans";
            this.btnSendAccountTrans.Size = new System.Drawing.Size(220, 35);
            this.btnSendAccountTrans.TabIndex = 3;
            this.btnSendAccountTrans.Text = "Ekstre Hareketlerini Gönder";
            this.btnSendAccountTrans.UseVisualStyleBackColor = false;
            this.btnSendAccountTrans.Click += new System.EventHandler(this.btnSendAccountTrans_Click);
            // 
            // frmCurrentAccountTransactions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 160);
            this.Controls.Add(this.lblAccountCode);
            this.Controls.Add(this.txtAccountCode);
            this.Controls.Add(this.chkAllTransactions);
            this.Controls.Add(this.btnSendAccountTrans);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmCurrentAccountTransactions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cari Hesap Hareketleri";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}