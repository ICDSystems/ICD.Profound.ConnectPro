using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	public abstract class AbstractSourceSelectPresenter<TView> : AbstractPresenter<TView>, ISourceSelectPresenter<TView>
		where TView : class, ISourceSelectView
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		public event SourcePressedCallback OnSourcePressed;

		private readonly ReferencedSourceSelectPresenterFactory m_ChildrenFactory;
		private readonly IcdHashSet<ISource> m_RoutedSources;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;
		private ISource m_ActiveSource;

		#region Properties

		/// <summary>
		/// Gets the number of sources currently listed.
		/// </summary>
		protected int SourceCount { get { return m_RefreshSection.Execute(() => m_Sources.Length); } }

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
		protected AbstractSourceSelectPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSourceSelectPresenterFactory(nav, ItemFactory);

			m_Sources = new ISource[0];
			m_RoutedSources = new IcdHashSet<ISource>();
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
		protected override void Refresh(TView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				m_Sources =
					Room == null
						? new ISource[0]
						: Room.Routing.GetCoreSources().ToArray();

				foreach (IReferencedSourceSelectPresenter presenter in m_ChildrenFactory.BuildChildren(m_Sources))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Source == m_ActiveSource;
					presenter.ShowView(true);
					presenter.Routed = presenter.Source != null && m_RoutedSources.Contains(presenter.Source);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the sources that are currently routed to displays.
		/// </summary>
		/// <param name="routedSources"></param>
		public void SetRoutedSources(IEnumerable<ISource> routedSources)
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
