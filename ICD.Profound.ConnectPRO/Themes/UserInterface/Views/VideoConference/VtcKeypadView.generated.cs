using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcKeypadView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_Key0Button;
		private VtProButton m_Key1Button;
		private VtProButton m_Key2Button;
		private VtProButton m_Key3Button;
		private VtProButton m_Key4Button;
		private VtProButton m_Key5Button;
		private VtProButton m_Key6Button;
		private VtProButton m_Key7Button;
		private VtProButton m_Key8Button;
		private VtProButton m_Key9Button;

		private VtProTextEntry m_TextEntry;

		private VtProButton m_BackspaceButton;
		private VtProButton m_DialButton;
		private VtProButton m_KeyboardButton;

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
				DigitalVisibilityJoin = 131
			};

			m_Key0Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3741
			};

			m_Key1Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3728
			};

			m_Key2Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3729
			};

			m_Key3Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3730
			};

			m_Key4Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3718
			};

			m_Key5Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3719
			};

			m_Key6Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3720
			};

			m_Key7Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3706
			};

			m_Key8Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3707
			};

			m_Key9Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3708
			};

			m_TextEntry = new VtProTextEntry(panel, m_Subpage)
			{
				SerialOutputJoin = 3030
			};
			m_TextEntry.SerialLabelJoins.Add(3030);

			m_BackspaceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3752
			};

			m_DialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3754
			};

			m_KeyboardButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3755
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_Key0Button;
			yield return m_Key1Button;
			yield return m_Key2Button;
			yield return m_Key3Button;
			yield return m_Key4Button;
			yield return m_Key5Button;
			yield return m_Key6Button;
			yield return m_Key7Button;
			yield return m_Key8Button;
			yield return m_Key9Button;

			yield return m_TextEntry;
			yield return m_BackspaceButton;
			yield return m_DialButton;
			yield return m_KeyboardButton;
		}
	}
}