namespace FileContainerUI
{
    partial class WelcomeForm
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
            this.flatButton2 = new FileContainerUI.Buttons.FlatButton();
            this.flatButton1 = new FileContainerUI.Buttons.FlatButton();
            this.SuspendLayout();
            // 
            // flatButton2
            // 
            this.flatButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flatButton2.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.flatButton2.Location = new System.Drawing.Point(13, 53);
            this.flatButton2.Name = "flatButton2";
            this.flatButton2.Size = new System.Drawing.Size(128, 35);
            this.flatButton2.TabIndex = 1;
            this.flatButton2.Text = "Open Existing";
            this.flatButton2.UseVisualStyleBackColor = true;
            this.flatButton2.Click += new System.EventHandler(this.flatButton2_Click);
            // 
            // flatButton1
            // 
            this.flatButton1.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.flatButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flatButton1.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.flatButton1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.flatButton1.Location = new System.Drawing.Point(12, 12);
            this.flatButton1.Name = "flatButton1";
            this.flatButton1.Size = new System.Drawing.Size(128, 35);
            this.flatButton1.TabIndex = 0;
            this.flatButton1.Text = "Create New";
            this.flatButton1.UseVisualStyleBackColor = false;
            this.flatButton1.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(153, 99);
            this.Controls.Add(this.flatButton2);
            this.Controls.Add(this.flatButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Container";
            this.ResumeLayout(false);

        }

        #endregion

        private Buttons.FlatButton flatButton1;
        private Buttons.FlatButton flatButton2;
    }
}