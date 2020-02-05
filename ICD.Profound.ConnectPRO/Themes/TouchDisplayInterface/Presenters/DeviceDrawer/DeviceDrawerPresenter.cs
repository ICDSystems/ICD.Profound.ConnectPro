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
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.DeviceDrawer
{
	[PresenterBinding(typeof(IDeviceDrawerPresenter))]
	public sealed class DeviceDrawerPresenter : AbstractTouchDisplayPresenter<IDeviceDrawerView>, IDeviceDrawerPresenter
	{
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


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public DeviceDrawerPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
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
				
				var vibeBoard = Room == null ? null : Room.Originators.GetInstance<VibeBoard>();
				if (vibeBoard == null)
					return;

				var appControl = vibeBoard.Controls.GetControl<VibeBoardAppControl>();
				var packageNames = s_Apps.Select(app => appControl.GetPackageName(app)).ToList();
				view.SetAppButtonIcons(packageNames);
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
			                  })
			           .Distinct(s =>
			                     {
				                     ConnectProSource source = s as ConnectProSource;
				                     return source == null ? null : source.Icon;
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
			if (Room == null)
				return;

			var vibeBoard = Room.Originators.GetInstanceRecursive<VibeBoard>();
			if (vibeBoard == null)
				return;

			var appControl = vibeBoard.Controls.GetControl<VibeBoardAppControl>();
			appControl.LaunchApp(s_Apps[e.Data]);
		}

		#endregion
	}
}
