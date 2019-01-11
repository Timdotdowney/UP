namespace SlideModel
{
	public class RemoteSlideAttrs
	{
		public string type = "";

		public string id = "";

		public int iter;

		public bool minimized;

		public string getImageURL(string baseURL, string classroom, string lecture, bool ink)
		{
			string str = baseURL + "/classrooms/" + classroom + "/lectures/" + lecture;
			str = ((!(type == "ss")) ? (str + "/slides/" + type) : (str + "/submissions/" + type));
			str = str + "-" + id + "-";
			if (!ink)
			{
				return str + "0.png";
			}
			return str + iter + ".png";
		}
	}
}
