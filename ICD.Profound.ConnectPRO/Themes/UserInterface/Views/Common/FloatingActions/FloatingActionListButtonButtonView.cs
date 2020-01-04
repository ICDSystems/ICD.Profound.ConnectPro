using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	[ViewBinding(typeof(IFloatingActionListButtonView))]
	public sealed partial class FloatingActionListButtonButtonView : AbstractFloatingActionView, IFloatingActionListButtonView
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public FloatingActionListButtonButtonView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="active"></param>
		public override void SetActive(bool active)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the enabled state of the option.
		/// </summary>
		/// <param name="enabled"></param>
		public override void SetEnabled(bool enabled)
		{
			throw new NotImplementedException();
		}

		public void SetIcon(string icon)
		{
			throw new NotImplementedException();
		}
	}
}
