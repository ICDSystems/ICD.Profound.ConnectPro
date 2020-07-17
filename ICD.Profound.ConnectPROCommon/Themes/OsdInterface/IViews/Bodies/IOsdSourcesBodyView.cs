namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies
{
    public interface IOsdSourcesBodyView : IOsdView
	{
		/// <summary>
		/// Sets the name of the room.
		/// </summary>
		/// <param name="name"></param>
		void SetRoomName(string name);

		/// <summary>
		/// Sets the icon for the source at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		void SetIcon(ushort index, string icon);

		/// <summary>
		/// Sets the label text for the source at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetLabel(ushort index, string label);

		/// <summary>
		/// Sets the description text for the source at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="description"></param>
		void SetDescription(ushort index, string description);
	}
}
