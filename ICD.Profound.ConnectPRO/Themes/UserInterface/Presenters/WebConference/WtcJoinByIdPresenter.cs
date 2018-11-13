﻿using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcJoinByIdPresenter : AbstractWtcPresenter<IWtcJoinByIdView>, IWtcJoinByIdPresenter
	{
		public WtcJoinByIdPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}
	}
}