﻿using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters
{
	public interface IOsdHelloPresenter : IOsdPresenter<IOsdHelloView>
	{
		event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		/// <summary>
		/// Gets whether the presenter is in the main area (true) or the notification area (false)
		/// </summary>
		bool MainPageView { get; }
	}
}
