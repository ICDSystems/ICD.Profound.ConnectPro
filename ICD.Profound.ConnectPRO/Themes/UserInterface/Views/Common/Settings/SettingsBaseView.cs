﻿using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsBaseView : AbstractPopupView, ISettingsBaseView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		public override event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnListItemPressed;

		/// <summary>
		/// Raised when the user presses the save button.
		/// </summary>
		public event EventHandler OnSaveButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsBaseView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;
			OnListItemPressed = null;
			OnSaveButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetItemSelected(ushort index, bool selected)
		{
			m_ItemList.SetItemSelected(index, selected);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_SaveButton.OnPressed += SaveButtonOnPressed;
			m_ItemList.OnButtonClicked += ItemListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_SaveButton.OnPressed -= SaveButtonOnPressed;
			m_ItemList.OnButtonClicked -= ItemListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="uShortEventArgs"></param>
		private void ItemListOnButtonClicked(object sender, UShortEventArgs uShortEventArgs)
		{
			OnListItemPressed.Raise(this, new UShortEventArgs(uShortEventArgs.Data));
		}

		/// <summary>
		/// Called when the user presses the save button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SaveButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSaveButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
