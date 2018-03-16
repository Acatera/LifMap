namespace LiFMap
{
    partial class WorldMapForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.txtMinQuality = new System.Windows.Forms.TextBox();
            this.lblMinQuality = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cbSubstances = new System.Windows.Forms.ComboBox();
            this.lblMaterialId = new System.Windows.Forms.Label();
            this.cbFollowPlayer = new System.Windows.Forms.CheckBox();
            this.btnCenterPlayer = new System.Windows.Forms.Button();
            this.lbOutput = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnShowPlayerPos = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRenderWorld = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.numElevation = new System.Windows.Forms.NumericUpDown();
            this.numDepth = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numElevation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepth)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbSearch);
            this.panel1.Controls.Add(this.cbFollowPlayer);
            this.panel1.Controls.Add(this.btnCenterPlayer);
            this.panel1.Controls.Add(this.lbOutput);
            this.panel1.Controls.Add(this.numDepth);
            this.panel1.Controls.Add(this.numElevation);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnShowPlayerPos);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnRenderWorld);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(562, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(207, 526);
            this.panel1.TabIndex = 2;
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.txtMinQuality);
            this.gbSearch.Controls.Add(this.lblMinQuality);
            this.gbSearch.Controls.Add(this.btnSearch);
            this.gbSearch.Controls.Add(this.cbSubstances);
            this.gbSearch.Controls.Add(this.lblMaterialId);
            this.gbSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSearch.Location = new System.Drawing.Point(4, 163);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(199, 121);
            this.gbSearch.TabIndex = 17;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search:";
            // 
            // txtMinQuality
            // 
            this.txtMinQuality.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMinQuality.Location = new System.Drawing.Point(3, 63);
            this.txtMinQuality.Name = "txtMinQuality";
            this.txtMinQuality.Size = new System.Drawing.Size(193, 20);
            this.txtMinQuality.TabIndex = 4;
            this.txtMinQuality.Text = "80";
            // 
            // lblMinQuality
            // 
            this.lblMinQuality.AutoSize = true;
            this.lblMinQuality.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMinQuality.Location = new System.Drawing.Point(3, 50);
            this.lblMinQuality.Name = "lblMinQuality";
            this.lblMinQuality.Size = new System.Drawing.Size(63, 13);
            this.lblMinQuality.TabIndex = 3;
            this.lblMinQuality.Text = "Min. quality:";
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSearch.Location = new System.Drawing.Point(3, 95);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(193, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cbSubstances
            // 
            this.cbSubstances.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbSubstances.FormattingEnabled = true;
            this.cbSubstances.Location = new System.Drawing.Point(3, 29);
            this.cbSubstances.Name = "cbSubstances";
            this.cbSubstances.Size = new System.Drawing.Size(193, 21);
            this.cbSubstances.TabIndex = 5;
            // 
            // lblMaterialId
            // 
            this.lblMaterialId.AutoSize = true;
            this.lblMaterialId.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMaterialId.Location = new System.Drawing.Point(3, 16);
            this.lblMaterialId.Name = "lblMaterialId";
            this.lblMaterialId.Size = new System.Drawing.Size(61, 13);
            this.lblMaterialId.TabIndex = 1;
            this.lblMaterialId.Text = "Material ID:";
            // 
            // cbFollowPlayer
            // 
            this.cbFollowPlayer.AutoSize = true;
            this.cbFollowPlayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbFollowPlayer.Location = new System.Drawing.Point(4, 126);
            this.cbFollowPlayer.Name = "cbFollowPlayer";
            this.cbFollowPlayer.Size = new System.Drawing.Size(199, 17);
            this.cbFollowPlayer.TabIndex = 15;
            this.cbFollowPlayer.Text = "Follow player";
            this.cbFollowPlayer.UseVisualStyleBackColor = true;
            // 
            // btnCenterPlayer
            // 
            this.btnCenterPlayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCenterPlayer.Location = new System.Drawing.Point(4, 103);
            this.btnCenterPlayer.Name = "btnCenterPlayer";
            this.btnCenterPlayer.Size = new System.Drawing.Size(199, 23);
            this.btnCenterPlayer.TabIndex = 14;
            this.btnCenterPlayer.Text = "Center player";
            this.btnCenterPlayer.UseVisualStyleBackColor = true;
            // 
            // lbOutput
            // 
            this.lbOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbOutput.FormattingEnabled = true;
            this.lbOutput.Location = new System.Drawing.Point(4, 284);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(199, 186);
            this.lbOutput.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(4, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Location = new System.Drawing.Point(4, 470);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.Location = new System.Drawing.Point(4, 483);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "label3";
            // 
            // btnShowPlayerPos
            // 
            this.btnShowPlayerPos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnShowPlayerPos.Location = new System.Drawing.Point(4, 27);
            this.btnShowPlayerPos.Name = "btnShowPlayerPos";
            this.btnShowPlayerPos.Size = new System.Drawing.Size(199, 23);
            this.btnShowPlayerPos.TabIndex = 5;
            this.btnShowPlayerPos.Text = "Show player";
            this.btnShowPlayerPos.UseVisualStyleBackColor = true;
            this.btnShowPlayerPos.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(4, 496);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(4, 509);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // btnRenderWorld
            // 
            this.btnRenderWorld.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRenderWorld.Location = new System.Drawing.Point(4, 4);
            this.btnRenderWorld.Name = "btnRenderWorld";
            this.btnRenderWorld.Size = new System.Drawing.Size(199, 23);
            this.btnRenderWorld.TabIndex = 1;
            this.btnRenderWorld.Text = "Render";
            this.btnRenderWorld.UseVisualStyleBackColor = true;
            this.btnRenderWorld.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(562, 526);
            this.panel2.TabIndex = 3;
            // 
            // numElevation
            // 
            this.numElevation.Dock = System.Windows.Forms.DockStyle.Top;
            this.numElevation.Location = new System.Drawing.Point(4, 63);
            this.numElevation.Maximum = new decimal(new int[] {
            7000,
            0,
            0,
            0});
            this.numElevation.Name = "numElevation";
            this.numElevation.Size = new System.Drawing.Size(199, 20);
            this.numElevation.TabIndex = 18;
            this.numElevation.Value = new decimal(new int[] {
            5100,
            0,
            0,
            0});
            // 
            // numDepth
            // 
            this.numDepth.Dock = System.Windows.Forms.DockStyle.Top;
            this.numDepth.Location = new System.Drawing.Point(4, 83);
            this.numDepth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numDepth.Name = "numDepth";
            this.numDepth.Size = new System.Drawing.Size(199, 20);
            this.numDepth.TabIndex = 19;
            this.numDepth.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // WorldMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 526);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "WorldMapForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorldMapForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numElevation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        //private PictureBoxInterpolatedZoom pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRenderWorld;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnShowPlayerPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbOutput;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCenterPlayer;
        private System.Windows.Forms.CheckBox cbFollowPlayer;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblMaterialId;
        private System.Windows.Forms.TextBox txtMinQuality;
        private System.Windows.Forms.Label lblMinQuality;
        private System.Windows.Forms.ComboBox cbSubstances;
        private System.Windows.Forms.NumericUpDown numDepth;
        private System.Windows.Forms.NumericUpDown numElevation;
    }
}

