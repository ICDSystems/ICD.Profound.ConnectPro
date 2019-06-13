using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Conference
{
	public partial class OsdConferenceView
	{
		private VtProSubpage m_Subpage;

		//left panel
		private VtProSimpleLabel m_CurrentBookingTimeLabel;
		private VtProSimpleLabel m_CurrentBookingNameLabel;
		private VtProSimpleLabel m_CurrentBookingHostLabel;

		// right panel
		private VtProDynamicIconObject m_SourceIcon;
		private VtProSimpleLabel m_SourceNameLabel;
		private VtProSimpleLabel m_SourceDescriptionLabel;

		// connecting banner
		private VtProImageObject m_ConnectingBanner;

		// disconnecting banner
		private VtProImageObject m_DisconnectingBanner;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 600
			};

			m_CurrentBookingTimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 601,
				DigitalVisibilityJoin = 601
			};

			m_CurrentBookingNameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 602,
				DigitalVisibilityJoin = 602
			};

			m_CurrentBookingHostLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 603
			};

			m_SourceIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 604
			};

			m_SourceNameLabel = new VtProSimpleLabel(Panel, m_Subpage)
			{
				IndirectTextJoin = 605
			};

			m_SourceDescriptionLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 606
			};

			m_ConnectingBanner = new VtProImageObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 606
			};

			m_DisconnectingBanner = new VtProImageObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 607
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CurrentBookingTimeLabel;
			yield return m_CurrentBookingNameLabel;
			yield return m_CurrentBookingHostLabel;
			yield return m_SourceIcon;
			yield return m_SourceNameLabel;
			yield return m_SourceDescriptionLabel;
			yield return m_ConnectingBanner;
			yield return m_DisconnectingBanner;
		}
	}
}
