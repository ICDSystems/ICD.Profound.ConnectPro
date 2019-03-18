using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
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
		private const eDisplayColor DEFAULT_COLOR = eDisplayColor.White;
		private const string DEFAULT_LINE_1 = "ALL DISPLAYS";
		private const string DEFAULT_LINE_2 = "";
		private const string DEFAULT_SOURCE_NAME = "Mixed Sources";
		private const string DEFAULT_ICON = "";

		public event EventHandler OnAdvancedModePressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<MenuDisplaysPresenterDisplay> m_Displays;

		protected override List<MenuDisplaysPresenterDisplay> Displays
		{
			get { return m_Displays.ToList(1); }
		}

		public MenuCombinedSimpleModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Displays = new List<MenuDisplaysPresenterDisplay>();
		}

		protected override void Refresh(IMenuCombinedSimpleModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetAdvancedModeEnabled(true);

				// TODO

				eDisplayColor color;
				string icon;
				string line1;
				string line2;
				string sourceName;

				if (!m_Displays.Select(d => d.Color).Unanimous(out color))
					color = DEFAULT_COLOR;
				string textColor = Colors.DisplayColorToTextColor(color);

				if (!m_Displays.Select(d => d.Icon).Unanimous(out icon))
					icon = Icons.GetDisplayIcon(DEFAULT_ICON, eDisplayColor.White);

				if (!m_Displays.Select(d => d.Line1).Unanimous(out line1))
					line1 = HtmlUtils.FormatColoredText(DEFAULT_LINE_1, textColor);

				if (!m_Displays.Select(d => d.Line2).Unanimous(out line2))
					line2 = HtmlUtils.FormatColoredText(DEFAULT_LINE_2, textColor);
				
				if (!m_Displays.Select(d => d.SourceName).Unanimous(out sourceName))
					sourceName = HtmlUtils.FormatColoredText(DEFAULT_SOURCE_NAME, textColor);

				view.SetDisplayColor(color);
				view.SetDisplayIcon(icon);
				view.SetDisplayLine1Text(line1);
				view.SetDisplayLine2Text(line2);
				view.SetDisplaySourceText(sourceName);
				view.SetDisplaySpeakerButtonActive(m_Displays.Any(d => d.AudioActive));
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public override void SetRoom(IConnectProRoom room)
		{
			int displayCount = room == null ? 0 : room.Routing.GetDisplayDestinations().Count();

			// remake displays list
			m_Displays.Clear();
			m_Displays.AddRange(Enumerable.Range(0, displayCount).Select(i => new MenuDisplaysPresenterDisplay()));

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
			OnAdvancedModePressed.Raise(this);
		}

		private void ViewOnDisplayButtonPressed(object sender, EventArgs args)
		{
			if (!m_Displays.Any())
				return;

			DisplayButtonPressed(m_Displays[0]);
		}

		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs args)
		{
			if (!m_Displays.Any())
				return;

			DisplaySpeakerButtonPressed(m_Displays[0]);
		}

		#endregion
	}
}
