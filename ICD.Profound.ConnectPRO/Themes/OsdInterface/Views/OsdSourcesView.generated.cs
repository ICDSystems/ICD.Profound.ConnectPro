﻿using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public sealed partial class OsdSourcesView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomLabel;

		private VtProSimpleLabel m_SourceLabel1;
		private VtProSimpleLabel m_SourceLabel2;
		private VtProSimpleLabel m_SourceLabel3;
		private VtProSimpleLabel m_SourceLabel4;
		private VtProSimpleLabel m_SourceLabel5;
		private VtProSimpleLabel m_SourceLabel6;
		private VtProSimpleLabel m_SourceLabel7;
		private VtProSimpleLabel m_SourceLabel8;

		private VtProDynamicIconObject m_SourceIcon1;
		private VtProDynamicIconObject m_SourceIcon2;
		private VtProDynamicIconObject m_SourceIcon3;
		private VtProDynamicIconObject m_SourceIcon4;
		private VtProDynamicIconObject m_SourceIcon5;
		private VtProDynamicIconObject m_SourceIcon6;
		private VtProDynamicIconObject m_SourceIcon7;
		private VtProDynamicIconObject m_SourceIcon8;

		private Dictionary<ushort, VtProSimpleLabel> m_Labels;
		private Dictionary<ushort, VtProDynamicIconObject> m_Icons;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 200
			};

			m_RoomLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 201
			};

			// Source labels
			m_SourceLabel1 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 202
			};

			m_SourceLabel2 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 203
			};

			m_SourceLabel3 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 204
			};

			m_SourceLabel4 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 205
			};

			m_SourceLabel5 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 206
			};

			m_SourceLabel6 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 207
			};

			m_SourceLabel7 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 208
			};

			m_SourceLabel8 = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 209
			};

			// Source Icons
			m_SourceIcon1 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 210
			};

			m_SourceIcon2 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 211
			};

			m_SourceIcon3 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 212
			};

			m_SourceIcon4 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 213
			};

			m_SourceIcon5 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 214
			};

			m_SourceIcon6 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 215
			};

			m_SourceIcon7 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 216
			};

			m_SourceIcon8 = new VtProDynamicIconObject(panel, m_Subpage)
			{
				IndirectTextJoin = 217
			};

			m_Labels = new Dictionary<ushort, VtProSimpleLabel>
			{
				{ 0, m_SourceLabel1 },
				{ 1, m_SourceLabel2 },
				{ 2, m_SourceLabel3 },
				{ 3, m_SourceLabel4 },
				{ 4, m_SourceLabel5 },
				{ 5, m_SourceLabel6 },
				{ 6, m_SourceLabel7 },
				{ 7, m_SourceLabel8 },
			};

			m_Icons = new Dictionary<ushort, VtProDynamicIconObject>
			{
				{ 0, m_SourceIcon1 },
				{ 1, m_SourceIcon2 },
				{ 2, m_SourceIcon3 },
				{ 3, m_SourceIcon4 },
				{ 4, m_SourceIcon5 },
				{ 5, m_SourceIcon6 },
				{ 6, m_SourceIcon7 },
				{ 7, m_SourceIcon8 },
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomLabel;

			yield return m_SourceLabel1;
			yield return m_SourceLabel2;
			yield return m_SourceLabel3;
			yield return m_SourceLabel4;
			yield return m_SourceLabel5;
			yield return m_SourceLabel6;
			yield return m_SourceLabel7;
			yield return m_SourceLabel8;

			yield return m_SourceIcon1;
			yield return m_SourceIcon2;
			yield return m_SourceIcon3;
			yield return m_SourceIcon4;
			yield return m_SourceIcon5;
			yield return m_SourceIcon6;
			yield return m_SourceIcon7;
			yield return m_SourceIcon8;
		}
	}
}
