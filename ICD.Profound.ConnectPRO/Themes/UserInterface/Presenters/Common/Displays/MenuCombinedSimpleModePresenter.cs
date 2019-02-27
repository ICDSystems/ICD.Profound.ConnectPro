using System;
using ICD.Common.Utils;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuCombinedSimpleModePresenter : AbstractUiPresenter<IMenuCombinedSimpleModeView>, IMenuCombinedSimpleModePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public MenuCombinedSimpleModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IMenuCombinedSimpleModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetAdvancedModeEnabled(true);

				// TODO
				view.SetDisplayIcon(Icons.GetDisplayIcon("videoConferencing", eDisplayColor.Grey));
				view.SetDisplayLine1Text("Line 1");
				view.SetDisplayLine2Text("Line 2");
				view.SetDisplaySourceText("Source");
				view.SetDisplaySpeakerButtonActive(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);
		}

		#region View Callbacks

		protected override void Subscribe(IMenuCombinedSimpleModeView view)
		{
			base.Subscribe(view);

			view.OnAdvancedModeButtonPressed += ViewOnAdvancedModeButtonPressed;
			view.OnDisplayButtonPressed += ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed += ViewOnDisplaySpeakerButtonPressed;
		}

		protected override void Unsubscribe(IMenuCombinedSimpleModeView view)
		{
			base.Unsubscribe(view);

			view.OnAdvancedModeButtonPressed -= ViewOnAdvancedModeButtonPressed;
			view.OnDisplayButtonPressed -= ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed -= ViewOnDisplaySpeakerButtonPressed;
		}

		private void ViewOnAdvancedModeButtonPressed(object sender, EventArgs args)
		{
			Navigation.NavigateTo<IMenuCombinedAdvancedModePresenter>();
		}

		private void ViewOnDisplayButtonPressed(object sender, EventArgs args)
		{
			// show control page if source has one
			throw new NotImplementedException();
		}

		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs args)
		{
			// should be green if any audio is routed at all
		}

		#endregion
	}
}
