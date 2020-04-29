using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Lists;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings
{
	[ViewBinding(typeof(ISettingsBaseView))]
	public sealed partial class SettingsBaseView : AbstractPopupView, ISettingsBaseView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		public override event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnPrimaryListItemPressed;
		
		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnSecondaryListItemPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsBaseView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;
			OnPrimaryListItemPressed = null;

			base.Dispose();
		}

		#region Methods

		#region Primary List

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		public void SetPrimaryButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons)
		{
			if (labelsAndIcons == null)
				throw new ArgumentNullException("labelsAndIcons");

			ButtonListItem[] items = labelsAndIcons.Select(kvp => new ButtonListItem(kvp.Key, kvp.Value)).ToArray();
			m_PrimaryItemList.SetItems(items);
		}

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetPrimaryButtonVisible(ushort index, bool visible)
		{
			m_PrimaryItemList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetPrimaryButtonSelected(ushort index, bool selected)
		{
			m_PrimaryItemList.SetItemSelected(index, selected);
		}

		#endregion

		#region Secondary List

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="visible"></param>
		public void SetSecondaryButtonsVisibility(bool visible)
		{
			m_SecondaryItemList.Show(visible);
		}

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		public void SetSecondaryButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons)
		{
			if (labelsAndIcons == null)
				throw new ArgumentNullException("labelsAndIcons");

			ButtonListItem[] items = labelsAndIcons.Select(kvp => new ButtonListItem(kvp.Key, kvp.Value)).ToArray();
			m_SecondaryItemList.SetItems(items);
		}

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetSecondaryButtonVisible(ushort index, bool visible)
		{
			m_SecondaryItemList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetSecondaryButtonSelected(ushort index, bool selected)
		{
			m_SecondaryItemList.SetItemSelected(index, selected);
		}

		#endregion

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_PrimaryItemList.OnButtonClicked += PrimaryItemListOnButtonClicked;
			m_SecondaryItemList.OnButtonClicked += SecondaryItemListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_PrimaryItemList.OnButtonClicked -= PrimaryItemListOnButtonClicked;
			m_SecondaryItemList.OnButtonClicked -= SecondaryItemListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PrimaryItemListOnButtonClicked(object sender, UShortEventArgs eventArgs)
		{
			OnPrimaryListItemPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SecondaryItemListOnButtonClicked(object sender, UShortEventArgs eventArgs)
		{
			OnSecondaryListItemPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
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
