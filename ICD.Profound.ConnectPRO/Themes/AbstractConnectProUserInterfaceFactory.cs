using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes
{
	public abstract class AbstractConnectProUserInterfaceFactory<TUserInterface> : AbstractUserInterfaceFactory
		where TUserInterface : IUserInterface
	{
		private readonly ConnectProTheme m_Theme;

		private readonly IcdHashSet<TUserInterface> m_UserInterfaces;
		private readonly SafeCriticalSection m_UserInterfacesSection;

		/// <summary>
		/// Gets the theme.
		/// </summary>
		protected ConnectProTheme Theme { get { return m_Theme; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		protected AbstractConnectProUserInterfaceFactory(ConnectProTheme theme)
		{
			m_Theme = theme;

			m_UserInterfaces = new IcdHashSet<TUserInterface>();
			m_UserInterfacesSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Disposes the instantiated UIs.
		/// </summary>
		public override void Clear()
		{
			m_UserInterfacesSection.Enter();

			try
			{
				m_UserInterfaces.ForEach(ui => ui.Dispose());
				m_UserInterfaces.Clear();
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
		}

		/// <summary>
		/// Gets the instantiated user interfaces.
		/// </summary>
		public override IEnumerable<IUserInterface> GetUserInterfaces()
		{
			return m_UserInterfacesSection.Execute(() => m_UserInterfaces.Cast<IUserInterface>().ToArray());
		}

		/// <summary>
		/// Instantiates the user interfaces for the originators in the core.
		/// </summary>
		/// <returns></returns>
		public override void BuildUserInterfaces()
		{
			m_UserInterfacesSection.Enter();

			try
			{
				Clear();

				IEnumerable<TUserInterface> uis = GetRooms().SelectMany(r => CreateUserInterfaces(r));

				m_UserInterfaces.AddRange(uis);
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}

			ReassignUserInterfaces();
		}

		public override void ReassignUserInterfaces()
		{
			AssignUserInterfaces(GetRooms());
		}

		/// <summary>
		/// Assigns the rooms to the existing user interfaces.
		/// </summary>
		public override void AssignUserInterfaces(IEnumerable<IRoom> rooms)
		{
			AssignUserInterfaces(rooms.OfType<IConnectProRoom>());
		}

		/// <summary>
		/// Assigns the rooms to the existing user interfaces.
		/// </summary>
		public void AssignUserInterfaces(IEnumerable<IConnectProRoom> rooms)
		{
			m_UserInterfacesSection.Enter();

			try
			{
				foreach (IConnectProRoom room in rooms)
				{
					foreach (TUserInterface ui in m_UserInterfaces)
					{
						if (ui.Room == room)
							continue;

						// Determine if the room contains the originator assigned to the UI
						if (!RoomContainsOriginator(room, ui))
							continue;

						// The room assigned to the UI already contains the current room
						if (ui.Room != null && ui.Room.ContainsRoom(room))
							continue;

						// Determine if the UI was already setup for a different room
						if (ui.Room != null && !room.ContainsRoom(ui.Room))
						{
							m_Theme.Log(eSeverity.Warning,
							            "Unable to assign {0} to {1} - A different room is already assigned", room,
							            typeof(TUserInterface).Name);
							continue;
						}

						ui.SetRoom(room);
					}
				}
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
		}

		public override void ActivateUserInterfaces()
		{
			m_UserInterfacesSection.Enter();

			try
			{
				foreach (TUserInterface ui in m_UserInterfaces)
					ui.Activate();
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected abstract IEnumerable<TUserInterface> CreateUserInterfaces(IConnectProRoom room);

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected abstract bool RoomContainsOriginator(IRoom room, TUserInterface ui);

		/// <summary>
		/// Gets the rooms for the user interfaces. Combine rooms will override any child individual rooms.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConnectProRoom> GetRooms()
		{
			return m_Theme.Core.Originators.GetChildren<IConnectProRoom>();
		}

		#endregion
	}
}
