using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcReferencedContactsView : AbstractComponentView, IVtcReferencedContactsView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VtcReferencedContactsView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}
	}
}
