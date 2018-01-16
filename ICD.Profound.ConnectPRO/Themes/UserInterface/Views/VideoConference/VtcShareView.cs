using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcShareView : AbstractView, IVtcShareView
	{
		public event EventHandler<UShortEventArgs> OnSourceButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcShareView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetButtonLabel(ushort index, string label)
		{
			m_ButtonList.SetItemLabel(index, label);
		}

		/// <summary>
		/// Sets the icon for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public void SetButtonIcon(ushort index, string icon)
		{
			m_ButtonList.SetItemIcon(index, icon);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
		}

		private void ButtonListOnButtonClicked(object sender, UShortEventArgs uShortEventArgs)
		{
			OnSourceButtonPressed.Raise(this, new UShortEventArgs(uShortEventArgs.Data));
		}

		#endregion
	}
}
