using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.CableTv
{
	public sealed class ReferencedCableTvPresenter : AbstractUiComponentPresenter<IReferencedCableTvView>, IReferencedCableTvPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private Station m_Station;

		public Station Station
		{
			get { return m_Station; }
			set
			{
				if (value == m_Station)
					return;

				m_Station = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedCableTvPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedCableTvView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string path = m_Station.Url;

				view.SetIconPath(path);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedCableTvView view)
		{
			base.Subscribe(view);

			view.OnIconPressed += ViewOnIconPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedCableTvView view)
		{
			base.Unsubscribe(view);

			view.OnIconPressed -= ViewOnIconPressed;
		}

		private void ViewOnIconPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}
	}
}
