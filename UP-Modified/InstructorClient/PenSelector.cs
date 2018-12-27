using Microsoft.Ink;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InstructorClient
{
	public class PenSelector : Form
	{
		private IContainer components = null;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Panel shapePanel;

		private RadioButton radioRectanglePen;

		private RadioButton radioBallPen;

		private TrackBar widthSelector;

		private TrackBar heightSelector;

		private Button buttonOK;

		private Button buttonCancel;

		private TrackBar transparencySelector;

		private Label label5;

		private Button restoreDefaultButton;

		private int penIndex;

		public PenInfo NewPen => new PenInfo(PenWidth, PenHeight, PenShape, PenTransparency);

		public float PenWidth => (float)widthSelector.Value;

		public float PenHeight => (float)heightSelector.Value;

		public byte PenTransparency => (byte)transparencySelector.Value;

		public PenTip PenShape
		{
			get
			{
				if (!radioBallPen.Checked)
				{
					if (!radioRectanglePen.Checked)
					{
						return PenTip.Ball;
					}
					return PenTip.Rectangle;
				}
				return PenTip.Ball;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			shapePanel = new System.Windows.Forms.Panel();
			radioRectanglePen = new System.Windows.Forms.RadioButton();
			radioBallPen = new System.Windows.Forms.RadioButton();
			widthSelector = new System.Windows.Forms.TrackBar();
			heightSelector = new System.Windows.Forms.TrackBar();
			buttonOK = new System.Windows.Forms.Button();
			buttonCancel = new System.Windows.Forms.Button();
			transparencySelector = new System.Windows.Forms.TrackBar();
			label5 = new System.Windows.Forms.Label();
			restoreDefaultButton = new System.Windows.Forms.Button();
			shapePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)widthSelector).BeginInit();
			((System.ComponentModel.ISupportInitialize)heightSelector).BeginInit();
			((System.ComponentModel.ISupportInitialize)transparencySelector).BeginInit();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(9, 10);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(262, 13);
			label1.TabIndex = 0;
			label1.Text = "Please customize your pen using the properties below:";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 34);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(60, 13);
			label2.TabIndex = 1;
			label2.Text = "Pen Width:";
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 94);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(63, 13);
			label3.TabIndex = 2;
			label3.Text = "Pen Height:";
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(9, 206);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(63, 13);
			label4.TabIndex = 3;
			label4.Text = "Pen Shape:";
			shapePanel.Controls.Add(radioRectanglePen);
			shapePanel.Controls.Add(radioBallPen);
			shapePanel.Location = new System.Drawing.Point(78, 206);
			shapePanel.Name = "shapePanel";
			shapePanel.Size = new System.Drawing.Size(162, 26);
			shapePanel.TabIndex = 4;
			radioRectanglePen.AutoSize = true;
			radioRectanglePen.Location = new System.Drawing.Point(75, 4);
			radioRectanglePen.Name = "radioRectanglePen";
			radioRectanglePen.Size = new System.Drawing.Size(83, 17);
			radioRectanglePen.TabIndex = 1;
			radioRectanglePen.Text = "Rectangular";
			radioRectanglePen.UseVisualStyleBackColor = true;
			radioBallPen.AutoSize = true;
			radioBallPen.Checked = true;
			radioBallPen.Location = new System.Drawing.Point(3, 6);
			radioBallPen.Name = "radioBallPen";
			radioBallPen.Size = new System.Drawing.Size(57, 17);
			radioBallPen.TabIndex = 0;
			radioBallPen.TabStop = true;
			radioBallPen.Text = "Round";
			radioBallPen.UseVisualStyleBackColor = true;
			widthSelector.Location = new System.Drawing.Point(85, 34);
			widthSelector.Maximum = 2000;
			widthSelector.Name = "widthSelector";
			widthSelector.Size = new System.Drawing.Size(299, 45);
			widthSelector.TabIndex = 5;
			widthSelector.TickFrequency = 50;
			heightSelector.Location = new System.Drawing.Point(85, 94);
			heightSelector.Maximum = 2000;
			heightSelector.Name = "heightSelector";
			heightSelector.Size = new System.Drawing.Size(299, 45);
			heightSelector.TabIndex = 6;
			heightSelector.TickFrequency = 50;
			buttonOK.Location = new System.Drawing.Point(12, 238);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new System.Drawing.Size(124, 34);
			buttonOK.TabIndex = 7;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += new System.EventHandler(buttonOK_Click);
			buttonCancel.Location = new System.Drawing.Point(157, 238);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(124, 34);
			buttonCancel.TabIndex = 8;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
			transparencySelector.Location = new System.Drawing.Point(85, 155);
			transparencySelector.Maximum = 255;
			transparencySelector.Name = "transparencySelector";
			transparencySelector.Size = new System.Drawing.Size(299, 45);
			transparencySelector.TabIndex = 11;
			transparencySelector.TickFrequency = 5;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(9, 155);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(75, 13);
			label5.TabIndex = 12;
			label5.Text = "Transparency:";
			restoreDefaultButton.Location = new System.Drawing.Point(297, 238);
			restoreDefaultButton.Name = "restoreDefaultButton";
			restoreDefaultButton.Size = new System.Drawing.Size(124, 34);
			restoreDefaultButton.TabIndex = 13;
			restoreDefaultButton.Text = "Restore Defaults";
			restoreDefaultButton.UseVisualStyleBackColor = true;
			restoreDefaultButton.Click += new System.EventHandler(restoreDefaultButton_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(433, 283);
			base.Controls.Add(restoreDefaultButton);
			base.Controls.Add(label5);
			base.Controls.Add(transparencySelector);
			base.Controls.Add(buttonCancel);
			base.Controls.Add(buttonOK);
			base.Controls.Add(heightSelector);
			base.Controls.Add(widthSelector);
			base.Controls.Add(shapePanel);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PenSelector";
			base.ShowInTaskbar = false;
			Text = "Customize Pens";
			shapePanel.ResumeLayout(false);
			shapePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)widthSelector).EndInit();
			((System.ComponentModel.ISupportInitialize)heightSelector).EndInit();
			((System.ComponentModel.ISupportInitialize)transparencySelector).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		public PenSelector(PenInfo pi, int index)
		{
			InitializeComponent();
			setControls(pi);
			penIndex = index;
		}

		private void setControls(PenInfo pi)
		{
			try
			{
				widthSelector.Value = (int)pi.width;
			}
			catch (Exception)
			{
			}
			try
			{
				heightSelector.Value = (int)pi.height;
			}
			catch (Exception)
			{
			}
			try
			{
				transparencySelector.Value = pi.transparency;
			}
			catch (Exception)
			{
			}
			if (pi.tip == PenTip.Ball)
			{
				radioBallPen.Checked = true;
				radioRectanglePen.Checked = false;
			}
			else if (pi.tip == PenTip.Rectangle)
			{
				radioBallPen.Checked = false;
				radioRectanglePen.Checked = true;
			}
			else
			{
				radioBallPen.Checked = true;
				radioRectanglePen.Checked = false;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void restoreDefaultButton_Click(object sender, EventArgs e)
		{
			setControls(CustomPenAttributes.DEFAULT_PENS[penIndex]);
		}
	}
}
