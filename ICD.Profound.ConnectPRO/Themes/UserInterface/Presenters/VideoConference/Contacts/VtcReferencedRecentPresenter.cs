using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcReferencedRecentPresenter : AbstractVtcReferencedContactsPresenterBase, IVtcReferencedRecentPresenter
	{
		private ITraditionalParticipant m_Recent;

		public ITraditionalParticipant Recent
		{
			get { return m_Recent; }
			set
			{
				if (value == m_Recent)
					return;

				m_Recent = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedRecentPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			if (Recent == null)
				return string.Empty;

			string name =
				string.IsNullOrEmpty(Recent.Name)
					? Recent.Number
					: Recent.Name;

			if (string.IsNullOrEmpty(name))
				name = "Unknown";

			return string.Format("{0} - {1}", Recent.End ?? Recent.GetStartOrDialTime(), name);
		}

		/// <summary>
		/// Override to control the favorite state for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return false;
		}

		/// <summary>
		/// Override to control the visibility of the favorite button for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavoriteVisible()
		{
			return false;
		}

		protected override void SetModel(object model)
		{
			Recent = model as ITraditionalParticipant;
		}

		protected override void Dial()
		{
			var dialer = ActiveConferenceControl;
			if (dialer != null && m_Recent != null)
				dialer.Dial(new SipDialContext { DialString = m_Recent.Number });
		}
	}
}
