using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes
{
	public abstract class AbstractConnectProUserInterfaceFactory<TUserInterface, TOriginator> :
		IConnectProUserInterfaceFactory
		where TUserInterface : IUserInterface
		where TOriginator : class, IOriginator
	{
		private readonly ConnectProTheme m_Theme;

		private readonly IcdHashSet<TUserInterface> m_UserInterfaces;
		private readonly SafeCriticalSection m_UserInterfacesSection;

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

		#region Properties

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public IEnumerable<IUserInterface> GetUserInterfaces()
		{
			m_UserInterfacesSection.Enter();

			try
			{
				return m_UserInterfaces.Cast<IUserInterface>().ToArray(m_UserInterfaces.Count);
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
			
		}

		#endregion

		#region Methods

		/// <summary>
		/// Disposes the instantiated UIs.
		/// </summary>
		public void Clear()
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
		/// Instantiates the user interfaces for the originators in the core.
		/// </summary>
		/// <returns></returns>
		public void BuildUserInterfaces()
		{
			m_UserInterfacesSection.Enter();

			try
			{
				Clear();

				var ids = new IcdHashSet<int>();
				GetRooms().ForEach(r => ids.AddRange(r.Originators.GetIds()));

				IEnumerable <TUserInterface> uis = GetOriginatorsForUserInterface().Where(o => ids.Contains(o.Id)).Select(originator => CreateUserInterface(originator));

				if (uis.Any())
				{
					m_UserInterfaces.AddRange(uis);
				}

				AssignUserInterfaces(GetRooms());
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
		}

		/// <summary>
		/// Assigns the rooms to the existing user interfaces.
		/// </summary>
		public void AssignUserInterfaces(IEnumerable<IConnectProRoom> rooms)
		{
			m_UserInterfacesSection.Enter();

			try
			{
				IcdHashSet<TUserInterface> visited = new IcdHashSet<TUserInterface>();

				foreach (IConnectProRoom room in rooms)
				{
					foreach (TUserInterface ui in m_UserInterfaces)
					{
						if (visited.Contains(ui))
						{
							m_Theme.Log(eSeverity.Warning,
								"Unable to assign {0} to {1} - A different room is already assigned", room,
								typeof(TUserInterface).Name);
							continue;
						}

						if (RoomContainsOriginator(room, ui))
						{
							ui.SetRoom(room);
							visited.Add(ui);
						}
					}
				}
			}
			finally
			{
				m_UserInterfacesSection.Leave();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Override to control which originators are selected for UI instantiation.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<TOriginator> GetOriginatorsForUserInterface()
		{
			return m_Theme.Core.Originators.GetChildren<TOriginator>();
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		protected abstract TUserInterface CreateUserInterface(TOriginator originator);

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
