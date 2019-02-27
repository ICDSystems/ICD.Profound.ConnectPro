namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IReferencedRouteListItemView : IUiView
	{
		/// <summary>
		/// Sets the text of the room label.
		/// </summary>
		/// <param name="room"></param>
		void SetRoomLabelText(string room);

		/// <summary>
		/// Sets the text of the display label.
		/// </summary>
		/// <param name="display"></param>
		void SetDisplayLabelText(string display);

		/// <summary>
		/// Sets the text of the source label.
		/// </summary>
		/// <param name="source"></param>
		void SetSourceLabelText(string source);
	}
}
