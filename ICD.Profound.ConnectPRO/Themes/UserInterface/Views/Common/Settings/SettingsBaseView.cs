using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Lists;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
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
		public event EventHandler<UShortEventArgs> OnListItemPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		public event EventHandler OnBackButtonPressed;

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
			OnBackButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetItemSelected(ushort index, bool selected)
		{
			m_ItemList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		public void SetButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons)
		{
			if (labelsAndIcons == null)
				throw new ArgumentNullException("labelsAndIcons");

			ButtonListItem[] items = labelsAndIcons.Select(kvp => new ButtonListItem(kvp.Key, kvp.Value)).ToArray();
			m_ItemList.SetItems(items);
		}

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetButtonVisible(ushort index, bool visible)
		{
			m_ItemList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ItemList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the visibility of the back button.
		/// </summary>
		/// <param name="visible"></param>
		public void SetBackButtonVisible(bool visible)
		{
			m_BackButton.Show(visible);
		}

		/// <summary>
		/// Sets the text for the title label.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="leaf"></param>
		public void SetTitle(string parent, string leaf)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins[0], parent);
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins[1], leaf);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_ItemList.OnButtonClicked += ItemListOnButtonClicked;
			m_BackButton.OnPressed += BackButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_ItemList.OnButtonClicked -= ItemListOnButtonClicked;
			m_BackButton.OnPressed -= BackButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ItemListOnButtonClicked(object sender, UShortEventArgs eventArgs)
		{
			OnListItemPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
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

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BackButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnBackButtonPressed.Raise(this);
		}

		#endregion
	}
}
