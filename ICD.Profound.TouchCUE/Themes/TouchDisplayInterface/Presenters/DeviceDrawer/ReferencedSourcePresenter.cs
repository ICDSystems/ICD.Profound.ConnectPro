using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.DeviceDrawer
{
	[PresenterBinding(typeof(IReferencedSourcePresenter))]
	public sealed class ReferencedSourcePresenter : AbstractTouchDisplayComponentPresenter<IReferencedSourceView>, IReferencedSourcePresenter
	{
		public event EventHandler OnSourcePressed;

		private readonly SafeCriticalSection m_RefreshSection;

		public ISource Source { get; set; }

		public eSourceState SourceState { get; set; }

		public ReferencedSourcePresenter(INavigationController nav, IViewFactory views, TouchCueTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IReferencedSourceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetIcon(TouchCueIcons.GetSourceIcon(Source, eTouchCueColor.White));

				string sourceName = Source.GetName(Room.IsCombineRoom()) ?? string.Empty;
				view.SetNameText(sourceName);

				string description = Source.Description ?? string.Empty;
				view.SetDescriptionText(description);

				view.SetButtonMode(GetButtonMode(SourceState));
				view.SetButtonEnabled(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private static eDeviceButtonMode GetButtonMode(eSourceState state)
		{
			switch (state)
			{
				case eSourceState.Active:
					return eDeviceButtonMode.Active;
				case eSourceState.Processing:
					return eDeviceButtonMode.Processing;
				case eSourceState.Masked:
				case eSourceState.Inactive:
				default:
					return eDeviceButtonMode.Inactive;
			}
		}
		
		#region View Callbacks

		protected override void Subscribe(IReferencedSourceView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		protected override void Unsubscribe(IReferencedSourceView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, EventArgs e)
		{
			OnSourcePressed.Raise(this);
		}

		#endregion
	}
}
