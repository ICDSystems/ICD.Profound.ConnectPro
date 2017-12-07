using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
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
		private readonly ReferencedSourceSelectPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;

		/// <summary>
		/// Gets the number of sources currently listed.
		/// </summary>
		protected int SourceCount { get { return m_RefreshSection.Execute(() => m_Sources.Length); } }

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
		}

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
					presenter.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedSourceSelectView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}
	}
}
