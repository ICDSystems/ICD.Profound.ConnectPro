using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public abstract class AbstractUnifyBarButtonUi : IUnifyBarButtonUi
	{
		/// <summary>
		/// Raised when the button visibility changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnVisibleChanged;

		private ICommercialRoom m_Room;
		private UnifyBarMainButton m_Button;
		private string m_Label;
		private bool m_Enabled;
		private bool m_Selected;
		private eMainButtonIcon m_Icon;
		private eMainButtonType m_Type;
		private bool m_Visible;

		#region Properties

		/// <summary>
		/// Gets the room.
		/// </summary>
		[CanBeNull]
		public ICommercialRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the button.
		/// </summary>
		[CanBeNull]
		public UnifyBarMainButton Button { get { return m_Button; } }

		/// <summary>
		/// Gets the button visibility.
		/// </summary>
		public bool Visible
		{
			get { return m_Visible; }
			protected set
			{
				if (value == m_Visible)
					return;

				m_Visible = value;

				OnVisibleChanged.Raise(this, m_Visible);
			}
		}

		/// <summary>
		/// Gets/sets the label.
		/// </summary>
		protected string Label
		{
			get { return m_Label; }
			set
			{
				if (value == m_Label)
					return;

				m_Label = value;

				Refresh();
			}
		}

		/// <summary>
		/// Gets/sets the enabled state.
		/// </summary>
		protected bool Enabled
		{
			get { return m_Enabled; }
			set
			{
				if (value == m_Enabled)
					return;

				m_Enabled = value;

				Refresh();
			}
		}

		/// <summary>
		/// Gets/sets the selected state.
		/// </summary>
		protected bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				Refresh();
			}
		}

		/// <summary>
		/// Gets/sets the icon.
		/// </summary>
		protected eMainButtonIcon Icon
		{
			get { return m_Icon; }
			set
			{
				if (value == m_Icon)
					return;

				m_Icon = value;

				Refresh();
			}
		}

		/// <summary>
		/// Gets/sets the type.
		/// </summary>
		protected eMainButtonType Type
		{
			get { return m_Type; }
			set
			{
				if (value == m_Type)
					return;

				m_Type = value;

				Refresh();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			SetButton(null);
			SetRoom(null);
		}

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public virtual void SetRoom([CanBeNull] ICommercialRoom room)
		{
			if (room == m_Room)
				return;

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);
		}

		/// <summary>
		/// Sets the button.
		/// </summary>
		/// <param name="button"></param>
		public void SetButton([CanBeNull] UnifyBarMainButton button)
		{
			if (button == m_Button)
				return;

			Unsubscribe(m_Button);
			m_Button = button;
			Subscribe(m_Button);

			Refresh();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected abstract void HandleButtonPress(bool pressed);

		/// <summary>
		/// Pushes the UI state to the device.
		/// </summary>
		private void Refresh()
		{
			if (m_Button == null)
				return;

			m_Button.Label = m_Label;
			m_Button.Enabled = m_Enabled;
			m_Button.Selected = m_Selected;
			m_Button.Icon = m_Icon;
			m_Button.Type = m_Type;
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe([CanBeNull] ICommercialRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe([CanBeNull] ICommercialRoom room)
		{
		}

		#endregion

		#region Button Callbacks

		/// <summary>
		/// Subscribe to the button events.
		/// </summary>
		/// <param name="button"></param>
		private void Subscribe([CanBeNull] UnifyBarMainButton button)
		{
			if (button == null)
				return;

			button.OnPressedChanged += ButtonOnPressedChanged;
		}

		/// <summary>
		/// Unsubscribe from the button events.
		/// </summary>
		/// <param name="button"></param>
		private void Unsubscribe([CanBeNull] UnifyBarMainButton button)
		{
			if (button == null)
				return;

			button.OnPressedChanged -= ButtonOnPressedChanged;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonOnPressedChanged(object sender, BoolEventArgs eventArgs)
		{
			HandleButtonPress(eventArgs.Data);
		}

		#endregion
	}
}
