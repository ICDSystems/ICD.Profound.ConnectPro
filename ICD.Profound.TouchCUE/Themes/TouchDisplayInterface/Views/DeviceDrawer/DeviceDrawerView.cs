using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.DeviceDrawer
{
	[ViewBinding(typeof(IDeviceDrawerView))]
	public sealed partial class DeviceDrawerView : AbstractTouchDisplayView, IDeviceDrawerView
	{
		public event EventHandler<UShortEventArgs> OnAppButtonPressed; 

		private readonly List<IReferencedSourceView> m_ChildList;

		public DeviceDrawerView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSourceView>();
		}

		public IEnumerable<IReferencedSourceView> GetChildComponentViews(ITouchDisplayViewFactory views, ushort count)
		{
			return GetChildViews(views, m_SourceList, m_ChildList, count);
		}

		public void SetAppButtonIcons(IEnumerable<string> packageNames)
		{
			List<string> packageNamesList = packageNames.ToList();
			m_AppButtonList.SetNumberOfItems((ushort)packageNamesList.Count);
			for (ushort i = 0; i < packageNamesList.Count; i++)
			{
				m_AppButtonList.SetItemIcon(i, packageNamesList[i]);
				m_AppButtonList.SetItemVisible(i, true);
			}
		}

		public void SetAppButtonLabels(IEnumerable<string> appNames)
		{
			List<string> appNamesList = appNames.ToList();
			m_AppButtonList.SetNumberOfItems((ushort)appNamesList.Count);
			for (ushort i = 0; i < appNamesList.Count; i++)
			{
				m_AppButtonList.SetItemLabel(i, appNamesList[i]);
				m_AppButtonList.SetItemVisible(i, true);
			}
		}

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AppButtonList.OnButtonClicked += AppButtonListOnOnButtonClicked;
		}

		protected override void UnsubscribeControls()
		{
			base.SubscribeControls();

			m_AppButtonList.OnButtonClicked -= AppButtonListOnOnButtonClicked;
		}

		private void AppButtonListOnOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnAppButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}
	}
}