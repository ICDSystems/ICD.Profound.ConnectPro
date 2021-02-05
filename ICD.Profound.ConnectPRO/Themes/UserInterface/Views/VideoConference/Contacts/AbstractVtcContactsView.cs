using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public abstract class AbstractVtcContactsView : AbstractUiView, IVtcContactsView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		public abstract event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the recent button.
		/// </summary>
		public abstract event EventHandler OnRecentButtonPressed;

		/// <summary>
		/// Raised when the user presses the call button.
		/// </summary>
		public abstract event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		public abstract event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		public abstract event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		public abstract event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		public abstract event EventHandler OnManualDialButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractVtcContactsView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public abstract IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Sets the selected state of the recent button.
		/// </summary>
		/// <param name="selected"></param>
		public abstract void SetRecentButtonSelected(bool selected);

		/// <summary>
		/// Sets the enabled state of the call button.
		/// </summary>
		/// <param name="enabled"></param>
		public abstract void SetCallButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		public abstract void SetHangupButtonEnabled(bool enabled);
	}
}
