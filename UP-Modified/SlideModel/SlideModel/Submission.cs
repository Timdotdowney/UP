using System;
using System.Drawing;

namespace SlideModel
{
	[Serializable]
	public class Submission
	{
		private object writeLock = new object();

		private Slide parentSlide;

		private EventHandler doubleClick;

		private Image image;

		private string id;

		private double time;

		private bool isBordered;

		public bool Bordered => isBordered;

		public EventHandler DoubleClick => doubleClick;

		public Slide ParentSlide => parentSlide;

		public Image Image
		{
			get
			{
				lock (writeLock)
				{
					return image;
				}
			}
			set
			{
				SetImage(value);
			}
		}

		public string Id => id;

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

		public RemoteSlideAttrs getAttrs()
		{
			RemoteSlideAttrs remoteSlideAttrs = new RemoteSlideAttrs();
			remoteSlideAttrs.type = "ss";
			remoteSlideAttrs.id = id;
			remoteSlideAttrs.iter = 0;
			return remoteSlideAttrs;
		}

		public void toggleBordered()
		{
			isBordered = !isBordered;
		}

		public void SetImage(Image i)
		{
			lock (writeLock)
			{
				image = i;
			}
		}

		public bool isSameSubmission(Submission s)
		{
			return id == s.Id;
		}

		public bool isNewerThan(Submission s)
		{
			return isSameSubmission(s) && time > s.Time;
		}

		public Submission(EventHandler e, Slide parent, Image image, string id, double t)
		{
			doubleClick = e;
			this.id = id;
			time = t;
			isBordered = false;
			this.image = image;
			parentSlide = parent;
		}

		public override string ToString()
		{
			return "Submission ID:" + Id + " on slide TYPE:" + parentSlide.Type + " ID:" + parentSlide.Id;
		}
	}
}
