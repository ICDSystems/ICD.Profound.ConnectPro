using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class SourceSelectDualView : AbstractSourceSelectView, ISourceSelectDualView
	{
		private readonly List<IReferencedSourceSelectView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SourceSelectDualView(ISigInputOutput panel, ConnectProTheme theme)
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
		public override IEnumerable<IReferencedSourceSelectView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_SourceList, m_ChildList, count);
		}
	}
}
