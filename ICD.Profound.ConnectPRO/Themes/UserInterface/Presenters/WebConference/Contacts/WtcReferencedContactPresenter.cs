using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedContactPresenter : AbstractUiComponentPresenter<IWtcReferencedContactView>, IWtcReferencedContactPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		public IContact Contact { get; set; }

		public IWebConferenceDeviceControl ActiveConferenceControl { set; private get; }

		public bool Selected { get; set; }

		public WtcReferencedContactPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			base.Dispose();

			OnPressed = null;
		}

		protected override void Refresh(IWtcReferencedContactView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetContactName(Contact.Name);
				view.SetButtonSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void Dial()
		{
			// todo select best dial context
			ActiveConferenceControl.Dial(Contact.GetDialContexts().First());
		}

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedContactView view)
		{
			base.Subscribe(view);

			view.OnContactPressed += ViewOnOnContactPressed;
		}

		private void ViewOnOnContactPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}