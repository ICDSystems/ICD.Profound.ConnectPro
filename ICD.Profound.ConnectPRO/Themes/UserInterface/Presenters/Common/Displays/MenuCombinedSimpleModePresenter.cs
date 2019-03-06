using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenuCombinedSimpleModePresenter))]
	public sealed class MenuCombinedSimpleModePresenter : AbstractDisplaysPresenter<IMenuCombinedSimpleModeView>, IMenuCombinedSimpleModePresenter
	{
		public event EventHandler OnAdvancedModePressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly MenuDisplaysPresenterDisplay m_Display;

		protected override List<MenuDisplaysPresenterDisplay> Displays
		{
			get { return m_Display.Yield().ToList(1); }
		}

		public MenuCombinedSimpleModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Display = new MenuDisplaysPresenterDisplay();
		}

		protected override void Refresh(IMenuCombinedSimpleModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetAdvancedModeEnabled(true);

				// TODO
				view.SetDisplayIcon(m_Display.Icon);
				view.SetDisplayLine1Text(m_Display.Line1);
				view.SetDisplayLine2Text(m_Display.Line2);
				view.SetDisplaySourceText(m_Display.SourceName);
				view.SetDisplaySpeakerButtonActive(m_Display.ShowSpeaker);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
			OnAdvancedModePressed.Raise(this);
		}

		private void ViewOnDisplayButtonPressed(object sender, EventArgs args)
		{
			DisplayButtonPressed(m_Display);
		}

		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs args)
		{
			DisplaySpeakerButtonPressed(m_Display);
		}

		#endregion
	}
}
