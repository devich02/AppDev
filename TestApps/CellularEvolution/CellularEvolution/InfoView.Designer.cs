namespace CellularEvolution
{
    partial class InfoView
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboPainterType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCellLocation = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblAgentInfo = new System.Windows.Forms.Label();
            this.lblResourceMax = new System.Windows.Forms.Label();
            this.lblReplenishRate = new System.Windows.Forms.Label();
            this.lblResourceCount = new System.Windows.Forms.Label();
            this.lblCellType = new System.Windows.Forms.Label();
            this.lblBrushSize = new System.Windows.Forms.Label();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.chkRandomAgents = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Painter";
            // 
            // comboPainterType
            // 
            this.comboPainterType.FormattingEnabled = true;
            this.comboPainterType.Items.AddRange(new object[] {
            "Grass",
            "Water",
            "Food",
            "Mud",
            "Rock"});
            this.comboPainterType.Location = new System.Drawing.Point(124, 6);
            this.comboPainterType.Name = "comboPainterType";
            this.comboPainterType.Size = new System.Drawing.Size(341, 39);
            this.comboPainterType.TabIndex = 1;
            this.comboPainterType.Text = "Grass";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCellLocation);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lblResourceMax);
            this.groupBox1.Controls.Add(this.lblReplenishRate);
            this.groupBox1.Controls.Add(this.lblResourceCount);
            this.groupBox1.Controls.Add(this.lblCellType);
            this.groupBox1.Location = new System.Drawing.Point(18, 194);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 788);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cell";
            // 
            // lblCellLocation
            // 
            this.lblCellLocation.AutoSize = true;
            this.lblCellLocation.Location = new System.Drawing.Point(6, 50);
            this.lblCellLocation.Name = "lblCellLocation";
            this.lblCellLocation.Size = new System.Drawing.Size(132, 32);
            this.lblCellLocation.TabIndex = 5;
            this.lblCellLocation.Text = "Location:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblAgentInfo);
            this.groupBox2.Location = new System.Drawing.Point(11, 293);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1028, 484);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Agent";
            // 
            // lblAgentInfo
            // 
            this.lblAgentInfo.AutoSize = true;
            this.lblAgentInfo.Location = new System.Drawing.Point(6, 61);
            this.lblAgentInfo.Name = "lblAgentInfo";
            this.lblAgentInfo.Size = new System.Drawing.Size(0, 32);
            this.lblAgentInfo.TabIndex = 0;
            // 
            // lblResourceMax
            // 
            this.lblResourceMax.AutoSize = true;
            this.lblResourceMax.Location = new System.Drawing.Point(6, 258);
            this.lblResourceMax.Name = "lblResourceMax";
            this.lblResourceMax.Size = new System.Drawing.Size(338, 32);
            this.lblResourceMax.TabIndex = 3;
            this.lblResourceMax.Text = "Local resource maximum:";
            // 
            // lblReplenishRate
            // 
            this.lblReplenishRate.AutoSize = true;
            this.lblReplenishRate.Location = new System.Drawing.Point(5, 212);
            this.lblReplenishRate.Name = "lblReplenishRate";
            this.lblReplenishRate.Size = new System.Drawing.Size(210, 32);
            this.lblReplenishRate.TabIndex = 2;
            this.lblReplenishRate.Text = "Replenish Rate";
            // 
            // lblResourceCount
            // 
            this.lblResourceCount.AutoSize = true;
            this.lblResourceCount.Location = new System.Drawing.Point(6, 162);
            this.lblResourceCount.Name = "lblResourceCount";
            this.lblResourceCount.Size = new System.Drawing.Size(227, 32);
            this.lblResourceCount.TabIndex = 1;
            this.lblResourceCount.Text = "Resource Count:";
            // 
            // lblCellType
            // 
            this.lblCellType.AutoSize = true;
            this.lblCellType.Location = new System.Drawing.Point(6, 110);
            this.lblCellType.Name = "lblCellType";
            this.lblCellType.Size = new System.Drawing.Size(93, 32);
            this.lblCellType.TabIndex = 0;
            this.lblCellType.Text = "Type: ";
            // 
            // lblBrushSize
            // 
            this.lblBrushSize.AutoSize = true;
            this.lblBrushSize.Location = new System.Drawing.Point(471, 9);
            this.lblBrushSize.Name = "lblBrushSize";
            this.lblBrushSize.Size = new System.Drawing.Size(155, 32);
            this.lblBrushSize.TabIndex = 3;
            this.lblBrushSize.Text = "Brush size:";
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Location = new System.Drawing.Point(18, 66);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(202, 67);
            this.btnSaveMap.TabIndex = 4;
            this.btnSaveMap.Text = "Save Map";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.btnSaveMap_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Location = new System.Drawing.Point(226, 66);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(183, 67);
            this.btnLoadMap.TabIndex = 5;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Evolutionary Map Files (*.evmap) | *.evmap";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Evolutionary Map Files (*.evmap) | *.evmap";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(364, 32);
            this.label2.TabIndex = 6;
            this.label2.Text = "Iterations per visual update:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(388, 150);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(304, 38);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkRandomAgents
            // 
            this.chkRandomAgents.AutoSize = true;
            this.chkRandomAgents.Checked = true;
            this.chkRandomAgents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRandomAgents.Location = new System.Drawing.Point(430, 82);
            this.chkRandomAgents.Name = "chkRandomAgents";
            this.chkRandomAgents.Size = new System.Drawing.Size(319, 36);
            this.chkRandomAgents.TabIndex = 8;
            this.chkRandomAgents.Text = "Insert random agents";
            this.chkRandomAgents.UseVisualStyleBackColor = true;
            // 
            // InfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 994);
            this.Controls.Add(this.chkRandomAgents);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLoadMap);
            this.Controls.Add(this.btnSaveMap);
            this.Controls.Add(this.lblBrushSize);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboPainterType);
            this.Controls.Add(this.label1);
            this.Name = "InfoView";
            this.Text = "InfoView";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboPainterType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblAgentInfo;
        private System.Windows.Forms.Label lblResourceMax;
        private System.Windows.Forms.Label lblReplenishRate;
        private System.Windows.Forms.Label lblResourceCount;
        private System.Windows.Forms.Label lblCellType;
        private System.Windows.Forms.Label lblCellLocation;
        private System.Windows.Forms.Label lblBrushSize;
        private System.Windows.Forms.Button btnSaveMap;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox chkRandomAgents;
    }
}