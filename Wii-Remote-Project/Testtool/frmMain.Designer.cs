namespace Testtool
{
    partial class frmMain
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
            this.btnDiretionUp = new System.Windows.Forms.Button();
            this.btnDirectionDown = new System.Windows.Forms.Button();
            this.btnDirectionRight = new System.Windows.Forms.Button();
            this.btnDirectionLeft = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDiretionUp
            // 
            this.btnDiretionUp.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDiretionUp.Location = new System.Drawing.Point(242, 53);
            this.btnDiretionUp.Name = "btnDiretionUp";
            this.btnDiretionUp.Size = new System.Drawing.Size(40, 90);
            this.btnDiretionUp.TabIndex = 1;
            this.btnDiretionUp.UseVisualStyleBackColor = false;
            // 
            // btnDirectionDown
            // 
            this.btnDirectionDown.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDirectionDown.Location = new System.Drawing.Point(242, 177);
            this.btnDirectionDown.Name = "btnDirectionDown";
            this.btnDirectionDown.Size = new System.Drawing.Size(40, 90);
            this.btnDirectionDown.TabIndex = 2;
            this.btnDirectionDown.UseVisualStyleBackColor = false;
            // 
            // btnDirectionRight
            // 
            this.btnDirectionRight.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDirectionRight.Location = new System.Drawing.Point(279, 140);
            this.btnDirectionRight.Name = "btnDirectionRight";
            this.btnDirectionRight.Size = new System.Drawing.Size(90, 40);
            this.btnDirectionRight.TabIndex = 3;
            this.btnDirectionRight.UseVisualStyleBackColor = false;
            // 
            // btnDirectionLeft
            // 
            this.btnDirectionLeft.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDirectionLeft.Location = new System.Drawing.Point(154, 140);
            this.btnDirectionLeft.Name = "btnDirectionLeft";
            this.btnDirectionLeft.Size = new System.Drawing.Size(90, 40);
            this.btnDirectionLeft.TabIndex = 4;
            this.btnDirectionLeft.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(651, 487);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1208, 1391);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDirectionLeft);
            this.Controls.Add(this.btnDirectionRight);
            this.Controls.Add(this.btnDirectionDown);
            this.Controls.Add(this.btnDiretionUp);
            this.Name = "frmMain";
            this.Text = "Wii Remote Testtool";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDiretionUp;
        private System.Windows.Forms.Button btnDirectionDown;
        private System.Windows.Forms.Button btnDirectionRight;
        private System.Windows.Forms.Button btnDirectionLeft;
        private System.Windows.Forms.Button btnRefresh;
    }
}

