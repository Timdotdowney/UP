using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InstructorClient
{
	public class DimensionChooser : Form
	{
		private int width;

		private int height;

		private int defaultWidth;

		private int defaultHeight;

		private IContainer components = null;

		private Label label1;

		private Label label2;

		private TextBox widthBox;

		private TextBox heightBox;

		private CheckBox preserveRatio;

		private Button okButton;

		private Button cancelButton;

		private Button defaultsButton;

		public int SelectedWidth => width;

		public int SelectedHeight => height;

		public DimensionChooser(string toChoose, int initialWidth, int initialHeight, int defaultWidth, int defaultHeight)
		{
			InitializeComponent();
			Text = "Set " + toChoose + " Size...";
			width = initialWidth;
			height = initialHeight;
			this.defaultWidth = defaultWidth;
			this.defaultHeight = defaultHeight;
			base.AcceptButton = okButton;
			base.Shown += DimensionChooser_Shown;
			restoreSizes();
		}

		private void DimensionChooser_Shown(object sender, EventArgs e)
		{
			widthBox.Focus();
		}

		private bool verifyInput()
		{
			int num = default(int);
			if (widthBox.Text.Length >= 1 && int.TryParse(widthBox.Text, out num) && num >= 1)
			{
				int num2 = default(int);
				if (heightBox.Text.Length >= 1 && int.TryParse(heightBox.Text, out num2) && num2 >= 1)
				{
					if (preserveRatio.Checked)
					{
						double num3 = (double)num / (double)width;
						width = num;
						height = (int)Math.Round((double)height * num3, 0, MidpointRounding.AwayFromZero);
					}
					else
					{
						width = num;
						height = num2;
					}
					restoreSizes();
					return true;
				}
				MessageBox.Show("You have entered an invalid height!", "Ubiquitous Presenter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return false;
			}
			MessageBox.Show("You have entered an invalid width!", "Ubiquitous Presenter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			return false;
		}

		private void restoreSizes()
		{
			widthBox.Text = width.ToString();
			heightBox.Text = height.ToString();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (verifyInput())
			{
				base.DialogResult = DialogResult.OK;
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			restoreSizes();
			base.DialogResult = DialogResult.Cancel;
		}

		private void defaultsButton_Click(object sender, EventArgs e)
		{
			width = defaultWidth;
			height = defaultHeight;
			restoreSizes();
			base.DialogResult = DialogResult.OK;
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
			widthBox = new System.Windows.Forms.TextBox();
			heightBox = new System.Windows.Forms.TextBox();
			preserveRatio = new System.Windows.Forms.CheckBox();
			okButton = new System.Windows.Forms.Button();
			cancelButton = new System.Windows.Forms.Button();
			defaultsButton = new System.Windows.Forms.Button();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(10, 7);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 0;
			label1.Text = "Width:";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(7, 32);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 13);
			label2.TabIndex = 1;
			label2.Text = "Height:";
			widthBox.Location = new System.Drawing.Point(54, 4);
			widthBox.Name = "widthBox";
			widthBox.Size = new System.Drawing.Size(88, 20);
			widthBox.TabIndex = 1;
			heightBox.Location = new System.Drawing.Point(54, 32);
			heightBox.Name = "heightBox";
			heightBox.Size = new System.Drawing.Size(88, 20);
			heightBox.TabIndex = 2;
			preserveRatio.AutoSize = true;
			preserveRatio.Checked = true;
			preserveRatio.CheckState = System.Windows.Forms.CheckState.Checked;
			preserveRatio.Location = new System.Drawing.Point(27, 58);
			preserveRatio.Name = "preserveRatio";
			preserveRatio.Size = new System.Drawing.Size(96, 17);
			preserveRatio.TabIndex = 3;
			preserveRatio.Text = "Preserve Ratio";
			preserveRatio.UseVisualStyleBackColor = true;
			okButton.Location = new System.Drawing.Point(8, 86);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(63, 23);
			okButton.TabIndex = 4;
			okButton.Text = "OK";
			okButton.UseVisualStyleBackColor = true;
			okButton.Click += new System.EventHandler(okButton_Click);
			cancelButton.Location = new System.Drawing.Point(79, 86);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(63, 23);
			cancelButton.TabIndex = 5;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += new System.EventHandler(cancelButton_Click);
			defaultsButton.Location = new System.Drawing.Point(148, 86);
			defaultsButton.Name = "defaultsButton";
			defaultsButton.Size = new System.Drawing.Size(94, 23);
			defaultsButton.TabIndex = 6;
			defaultsButton.Text = "Restore Defaults";
			defaultsButton.UseVisualStyleBackColor = true;
			defaultsButton.Click += new System.EventHandler(defaultsButton_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(251, 151);
			base.ControlBox = false;
			base.Controls.Add(defaultsButton);
			base.Controls.Add(cancelButton);
			base.Controls.Add(okButton);
			base.Controls.Add(preserveRatio);
			base.Controls.Add(heightBox);
			base.Controls.Add(widthBox);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DimensionChooser";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			Text = "Choose Dimensions...";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
