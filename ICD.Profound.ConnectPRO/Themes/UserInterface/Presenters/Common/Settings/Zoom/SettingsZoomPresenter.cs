using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.SettingsTree.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom.SubSettings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
	[PresenterBinding(typeof(ISettingsZoomPresenter))]
	public sealed class SettingsZoomPresenter : AbstractSettingsNodeBasePresenter<ISettingsZoomView, ZoomSettingsLeaf>,
	                                            ISettingsZoomPresenter
	{
		private const ushort INDEX_GENERAL = 0;
		private const ushort INDEX_ADVANCED = 1;

		private static readonly BiDictionary<ushort, Type> s_IndexToPresenterType =
			new BiDictionary<ushort, Type>
			{
				{INDEX_GENERAL, typeof(ISettingsZoomGeneralPresenter)},
				{INDEX_ADVANCED, typeof(ISettingsZoomAdvancedPresenter)}
			};

		private readonly SafeCriticalSection m_RefreshSection;

		private ushort m_Index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsZoomPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsZoomView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetGeneralButtonSelected(m_Index == INDEX_GENERAL);
				view.SetAdvancedButtonSelection(m_Index == INDEX_ADVANCED);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void NavigateTo(ushort index)
		{
			m_Index = index;

			// Hide the old subpage
			foreach (Type type in s_IndexToPresenterType.Where(kvp => kvp.Key != m_Index).Select(kvp => kvp.Value))
				Navigation.LazyLoadPresenter(type).ShowView(false);

			// Show the new subpage
			var presenter = Navigation.LazyLoadPresenter(s_IndexToPresenterType.GetValue(m_Index)) as ISettingsZoomSubPresenter;
            if (presenter == null)
				throw new InvalidProgramException("Expected a presenter of type " + typeof(ISettingsZoomSubPresenter).Name);

			presenter.Settings = Node;
			presenter.ShowView(true);

			RefreshIfVisible();
		}

		#region New region

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsZoomView view)
		{
			base.Subscribe(view);

			view.OnAdvancedButtonPressed += ViewOnAdvancedButtonPressed;
			view.OnGeneralButtonPressed += ViewOnGeneralButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsZoomView view)
		{
			base.Unsubscribe(view);

			view.OnAdvancedButtonPressed -= ViewOnAdvancedButtonPressed;
			view.OnGeneralButtonPressed -= ViewOnGeneralButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the general button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnGeneralButtonPressed(object sender, EventArgs eventArgs)
		{
			NavigateTo(INDEX_GENERAL);
		}

		/// <summary>
		/// Called when the user presses the advanced button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnAdvancedButtonPressed(object sender, EventArgs eventArgs)
		{
			NavigateTo(INDEX_ADVANCED);
		}

		/// <summary>
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			if (args.Data)
				return;

			// Hide all of the subpages before this presenter is hidden
			foreach (Type presenterType in s_IndexToPresenterType.Values)
				Navigation.LazyLoadPresenter(presenterType).ShowView(false);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// When the view becomes visible show the child presenter
			if (args.Data)
				NavigateTo(m_Index);
		}

		#endregion
	}
}
