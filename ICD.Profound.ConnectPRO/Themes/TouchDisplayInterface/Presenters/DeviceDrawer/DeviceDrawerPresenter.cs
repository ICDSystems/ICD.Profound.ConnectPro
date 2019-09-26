using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
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
		private readonly ReferencedSourcePresenterFactory m_SourceFactory;
		private readonly SafeCriticalSection m_RefreshSection;

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
		}

		protected override void Refresh(IDeviceDrawerView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				IEnumerable<ISource> sources = GetSources();
				foreach (var source in m_SourceFactory.BuildChildren(sources))
				{
					source.Refresh();
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
		
		private void ChildOnSourcePressed(object sender, System.EventArgs e)
		{
			// route the source
		}

		#endregion
	}
}
