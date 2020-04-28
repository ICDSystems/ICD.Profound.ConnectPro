using System;
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Header
{
	[PresenterBinding(typeof(IReferencedHeaderButtonPresenter))]
	public sealed class ReferencedHeaderButtonPresenter : AbstractTouchDisplayComponentPresenter<IReferencedHeaderButtonView>, IReferencedHeaderButtonPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private HeaderButtonModel m_Model;
		public HeaderButtonModel Model
		{
			get { return m_Model; }
			set
			{
				if (m_Model == value)
					return;

				Unsubscribe(m_Model);
				m_Model = value;
				Subscribe(m_Model);
			}
		}

		public ReferencedHeaderButtonPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IReferencedHeaderButtonView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetIcon(Model.Selected ? null : Model.Icon);
				view.SetButtonEnabled(Model.Enabled);
				view.SetButtonMode(Model.Selected ? eHeaderButtonMode.Close : Model.Mode);
				view.SetLabelText(Model.LabelText);
				view.SetButtonBackgroundVisible(Model.Level > 0);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IReferencedHeaderButtonView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		protected override void Unsubscribe(IReferencedHeaderButtonView view)
		{
			base.Subscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		private void ViewOnPressed(object sender, EventArgs e)
		{
			if (Model != null && Model.Callback != null)
				Model.Callback();
		}

		#endregion

		#region Model Callbacks 

		private void Subscribe(HeaderButtonModel model)
		{
			if (model == null)
				return;

			model.OnPropertyChanged += ModelOnPropertyChanged;
		}

		private void Unsubscribe(HeaderButtonModel model)
		{
			if (model == null)
				return;

			model.OnPropertyChanged -= ModelOnPropertyChanged;
		}

		private void ModelOnPropertyChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
