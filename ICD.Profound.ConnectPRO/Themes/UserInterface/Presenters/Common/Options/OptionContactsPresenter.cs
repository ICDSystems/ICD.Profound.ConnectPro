using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Options
{
	public sealed class OptionContactsPresenter : AbstractOptionPresenter<IOptionContactsView>, IOptionContactsPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OptionContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override ushort GetMode()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}
	}
}
