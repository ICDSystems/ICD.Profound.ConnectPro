using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Devices.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IConferenceBasePresenter))]
	public sealed class ConferenceBasePresenter : AbstractPopupPresenter<IConferenceBaseView>, IConferenceBasePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IConferencePresenter> m_ConferencePresenters;
		private readonly Dictionary<ITouchDisplayPresenter, HeaderButtonModel> m_PresenterButtons;
		
		private IConferenceDeviceControl m_SubscribedConferenceControl;

		public IConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_SubscribedConferenceControl; }
			set
			{
				if (m_SubscribedConferenceControl == value)
					return;

				Unsubscribe(m_SubscribedConferenceControl);
				m_SubscribedConferenceControl = value;
				Subscribe(m_SubscribedConferenceControl);

				foreach (var presenter in m_ConferencePresenters)
					presenter.ActiveConferenceControl = m_SubscribedConferenceControl;

				if (m_SubscribedConferenceControl == null)
					ShowView(false);
			}
		}

		public ConferenceBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenters = Navigation.LazyLoadPresenters<IConferencePresenter>().ToList();
			var startMeetingButton = new HeaderButtonModel(0, 0, StartMeetingPressed)
			{
				Icon = TouchCueIcons.GetIcon("videoconference"),
				LabelText = "Start Meeting"
			};

			m_PresenterButtons = new Dictionary<ITouchDisplayPresenter, HeaderButtonModel>()
			{
				{Navigation.LazyLoadPresenter<IStartConferencePresenter>(), startMeetingButton}
			};
		}

		protected override void Refresh(IConferenceBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				foreach (var presenterButton in m_PresenterButtons)
				{
					ITouchDisplayPresenter presenter = presenterButton.Key;
					HeaderButtonModel button = presenterButton.Value;
					button.Mode = presenter.IsViewVisible ? eHeaderButtonMode.Close : eHeaderButtonMode.Orange;
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetControl(IDeviceControl control)
		{
			IConferenceDeviceControl conferenceControl = control as IConferenceDeviceControl;
			if (conferenceControl == null)
				return;

			ActiveConferenceControl = conferenceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IWebConferenceDeviceControl;
		}

		#region Header Button Callbacks

		private void StartMeetingPressed()
		{
			IStartConferencePresenter presenter = Navigation.LazyLoadPresenter<IStartConferencePresenter>();
			presenter.ShowView(!presenter.IsViewVisible);
			Refresh();
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(IConferenceDeviceControl control)
		{
			// todo - replace start conference with active conference when conference is detected
		}

		private void Unsubscribe(IConferenceDeviceControl control)
		{
			// todo
		}

		#endregion

		#region View Callbacks

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			IHeaderPresenter header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			foreach (var button in m_PresenterButtons)
				if (args.Data)
					header.AddRightButton(button.Value);
				else
					header.RemoveRightButton(button.Value);

			if (!args.Data)
			{
				foreach (var presenter in m_ConferencePresenters)
					presenter.ShowView(false);

				if (Room != null)
					Room.FocusSource = null;
			}
			else
				Navigation.NavigateTo<IStartConferencePresenter>();

			Refresh();
		}

		#endregion
	}
}
