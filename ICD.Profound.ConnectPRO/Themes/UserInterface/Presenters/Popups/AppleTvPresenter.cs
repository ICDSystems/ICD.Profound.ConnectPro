using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups
{
	public sealed class AppleTvPresenter : AbstractPopupPresenter<IAppleTvView>, IAppleTvPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AppleTvPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IAppleTvView view)
		{
			base.Subscribe(view);

			view.OnDPadUpButtonPressed += ViewOnDPadUpButtonPressed;
			view.OnDPadDownButtonPressed += ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed += ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed += ViewOnDPadRightButtonPressed;
			view.OnDPadButtonReleased += ViewOnDPadButtonReleased;
			view.OnMenuButtonPressed += ViewOnMenuButtonPressed;
			view.OnPlayButtonPressed += ViewOnPlayButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IAppleTvView view)
		{
			base.Unsubscribe(view);

			view.OnDPadUpButtonPressed -= ViewOnDPadUpButtonPressed;
			view.OnDPadDownButtonPressed -= ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed -= ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed -= ViewOnDPadRightButtonPressed;
			view.OnDPadButtonReleased -= ViewOnDPadButtonReleased;
			view.OnMenuButtonPressed -= ViewOnMenuButtonPressed;
			view.OnPlayButtonPressed -= ViewOnPlayButtonPressed;
		}

		private void ViewOnDPadUpButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnDPadDownButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnDPadLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnDPadRightButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnDPadButtonReleased(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnMenuButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnPlayButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}