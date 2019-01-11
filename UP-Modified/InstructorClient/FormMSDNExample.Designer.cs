namespace InstructorClient
{
    partial class FormMSDNExample
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPolylineCusps = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBezierPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBezierCusps = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSelfIntersections = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.menuDisplay});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(800, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // menuDisplay
            // 
            this.menuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAllPoints,
            this.menuPolylineCusps,
            this.menuBezierPoints,
            this.menuBezierCusps,
            this.menuSelfIntersections});
            this.menuDisplay.Name = "menuDisplay";
            this.menuDisplay.Size = new System.Drawing.Size(57, 20);
            this.menuDisplay.Text = "Display";
            // 
            // menuAllPoints
            // 
            this.menuAllPoints.Name = "menuAllPoints";
            this.menuAllPoints.Size = new System.Drawing.Size(180, 22);
            this.menuAllPoints.Text = "All Points";
            this.menuAllPoints.Click += new System.EventHandler(this.menuDisplayItem_Click);
            // 
            // menuPolylineCusps
            // 
            this.menuPolylineCusps.Name = "menuPolylineCusps";
            this.menuPolylineCusps.Size = new System.Drawing.Size(180, 22);
            this.menuPolylineCusps.Text = "Polyline Cusps";
            this.menuPolylineCusps.Click += new System.EventHandler(this.menuDisplayItem_Click);
            // 
            // menuBezierPoints
            // 
            this.menuBezierPoints.Name = "menuBezierPoints";
            this.menuBezierPoints.Size = new System.Drawing.Size(180, 22);
            this.menuBezierPoints.Text = "Bezier Points";
            this.menuBezierPoints.Click += new System.EventHandler(this.menuDisplayItem_Click);
            // 
            // menuBezierCusps
            // 
            this.menuBezierCusps.Name = "menuBezierCusps";
            this.menuBezierCusps.Size = new System.Drawing.Size(180, 22);
            this.menuBezierCusps.Text = "Bezier Cusps";
            this.menuBezierCusps.Click += new System.EventHandler(this.menuDisplayItem_Click);
            // 
            // menuSelfIntersections
            // 
            this.menuSelfIntersections.Name = "menuSelfIntersections";
            this.menuSelfIntersections.Size = new System.Drawing.Size(180, 22);
            this.menuSelfIntersections.Text = "Self Intersections";
            this.menuSelfIntersections.Click += new System.EventHandler(this.menuDisplayItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClear,
            this.menuExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menuClear
            // 
            this.menuClear.Name = "menuClear";
            this.menuClear.Size = new System.Drawing.Size(180, 22);
            this.menuClear.Text = "Clear";
            this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(180, 22);
            this.menuExit.Text = "Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // FormMSDNExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FormMSDNExample";
            this.Text = "FormMSDNExample";
            this.Load += new System.EventHandler(this.Form_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Paint);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuDisplay;
        private System.Windows.Forms.ToolStripMenuItem menuAllPoints;
        private System.Windows.Forms.ToolStripMenuItem menuPolylineCusps;
        private System.Windows.Forms.ToolStripMenuItem menuBezierPoints;
        private System.Windows.Forms.ToolStripMenuItem menuBezierCusps;
        private System.Windows.Forms.ToolStripMenuItem menuSelfIntersections;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuClear;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
    }
}