using Microsoft.Ink;
using SlideModel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace InstructorClient
{
	public class SlideListThumb : Panel
	{
		public static float ASPECT_RATIO = 1.33333325f;

		private Slide remote;

		private SlideList slideList;

		private bool selected;

		private object writeLock = new object();

		private static Pen curInstrSlidePen = new Pen(Color.Blue, 5f);

		private static Brush noImageBrush = Brushes.White;

		private SlidePreviewPanel previewPanel;

		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		public Slide Remote
		{
			get
			{
				lock (writeLock)
				{
					return remote;
				}
			}
			set
			{
				lock (writeLock)
				{
					remote = value;
				}
			}
		}

		public SlideListThumb(SlideList parent, Slide remote)
		{
			curInstrSlidePen.Alignment = PenAlignment.Inset;
			previewPanel = new SlidePreviewPanel(getImageMethod, getInkMethod, convertSizeMethod);
			base.Click += SlideListThumb_Click;
			base.MouseUp += SlideListThumb_MouseUp;
			base.MouseMove += SlideListThumb_MouseMove;
			Text = remote.ToString();
			this.remote = remote;
			slideList = parent;
			base.Resize += SlideListThumb_Resize;
			Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		private void SlideListThumb_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && !base.ClientRectangle.Contains(e.Location))
			{
				previewPanel.showPreview(this);
			}
		}

		private void SlideListThumb_MouseUp(object sender, MouseEventArgs e)
		{
			previewPanel.hidePreview();
		}

		private Image getImageMethod()
		{
			return remote.InstructorImage;
		}

		private Ink getInkMethod()
		{
			return remote.Ink;
		}

		private int convertSizeMethod(float original)
		{
			return (int)remote.convertSizeForInkTransmission(original);
		}

		private void SlideListThumb_Click(object sender, EventArgs e)
		{
			Selected = true;
			slideList.slideThumb_Click(this);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (remote.InstructorImage != null)
			{
				e.Graphics.DrawImage(remote.InstructorImage, 0, 0, base.Width, base.Height);
			}
			else
			{
				e.Graphics.FillRectangle(noImageBrush, 0, 0, base.Width, base.Height);
			}
			Renderer renderer = new Renderer();
			renderer.Scale((float)base.Width / 1024f, (float)base.Height / 786f);
			renderer.Draw(e.Graphics, remote.Ink.Strokes);
			if (selected)
			{
				e.Graphics.DrawRectangle(curInstrSlidePen, 0, 0, base.Width, base.Height);
			}
		}

		private void SlideListThumb_Resize(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			base.Width = control.Width;
			base.Height = (int)((double)base.Width * 0.75);
			Invalidate();
		}
	}
}
