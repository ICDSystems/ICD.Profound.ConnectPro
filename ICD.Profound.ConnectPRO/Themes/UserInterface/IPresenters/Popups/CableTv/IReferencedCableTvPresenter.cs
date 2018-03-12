﻿using System;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv
{
	public interface IReferencedCableTvPresenter : IPresenter<IReferencedCableTvView>
	{
		event EventHandler OnPressed;

		Station Station { get; set; }
	}
}
