using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
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

		[CanBeNull]
		private ISource m_Source;

		[CanBeNull]
		private IRoom m_RoomForSource;

		#region Properties

		/// <summary>
		/// Gets/sets the source for the presenter.
		/// </summary>
		[CanBeNull]
		public ISource Source
		{
			get { return m_Source; }
			set
			{
				SetSource(value);
			}
		}

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

		public bool SourceOnline
		{
			get { return m_Cache.SourceOnline; }
			set
			{
				if(m_Cache.SetSourceOnline(value))
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
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateSource();
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
				view.SetRoutedState(SourceOnline ? m_Cache.SourceState : eSourceState.Error);
				view.Enable(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void UpdateAnySourceOnline()
		{
			SourceOnline = m_Source != null && m_Source.GetDevices().Any(d => d.IsOnline);
		}

		private void UpdateSource()
		{
			bool combined = RoomForSource != null && (RoomForSource.CombineState || RoomForSource.IsCombineRoom());
			if (m_Cache.SetSource(RoomForSource, Source, combined))
				RefreshIfVisible();
		}

		#region Source Callbacks

		private void SetSource(ISource source)
		{
			if (source == m_Source)
				return;

			Unsubscribe(m_Source);

			m_Source = source;

			Subscribe(m_Source);

			// Get the room that contains the source
			RoomForSource = Room == null || Source == null ? null : Room.Routing.Sources.GetRoomForSource(Source);

			UpdateAnySourceOnline();
			UpdateSource();
		}

		private void Subscribe(ISource source)
		{
			if (source == null)
				return;

			source.GetDevices().ForEach(Subscribe);
		}

		private void Unsubscribe(ISource source)
		{
			if (source == null)
				return;

			source.GetDevices().ForEach(Unsubscribe);
		}

		#region Source Device Callbacks

		private void Subscribe(IDeviceBase sourceDevice)
		{
			if (sourceDevice == null)
				return;

			sourceDevice.OnIsOnlineStateChanged += SourceDeviceOnIsOnlineStateChanged;
		}

		private void Unsubscribe(IDeviceBase sourceDevice)
		{
			if (sourceDevice == null)
				return;

			sourceDevice.OnIsOnlineStateChanged -= SourceDeviceOnIsOnlineStateChanged;
		}

		private void SourceDeviceOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs deviceBaseOnlineStateApiEventArgs)
		{
			UpdateAnySourceOnline();
		}

		#endregion

		#endregion

		#region Room For Source Callbacks

		/// <summary>
		/// Subscribe to the room for the current source.
		/// </summary>
		/// <param name="roomForSource"></param>
		private void SubscribeRoomForSource(IRoom roomForSource)
		{
			if (roomForSource == null)
				return;

			roomForSource.OnCombineStateChanged += RoomForSourceOnCombineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room for the current source.
		/// </summary>
		/// <param name="roomForSource"></param>
		private void UnsubscribeRoomForSource(IRoom roomForSource)
		{
			if (roomForSource == null)
				return;

			roomForSource.OnCombineStateChanged -= RoomForSourceOnCombineStateChanged;
		}

		/// <summary>
		/// Called when the room for the current source changes combine state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomForSourceOnCombineStateChanged(object sender, BoolEventArgs e)
		{
			UpdateSource();
		}

		#endregion

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
			//vDon't pass presses if source offline
			if (SourceOnline)
				OnPressed.Raise(this);
		}

		#endregion
	}
}
