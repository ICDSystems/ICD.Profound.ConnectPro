using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	[ViewBinding(typeof(ISourceSelectView))]
	public sealed partial class SourceSelectView : AbstractUiView, ISourceSelectView
	{
		/// <summary>
		/// Single row of sources along the top half of the panel.
		/// </summary>
		private const ushort SUBPAGE_DUAL_DISPLAYS = 112;

		/// <summary>
		/// Full 4x2 grid of sources.
		/// </summary>
		private const ushort SUBPAGE_8 = 101;

		/// <summary>
		/// 4 sources along the middle of the screen.
		/// </summary>
		private const ushort SUBPAGE_4 = 102;

		/// <summary>
		/// 3 sources along the middle of the screen.
		/// </summary>
		private const ushort SUBPAGE_3 = 103;

		/// <summary>
		/// 2 sources along the middle of the screen.
		/// </summary>
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
		private bool m_Combined;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SourceSelectView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSourceSelectView>();
		}

		#region Methods

		/// <summary>
		/// Sets the number of sources that are available.
		/// </summary>
		/// <param name="count"></param>
		public void SetSourceCount(ushort count)
		{
			if (count == m_SourceCount)
				return;

			m_SourceCount = count;

			UpdateVisibility();
		}

		/// <summary>
		/// Sets the number of displays that are available.
		/// </summary>
		/// <param name="count"></param>
		public void SetDisplayCount(ushort count)
		{
			if (count == m_DisplayCount)
				return;

			m_DisplayCount = count;

			UpdateVisibility();
		}

		/// <summary>
		/// Sets the combined state of the room.
		/// </summary>
		public void SetCombined(bool combined)
		{
			if (combined == m_Combined)
				return;

			m_Combined = combined;

			UpdateVisibility();
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
		/// Scrolls back to the first item in the list.
		/// </summary>
		public void ResetScrollPosition()
		{
			m_SourceList.ScrollToItem(0);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// SourceSelectView is actually controlling the visibility of 5 overlapped
		/// subpages, so we toggle between which subpages are visible based on the
		/// number of sources and destinations.
		/// </summary>
		private void UpdateVisibility()
		{
			// Default to top row
			ushort visible = SUBPAGE_DUAL_DISPLAYS;

			// If we're not combined and there are fewer than 2 displays use one of the source subpages
			if (!m_Combined && m_DisplayCount < 2)
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

		#endregion
	}
}
