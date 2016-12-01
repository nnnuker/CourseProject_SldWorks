namespace WindowsForms
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
            this.loadElementsButton = new System.Windows.Forms.Button();
            this.createMates = new System.Windows.Forms.Button();
            this.restartButton = new System.Windows.Forms.Button();
            this.shellButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadElementsButton
            // 
            this.loadElementsButton.Location = new System.Drawing.Point(13, 13);
            this.loadElementsButton.Name = "loadElementsButton";
            this.loadElementsButton.Size = new System.Drawing.Size(75, 23);
            this.loadElementsButton.TabIndex = 0;
            this.loadElementsButton.Text = "Add";
            this.loadElementsButton.UseVisualStyleBackColor = true;
            this.loadElementsButton.Click += new System.EventHandler(this.loadElementsButton_Click);
            // 
            // createMates
            // 
            this.createMates.Location = new System.Drawing.Point(13, 62);
            this.createMates.Name = "createMates";
            this.createMates.Size = new System.Drawing.Size(75, 23);
            this.createMates.TabIndex = 1;
            this.createMates.Text = "Mates";
            this.createMates.UseVisualStyleBackColor = true;
            this.createMates.Click += new System.EventHandler(this.createMates_Click);
            // 
            // restartButton
            // 
            this.restartButton.Location = new System.Drawing.Point(13, 115);
            this.restartButton.Name = "restartButton";
            this.restartButton.Size = new System.Drawing.Size(75, 23);
            this.restartButton.TabIndex = 2;
            this.restartButton.Text = "Restart";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
            // 
            // shellButton
            // 
            this.shellButton.Location = new System.Drawing.Point(95, 13);
            this.shellButton.Name = "shellButton";
            this.shellButton.Size = new System.Drawing.Size(75, 23);
            this.shellButton.TabIndex = 3;
            this.shellButton.Text = "Shell";
            this.shellButton.UseVisualStyleBackColor = true;
            this.shellButton.Click += new System.EventHandler(this.shellButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 180);
            this.Controls.Add(this.shellButton);
            this.Controls.Add(this.restartButton);
            this.Controls.Add(this.createMates);
            this.Controls.Add(this.loadElementsButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loadElementsButton;
        private System.Windows.Forms.Button createMates;
        private System.Windows.Forms.Button restartButton;
        private System.Windows.Forms.Button shellButton;
    }
}

