using ICD.Common.Properties;
using ICD.Connect.Conferencing.ConferenceManagers.History;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	[PresenterBinding(typeof(IVtcReferencedRecentPresenter))]
	public sealed class VtcReferencedRecentPresenter : AbstractVtcReferencedContactsPresenterBase, IVtcReferencedRecentPresenter
	{
		private IHistoricalParticipant m_Recent;

		[CanBeNull]
		public IHistoricalParticipant Recent
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

			return string.Format("{0} - {1}", Recent.StartTime, name);
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
			Recent = model as IHistoricalParticipant;
		}

		protected override void Dial()
		{
			if (Room == null)
				return;

			if (ActiveConferenceControl != null && m_Recent != null)
				Room.Dialing.Dial(ActiveConferenceControl, new SipDialContext {DialString = m_Recent.Number});
		}
	}
}
