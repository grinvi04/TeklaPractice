namespace Tekla_Practice
{
    partial class Form4
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
            this.button_createBeam = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_createBeam
            // 
            this.button_createBeam.Location = new System.Drawing.Point(13, 13);
            this.button_createBeam.Name = "button_createBeam";
            this.button_createBeam.Size = new System.Drawing.Size(75, 23);
            this.button_createBeam.TabIndex = 0;
            this.button_createBeam.Text = "Beam생성";
            this.button_createBeam.UseVisualStyleBackColor = true;
            this.button_createBeam.Click += new System.EventHandler(this.button_createBeam_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button_createBeam);
            this.Name = "Form4";
            this.Text = "Form4";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_createBeam;
    }
}