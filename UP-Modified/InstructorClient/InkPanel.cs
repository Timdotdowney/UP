using Microsoft.Ink;
using NetworkIO;
using SlideModel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace InstructorClient
{
	public class InkPanel : Panel
	{
		public const int UPLOAD_THRESHOLD_MS = 1000;

		public const float CANONICAL_WIDTH = 1024f;

		public const float CANONICAL_HEIGHT = 786f;

		public const float ASPECT_RATIO = 1.302799f;

		private const int DELAYED_PACKET_LIMIT = 15;

		public InkOverlay inkOverlay;

		public InkChangedDelegate inkChangedDelegate;

		private Slide slide;

		private InkOverlayEditingMode prevMode;

		public DateTime lastModTime = DateTime.Now;

		public DateTime lastUploadTime = DateTime.Now;

		public object uploadInkLock = new object();

		public static TimeSpan UPLOAD_THRESHOLD_TS = new TimeSpan(0, 0, 0, 0, 1000);

		public InkPanel otherPanel;

		private bool isPrimaryPanel;

		private int numDelayedPackets;

		public Slide Slide
		{
			get
			{
				return slide;
			}
			set
			{
				slide = value;
				if (value != null)
				{
					inkOverlay.Enabled = false;
					inkOverlay.Ink = value.Ink;
					inkOverlay.Enabled = true;
					base.Enabled = true;
				}
				else
				{
					base.Enabled = false;
				}
				Invalidate();
			}
		}

		public Ink SlideInk => inkOverlay.Ink;

		public bool InkEnabled
		{
			get
			{
				return inkOverlay.Enabled;
			}
			set
			{
				inkOverlay.Enabled = value;
			}
		}

		public InkOverlayEditingMode InkEditingMode
		{
			get
			{
				return inkOverlay.EditingMode;
			}
			set
			{
				prevMode = value;
				inkOverlay.EditingMode = value;
			}
		}

		public Color Color
		{
			get
			{
				return inkOverlay.DefaultDrawingAttributes.Color;
			}
			set
			{
				inkOverlay.DefaultDrawingAttributes.Color = value;
			}
		}

		public PenInfo PenAttributes
		{
			get
			{
				return new PenInfo(inkOverlay.DefaultDrawingAttributes.Width, inkOverlay.DefaultDrawingAttributes.Height, inkOverlay.DefaultDrawingAttributes.PenTip, inkOverlay.DefaultDrawingAttributes.Transparency);
			}
			set
			{
				inkOverlay.DefaultDrawingAttributes.Width = value.width;
				inkOverlay.DefaultDrawingAttributes.Height = value.height;
				inkOverlay.DefaultDrawingAttributes.PenTip = value.tip;
				inkOverlay.DefaultDrawingAttributes.Transparency = value.transparency;
			}
		}

		public InkPanel()
		{
			doSetup(true);
		}

		public InkPanel(bool isPrimary)
		{
			doSetup(isPrimary);
		}

		private void doSetup(bool isPrimary)
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			inkOverlay = new InkOverlay(base.Handle);
			inkOverlay.EraserMode = InkOverlayEraserMode.StrokeErase;
			prevMode = inkOverlay.EditingMode;
			inkOverlay.CursorInRange += inRangeHandler;
			base.Paint += paintHandler;
			inkOverlay.AutoRedraw = true;
			inkOverlay.Stroke += strokeHandler;
			inkOverlay.StrokesDeleting += strokesDeletingHandler;
			base.Resize += InkPanel_Resize;
			inkOverlay.NewPackets += inkPacketHandler;
			Matrix matrix = new Matrix();
			matrix.Scale(1f, 1f, MatrixOrder.Append);
			inkOverlay.Renderer.SetViewTransform(matrix);
			base.Enabled = true;
			isPrimaryPanel = isPrimary;
		}

		private void paintHandler(object sender, PaintEventArgs e)
		{
			if (slide != null)
			{
				if (isPrimaryPanel)
				{
					if (slide.InstructorImage != null)
					{
						e.Graphics.DrawImage(slide.InstructorImage, new Rectangle(0, 0, slide.convertSizeForImage(base.Width), slide.convertSizeForImage(base.Height)));
					}
				}
				else if (slide.StudentImage != null)
				{
					e.Graphics.DrawImage(slide.StudentImage, new Rectangle(0, 0, slide.convertSizeForImage(base.Width), slide.convertSizeForImage(base.Height)));
				}
				if (slide.Ink != null)
				{
					inkOverlay.Renderer.Draw(e.Graphics, Slide.Ink.Strokes);
				}
			}
		}

		private void strokeHandler(object sender, InkCollectorStrokeEventArgs e)
		{
			if (base.Enabled)
			{
				Monitor.Enter(this);
				try
				{
					Slide.Dirty = true;
					if (Slide.Ink != null)
					{
						Slide.Ink.Strokes.Add(e.Stroke);
					}
					inkChangedDelegate();
					if (otherPanel != null)
					{
						otherPanel.Invalidate();
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		protected virtual void inkPacketHandler(object sender, InkCollectorNewPacketsEventArgs args)
		{
			if (otherPanel != null && numDelayedPackets++ >= 15)
			{
				otherPanel.Invalidate();
				numDelayedPackets = 0;
			}
		}

		private void strokesDeletingHandler(object sender, InkOverlayStrokesDeletingEventArgs e)
		{
			Monitor.Enter(this);
			try
			{
				if (Slide.Ink != null)
				{
					Strokes.StrokesEnumerator enumerator = e.StrokesToDelete.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Stroke current = enumerator.Current;
							if (!Slide.Ink.Strokes.Contains(current))
							{
								e.StrokesToDelete.Remove(current);
							}
						}
					}
					finally
					{
						(enumerator as IDisposable)?.Dispose();
					}
				}
				lastModTime = DateTime.Now;
				if (otherPanel != null)
				{
					otherPanel.Invalidate();
				}
				inkChangedDelegate();
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private void inRangeHandler(object sender, InkCollectorCursorInRangeEventArgs e)
		{
			if (e.Cursor.Inverted)
			{
				if (inkOverlay.EditingMode != InkOverlayEditingMode.Delete)
				{
					prevMode = inkOverlay.EditingMode;
					inkOverlay.EditingMode = InkOverlayEditingMode.Delete;
				}
			}
			else if (inkOverlay.EditingMode == InkOverlayEditingMode.Delete && prevMode != InkOverlayEditingMode.Delete)
			{
				inkOverlay.EditingMode = prevMode;
			}
		}

		public void Clear()
		{
			if (Slide != null && Slide.Ink != null)
			{
				Slide.Ink.DeleteStrokes();
			}
			Slide = null;
		}

		private void InkPanel_Resize(object sender, EventArgs e)
		{
			ScaleInk();
		}

		public void ScaleInk()
		{
			if (base.Width >= 5 || base.Height >= 5)
			{
				Matrix matrix = new Matrix();
				float scaleX = (float)base.Width / ((slide != null) ? slide.convertSizeForInk(1024f) : 1024f);
				float scaleY = (float)base.Height / ((slide != null) ? slide.convertSizeForInk(786f) : 786f);
				float offsetX = 0f;
				float offsetY = 0f;
				if (Slide != null)
				{
					inkOverlay.Renderer.SetViewTransform(new Matrix());
				}
				matrix.Scale(scaleX, scaleY, MatrixOrder.Append);
				matrix.Translate(offsetX, offsetY, MatrixOrder.Append);
				inkOverlay.Renderer.SetViewTransform(matrix);
				Invalidate();
			}
		}

		public void ClearInk(WebService.SyncMode mode)
		{
			lastModTime = DateTime.Now;
			if (Slide.Ink != null)
			{
				Slide.Ink.DeleteStrokes();
			}
			Slide.Dirty = true;
			Invalidate();
			inkChangedDelegate();
		}
	}
}
