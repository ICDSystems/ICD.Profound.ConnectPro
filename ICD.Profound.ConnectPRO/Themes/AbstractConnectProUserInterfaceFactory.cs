using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPRO.Themes
{
	public abstract class AbstractConnectProUserInterfaceFactory<TUserInterface, TOriginator> : IConnectProUserInterfaceFactory
		where TUserInterface : IUserInterface
		where TOriginator : IOriginator
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

				IEnumerable<TUserInterface> uis =
					m_Theme.Core
					       .Originators
					       .OfType<TOriginator>()
					       .Select(originator => CreateUserInterface(originator));

				m_UserInterfaces.AddRange(uis);

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
		public void AssignUserInterfaces(IEnumerable<IRoom> rooms)
		{
			m_UserInterfacesSection.Enter();

			try
			{
				foreach (IRoom room in rooms)
				{
					foreach (TUserInterface ui in m_UserInterfaces)
					{
						if (RoomContainsOriginator(room, ui))
							ui.SetRoom(room);
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
		private IEnumerable<IRoom> GetRooms()
		{
			return m_Theme.Core.Originators.GetChildren<IRoom>();
		}

		#endregion
	}
}
