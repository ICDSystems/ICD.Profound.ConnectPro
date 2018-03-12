﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.CableTv
{
	public sealed partial class ReferencedCableTvView : AbstractComponentView, IReferencedCableTvView
	{
		public event EventHandler OnIconPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedCableTvView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnIconPressed = null;

			base.Dispose();
		}

		public void SetIconPath(string path)
		{
			m_Icon.SetIconPath(path);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Icon.OnPressed += IconOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Icon.OnPressed -= IconOnPressed;
		}

		private void IconOnPressed(object sender, EventArgs eventArgs)
		{
			OnIconPressed.Raise(this);
		}

		#endregion
	}
}