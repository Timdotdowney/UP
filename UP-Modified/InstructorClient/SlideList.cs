using SlideModel;
using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace InstructorClient
{
	public class SlideList : Panel
	{
		public delegate void ChangeSlideDelegate(int slide);

		public delegate void RedoLayoutDelegate();

		private ArrayList slides;

		private Panel thumbColumn;

		private VScrollBar fakeScrollBar;

		private int selectedIndex;

		private int prevSelectedIndex;

		private bool prevWasShowingSubmissions;

		private UpdateMainSlideDelegate slideChangeDelegate;

		public bool PreviousSlideHadSubmissions
		{
			get
			{
				return prevWasShowingSubmissions;
			}
			set
			{
				prevWasShowingSubmissions = value;
			}
		}

		public ArrayList Slides => slides;

		public int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				ChangeSelected(value);
			}
		}

		public Slide SelectedSlide
		{
			get
			{
				if (slides != null)
				{
					if (slides.Count <= 0)
					{
						return null;
					}
					return (Slide)slides[selectedIndex];
				}
				return null;
			}
		}

		private SlideListThumb SelectedThumb
		{
			get
			{
				if (slides != null)
				{
					if (slides.Count <= 0)
					{
						return null;
					}
					return (SlideListThumb)thumbColumn.Controls[selectedIndex];
				}
				return null;
			}
		}

		public UpdateMainSlideDelegate SlideChangeDelegate
		{
			get
			{
				return slideChangeDelegate;
			}
			set
			{
				slideChangeDelegate = value;
			}
		}

		public SlideList()
		{
			slides = new ArrayList();
			InitializeComponent();
			thumbColumn = new Panel();
			thumbColumn.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
			thumbColumn.Width = base.Width - 17;
			thumbColumn.Height = 1000;
			thumbColumn.BackColor = Color.LightGray;
			base.Controls.Add(thumbColumn);
			fakeScrollBar = new VScrollBar();
			fakeScrollBar.Enabled = false;
			fakeScrollBar.Width = 17;
			base.Controls.Add(fakeScrollBar);
			base.Layout += SlideList_Layout;
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			ResumeLayout(false);
		}

		private void SlideList_Layout(object sender, EventArgs e)
		{
			thumbColumn.Width = base.Width - 19;
			thumbColumn.Height = (int)((double)thumbColumn.Width * 0.75 + 2.0) * slides.Count - 2;
			int num = 0;
			int num3 = (int)((double)thumbColumn.Width * 0.75);
			int num2 = 2;
			bool flag = false;
			foreach (Control control in thumbColumn.Controls)
			{
				control.Top = num;
				control.Left = 0;
				control.Height = num3;
				control.Width = thumbColumn.Width;
				num += num3 + num2;
			}
			int num4 = num;
			Rectangle clientRectangle = base.ClientRectangle;
			if (num4 >= clientRectangle.Height)
			{
				if (!AutoScroll)
				{
					flag = true;
				}
				fakeScrollBar.Left = 0;
				fakeScrollBar.Top = 0;
				fakeScrollBar.Width = 1;
				fakeScrollBar.Height = 1;
				fakeScrollBar.Hide();
				AutoScroll = true;
			}
			else
			{
				if (AutoScroll)
				{
					flag = true;
				}
				fakeScrollBar.Left = thumbColumn.Width + 2;
				fakeScrollBar.Width = 17;
				fakeScrollBar.Top = 0;
				VScrollBar vScrollBar = fakeScrollBar;
				clientRectangle = base.ClientRectangle;
				vScrollBar.Height = clientRectangle.Height;
				fakeScrollBar.Show();
				AutoScroll = false;
			}
			if (flag)
			{
				ResumeLayout();
				PerformLayout();
				SuspendLayout();
			}
		}

		public int CountType(string type)
		{
			int num = 0;
			foreach (Slide slide in slides)
			{
				if (slide.Type == type)
				{
					num++;
				}
			}
			return num;
		}

		public void gotoPreviousSlide()
		{
			ChangeSelected(prevSelectedIndex);
		}

		private void ChangeSelected(int newSel)
		{
			if (newSel < 0 || (newSel >= slides.Count && slides.Count > 0))
			{
				throw new ArgumentException("Index must be >= 0 and < number of items in list.");
			}
			if (selectedIndex < slides.Count)
			{
				SlideListThumb slideListThumb3 = (SlideListThumb)thumbColumn.Controls[selectedIndex];
				slideListThumb3.Selected = false;
			}
			if (slides.Count > 0)
			{
				SlideListThumb slideListThumb2 = (SlideListThumb)thumbColumn.Controls[newSel];
				slideListThumb2.Selected = true;
			}
			prevSelectedIndex = selectedIndex;
			selectedIndex = newSel;
			prevWasShowingSubmissions = false;
			if (slideChangeDelegate != null)
			{
				slideChangeDelegate(SelectedSlide, selectedIndex, slides.Count);
			}
			ScrollControlIntoView(SelectedThumb);
			Invalidate();
			//base.Parent.Invalidate();
		}

		public SlideListThumb CreateNewThumb(Slide s)
		{
			return new SlideListThumb(this, s);
		}

		public void setSlideList(ArrayList newSlides)
		{
			if (slides.Count > 0)
			{
				Clear();
			}
			SuspendLayout();
			foreach (Slide newSlide in newSlides)
			{
				slides.Add(newSlide);
				thumbColumn.Controls.Add(CreateNewThumb(newSlide));
			}
			ResumeLayout();
			PerformLayout();
			Invalidate();
			new Thread(changeSlideFromThread).Start();
		}

		private void changeSlideFromThread()
		{
			ChangeSlideDelegate method = ChangeSelected;
			Invoke(method, 0);
		}

		public void Insert(int index, Slide slide)
		{
			if (index < slides.Count)
			{
				slides.Insert(index, slide);
			}
			else
			{
				slides.Add(slide);
			}
			if (selectedIndex >= index)
			{
				selectedIndex++;
			}
			if (selectedIndex >= slides.Count)
			{
				selectedIndex = slides.Count - 1;
			}
			SlideListThumb t = CreateNewThumb(slide);
			Insert(index, t);
		}

		private void Insert(int index, SlideListThumb t)
		{
			SuspendLayout();
			thumbColumn.Controls.Add(t);
			if (index < thumbColumn.Controls.Count - 1)
			{
				thumbColumn.Controls.SetChildIndex(t, index);
			}
			ResumeLayout(false);
			PerformLayout();
			Invalidate();
			new Thread(threadRedoLayout).Start();
		}

		private void threadRedoLayout()
		{
			RedoLayoutDelegate method = redoLayout;
			Invoke(method);
		}

		private void redoLayout()
		{
			PerformLayout();
			Invalidate();
		}

		public string getNextAvailableId(string type)
		{
			int num = 0;
			foreach (Slide slide in slides)
			{
				num += ((slide.Type == type) ? 1 : 0);
			}
			return Convert.ToString(num);
		}

		public void Clear()
		{
			slides.Clear();
			thumbColumn.Controls.Clear();
			selectedIndex = 0;
		}

		public void slideThumb_Click(SlideListThumb thumb)
		{
			ChangeSelected(thumbColumn.Controls.GetChildIndex(thumb));
		}

		public void InvalidateCurrentThumbnail()
		{
			thumbColumn.Controls[selectedIndex].Invalidate();
		}
	}
}
