using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Combine;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	[PresenterBinding(typeof(ISourceSelectPresenter))]
	public sealed class SourceSelectPresenter : AbstractUiPresenter<ISourceSelectView>, ISourceSelectPresenter
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		public event SourcePressedCallback OnSourcePressed;

		private readonly ReferencedSourceSelectPresenterFactory m_ChildrenFactory;
		private readonly Dictionary<ISource, eSourceState> m_RoutedSources;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;
		private ISource m_SelectedSource;
		private ushort m_DisplayCount;

		#region Properties

		/// <summary>
		/// Gets/sets the source that is currently selected for routing.
		/// </summary>
		public ISource SelectedSource
		{
			get { return m_SelectedSource; }
			set
			{
				if (value == m_SelectedSource)
					return;

				m_SelectedSource = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SourceSelectPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSourceSelectPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_RoutedSources = new Dictionary<ISource, eSourceState>();

			UpdateSources();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSourcePressed = null;

			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISourceSelectView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IReferencedSourceSelectPresenter presenter in m_ChildrenFactory)
				{
					presenter.Selected = presenter.Source == m_SelectedSource;
					presenter.ShowView(true);

					presenter.SourceState =
						presenter.Source == null
							? eSourceState.Inactive
							: m_RoutedSources.GetDefault(presenter.Source);
				}

				view.SetSourceCount((ushort)m_Sources.Length);
				view.SetDisplayCount(m_DisplayCount);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateSources();

			m_DisplayCount =
					room == null
						? (ushort)0
						: (ushort)room.Routing.Destinations.DisplayDestinationsCount;
		}

		/// <summary>
		/// Sets the sources that are currently routed to displays.
		/// </summary>
		/// <param name="routedSources"></param>
		public void SetRoutedSources(IDictionary<ISource, eSourceState> routedSources)
		{
			if (routedSources == null)
				throw new ArgumentNullException("routedSources");

			m_RefreshSection.Enter();

			try
			{
				if (routedSources.DictionaryEqual(m_RoutedSources))
					return;

				m_RoutedSources.Clear();
				m_RoutedSources.AddRange(routedSources);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Rebuilds the collection of sources.
		/// </summary>
		private void UpdateSources()
		{
			m_Sources = Room == null ? new ISource[0] : GetSources(Room).ToArray();
			m_ChildrenFactory.BuildChildren(m_Sources);
		}

		/// <summary>
		/// Gets the available sources for the room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		private static IEnumerable<ISource> GetSources(IConnectProRoom room)
		{
			return room == null
				? Enumerable.Empty<ISource>()
				: room.Routing
				      .Sources
				      .GetRoomSourcesForUi();
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IReferencedSourceSelectView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedSourceSelectPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedSourceSelectPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the child source.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IReferencedSourceSelectPresenter child = sender as IReferencedSourceSelectPresenter;
			if (child == null)
				return;

			SourcePressedCallback handler = OnSourcePressed;
			if (handler != null)
				handler(this, child.Source);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			ConnectProCombineRoom combineRoom = room as ConnectProCombineRoom;
			if (combineRoom != null)
				combineRoom.OnCombinedAdvancedModeChanged += CombineRoomOnCombinedAdvancedModeChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			ConnectProCombineRoom combine = room as ConnectProCombineRoom;
			if (combine != null)
				combine.OnCombinedAdvancedModeChanged -= CombineRoomOnCombinedAdvancedModeChanged;
		}

		/// <summary>
		/// Called when the combine advanced mode changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CombineRoomOnCombinedAdvancedModeChanged(object sender, CombineAdvancedModeEventArgs eventArgs)
		{
			UpdateSources();
		}

		#endregion
	}
}
