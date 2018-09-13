using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class ReferencedScheduleView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_BackgroundButton;
		private VtProDynamicIconObject m_BookingIcon;
		private VtProSimpleLabel m_StartTimeLabel;
		private VtProSimpleLabel m_BodyLabel;
		private VtProSimpleLabel m_EndTimeLabel;
		private VtProSimpleLabel m_PresenterNameLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				DigitalEnableJoin = 2
			};

			m_BookingIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 1
			};

			m_StartTimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};

			m_BodyLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3
			};

			m_EndTimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 4
			};

			m_PresenterNameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 5
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_BackgroundButton;
			yield return m_BookingIcon;
			yield return m_StartTimeLabel;
			yield return m_BodyLabel;
			yield return m_EndTimeLabel;
			yield return m_PresenterNameLabel;
		}
	}
}
