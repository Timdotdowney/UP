namespace InstructorClient
{
    partial class FormReadStrokes
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
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.panelNav = new System.Windows.Forms.Panel();
            this.panelDraw = new System.Windows.Forms.Panel();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelIndex = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBoxScaleX = new System.Windows.Forms.TextBox();
            this.textBoxScaleY = new System.Windows.Forms.TextBox();
            this.labelScaleX = new System.Windows.Forms.Label();
            this.labelScaleY = new System.Windows.Forms.Label();
            this.buttonRescale = new System.Windows.Forms.Button();
            this.panelNav.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPrev
            // 
            this.buttonPrev.Location = new System.Drawing.Point(20, 12);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(75, 23);
            this.buttonPrev.TabIndex = 0;
            this.buttonPrev.Text = "Prev";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(101, 12);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 1;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // panelNav
            // 
            this.panelNav.Controls.Add(this.buttonRescale);
            this.panelNav.Controls.Add(this.labelScaleY);
            this.panelNav.Controls.Add(this.labelScaleX);
            this.panelNav.Controls.Add(this.textBoxScaleY);
            this.panelNav.Controls.Add(this.textBoxScaleX);
            this.panelNav.Controls.Add(this.buttonNext);
            this.panelNav.Controls.Add(this.buttonPrev);
            this.panelNav.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelNav.Location = new System.Drawing.Point(0, 0);
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(742, 44);
            this.panelNav.TabIndex = 2;
            // 
            // panelDraw
            // 
            this.panelDraw.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelDraw.Location = new System.Drawing.Point(0, 44);
            this.panelDraw.Name = "panelDraw";
            this.panelDraw.Size = new System.Drawing.Size(720, 540);
            this.panelDraw.TabIndex = 3;
            this.panelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDraw_Paint);
            this.panelDraw.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseDown);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLocation,
            this.toolStripStatusLabelIndex});
            this.statusStripMain.Location = new System.Drawing.Point(0, 601);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(742, 22);
            this.statusStripMain.TabIndex = 0;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLocation
            // 
            this.toolStripStatusLabelLocation.Name = "toolStripStatusLabelLocation";
            this.toolStripStatusLabelLocation.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelIndex
            // 
            this.toolStripStatusLabelIndex.Name = "toolStripStatusLabelIndex";
            this.toolStripStatusLabelIndex.Size = new System.Drawing.Size(0, 17);
            // 
            // textBoxScaleX
            // 
            this.textBoxScaleX.Location = new System.Drawing.Point(244, 13);
            this.textBoxScaleX.Name = "textBoxScaleX";
            this.textBoxScaleX.Size = new System.Drawing.Size(63, 20);
            this.textBoxScaleX.TabIndex = 2;
            this.textBoxScaleX.Text = "nan";
            // 
            // textBoxScaleY
            // 
            this.textBoxScaleY.Location = new System.Drawing.Point(376, 14);
            this.textBoxScaleY.Name = "textBoxScaleY";
            this.textBoxScaleY.Size = new System.Drawing.Size(63, 20);
            this.textBoxScaleY.TabIndex = 3;
            this.textBoxScaleY.Text = "nan";
            // 
            // labelScaleX
            // 
            this.labelScaleX.AutoSize = true;
            this.labelScaleX.Location = new System.Drawing.Point(194, 17);
            this.labelScaleX.Name = "labelScaleX";
            this.labelScaleX.Size = new System.Drawing.Size(44, 13);
            this.labelScaleX.TabIndex = 4;
            this.labelScaleX.Text = "Scale X";
            // 
            // labelScaleY
            // 
            this.labelScaleY.AutoSize = true;
            this.labelScaleY.Location = new System.Drawing.Point(326, 17);
            this.labelScaleY.Name = "labelScaleY";
            this.labelScaleY.Size = new System.Drawing.Size(44, 13);
            this.labelScaleY.TabIndex = 5;
            this.labelScaleY.Text = "Scale Y";
            // 
            // buttonRescale
            // 
            this.buttonRescale.Location = new System.Drawing.Point(457, 12);
            this.buttonRescale.Name = "buttonRescale";
            this.buttonRescale.Size = new System.Drawing.Size(75, 23);
            this.buttonRescale.TabIndex = 6;
            this.buttonRescale.Text = "Rescale";
            this.buttonRescale.UseVisualStyleBackColor = true;
            this.buttonRescale.Click += new System.EventHandler(this.Rescale);
            // 
            // FormReadStrokes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(742, 623);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.panelDraw);
            this.Controls.Add(this.panelNav);
            this.Name = "FormReadStrokes";
            this.Text = "FormReadStrokes";
            this.Load += new System.EventHandler(this.FormReadStrokes_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormReadStrokes_Paint);
            this.panelNav.ResumeLayout(false);
            this.panelNav.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Panel panelNav;
        private System.Windows.Forms.Panel panelDraw;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLocation;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelIndex;
        private System.Windows.Forms.TextBox textBoxScaleX;
        private System.Windows.Forms.Label labelScaleY;
        private System.Windows.Forms.Label labelScaleX;
        private System.Windows.Forms.TextBox textBoxScaleY;
        private System.Windows.Forms.Button buttonRescale;
    }
}