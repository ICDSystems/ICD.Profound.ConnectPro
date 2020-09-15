using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.HeaderNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.VisibilityTree
{
	public sealed class OsdVisibilityTree
	{
		// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
		private readonly IVisibilityNode m_HeaderNotificationVisibility;
		private readonly IVisibilityNode m_BodyVisibility;
		private readonly IVisibilityNode m_FooterNotificationVisibility;
		// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

		public IVisibilityNode BodyVisibility { get { return m_BodyVisibility; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		public OsdVisibilityTree(INavigationController navigationController)
		{
			// Header notifications at the top of the CUE
			m_HeaderNotificationVisibility = new SingleVisibilityNode();
			m_HeaderNotificationVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdTouchFreeHeaderNotificationPresenter>());
			m_HeaderNotificationVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdUpcomingMeetingIndicatorPresenter>());

			// Main presenters occupying the middle body portion of the CUE
			m_BodyVisibility = new SingleVisibilityNode();
			m_BodyVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdScheduleBodyPresenter>());
			m_BodyVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdSourcesBodyPresenter>());
			m_BodyVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdConferenceBodyPresenter>());

			// Notifications at the bottom of the CUE
			m_FooterNotificationVisibility = new NotificationVisibilityNode(navigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>());
			m_FooterNotificationVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdIncomingCallFooterNotificationPresenter>());
			m_FooterNotificationVisibility.AddPresenter(navigationController.LazyLoadPresenter<IOsdMuteFooterNotificationPresenter>());

			// These presenters are initially visible
			navigationController.NavigateTo<IOsdHelloFooterNotificationPresenter>();

			// Always visible
			navigationController.LazyLoadPresenter<IOsdHeaderPresenter>();
			navigationController.LazyLoadPresenter<IOsdBackgroundPresenter>();
		}
	}
}