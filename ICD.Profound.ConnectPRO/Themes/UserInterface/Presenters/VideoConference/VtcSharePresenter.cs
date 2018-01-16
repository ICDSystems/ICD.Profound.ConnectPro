using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcSharePresenter : AbstractPresenter<IVtcShareView>, IVtcSharePresenter
	{
		private const int SOURCE_COUNT = 8;

		private ISource[] m_Sources;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcSharePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Sources = new ISource[0];
		}

		protected override void Refresh(IVtcShareView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				m_Sources =
					Room == null
						? new ISource[0]
						: Room.Routing.GetCoreSources().ToArray();

				for (ushort index = 0; index < SOURCE_COUNT; index++)
				{
					ISource source;
					m_Sources.TryElementAt(index, out source);

					ConnectProSource connectProSource = source as ConnectProSource;
					IRoom room = Room == null || source == null ? null : Room.Routing.GetRoomForSource(source);
					bool combine = room != null && room.CombineState;

					view.SetButtonIcon(index, connectProSource == null ? null : connectProSource.Icon);
					view.SetButtonLabel(index, source == null ? null : source.GetNameOrDeviceName(combine));
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IVtcShareView view)
		{
			base.Subscribe(view);

			view.OnSourceButtonPressed += ViewOnSourceButtonPressed;
		}

		protected override void Unsubscribe(IVtcShareView view)
		{
			base.Unsubscribe(view);

			view.OnSourceButtonPressed -= ViewOnSourceButtonPressed;
		}

		private void ViewOnSourceButtonPressed(object sender, UShortEventArgs uShortEventArgs)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
