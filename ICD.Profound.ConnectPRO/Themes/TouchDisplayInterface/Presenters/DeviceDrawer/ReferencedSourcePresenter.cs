using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.DeviceDrawer
{
	[PresenterBinding(typeof(IReferencedSourcePresenter))]
	public sealed class ReferencedSourcePresenter : AbstractTouchDisplayComponentPresenter<IReferencedSourceView>, IReferencedSourcePresenter
	{
		public event EventHandler OnSourcePressed;

		private readonly SafeCriticalSection m_RefreshSection;

		public ISource Source { get; set; }

		public ReferencedSourcePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IReferencedSourceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				ConnectProSource connectProSource = Source as ConnectProSource;
				if (connectProSource != null && connectProSource.Icon != null)
					view.SetIcon(connectProSource.Icon);
				else
					view.SetIcon("laptop");

				string sourceName = Source.GetName(Room.IsCombineRoom()) ?? string.Empty;
				view.SetNameText(sourceName);

				string description = Source.Description ?? string.Empty;
				view.SetDescriptionText(description);
			}
			finally
			{
				m_RefreshSection.Leave();
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
