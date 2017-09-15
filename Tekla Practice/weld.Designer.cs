namespace Tekla_Practice
{
    partial class weld
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
            this.button_weld = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_weld
            // 
            this.button_weld.Location = new System.Drawing.Point(13, 13);
            this.button_weld.Name = "button_weld";
            this.button_weld.Size = new System.Drawing.Size(75, 23);
            this.button_weld.TabIndex = 0;
            this.button_weld.Text = "weld";
            this.button_weld.UseVisualStyleBackColor = true;
            this.button_weld.Click += new System.EventHandler(this.button_weld_Click);
            // 
            // weld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button_weld);
            this.Name = "weld";
            this.Text = "weld";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_weld;
    }
}