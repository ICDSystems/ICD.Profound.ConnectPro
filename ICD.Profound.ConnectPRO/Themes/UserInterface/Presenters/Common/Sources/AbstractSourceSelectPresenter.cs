﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Destinations;
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
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly Dictionary<IDestination, ISource> m_Routing;

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
			m_Routing = new Dictionary<IDestination, ISource>();

			m_Sources = new ISource[0];
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
				m_Sources =
					Room == null
						? new ISource[0]
						: Room.Routing.GetCoreSources().ToArray();

				foreach (IReferencedSourceSelectPresenter presenter in m_ChildrenFactory.BuildChildren(m_Sources))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Source == m_ActiveSource;
					presenter.Routed = m_Routing.ContainsValue(presenter.Source);

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
