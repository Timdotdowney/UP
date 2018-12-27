using NetworkIO;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace InstructorClient
{
	public class CustomColors
	{
		public const int BACK_COLOR = 5;

		public const string COLOR_LIST_KEY = "COLOR_LIST_";

		public const string DIALOG_COLOR_KEY = "DIALOG_COLORS_";

		private static Color[] DEFAULT_COLORS = new Color[6]
		{
			Color.Black,
			Color.Red,
			Color.Green,
			Color.Blue,
			Color.Yellow,
			Color.White
		};

		private static Color DEFAULT_COLOR = Color.Black;

		private Color[] colors;

		private int[] dialogColors;

		public CustomColors()
		{
			ArrayList arrayList = new ArrayList();
			colors = new Color[DEFAULT_COLORS.Length];
			string stringProperty;
			int k;
			for (k = 0; k < colors.Length; k++)
			{
				stringProperty = PropertiesManager.GetStringProperty("COLOR_LIST_" + k);
				if (stringProperty == null)
				{
					colors[k] = DEFAULT_COLORS[k];
				}
				else
				{
					colors[k] = Color.FromArgb(int.Parse(stringProperty));
				}
			}
			k = 0;
			while ((stringProperty = PropertiesManager.GetStringProperty("DIALOG_COLORS_" + k)) != null)
			{
				arrayList.Add(stringProperty);
				k++;
			}
			dialogColors = new int[arrayList.Count + DEFAULT_COLORS.Length];
			for (k = 0; k < arrayList.Count; k++)
			{
				dialogColors[k] = int.Parse((string)arrayList[k]);
			}
		}

		public Color getColor(int index)
		{
			if (index >= colors.Length)
			{
				return DEFAULT_COLOR;
			}
			return colors[index];
		}

		public void changeColor(int index)
		{
			if (index < colors.Length)
			{
				ColorDialog colorDialog = new ColorDialog();
				colorDialog.AllowFullOpen = true;
				colorDialog.AnyColor = true;
				colorDialog.Color = colors[index];
				colorDialog.FullOpen = true;
				colorDialog.CustomColors = dialogColors;
				DialogResult dialogResult = colorDialog.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					colors[index] = colorDialog.Color;
					dialogColors = colorDialog.CustomColors;
					PropertiesManager.SetStringProperty("COLOR_LIST_" + index, colors[index].ToArgb().ToString());
					for (int i = 0; i < dialogColors.Length; i++)
					{
						PropertiesManager.SetStringProperty("DIALOG_COLORS_" + i, dialogColors[i].ToString());
					}
				}
			}
		}

		public Color restoreDefaultColor(int index)
		{
			if (index < colors.Length)
			{
				colors[index] = DEFAULT_COLORS[index];
				PropertiesManager.SetStringProperty("COLOR_LIST_" + index, colors[index].ToArgb().ToString());
				return colors[index];
			}
			return DEFAULT_COLOR;
		}
	}
}
