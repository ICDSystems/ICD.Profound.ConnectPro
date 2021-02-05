using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	[PresenterBinding(typeof(IReferencedSettingsTouchFreePresenter))]
	public sealed class ReferencedSettingsTouchFreePresenter : AbstractUiComponentPresenter<IReferencedSettingsTouchFreeView>,
														  IReferencedSettingsTouchFreePresenter
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private ISource m_Source;
		private bool m_Selected;

		#region Properties

		/// <summary>
		/// Gets/sets the source for this presenter.
		/// </summary>
		public ISource Source
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				m_Source = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the selected state of the presenter.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSettingsTouchFreePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedSettingsTouchFreeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				ConnectProSource cpSource = m_Source as ConnectProSource;

				string name = m_Source == null ? null : m_Source.Name;
				string icon = cpSource == null
					? Icons.GetSourceIcon(null, eSourceColor.White)
					: Icons.GetSourceIcon(cpSource.Icon, eSourceColor.White);

				view.SetSelected(m_Selected);
				view.SetText(name);
				view.SetIcon(icon);
				view.Enable(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedSettingsTouchFreeView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedSettingsTouchFreeView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}