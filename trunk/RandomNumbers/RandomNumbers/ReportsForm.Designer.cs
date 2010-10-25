namespace RandomNumbers.Tests {
    partial class ReportsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.reportBox = new System.Windows.Forms.ComboBox();
            this.reportBody = new System.Windows.Forms.RichTextBox();
            this.saveReportDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveReportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // reportBox
            // 
            this.reportBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.reportBox.FormattingEnabled = true;
            this.reportBox.Location = new System.Drawing.Point(344, 13);
            this.reportBox.Name = "reportBox";
            this.reportBox.Size = new System.Drawing.Size(121, 21);
            this.reportBox.TabIndex = 0;
            this.reportBox.SelectedIndexChanged += new System.EventHandler(this.reportBox_SelectedIndexChanged);
            // 
            // reportBody
            // 
            this.reportBody.Location = new System.Drawing.Point(12, 41);
            this.reportBody.Name = "reportBody";
            this.reportBody.ReadOnly = true;
            this.reportBody.Size = new System.Drawing.Size(453, 236);
            this.reportBody.TabIndex = 1;
            this.reportBody.Text = "";
            // 
            // saveReportDialog
            // 
            this.saveReportDialog.Title = "Save Report";
            // 
            // saveReportButton
            // 
            this.saveReportButton.Location = new System.Drawing.Point(13, 13);
            this.saveReportButton.Name = "saveReportButton";
            this.saveReportButton.Size = new System.Drawing.Size(75, 23);
            this.saveReportButton.TabIndex = 2;
            this.saveReportButton.Text = "Save Report";
            this.saveReportButton.UseVisualStyleBackColor = true;
            this.saveReportButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 289);
            this.Controls.Add(this.saveReportButton);
            this.Controls.Add(this.reportBody);
            this.Controls.Add(this.reportBox);
            this.Name = "ReportsForm";
            this.Text = "Reports Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox reportBox;
        private System.Windows.Forms.RichTextBox reportBody;
        private System.Windows.Forms.SaveFileDialog saveReportDialog;
        private System.Windows.Forms.Button saveReportButton;

    }
}