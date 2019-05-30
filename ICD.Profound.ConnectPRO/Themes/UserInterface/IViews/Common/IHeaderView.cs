namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IHeaderView : IUiView
	{
		/// <summary>
		/// Sets the room name label text.
		/// </summary>
		/// <param name="name"></param>
		void SetRoomName(string name);

		/// <summary>
		/// Sets the time label text.
		/// </summary>
		/// <param name="label"></param>
		void SetTimeLabel(string label);
	}
}
