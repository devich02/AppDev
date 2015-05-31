namespace GameOfLife
{
    partial class Props
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
            this.numIterationDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numIterationDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // numIterationDelay
            // 
            this.numIterationDelay.Location = new System.Drawing.Point(93, 7);
            this.numIterationDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numIterationDelay.Name = "numIterationDelay";
            this.numIterationDelay.Size = new System.Drawing.Size(120, 20);
            this.numIterationDelay.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Iteration Delay";
            // 
            // Props
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numIterationDelay);
            this.Name = "Props";
            this.Text = "Props";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Props_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numIterationDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numIterationDelay;
        private System.Windows.Forms.Label label1;
    }
}