using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Administrative
{
	public sealed partial class SettingsClockView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_HelpLabel;
		private VtProButton m_24HourButton;
		private VtProButton m_HourIncrementButton;
		private VtProButton m_HourDecrementButton;
		private VtProButton m_MinuteIncrementButton;
		private VtProButton m_MinuteDecrementButton;
		private VtProButton m_AmPmButton;
		private VtProSimpleLabel m_HourLabel;
		private VtProSimpleLabel m_MinuteLabel;
		private VtProImageObject m_BackgroundImage;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 148
			};

			m_HelpLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 404
			};

			m_24HourButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 401
			};

			m_HourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 402
			};

			m_HourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 403
			};

			m_MinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 404
			};

			m_MinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 405
			};

			m_AmPmButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 409,
				DigitalVisibilityJoin = 400
			};

			m_HourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 405
			};

			m_MinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 406
			};

			m_BackgroundImage = new VtProImageObject(panel, m_Subpage)
			{
				ModeAnalogJoin = 400
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_HelpLabel;
			yield return m_24HourButton;
			yield return m_HourIncrementButton;
			yield return m_HourDecrementButton;
			yield return m_MinuteIncrementButton;
			yield return m_MinuteDecrementButton;
			yield return m_AmPmButton;
			yield return m_HourLabel;
			yield return m_MinuteLabel;
			yield return m_BackgroundImage;
		}
	}
}
