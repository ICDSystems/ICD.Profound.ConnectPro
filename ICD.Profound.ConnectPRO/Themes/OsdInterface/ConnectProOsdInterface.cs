﻿using ICD.Common.Utils;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProOsdInterface : IUserInterface
	{
		private readonly IPanelDevice m_Panel;
		private readonly IOsdNavigationController m_NavigationController;

		private IConnectProRoom m_Room;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProOsdInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOnlineJoin();

			IOsdViewFactory viewFactory = new ConnectProOsdViewFactory(panel, theme);
			m_NavigationController = new ConnectProOsdNavigationController(viewFactory, theme);
		}

		/// <summary>
		/// Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOnlineJoin()
		{
			m_Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			SetRoom(null);

			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			m_Room = room;

			m_NavigationController.SetRoom(room);

			UpdatePanelOnlineJoin();
		}

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);
			builder.AppendProperty("Panel", m_Panel);
			return builder.ToString();
		}

		#endregion
	}
}
