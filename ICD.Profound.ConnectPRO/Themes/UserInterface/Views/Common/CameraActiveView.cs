﻿using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(ICameraActiveView))]
	public sealed partial class CameraActiveView : AbstractUiView, ICameraActiveView
	{
		/// <summary>
		/// Raised when a camera button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		/// <summary>
		/// Raised when a tab button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnTabButtonPressed;

		public CameraActiveView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
		
		/// <summary>
		/// Sets the camera selection list labels.
		/// </summary>
		/// <param name="labels"></param>
		public void SetCameraLabels(IEnumerable<string> labels)
		{
			if (labels == null)
				throw new ArgumentNullException("labels");

			string[] labelsArray = labels.ToArray();

			bool visible = labelsArray.Length > 1;

			m_CameraList.SetItemLabels(labelsArray);
			m_CameraList.Show(visible);
		}

		/// <summary>
		/// Sets the selection state of the camera button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetCameraSelected(ushort index, bool selected)
		{
			m_CameraList.SetItemSelected(index, selected);
		}

		public void SetTabSelected(ushort index, bool selected)
		{
			m_Tabs.SetItemSelected(index, selected);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Tabs.OnButtonPressed += TabsOnButtonPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Tabs.OnButtonPressed -= TabsOnButtonPressed;
		}

		private void TabsOnButtonPressed(object sender, UShortEventArgs e)
		{
			OnTabButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		#endregion
	}
}
