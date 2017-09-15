namespace Tekla_Practice
{
    partial class cutplane
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
            this.button_cutplane = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_cutplane
            // 
            this.button_cutplane.Location = new System.Drawing.Point(13, 13);
            this.button_cutplane.Name = "button_cutplane";
            this.button_cutplane.Size = new System.Drawing.Size(75, 23);
            this.button_cutplane.TabIndex = 0;
            this.button_cutplane.Text = "cutplane";
            this.button_cutplane.UseVisualStyleBackColor = true;
            this.button_cutplane.Click += new System.EventHandler(this.button_cutplane_Click);
            // 
            // cutplane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button_cutplane);
            this.Name = "cutplane";
            this.Text = "cutplane";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_cutplane;
    }
}