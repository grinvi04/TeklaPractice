namespace Tekla_Practice
{
    partial class DisplayPoint
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
            this.ButtonSelectPart = new System.Windows.Forms.Button();
            this.TextBoxPointOfPart = new System.Windows.Forms.TextBox();
            this.LabelPointOfPart = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonSelectPart
            // 
            this.ButtonSelectPart.Location = new System.Drawing.Point(11, 13);
            this.ButtonSelectPart.Name = "ButtonSelectPart";
            this.ButtonSelectPart.Size = new System.Drawing.Size(75, 23);
            this.ButtonSelectPart.TabIndex = 0;
            this.ButtonSelectPart.Text = "부재 선택";
            this.ButtonSelectPart.UseVisualStyleBackColor = true;
            this.ButtonSelectPart.Click += new System.EventHandler(this.ButtonSelectPart_Click);
            // 
            // TextBoxPointOfPart
            // 
            this.TextBoxPointOfPart.Location = new System.Drawing.Point(11, 78);
            this.TextBoxPointOfPart.Multiline = true;
            this.TextBoxPointOfPart.Name = "TextBoxPointOfPart";
            this.TextBoxPointOfPart.ReadOnly = true;
            this.TextBoxPointOfPart.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxPointOfPart.Size = new System.Drawing.Size(529, 371);
            this.TextBoxPointOfPart.TabIndex = 1;
            // 
            // LabelPointOfPart
            // 
            this.LabelPointOfPart.AutoSize = true;
            this.LabelPointOfPart.Location = new System.Drawing.Point(11, 51);
            this.LabelPointOfPart.Name = "LabelPointOfPart";
            this.LabelPointOfPart.Size = new System.Drawing.Size(97, 12);
            this.LabelPointOfPart.TabIndex = 2;
            this.LabelPointOfPart.Text = "부재 포인트 정보";
            // 
            // DisplayPoint
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(547, 461);
            this.Controls.Add(this.LabelPointOfPart);
            this.Controls.Add(this.TextBoxPointOfPart);
            this.Controls.Add(this.ButtonSelectPart);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DisplayPoint";
            this.Text = "DisplayPoint";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonSelectPart;
        private System.Windows.Forms.TextBox TextBoxPointOfPart;
        private System.Windows.Forms.Label LabelPointOfPart;
    }
}