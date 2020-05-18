﻿using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Sources;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Sources;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Sources
{
	[PresenterBinding(typeof(IOsdSourcesPresenter))]
	public sealed class OsdSourcesPresenter : AbstractOsdPresenter<IOsdSourcesView>, IOsdSourcesPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdSourcesPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		protected override void Refresh(IOsdSourcesView view)
		{
			base.Refresh(view);

			string roomName = Room == null ? string.Empty : Room.Name;
			view.SetRoomName(roomName);

			ushort index = 0;
			foreach (ISource source in GetSources().Take(8).PadRight(8))
			{
				ConnectProSource proSource = source as ConnectProSource;

				string name = proSource == null ? string.Empty : proSource.CueName ?? source.Name;
				string description = proSource == null ? string.Empty : proSource.Description;
				string icon = proSource == null ? string.Empty : proSource.Icon;

				view.SetLabel(index, name);
				view.SetDescription(index, description);
				view.SetIcon(index, icon);

				index++;
			}
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
				                     return source == null ? null : source.CueNameOrIcon;
			                     });
		}
	}
}