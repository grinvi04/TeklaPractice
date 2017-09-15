namespace Tekla_Practice
{
    partial class Form5
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
            this.label_booleanPart_height = new System.Windows.Forms.Label();
            this.textBox_booleanPart_height = new System.Windows.Forms.TextBox();
            this.label_booleanPart_width = new System.Windows.Forms.Label();
            this.textBox_booleanPart_width = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_createBeam
            // 
            this.button_createBeam.Location = new System.Drawing.Point(13, 13);
            this.button_createBeam.Name = "button_createBeam";
            this.button_createBeam.Size = new System.Drawing.Size(75, 23);
            this.button_createBeam.TabIndex = 0;
            this.button_createBeam.Text = "beam생성";
            this.button_createBeam.UseVisualStyleBackColor = true;
            this.button_createBeam.Click += new System.EventHandler(this.button_createBeam_Click);
            // 
            // label_booleanPart_height
            // 
            this.label_booleanPart_height.AutoSize = true;
            this.label_booleanPart_height.Location = new System.Drawing.Point(95, 13);
            this.label_booleanPart_height.Name = "label_booleanPart_height";
            this.label_booleanPart_height.Size = new System.Drawing.Size(85, 12);
            this.label_booleanPart_height.TabIndex = 1;
            this.label_booleanPart_height.Text = "자를 부재 높이";
            // 
            // textBox_booleanPart_height
            // 
            this.textBox_booleanPart_height.Location = new System.Drawing.Point(97, 29);
            this.textBox_booleanPart_height.Name = "textBox_booleanPart_height";
            this.textBox_booleanPart_height.Size = new System.Drawing.Size(100, 21);
            this.textBox_booleanPart_height.TabIndex = 2;
            this.textBox_booleanPart_height.Text = "600";
            // 
            // label_booleanPart_width
            // 
            this.label_booleanPart_width.AutoSize = true;
            this.label_booleanPart_width.Location = new System.Drawing.Point(202, 13);
            this.label_booleanPart_width.Name = "label_booleanPart_width";
            this.label_booleanPart_width.Size = new System.Drawing.Size(85, 12);
            this.label_booleanPart_width.TabIndex = 3;
            this.label_booleanPart_width.Text = "자를 부재 길이";
            // 
            // textBox_booleanPart_width
            // 
            this.textBox_booleanPart_width.Location = new System.Drawing.Point(204, 29);
            this.textBox_booleanPart_width.Name = "textBox_booleanPart_width";
            this.textBox_booleanPart_width.Size = new System.Drawing.Size(100, 21);
            this.textBox_booleanPart_width.TabIndex = 4;
            this.textBox_booleanPart_width.Text = "600";
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 261);
            this.Controls.Add(this.textBox_booleanPart_width);
            this.Controls.Add(this.label_booleanPart_width);
            this.Controls.Add(this.textBox_booleanPart_height);
            this.Controls.Add(this.label_booleanPart_height);
            this.Controls.Add(this.button_createBeam);
            this.Name = "Form5";
            this.Text = "Form5";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_createBeam;
        private System.Windows.Forms.Label label_booleanPart_height;
        private System.Windows.Forms.TextBox textBox_booleanPart_height;
        private System.Windows.Forms.Label label_booleanPart_width;
        private System.Windows.Forms.TextBox textBox_booleanPart_width;
    }
}