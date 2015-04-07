namespace ServerV1
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
            this.bServerStop = new System.Windows.Forms.Button();
            this.bServerStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bServerStop
            // 
            this.bServerStop.Location = new System.Drawing.Point(160, 180);
            this.bServerStop.Name = "bServerStop";
            this.bServerStop.Size = new System.Drawing.Size(75, 23);
            this.bServerStop.TabIndex = 1;
            this.bServerStop.Text = "Стоп";
            this.bServerStop.UseVisualStyleBackColor = true;
            this.bServerStop.Click += new System.EventHandler(this.bServerStop_Click);
            // 
            // bServerStart
            // 
            this.bServerStart.Location = new System.Drawing.Point(37, 180);
            this.bServerStart.Name = "bServerStart";
            this.bServerStart.Size = new System.Drawing.Size(75, 23);
            this.bServerStart.TabIndex = 0;
            this.bServerStart.Text = "Старт";
            this.bServerStart.UseVisualStyleBackColor = true;
            this.bServerStart.Click += new System.EventHandler(this.bServerStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.bServerStop);
            this.Controls.Add(this.bServerStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bServerStop;
        private System.Windows.Forms.Button bServerStart;

    }
}

