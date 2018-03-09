using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class SourceSelectView : AbstractView, ISourceSelectView
	{
		private const ushort SUBPAGE_DUAL_DISPLAYS = 112;
		private const ushort SUBPAGE_8 = 101;
		private const ushort SUBPAGE_4 = 102;
		private const ushort SUBPAGE_3 = 103;
		private const ushort SUBPAGE_2 = 104;

		private static readonly ushort[] s_SubpageJoins =
		{
			SUBPAGE_DUAL_DISPLAYS,
			SUBPAGE_8,
			SUBPAGE_4,
			SUBPAGE_3,
			SUBPAGE_2
		};

		private readonly List<IReferencedSourceSelectView> m_ChildList;

		private ushort m_SourceCount;
		private ushort m_DisplayCount;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SourceSelectView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSourceSelectView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedSourceSelectView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_SourceList, m_ChildList, count);
		}

		/// <summary>
		/// Sets the number of sources that are available.
		/// </summary>
		/// <param name="count"></param>
		public void SetSourceCount(ushort count)
		{
			m_SourceCount = count;
			UpdateVisibility();
		}

		/// <summary>
		/// Sets the number of displays that are available.
		/// </summary>
		/// <param name="count"></param>
		public void SetDisplayCount(ushort count)
		{
			m_DisplayCount = count;
			UpdateVisibility();
		}

		/// <summary>
		/// SourceSelectView is actually controlling the visibility of 5 overlapped
		/// subpages, so we toggle between which subpages are visible based on the
		/// number of sources and destinations.
		/// </summary>
		private void UpdateVisibility()
		{
			ushort visible = SUBPAGE_DUAL_DISPLAYS;

			if (m_DisplayCount < 2)
			{
				visible = SUBPAGE_8;

				if (m_SourceCount <= 4)
					visible = SUBPAGE_4;

				if (m_SourceCount <= 3)
					visible = SUBPAGE_3;

				if (m_SourceCount <= 2)
					visible = SUBPAGE_2;
			}

			foreach (ushort subpage in s_SubpageJoins)
				Panel.SendInputDigital(subpage, subpage == visible);

			m_LeftArrowButton.Show(m_SourceCount > 4);
			m_RightArrowButton.Show(m_SourceCount > 4);
		}
	}
}
