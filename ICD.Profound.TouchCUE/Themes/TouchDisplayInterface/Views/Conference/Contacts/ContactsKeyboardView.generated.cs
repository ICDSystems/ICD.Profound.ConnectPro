using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.Contacts
{
	public sealed partial class ContactsKeyboardView
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

		private VtProButton m_KeyQButton;
		private VtProButton m_KeyWButton;
		private VtProButton m_KeyEButton;
		private VtProButton m_KeyRButton;
		private VtProButton m_KeyTButton;
		private VtProButton m_KeyYButton;
		private VtProButton m_KeyUButton;
		private VtProButton m_KeyIButton;
		private VtProButton m_KeyOButton;
		private VtProButton m_KeyPButton;

		private VtProButton m_KeyAButton;
		private VtProButton m_KeySButton;
		private VtProButton m_KeyDButton;
		private VtProButton m_KeyFButton;
		private VtProButton m_KeyGButton;
		private VtProButton m_KeyHButton;
		private VtProButton m_KeyJButton;
		private VtProButton m_KeyKButton;
		private VtProButton m_KeyLButton;

		private VtProButton m_KeyZButton;
		private VtProButton m_KeyXButton;
		private VtProButton m_KeyCButton;
		private VtProButton m_KeyVButton;
		private VtProButton m_KeyBButton;
		private VtProButton m_KeyNButton;
		private VtProButton m_KeyMButton;
		
		private VtProButton m_KeyPeriodButton;
		private VtProButton m_KeyCommaButton;
		private VtProButton m_KeySlashButton;
		private VtProButton m_KeySemiColonButton;
		private VtProButton m_KeyApostropheButton;

		private VtProSimpleLabel m_FeedbackText;
		private VtProTextEntry m_TextEntry;

		private VtProButton m_BackspaceButton;
		private VtProButton m_ShiftButton;
		private VtProButton m_CapsButton;
		private VtProButton m_SpaceButton;
		private VtProButton m_CloseButton;

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
				DigitalVisibilityJoin = 2690
			};

			m_Key0Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2700
			};
			m_Key0Button.DigitalLabelJoins.Add(2698);

			m_Key1Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2701
			};
			m_Key1Button.DigitalLabelJoins.Add(2698);

			m_Key2Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2702
			};
			m_Key2Button.DigitalLabelJoins.Add(2698);

			m_Key3Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2703
			};
			m_Key3Button.DigitalLabelJoins.Add(2698);

			m_Key4Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2704
			};
			m_Key4Button.DigitalLabelJoins.Add(2698);

			m_Key5Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2705
			};
			m_Key5Button.DigitalLabelJoins.Add(2698);

			m_Key6Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2706
			};
			m_Key6Button.DigitalLabelJoins.Add(2698);

			m_Key7Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2707
			};
			m_Key7Button.DigitalLabelJoins.Add(2698);

			m_Key8Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2708
			};
			m_Key8Button.DigitalLabelJoins.Add(2698);

			m_Key9Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2709
			};
			m_Key9Button.DigitalLabelJoins.Add(2698);

			m_KeyQButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2713
			};
			m_KeyQButton.DigitalLabelJoins.Add(2699);

			m_KeyWButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2714
			};
			m_KeyWButton.DigitalLabelJoins.Add(2699);

			m_KeyEButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2715
			};
			m_KeyEButton.DigitalLabelJoins.Add(2699);

			m_KeyRButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2716
			};
			m_KeyRButton.DigitalLabelJoins.Add(2699);

			m_KeyTButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2717
			};
			m_KeyTButton.DigitalLabelJoins.Add(2699);

			m_KeyYButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2718
			};
			m_KeyYButton.DigitalLabelJoins.Add(2699);

			m_KeyUButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2719
			};
			m_KeyUButton.DigitalLabelJoins.Add(2699);

			m_KeyIButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2720
			};
			m_KeyIButton.DigitalLabelJoins.Add(2699);

			m_KeyOButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2721
			};
			m_KeyOButton.DigitalLabelJoins.Add(2699);

			m_KeyPButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2722
			};
			m_KeyPButton.DigitalLabelJoins.Add(2699);

			m_KeyAButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2725
			};
			m_KeyAButton.DigitalLabelJoins.Add(2699);

			m_KeySButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2726
			};
			m_KeySButton.DigitalLabelJoins.Add(2699);

			m_KeyDButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2727
			};
			m_KeyDButton.DigitalLabelJoins.Add(2699);

			m_KeyFButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2728
			};
			m_KeyFButton.DigitalLabelJoins.Add(2699);

			m_KeyGButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2729
			};
			m_KeyGButton.DigitalLabelJoins.Add(2699);

			m_KeyHButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2730
			};
			m_KeyHButton.DigitalLabelJoins.Add(2699);

			m_KeyJButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2731
			};
			m_KeyJButton.DigitalLabelJoins.Add(2699);

			m_KeyKButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2732
			};
			m_KeyKButton.DigitalLabelJoins.Add(2699);

			m_KeyLButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2733
			};
			m_KeyLButton.DigitalLabelJoins.Add(2699);

			m_KeyZButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2737
			};
			m_KeyZButton.DigitalLabelJoins.Add(2699);

			m_KeyXButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2738
			};
			m_KeyXButton.DigitalLabelJoins.Add(2699);

			m_KeyCButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2739
			};
			m_KeyCButton.DigitalLabelJoins.Add(2699);

			m_KeyVButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2740
			};
			m_KeyVButton.DigitalLabelJoins.Add(2699);

			m_KeyBButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2741
			};
			m_KeyBButton.DigitalLabelJoins.Add(2699);

			m_KeyNButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2742
			};
			m_KeyNButton.DigitalLabelJoins.Add(2699);

			m_KeyMButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2743
			};
			m_KeyMButton.DigitalLabelJoins.Add(2699);

			m_KeyPeriodButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2760
			};
			m_KeyPeriodButton.DigitalLabelJoins.Add(2698);

			m_KeyCommaButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2751
			};
			m_KeyCommaButton.DigitalLabelJoins.Add(2698);

			m_KeySlashButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2761
			};
			m_KeySlashButton.DigitalLabelJoins.Add(2698);

			m_KeySemiColonButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2762
			};
			m_KeySemiColonButton.DigitalLabelJoins.Add(2698);

			m_KeyApostropheButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2763
			};
			m_KeyApostropheButton.DigitalLabelJoins.Add(2698);

			m_FeedbackText = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2700
			};

			m_TextEntry = new VtProTextEntry(panel, m_Subpage)
			{
				SerialOutputJoin = 3033
			};
			m_TextEntry.SerialLabelJoins.Add(3033);

			m_BackspaceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2752
			};

			m_ShiftButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2736
			};

			m_CapsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2724
			};

			m_SpaceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2747
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2755
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

			yield return m_KeyQButton;
			yield return m_KeyWButton;
			yield return m_KeyEButton;
			yield return m_KeyRButton;
			yield return m_KeyTButton;
			yield return m_KeyYButton;
			yield return m_KeyUButton;
			yield return m_KeyIButton;
			yield return m_KeyOButton;
			yield return m_KeyPButton;

			yield return m_KeyAButton;
			yield return m_KeySButton;
			yield return m_KeyDButton;
			yield return m_KeyFButton;
			yield return m_KeyGButton;
			yield return m_KeyHButton;
			yield return m_KeyJButton;
			yield return m_KeyKButton;
			yield return m_KeyLButton;

			yield return m_KeyZButton;
			yield return m_KeyXButton;
			yield return m_KeyCButton;
			yield return m_KeyVButton;
			yield return m_KeyBButton;
			yield return m_KeyNButton;
			yield return m_KeyMButton;
			
			yield return m_KeyPeriodButton;
			yield return m_KeyCommaButton;
			yield return m_KeySlashButton;
			yield return m_KeySemiColonButton;
			yield return m_KeyApostropheButton;

			yield return m_FeedbackText;
			yield return m_TextEntry;
			yield return m_BackspaceButton;
			yield return m_ShiftButton;
			yield return m_CapsButton;
			yield return m_SpaceButton;
			yield return m_CloseButton;
		}
	}
}
