using SlideModel;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InstructorClient
{
	public class SubmissionPreviewPanel : FlowLayoutPanel
	{
		private delegate void UpdateSubmissionListDelegate(ArrayList value);

		private IContainer components = null;

		public ArrayList SubmissionThumbList
		{
			set
			{
				UpdateSubmissionListDelegate method = _UpdateSubmissionList;
				Invoke(method, value);
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
			SuspendLayout();
			ResumeLayout(false);
		}

		public void changeThumbnailSize()
		{
			SubmissionThumb.changeThumbnailSize();
			foreach (Control control in base.Controls)
			{
				control.Width = SubmissionThumb.SubWidth;
				control.Height = SubmissionThumb.SubHeight;
			}
			Invalidate(true);
		}

		private void _UpdateSubmissionList(ArrayList value)
		{
			SuspendLayout();
			base.Controls.Clear();
			Control[] array = new Control[value.Count];
			for (int i = 0; i < value.Count; i++)
			{
				Submission sub = (Submission)value[i];
				array[i] = new SubmissionThumb(sub);
			}
			base.Controls.AddRange(array);
			ResumeLayout();
		}

		public SubmissionPreviewPanel()
		{
			InitializeComponent();
			AutoScroll = true;
			base.FlowDirection = FlowDirection.LeftToRight;
		}

		protected override Point ScrollToControl(Control activeControl)
		{
			return base.AutoScrollPosition;
		}

		internal void ActivateSubmission(SubmissionThumb submission)
		{
		}
	}
}
