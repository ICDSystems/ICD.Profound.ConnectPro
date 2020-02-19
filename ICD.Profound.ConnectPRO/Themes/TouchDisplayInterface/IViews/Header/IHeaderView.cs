using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header
{
	public interface IHeaderView : ITouchDisplayView
	{
		event EventHandler OnCenterButtonPressed;
		event EventHandler OnCollapseButtonPressed;

		void SetRoomName(string name);

		void SetTimeLabel(string time);

		void SetCenterButtonMode(eCenterButtonMode mode);

		void SetCenterButtonSelected(bool selected);

		void SetCenterButtonEnabled(bool enabled);

		void SetCenterButtonText(string text);

		void SetCollapsed(bool collapsed);

		IEnumerable<IReferencedHeaderButtonView> GetLeftButtonViews(ITouchDisplayViewFactory factory, ushort count);
		IEnumerable<IReferencedHeaderButtonView> GetRightButtonViews(ITouchDisplayViewFactory factory, ushort count);
	}

	public enum eCenterButtonMode : ushort
	{
		InstantMeeting = 0,
		DeviceDrawer = 1
	}
}