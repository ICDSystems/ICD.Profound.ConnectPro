﻿using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Background;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Background
{
	[ViewBinding(typeof(IBackgroundView))]
	public sealed partial class BackgroundView : AbstractTouchDisplayView, IBackgroundView
	{
		public BackgroundView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetBackgroundMode(eCueBackgroundMode mode)
		{
			m_Background.SetMode((ushort)mode);
		}
	}
}
