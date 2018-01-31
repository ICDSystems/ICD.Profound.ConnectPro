using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class DisplaysPresenter : AbstractPresenter<IDisplaysView>, IDisplaysPresenter
	{
		public event DestinationPressedCallback OnDestinationPressed;

		private readonly ReferencedDisplaysPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;
		private ISource m_ActiveSource;
		private readonly Dictionary<IDestination, ISource> m_Routing;
		private readonly IcdHashSet<ISource> m_ActiveAudio;

		/// <summary>
		/// Gets/sets the source that is currently active for routing.
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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public DisplaysPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedDisplaysPresenterFactory(nav, ItemFactory);
			m_Routing = new Dictionary<IDestination, ISource>();
			m_ActiveAudio = new IcdHashSet<ISource>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDestinationPressed = null;

			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		protected override void Refresh(IDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				IEnumerable<IDestination> destinations =
					Room == null
						? Enumerable.Empty<IDestination>()
						: Room.Routing.GetDisplayDestinations();

				foreach (IReferencedDisplaysPresenter presenter in m_ChildrenFactory.BuildChildren(destinations))
				{
					Subscribe(presenter);

					presenter.ActiveSource = m_ActiveSource;
					presenter.RoutedSource = m_Routing.GetDefault(presenter.Destination, null);
					presenter.ActiveAudio = presenter.RoutedSource != null && m_ActiveAudio.Contains(presenter.RoutedSource);

					presenter.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetRoutedSources(Dictionary<IDestination, ISource> routing)
		{
			m_RefreshSection.Enter();

			try
			{
				m_Routing.Clear();
				m_Routing.Update(routing);

				RefreshIfVisible();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetActiveAudioSources(IEnumerable<ISource> activeAudio)
		{
			m_RefreshSection.Enter();

			try
			{
				m_ActiveAudio.Clear();
				m_ActiveAudio.AddRange(activeAudio);

				RefreshIfVisible();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Perivate Methods

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IReferencedDisplaysPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		private IEnumerable<IReferencedDisplaysView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedDisplaysPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedDisplaysPresenter child)
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
			IReferencedDisplaysPresenter child = sender as IReferencedDisplaysPresenter;
			if (child == null)
				return;

			DestinationPressedCallback handler = OnDestinationPressed;
			if (handler != null)
				handler(this, sender as IReferencedDisplaysPresenter, child.Destination);
		}

		#endregion
	}
}
