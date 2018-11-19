using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcReferencedContactsPresenterFactory :
		AbstractUiListItemFactory<ModelPresenterTypeInfo, IVtcReferencedContactsPresenterBase, IVtcReferencedContactsView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public VtcReferencedContactsPresenterFactory(IConnectProNavigationController navigationController,
		                                             ListItemFactory<IVtcReferencedContactsView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(ModelPresenterTypeInfo model, IVtcReferencedContactsPresenterBase presenter,
		                                     IVtcReferencedContactsView view)
		{
			presenter.SetView(view);
			presenter.SetModel(model.Model);
		}

		/// <summary>
		/// Gets the presenter type for the given model instance.
		/// Override to fill lists with different presenters.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		protected override Type GetPresenterTypeForModel(ModelPresenterTypeInfo model)
		{
			switch (model.PresenterType)
			{
				case ModelPresenterTypeInfo.ePresenterType.Folder:
					return typeof(IVtcReferencedFolderPresenter);
				case ModelPresenterTypeInfo.ePresenterType.Contact:
					return typeof(IVtcReferencedContactsPresenter);
				case ModelPresenterTypeInfo.ePresenterType.Favorite:
					return typeof(IVtcReferencedFavoritesPresenter);
				case ModelPresenterTypeInfo.ePresenterType.Recent:
					return typeof(IVtcReferencedRecentPresenter);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public struct ModelPresenterTypeInfo
	{
		public enum ePresenterType
		{
			Folder,
			Contact,
			Favorite,
			Recent
		}

		private readonly ePresenterType m_PresenterType;
		private readonly object m_Model;

		public ePresenterType PresenterType { get { return m_PresenterType; } }

		public object Model { get { return m_Model; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presenterType"></param>
		/// <param name="model"></param>
		public ModelPresenterTypeInfo(ePresenterType presenterType, object model)
		{
			m_PresenterType = presenterType;
			m_Model = model;
		}
	}
}
