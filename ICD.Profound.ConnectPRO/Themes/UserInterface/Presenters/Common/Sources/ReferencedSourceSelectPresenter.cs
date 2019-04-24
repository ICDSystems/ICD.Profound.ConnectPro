using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	[PresenterBinding(typeof(IReferencedSourceSelectPresenter))]
	public sealed class ReferencedSourceSelectPresenter : AbstractUiComponentPresenter<IReferencedSourceSelectView>,
	                                                      IReferencedSourceSelectPresenter
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSourceSelectPresenterCache m_Cache;

		#region Properties

		/// <summary>
		/// Gets/sets the source for the presenter.
		/// </summary>
		public ISource Source
		{
			get { return m_Cache.Source; }
			set
			{
				if (value == m_Cache.Source)
					return;

				// Get the room that contains the source
				IRoom room = Room == null || value == null ? null : Room.Routing.GetRoomForSource(value);

				if (m_Cache.SetSource(room, value))
					RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the selected state of the presenter.
		/// </summary>
		public bool Selected
		{
			get { return m_Cache.Selected; }
			set
			{
				if (m_Cache.SetSelected(value))
					RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the routed state of the source.
		/// </summary>
		public eSourceState SourceState
		{
			get { return m_Cache.SourceState; }
			set
			{
				if (m_Cache.SetRouted(value))
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
		public ReferencedSourceSelectPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Cache = new ReferencedSourceSelectPresenterCache();
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
		protected override void Refresh(IReferencedSourceSelectView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetColor(m_Cache.Color);
				view.SetFeedbackText(m_Cache.Feedback);
				view.SetLine1Text(m_Cache.Line1);
				view.SetLine2Text(m_Cache.Line2);
				view.SetIcon(m_Cache.Icon);
				view.SetRoutedState(m_Cache.SourceState);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedSourceSelectView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedSourceSelectView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
