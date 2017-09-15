namespace Tekla_Practice
{
    partial class Form7
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
            this.button_selectColumn = new System.Windows.Forms.Button();
            this.button_selectGirder = new System.Windows.Forms.Button();
            this.button_directionCheck = new System.Windows.Forms.Button();
            this.label_singleRebar_length = new System.Windows.Forms.Label();
            this.textBox_singleRebar_length = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_selectColumn
            // 
            this.button_selectColumn.Location = new System.Drawing.Point(12, 12);
            this.button_selectColumn.Name = "button_selectColumn";
            this.button_selectColumn.Size = new System.Drawing.Size(88, 23);
            this.button_selectColumn.TabIndex = 0;
            this.button_selectColumn.Text = "Column 선택";
            this.button_selectColumn.UseVisualStyleBackColor = true;
            this.button_selectColumn.Click += new System.EventHandler(this.button_selectColumn_Click);
            // 
            // button_selectGirder
            // 
            this.button_selectGirder.Location = new System.Drawing.Point(106, 12);
            this.button_selectGirder.Name = "button_selectGirder";
            this.button_selectGirder.Size = new System.Drawing.Size(88, 23);
            this.button_selectGirder.TabIndex = 1;
            this.button_selectGirder.Text = "Girder 선택";
            this.button_selectGirder.UseVisualStyleBackColor = true;
            this.button_selectGirder.Click += new System.EventHandler(this.button_selectGirder_Click);
            // 
            // button_directionCheck
            // 
            this.button_directionCheck.Location = new System.Drawing.Point(12, 137);
            this.button_directionCheck.Name = "button_directionCheck";
            this.button_directionCheck.Size = new System.Drawing.Size(88, 23);
            this.button_directionCheck.TabIndex = 2;
            this.button_directionCheck.Text = "철근생성";
            this.button_directionCheck.UseVisualStyleBackColor = true;
            this.button_directionCheck.Click += new System.EventHandler(this.button_directionCheck_Click);
            // 
            // label_singleRebar_length
            // 
            this.label_singleRebar_length.AutoSize = true;
            this.label_singleRebar_length.Location = new System.Drawing.Point(13, 87);
            this.label_singleRebar_length.Name = "label_singleRebar_length";
            this.label_singleRebar_length.Size = new System.Drawing.Size(121, 12);
            this.label_singleRebar_length.TabIndex = 4;
            this.label_singleRebar_length.Text = "교차점에서 철근 길이";
            // 
            // textBox_singleRebar_length
            // 
            this.textBox_singleRebar_length.Location = new System.Drawing.Point(140, 84);
            this.textBox_singleRebar_length.Name = "textBox_singleRebar_length";
            this.textBox_singleRebar_length.Size = new System.Drawing.Size(100, 21);
            this.textBox_singleRebar_length.TabIndex = 5;
            this.textBox_singleRebar_length.Text = "300";
            // 
            // Form7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 261);
            this.Controls.Add(this.textBox_singleRebar_length);
            this.Controls.Add(this.label_singleRebar_length);
            this.Controls.Add(this.button_directionCheck);
            this.Controls.Add(this.button_selectGirder);
            this.Controls.Add(this.button_selectColumn);
            this.Name = "Form7";
            this.Text = "Form7";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_selectColumn;
        private System.Windows.Forms.Button button_selectGirder;
        private System.Windows.Forms.Button button_directionCheck;
        private System.Windows.Forms.Label label_singleRebar_length;
        private System.Windows.Forms.TextBox textBox_singleRebar_length;
    }
}