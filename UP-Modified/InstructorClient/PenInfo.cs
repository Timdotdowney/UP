using Microsoft.Ink;

namespace InstructorClient
{
	public class PenInfo
	{
		public float width;

		public float height;

		public PenTip tip;

		public byte transparency;

		public PenInfo(float w, float h, PenTip t, byte tp)
		{
			width = w;
			height = h;
			tip = t;
			transparency = tp;
		}
	}
}
