using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.DeviceDrawer
{
	[ViewBinding(typeof(IDeviceDrawerView))]
	public sealed partial class DeviceDrawerView : AbstractTouchDisplayView, IDeviceDrawerView
	{
		private readonly List<IReferencedSourceView> m_ChildList;

		public DeviceDrawerView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSourceView>();
		}

		public IEnumerable<IReferencedSourceView> GetChildComponentViews(ITouchDisplayViewFactory views, ushort count)
		{
			return GetChildViews(views, m_SourceList, m_ChildList, count);
		}
	}
}