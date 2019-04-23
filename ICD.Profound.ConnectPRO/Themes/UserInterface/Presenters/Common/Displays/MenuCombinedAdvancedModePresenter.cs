using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenuCombinedAdvancedModePresenter))]
	public sealed class MenuCombinedAdvancedModePresenter : AbstractDisplaysPresenter<IMenuCombinedAdvancedModeView>, IMenuCombinedAdvancedModePresenter
	{
		public event EventHandler OnSimpleModePressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedAdvancedDisplayPresenterFactory m_PresenterFactory;
		private readonly List<MenuDisplaysPresenterDisplay> m_Displays;

		protected override List<MenuDisplaysPresenterDisplay> Displays { get { return m_Displays; } }

		public MenuCombinedAdvancedModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new ReferencedAdvancedDisplayPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_Displays = new List<MenuDisplaysPresenterDisplay>();
		}

		protected override void Refresh(IMenuCombinedAdvancedModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				foreach (var presenter in m_PresenterFactory.BuildChildren(Displays))
				{
					presenter.ShowView(true);
					presenter.Refresh();
				}

				view.SetRouteSummaryButtonEnabled(true);
				view.SetSimpleModeButtonEnabled(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedAdvancedDisplayView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
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

		protected override void Subscribe(IMenuCombinedAdvancedModeView view)
		{
			base.Subscribe(view);
			
			view.OnSimpleModeButtonPressed += ViewOnSimpleModeButtonPressed;
			view.OnRouteSummaryButtonPressed += ViewOnRouteSummaryButtonPressed;
		}

		protected override void Unsubscribe(IMenuCombinedAdvancedModeView view)
		{
			base.Unsubscribe(view);
			
			view.OnSimpleModeButtonPressed -= ViewOnSimpleModeButtonPressed;
			view.OnRouteSummaryButtonPressed -= ViewOnRouteSummaryButtonPressed;
		}

		private void ViewOnSimpleModeButtonPressed(object sender, EventArgs e)
		{
			OnSimpleModePressed.Raise(this);
		}

		private void ViewOnRouteSummaryButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IMenuRouteSummaryPresenter>();
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedAdvancedDisplayPresenter presenter)
		{
			presenter.OnDisplayPressed += PresenterOnDisplayPressed;
			presenter.OnDisplaySpeakerPressed += PresenterOnDisplaySpeakerPressed;
		}

		private void Unsubscribe(IReferencedAdvancedDisplayPresenter presenter)
		{
			presenter.OnDisplayPressed -= PresenterOnDisplayPressed;
			presenter.OnDisplaySpeakerPressed -= PresenterOnDisplaySpeakerPressed;
		}

		private void PresenterOnDisplayPressed(object sender, EventArgs e)
		{
			var presenter = sender as IReferencedAdvancedDisplayPresenter;
			if (presenter == null || presenter.Model == null)
				return;

			DisplayButtonPressed(presenter.Model);
		}
		
		private void PresenterOnDisplaySpeakerPressed(object sender, EventArgs e)
		{
			var presenter = sender as IReferencedAdvancedDisplayPresenter;
			if (presenter == null || presenter.Model == null)
				return;

			DisplaySpeakerButtonPressed(presenter.Model);
		}

		#endregion
	}
}
