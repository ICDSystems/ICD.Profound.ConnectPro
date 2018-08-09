using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	public sealed class SourceSelectPresenter : AbstractPresenter<ISourceSelectView>, ISourceSelectPresenter
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		public event SourcePressedCallback OnSourcePressed;

		private readonly ReferencedSourceSelectPresenterFactory m_ChildrenFactory;
		private readonly Dictionary<ISource, eRoutedState> m_RoutedSources;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;
		private ISource m_ActiveSource;
		private ushort m_DisplayCount;

		#region Properties

		/// <summary>
		/// Gets/sets the source that is currently selected for routing.
		/// </summary>
		public ISource ActiveSource
		{
			get { return m_ActiveSource; }
			set
			{
				if (value == m_ActiveSource)
					return;

				m_ActiveSource = value;

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
		public SourceSelectPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSourceSelectPresenterFactory(nav, ItemFactory);

			m_Sources = new ISource[0];
			m_RoutedSources = new Dictionary<ISource, eRoutedState>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSourcePressed = null;

			UnsubscribeChildren();
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
				UnsubscribeChildren();

				foreach (IReferencedSourceSelectPresenter presenter in m_ChildrenFactory.BuildChildren(m_Sources))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Source == m_ActiveSource;
					presenter.ShowView(true);

					presenter.Routed = presenter.Source == null
						                   ? eRoutedState.Inactive
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
			m_Sources = GetSources(room).ToArray();

			m_DisplayCount =
					room == null
						? (ushort)0
						: (ushort)room.Routing.GetDisplayDestinations().Count();

			base.SetRoom(room);
		}

		private static IEnumerable<ISource> GetSources(IConnectProRoom room)
		{
			return room == null
				       ? Enumerable.Empty<ISource>()
					   : room.Routing
				             .GetCoreSources()
				             .Where(s =>
				                    {
					                    ConnectProSource source = s as ConnectProSource;
					                    return source == null || !source.Hide;
				                    });
		}

		/// <summary>
		/// Sets the sources that are currently routed to displays.
		/// </summary>
		/// <param name="routedSources"></param>
		public void SetRoutedSources(IEnumerable<KeyValuePair<ISource, eRoutedState>> routedSources)
		{
			m_RefreshSection.Enter();

			try
			{
				m_RoutedSources.Clear();
				m_RoutedSources.AddRange(routedSources);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		#region Private Methods

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IReferencedSourceSelectPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
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
	}
}
