using Microsoft.Ink;
using NetworkIO;
using System.Windows.Forms;

namespace InstructorClient
{
	public class CustomPenAttributes
	{
		public const string PEN_WIDTH = "PEN_WIDTH_";

		public const string PEN_HEIGHT = "PEN_HEIGHT_";

		public const string PEN_TRANS = "PEN_TRANS_";

		public const string PEN_TIP = "PEN_TIP_";

		public static PenInfo[] DEFAULT_PENS = new PenInfo[2]
		{
			new PenInfo(50f, 50f, PenTip.Ball, 0),
			new PenInfo(100f, 100f, PenTip.Ball, 0)
		};

		private PenInfo[] pens;

		public CustomPenAttributes()
		{
			pens = new PenInfo[DEFAULT_PENS.Length];
			for (int i = 0; i < pens.Length; i++)
			{
				string stringProperty7 = PropertiesManager.GetStringProperty("PEN_WIDTH_" + i);
				string stringProperty6 = PropertiesManager.GetStringProperty("PEN_HEIGHT_" + i);
				string stringProperty5 = PropertiesManager.GetStringProperty("PEN_TIP_" + i);
				string stringProperty4 = PropertiesManager.GetStringProperty("PEN_TRANS_" + i);
				if (stringProperty5 == null || stringProperty7 == null || stringProperty6 == null || stringProperty4 == null || !float.TryParse(stringProperty7, out float num) || !float.TryParse(stringProperty6, out num) || !byte.TryParse(stringProperty4, out byte _))
				{
					pens[i] = DEFAULT_PENS[i];
				}
				else
				{
					pens[i] = new PenInfo(float.Parse(stringProperty7), float.Parse(stringProperty6), penTipFromString(stringProperty5), byte.Parse(stringProperty4));
				}
			}
		}

		public PenInfo getPen(int i)
		{
			if (i >= pens.Length)
			{
				return pens[0];
			}
			return pens[i];
		}

		public void changePen(int i)
		{
			if (i < pens.Length)
			{
				PenSelector penSelector = new PenSelector(pens[i], i);
				DialogResult dialogResult = penSelector.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					pens[i] = penSelector.NewPen;
					PropertiesManager.SetStringProperty("PEN_WIDTH_" + i, pens[i].width.ToString());
					PropertiesManager.SetStringProperty("PEN_HEIGHT_" + i, pens[i].height.ToString());
					PropertiesManager.SetStringProperty("PEN_TIP_" + i, penTipToString(pens[i].tip));
					PropertiesManager.SetStringProperty("PEN_TRANS_" + i, pens[i].transparency.ToString());
				}
			}
		}

		public static string penTipToString(PenTip t)
		{
			switch (t)
			{
			case PenTip.Ball:
				return "ball";
			case PenTip.Rectangle:
				return "rectangle";
			default:
				return "ball";
			}
		}

		public static PenTip penTipFromString(string s)
		{
			if (!s.Equals("ball"))
			{
				if (!s.Equals("rectangle"))
				{
					return PenTip.Ball;
				}
				return PenTip.Rectangle;
			}
			return PenTip.Ball;
		}
	}
}
