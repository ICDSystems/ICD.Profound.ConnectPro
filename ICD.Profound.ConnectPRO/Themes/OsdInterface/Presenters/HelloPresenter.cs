using System;
using System.Collections.Generic;
using System.Text;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public class HelloPresenter : AbstractOsdPresenter<IHelloView>, IHelloPresenter
	{
		public HelloPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IHelloView view)
		{
			base.Refresh(view);

			view.SetLabelText("Hello.");
		}
	}
}
