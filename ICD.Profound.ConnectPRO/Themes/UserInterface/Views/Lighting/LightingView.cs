using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Lighting;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Lighting
{
	[ViewBinding(typeof(ILightingView))]
	public sealed partial class LightingView : AbstractUiView, ILightingView
	{
		public event EventHandler OnClosePressed;
		public event EventHandler<UShortEventArgs> OnPresetPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public LightingView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public LightingView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}

		public int MaxPresets { get { return MAX_PRESETS; } }

		public void SetPresetLabels(IEnumerable<string> labels)
		{
			if (labels == null)
				throw new ArgumentNullException("labels");

			string[] labelsArray = labels.ToArray();

			m_PresetButtonList.SetItemLabels(labelsArray);
		}

		public void SetPresetActive(ushort index, bool state)
		{
			m_PresetButtonList.SetItemSelected(index, state);
		}

		#region Controls Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_PresetButtonList.OnButtonClicked += PresetButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_PresetButtonList.OnButtonClicked -= PresetButtonListOnButtonClicked;
		}

		private void CloseButtonOnPressed(object sender, EventArgs args)
		{
			OnClosePressed.Raise(this);
		}

		private void PresetButtonListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnPresetPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		#endregion
	}
}