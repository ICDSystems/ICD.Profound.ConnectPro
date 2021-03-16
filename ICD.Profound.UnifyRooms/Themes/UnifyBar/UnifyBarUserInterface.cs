using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;
using ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar
{
	public sealed class UnifyBarUserInterface : AbstractUserInterface
	{
		private readonly UnifyBarDevice m_UnifyBar;
		private readonly UnifyRoomsTheme m_Theme;
		private readonly VolumeRepeater m_VolumeRepeater;
		private readonly VolumePointHelper m_VolumePointHelper;
		private readonly IUnifyBarButtonUi[] m_Buttons;

		private ICommercialRoom m_Room;

		#region Properties

		/// <summary>
		/// Gets the unify bar device.
		/// </summary>
		public UnifyBarDevice UnifyBar { get { return m_UnifyBar; } }

		/// <summary>
		/// Gets the room attached to this UI.
		/// </summary>
		public override IRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the target instance attached to this UI.
		/// </summary>
		public override object Target { get { return m_UnifyBar; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="unifyBar"></param>
		/// <param name="theme"></param>
		public UnifyBarUserInterface([NotNull] UnifyBarDevice unifyBar, [NotNull] UnifyRoomsTheme theme)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_UnifyBar = unifyBar;
			m_Theme = theme;
			m_VolumeRepeater = new VolumeRepeater();
			m_VolumePointHelper = new VolumePointHelper();

			m_Buttons = theme.UnifyBarButtons
			                 .Buttons
			                 .Where(b => b != eMainButton.None)
			                 .Take(6)
			                 .Select(b => InstantiateButton(b))
			                 .ToArray();

			Subscribe(m_UnifyBar);

			UpdateButtons();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			Unsubscribe(m_UnifyBar);
			SetRoom(null);

			m_VolumePointHelper.Dispose();
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IRoom room)
		{
			SetRoom(room as ICommercialRoom);
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(ICommercialRoom room)
		{
			if (room == m_Room)
				return;

			m_Room = room;

			foreach (IUnifyBarButtonUi button in m_Buttons)
				button.SetRoom(m_Room);

			UpdateVolumePoint();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Instantiates a button for the given type.
		/// </summary>
		/// <param name="buttonType"></param>
		/// <returns></returns>
		[NotNull]
		private IUnifyBarButtonUi InstantiateButton(eMainButton buttonType)
		{
			switch (buttonType)
			{
				case eMainButton.RoomPower:
					return new UnifyBarPowerButtonUi();
				case eMainButton.VolumeUp:
					return new UnifyBarVolumeUpButtonUi(m_VolumePointHelper, m_VolumeRepeater);
				case eMainButton.VolumeDown:
					return new UnifyBarVolumeDownButtonUi(m_VolumePointHelper, m_VolumeRepeater);
				case eMainButton.VolumeMute:
					return new UnifyBarMuteButtonUi(m_VolumePointHelper, m_VolumeRepeater);
				case eMainButton.PrivacyMute:
					return new UnifyBarPrivacyMuteButtonUi();
				case eMainButton.ToggleLights:
					return new UnifyBarLightsButtonUi();
				default:
					throw new ArgumentOutOfRangeException("buttonType");
			}
		}

		/// <summary>
		/// Updates the volume point that is being manipulated by the UI.
		/// </summary>
		private void UpdateVolumePoint()
		{
			m_VolumePointHelper.VolumePoint = Room == null ? null : Room.GetContextualVolumePoints().FirstOrDefault();
		}

		/// <summary>
		/// Assigns UI buttons to UnifyBar buttons.
		/// </summary>
		private void UpdateButtons()
		{
			// Unbind
			foreach (IUnifyBarButtonUi button in m_Buttons)
				button.SetButton(null);

			IUnifyBarButtonUi[] buttonUis = m_Buttons.Where(b => b.Visible).ToArray();
			UnifyBarMainButton[] deviceButtons = m_UnifyBar.SetMainButtonCount(buttonUis.Length).ToArray();

			// Bind 
			foreach (KeyValuePair<IUnifyBarButtonUi, UnifyBarMainButton> kvp in buttonUis.Zip(deviceButtons))
				kvp.Key.SetButton(kvp.Value);
		}

		#endregion

		#region Unify Bar Callbacks

		/// <summary>
		/// Subscribe to the unify bar events.
		/// </summary>
		/// <param name="unifyBar"></param>
		private void Subscribe([NotNull] UnifyBarDevice unifyBar)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			unifyBar.OnIsConnectedChanged += UnifyBarOnIsConnectedChanged;
			unifyBar.OnMainButtonsChanged += UnifyBarOnMainButtonsChanged;
		}

		/// <summary>
		/// Unsubscribe from the unify bar events.
		/// </summary>
		/// <param name="unifyBar"></param>
		private void Unsubscribe(UnifyBarDevice unifyBar)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			unifyBar.OnIsConnectedChanged -= UnifyBarOnIsConnectedChanged;
			unifyBar.OnMainButtonsChanged -= UnifyBarOnMainButtonsChanged;
		}

		/// <summary>
		/// Called when we connect/disconnect to the unify bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void UnifyBarOnIsConnectedChanged(object sender, BoolEventArgs eventArgs)
		{
			UpdateButtons();
		}

		/// <summary>
		/// Called when the buttons change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnMainButtonsChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		#endregion
	}
}
