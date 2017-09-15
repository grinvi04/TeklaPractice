namespace Tekla_Practice
{
    partial class Form6
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
            this.label_pointX = new System.Windows.Forms.Label();
            this.textBox_pointX = new System.Windows.Forms.TextBox();
            this.textBox_pointY = new System.Windows.Forms.TextBox();
            this.label_pointY = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_createBeam
            // 
            this.button_createBeam.Location = new System.Drawing.Point(13, 13);
            this.button_createBeam.Name = "button_createBeam";
            this.button_createBeam.Size = new System.Drawing.Size(85, 23);
            this.button_createBeam.TabIndex = 0;
            this.button_createBeam.Text = "Column생성";
            this.button_createBeam.UseVisualStyleBackColor = true;
            this.button_createBeam.Click += new System.EventHandler(this.button_createBeam_Click);
            // 
            // label_pointX
            // 
            this.label_pointX.AutoSize = true;
            this.label_pointX.Location = new System.Drawing.Point(13, 43);
            this.label_pointX.Name = "label_pointX";
            this.label_pointX.Size = new System.Drawing.Size(65, 12);
            this.label_pointX.TabIndex = 1;
            this.label_pointX.Text = "철근 X좌표";
            // 
            // textBox_pointX
            // 
            this.textBox_pointX.Location = new System.Drawing.Point(15, 59);
            this.textBox_pointX.Name = "textBox_pointX";
            this.textBox_pointX.Size = new System.Drawing.Size(100, 21);
            this.textBox_pointX.TabIndex = 2;
            this.textBox_pointX.Text = "200";
            // 
            // textBox_pointY
            // 
            this.textBox_pointY.Location = new System.Drawing.Point(15, 108);
            this.textBox_pointY.Name = "textBox_pointY";
            this.textBox_pointY.Size = new System.Drawing.Size(100, 21);
            this.textBox_pointY.TabIndex = 4;
            this.textBox_pointY.Text = "100";
            // 
            // label_pointY
            // 
            this.label_pointY.AutoSize = true;
            this.label_pointY.Location = new System.Drawing.Point(13, 92);
            this.label_pointY.Name = "label_pointY";
            this.label_pointY.Size = new System.Drawing.Size(65, 12);
            this.label_pointY.TabIndex = 3;
            this.label_pointY.Text = "철근 Y좌표";
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.textBox_pointY);
            this.Controls.Add(this.label_pointY);
            this.Controls.Add(this.textBox_pointX);
            this.Controls.Add(this.label_pointX);
            this.Controls.Add(this.button_createBeam);
            this.Name = "Form6";
            this.Text = "Form6";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_createBeam;
        private System.Windows.Forms.Label label_pointX;
        private System.Windows.Forms.TextBox textBox_pointX;
        private System.Windows.Forms.TextBox textBox_pointY;
        private System.Windows.Forms.Label label_pointY;
    }
}