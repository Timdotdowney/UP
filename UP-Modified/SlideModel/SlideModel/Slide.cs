using Microsoft.Ink;
using System;
using System.Collections;
using System.Drawing;

namespace SlideModel
{
	[Serializable]
	public class Slide
	{
		public const int STUDENT_VIEW = 0;

		public const int INSTRUCTOR_VIEW = 1;

		public const float MINIMIZE_FACTOR = 0.66f;

		private string type;

		private string id;

		private int iter;

		private bool current;

		private double time;

		private string title;

		private string text;

		private bool dirty;

		private bool isMinimized;

		private bool wasEverMinimized;

		[NonSerialized]
		private ArrayList submissionList;

		[NonSerialized]
		private Ink ink = new Ink();

		private byte[] serializedInk;

		private ArrayList images;

		private object writeLock = new object();

		public ArrayList SubmissionList => submissionList;

		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
			}
		}

		public int Iter
		{
			get
			{
				return iter;
			}
			set
			{
				iter = value;
			}
		}

		public bool Current
		{
			get
			{
				return current;
			}
			set
			{
				if (current != value)
				{
					current = value;
				}
			}
		}

		public bool Minimized
		{
			get
			{
				return isMinimized;
			}
			set
			{
				isMinimized = value;
				if (value)
				{
					wasEverMinimized = true;
				}
			}
		}

		public bool WasEverMinimized => wasEverMinimized;

		public double Time
		{
			get
			{
				return time;
			}
			set
			{
				time = value;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public Image StudentImage
		{
			get
			{
				lock (writeLock)
				{
					if (images.Count >= 1 && images[0] != null)
					{
						return getImage(0);
					}
					return null;
				}
			}
			set
			{
				SetStudentImage(value);
			}
		}

		public Image InstructorImage
		{
			get
			{
				lock (writeLock)
				{
					if (images.Count >= 1)
					{
						if (images.Count >= 2)
						{
							if (images[1] != null)
							{
								return getImage(1);
							}
							if (images[0] != null)
							{
								return getImage(0);
							}
							return null;
						}
						if (images[0] != null)
						{
							return getImage(0);
						}
						return null;
					}
					return null;
				}
			}
			set
			{
				SetInstructorImage(value);
			}
		}

		public Ink Ink
		{
			get
			{
				if (ink == null)
				{
					ink = new Ink();
				}
				return ink;
			}
			set
			{
				if (value == null)
				{
					ink = new Ink();
				}
				else
				{
					ink = value;
				}
			}
		}

		public RemoteSlideAttrs getAttrs()
		{
			RemoteSlideAttrs remoteSlideAttrs = new RemoteSlideAttrs();
			remoteSlideAttrs.type = type;
			remoteSlideAttrs.id = id;
			remoteSlideAttrs.iter = iter;
			remoteSlideAttrs.minimized = isMinimized;
			return remoteSlideAttrs;
		}

		public void SetStudentImage(Image i)
		{
			lock (writeLock)
			{
				if (images.Count < 1)
				{
					images.Add(i);
				}
				else
				{
					images[0] = i;
				}
			}
		}

		public void SetInstructorImage(Image i)
		{
			lock (writeLock)
			{
				if (images.Count < 1)
				{
					images.Add(null);
				}
				if (images.Count < 2)
				{
					images.Add(i);
				}
				else
				{
					images[1] = i;
				}
			}
		}

		public Slide()
		{
			submissionList = new ArrayList();
			images = new ArrayList();
			isMinimized = false;
			wasEverMinimized = false;
		}

		public Slide(int id, string title, string text, Image studentImage, Image instructorImage)
		{
			this.id = id.ToString();
			type = "sl";
			iter = 0;
			current = true;
			time = (double)DateTime.Now.Ticks;
			this.title = title;
			this.text = text;
			images = new ArrayList();
			SetStudentImage(studentImage);
			SetInstructorImage(instructorImage);
			submissionList = null;
			ink = null;
			dirty = false;
			isMinimized = false;
			wasEverMinimized = false;
		}

		public void clearInk()
		{
			Ink = null;
		}

		public override string ToString()
		{
			string text = " ";
			if (current)
			{
				text = "+";
			}
			return text + Type + " #" + Id + " @ " + Iter;
		}

		public bool isSameSlide(Slide o)
		{
			return o != null && !(Type != o.Type) && !(Id != o.Id);
		}

		public bool isSameSlide(string type, string id)
		{
			return !(Type != type) && !(Id != id);
		}

		private Image getImage(int index)
		{
			return (Image)((Image)images[index]).Clone();
		}

		public bool AddSubmission(Submission newSub)
		{
			Submission submission = null;
			foreach (Submission submission2 in submissionList)
			{
				if (newSub.isSameSubmission(submission2))
				{
					submission = submission2;
				}
			}
			if (submission != null)
			{
				if (!newSub.isNewerThan(submission))
				{
					return false;
				}
				int index = submissionList.IndexOf(submission);
				submissionList.Remove(submission);
				submissionList.Insert(index, newSub);
				return true;
			}
			submissionList.Add(newSub);
			return true;
		}

		public int convertSizeForImage(int size)
		{
			if (!Minimized)
			{
				return size;
			}
			return (int)((float)size * 0.66f);
		}

		public float convertSizeForInk(float size)
		{
			if (!Minimized)
			{
				return size;
			}
			return size / 0.66f;
		}

		public float convertSizeForInkTransmission(float size)
		{
			if (!wasEverMinimized)
			{
				return size;
			}
			return size / 0.66f;
		}

		public void serializeInk()
		{
			serializedInk = ink.Save();
		}

		public void deserializeInk()
		{
			ink = new Ink();
			if (serializedInk != null)
			{
				ink.Load(serializedInk);
			}
			submissionList = new ArrayList();
		}
	}
}
