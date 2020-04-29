using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.DeviceDrawer
{
	[PresenterBinding(typeof(IDeviceDrawerPresenter))]
	public sealed class DeviceDrawerPresenter : AbstractTouchDisplayPresenter<IDeviceDrawerView>, IDeviceDrawerPresenter
	{
		private const long APP_LAUNCH_FAIL_TIMEOUT = 5000L;

		public event EventHandler<SourceEventArgs> OnSourcePressed;

		private static readonly List<eVibeApps> s_Apps = new List<eVibeApps>()
		{
			eVibeApps.Chrome,
			eVibeApps.Youtube,
			eVibeApps.Slack,
			eVibeApps.Whiteboard,
			eVibeApps.Teams,
			eVibeApps.WebEx
		};

		private readonly ReferencedSourcePresenterFactory m_SourceFactory;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly Dictionary<ISource, eSourceState> m_CachedSourceStates;

		private VibeBoardAppControl m_SubscribedAppControl;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public DeviceDrawerPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_SourceFactory = new ReferencedSourcePresenterFactory(nav, ItemFactory, SubscribeChild, UnsubscribeChild);
			m_CachedSourceStates = new Dictionary<ISource, eSourceState>();
		}

		protected override void Refresh(IDeviceDrawerView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				IEnumerable<ISource> sources = GetSources();
				foreach (var presenter in m_SourceFactory.BuildChildren(sources))
				{
					presenter.SourceState = m_CachedSourceStates.ContainsKey(presenter.Source) 
						? m_CachedSourceStates[presenter.Source] 
						: eSourceState.Inactive;
					presenter.ShowView(true);
					presenter.Refresh();
				}

				var installedApps= m_SubscribedAppControl == null
					? Enumerable.Empty<eVibeApps>().ToList()
					: s_Apps
						//.Where(app => m_SubscribedAppControl.IsInstalled(app)) TODO
						.ToList();

				var packageNames = installedApps.Select(app => m_SubscribedAppControl.GetPackageName(app))
						.ToList();
				view.SetAppButtonIcons(packageNames);
				var appNames = installedApps.Select(a => a.ToString());
				view.SetAppButtonLabels(appNames);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetRoutedSources(Dictionary<ISource, eSourceState> sources)
		{
			m_CachedSourceStates.Clear();
			m_CachedSourceStates.AddRange(sources);

			RefreshIfVisible();
		}
		
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);
			
			Unsubscribe(m_SubscribedAppControl);

			var vibeBoard = room == null ? null : room.Originators.GetInstanceRecursive<VibeBoard>();
			m_SubscribedAppControl = vibeBoard == null ? null : vibeBoard.Controls.GetControl<VibeBoardAppControl>();

			Subscribe(m_SubscribedAppControl);
		}

		private IEnumerable<IReferencedSourceView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		private IEnumerable<ISource> GetSources()
		{
			if (Room == null)
				return Enumerable.Empty<ISource>();

			return Room.Routing
			           .Sources
			           .GetRoomSources()
			           .Where(s =>
			                  {
				                  ConnectProSource source = s as ConnectProSource;
				                  return source == null || !source.Hide;
			                  });
		}

		#region Child Callbacks

		private void SubscribeChild(IReferencedSourcePresenter child)
		{
			child.OnSourcePressed += ChildOnSourcePressed;
		}

		private void UnsubscribeChild(IReferencedSourcePresenter child)
		{
			child.OnSourcePressed -= ChildOnSourcePressed;
		}
		
		private void ChildOnSourcePressed(object sender, EventArgs e)
		{
			IReferencedSourcePresenter presenter = sender as IReferencedSourcePresenter;
			if (presenter == null || presenter.Source == null)
				return;

			OnSourcePressed.Raise(this, new SourceEventArgs(presenter.Source));
			RefreshIfVisible();
		}

		#endregion

		#region App Callbacks

		private void Subscribe(VibeBoardAppControl control)
		{
			if (control == null)
				return;

			control.OnAppLaunched += AppControlOnAppLaunched;
			control.OnAppLaunchFailed += AppControlOnAppLaunchFailed;
		}

		private void Unsubscribe(VibeBoardAppControl control)
		{
			if (control == null)
				return;

			control.OnAppLaunched -= AppControlOnAppLaunched;
			control.OnAppLaunchFailed -= AppControlOnAppLaunchFailed;
		}

		private void AppControlOnAppLaunched(object sender, EventArgs e)
		{
			ShowView(false);
		}

		private void AppControlOnAppLaunchFailed(object sender, EventArgs e)
		{
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>().Show("App is either not installed or failed to launch.",
				APP_LAUNCH_FAIL_TIMEOUT, GenericAlertPresenterButton.Dismiss);
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IDeviceDrawerView view)
		{
			base.Subscribe(view);

			view.OnAppButtonPressed += ViewOnOnAppButtonPressed;
		}

		protected override void Unsubscribe(IDeviceDrawerView view)
		{
			base.Unsubscribe(view);

			view.OnAppButtonPressed -= ViewOnOnAppButtonPressed;
		}

		private void ViewOnOnAppButtonPressed(object sender, UShortEventArgs e)
		{
			if (m_SubscribedAppControl == null)
				return;

			m_SubscribedAppControl.LaunchApp(s_Apps[e.Data]);
		}

		#endregion
	}
}
