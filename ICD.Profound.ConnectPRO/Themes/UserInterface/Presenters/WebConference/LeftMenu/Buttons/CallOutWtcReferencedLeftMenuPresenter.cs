using System;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(ICallOutWtcReferencedLeftMenuPresenter))]
	public sealed class CallOutWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                            ICallOutWtcReferencedLeftMenuPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CallOutWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                             ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			throw new NotImplementedException();
		}
	}
}
