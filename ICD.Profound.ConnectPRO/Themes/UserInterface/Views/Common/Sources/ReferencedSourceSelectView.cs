using System;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class ReferencedSourceSelectView : AbstractComponentView, IReferencedSourceSelectView
	{
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ReferencedSourceSelectView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetColorMode(ushort color)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIconMode(ushort icon)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine1Text(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetLine2Text(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the text for the feedback label.
		/// </summary>
		/// <param name="text"></param>
		public void SetFeedbackText(string text)
		{
			throw new NotImplementedException();
		}
	}
}
