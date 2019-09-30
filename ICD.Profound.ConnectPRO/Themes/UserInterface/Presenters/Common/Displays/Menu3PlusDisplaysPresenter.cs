using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenu3PlusDisplaysPresenter))]
	public sealed class Menu3PlusDisplaysPresenter : AbstractDisplaysPresenter<IMenu3PlusDisplaysView>, IMenu3PlusDisplaysPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedDisplayPresenterFactory m_PresenterFactory;

		public Menu3PlusDisplaysPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new ReferencedDisplayPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
		}

		protected override void Refresh(IMenu3PlusDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IReferencedDisplayPresenter presenter in m_PresenterFactory.BuildChildren(Displays))
				{
					presenter.ShowView(true);
					presenter.Refresh();
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedDisplayView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#region Child Callbacks

		private void Subscribe(IReferencedDisplayPresenter presenter)
		{
			presenter.OnDisplayPressed += PresenterOnDisplayPressed;
			presenter.OnDisplaySpeakerPressed += PresenterOnDisplaySpeakerPressed;
		}

		private void Unsubscribe(IReferencedDisplayPresenter presenter)
		{
			presenter.OnDisplayPressed -= PresenterOnDisplayPressed;
			presenter.OnDisplaySpeakerPressed -= PresenterOnDisplaySpeakerPressed;
		}

		private void PresenterOnDisplayPressed(object sender, EventArgs e)
		{
			var presenter = sender as IReferencedDisplayPresenter;
			if (presenter == null || presenter.Model == null)
				return;

			DisplayButtonPressed(presenter.Model);
		}
		
		private void PresenterOnDisplaySpeakerPressed(object sender, EventArgs e)
		{
			var presenter = sender as IReferencedDisplayPresenter;
			if (presenter == null || presenter.Model == null)
				return;

			DisplaySpeakerButtonPressed(presenter.Model);
		}

		#endregion
	}
}
