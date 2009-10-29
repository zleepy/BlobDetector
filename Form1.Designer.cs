namespace Histogram
{
    partial class Form1
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
            this.openButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.originalPictureBox = new System.Windows.Forms.PictureBox();
            this.horizontalHistPictureBox = new System.Windows.Forms.PictureBox();
            this.verticalHistPictureBox = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.maskPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalHistPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalHistPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(12, 12);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "Öppna bild...";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sökväg till bild";
            // 
            // originalPictureBox
            // 
            this.originalPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.originalPictureBox.Location = new System.Drawing.Point(12, 41);
            this.originalPictureBox.Name = "originalPictureBox";
            this.originalPictureBox.Size = new System.Drawing.Size(325, 285);
            this.originalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.originalPictureBox.TabIndex = 2;
            this.originalPictureBox.TabStop = false;
            this.originalPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.originalPictureBox_MouseMove);
            this.originalPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.originalPictureBox_Paint);
            // 
            // horizontalHistPictureBox
            // 
            this.horizontalHistPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalHistPictureBox.Location = new System.Drawing.Point(12, 332);
            this.horizontalHistPictureBox.Name = "horizontalHistPictureBox";
            this.horizontalHistPictureBox.Size = new System.Drawing.Size(325, 100);
            this.horizontalHistPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.horizontalHistPictureBox.TabIndex = 3;
            this.horizontalHistPictureBox.TabStop = false;
            // 
            // verticalHistPictureBox
            // 
            this.verticalHistPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalHistPictureBox.Location = new System.Drawing.Point(343, 41);
            this.verticalHistPictureBox.Name = "verticalHistPictureBox";
            this.verticalHistPictureBox.Size = new System.Drawing.Size(100, 285);
            this.verticalHistPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.verticalHistPictureBox.TabIndex = 4;
            this.verticalHistPictureBox.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Bilder |*.jpg;*.jpeg;*.png;*.gif|Alla filer *.*|*.*";
            this.openFileDialog1.ReadOnlyChecked = true;
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            this.openFileDialog1.Title = "Öppna bild";
            // 
            // maskPictureBox
            // 
            this.maskPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.maskPictureBox.Location = new System.Drawing.Point(343, 332);
            this.maskPictureBox.Name = "maskPictureBox";
            this.maskPictureBox.Size = new System.Drawing.Size(100, 100);
            this.maskPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.maskPictureBox.TabIndex = 5;
            this.maskPictureBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 444);
            this.Controls.Add(this.maskPictureBox);
            this.Controls.Add(this.verticalHistPictureBox);
            this.Controls.Add(this.horizontalHistPictureBox);
            this.Controls.Add(this.originalPictureBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.openButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalHistPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalHistPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox originalPictureBox;
        private System.Windows.Forms.PictureBox horizontalHistPictureBox;
        private System.Windows.Forms.PictureBox verticalHistPictureBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox maskPictureBox;
    }
}

