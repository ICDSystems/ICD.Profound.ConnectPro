using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	[PresenterBinding(typeof(IReferencedSettingsTouchFreePresenter))]
	public sealed class ReferencedSettingsTouchFreePresenter : AbstractUiComponentPresenter<IReferencedSettingsTouchFreeView>,
														  IReferencedSettingsTouchFreePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private readonly ReferencedSettingsTouchFreePresenterCache m_Cache;
               
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;
		
		[CanBeNull]
		private ISource m_Source;

		[CanBeNull]
		private IRoom m_RoomForSource;

		#region Properties

		/// <summary>
		/// Gets/sets the source for this presenter.
		/// </summary>
		public ISource Source { get { return m_Source; } set { SetSource(value); } }
             
		/// <summary>
		/// Gets/sets the room for the source.
		/// </summary>
		[CanBeNull]
		private IRoom RoomForSource
		{
			get { return m_RoomForSource; }
			set
			{
				if (value == m_RoomForSource)
					return;

				UnsubscribeRoomForSource(m_RoomForSource);
				m_RoomForSource = value;
				SubscribeRoomForSource(m_RoomForSource);

				UpdateSource();
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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSettingsTouchFreePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme, ReferencedSettingsTouchFreePresenterCache cache) : base(nav, views, theme)
		{
			m_Cache =  new ReferencedSettingsTouchFreePresenterCache();
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

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedSettingsTouchFreeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetSelected(m_Cache.Selected);
				view.SetText(m_Cache.Text);
				view.SetIcon(m_Cache.Icon);
				view.Enable(true);
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

			UpdateSource();
		}

		private void SetSource(ISource value)
		{
			throw new NotImplementedException();
		}

		private void UpdateSource()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Room For Source Callbacks

		private void SubscribeRoomForSource(IRoom roomForSource)
		{
			throw new NotImplementedException();
		}

		private void UnsubscribeRoomForSource(IRoom roomForSource)
		{
			throw new NotImplementedException();
		}

		#endregion
		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedSettingsTouchFreeView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedSettingsTouchFreeView view)
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