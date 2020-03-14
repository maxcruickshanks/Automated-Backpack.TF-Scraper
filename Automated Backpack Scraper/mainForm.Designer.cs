namespace Automated_Backpack_Scraper
{
    partial class mainForm
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
            this.queueButton = new System.Windows.Forms.Button();
            this.itemDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // queueButton
            // 
            this.queueButton.Enabled = false;
            this.queueButton.Location = new System.Drawing.Point(3, 2);
            this.queueButton.Name = "queueButton";
            this.queueButton.Size = new System.Drawing.Size(88, 23);
            this.queueButton.TabIndex = 0;
            this.queueButton.Text = "Check Queue";
            this.queueButton.UseVisualStyleBackColor = true;
            this.queueButton.Click += new System.EventHandler(this.queueButton_Click);
            // 
            // itemDisplay
            // 
            this.itemDisplay.AutoSize = true;
            this.itemDisplay.Location = new System.Drawing.Point(97, 7);
            this.itemDisplay.Name = "itemDisplay";
            this.itemDisplay.Size = new System.Drawing.Size(89, 13);
            this.itemDisplay.TabIndex = 1;
            this.itemDisplay.Text = "Current item: N/A";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(315, 29);
            this.Controls.Add(this.itemDisplay);
            this.Controls.Add(this.queueButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(331, 68);
            this.MinimumSize = new System.Drawing.Size(331, 68);
            this.Name = "mainForm";
            this.ShowIcon = false;
            this.Text = "Automated Backpack Scraper";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button queueButton;
        private System.Windows.Forms.Label itemDisplay;
    }
}

