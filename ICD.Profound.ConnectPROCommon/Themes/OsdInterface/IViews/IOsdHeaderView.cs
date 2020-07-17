namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews
{
	public interface IOsdHeaderView : IOsdView
	{
		/// <summary>
		/// Sets the current room name.
		/// </summary>
		/// <param name="name"></param
		void SetRoomName(string name);

		/// <summary>
		/// Sets the current time.
		/// </summary>
		/// <param name="time"></param
		void SetTimeLabel(string time);

		/// <summary>
		/// Sets the current touch-free face graphic.
		/// </summary>
		/// <param name="image"></param>
		void SetTouchFreeFaceImage(eTouchFreeFace image);
	}
}
