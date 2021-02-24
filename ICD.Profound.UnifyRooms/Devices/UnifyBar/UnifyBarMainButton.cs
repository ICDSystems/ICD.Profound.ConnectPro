using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	public sealed class UnifyBarMainButton : IDisposable
	{
		/// <summary>
		/// Raised when the user presses or releases the button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnPressedChanged;

		private readonly UnifyBarDevice m_Parent;
		private readonly int m_Index;

		private bool m_Pressed;
		private bool m_Selected;
		private bool m_Enabled;
		private string m_Label;
		private eMainButtonIcon m_Icon;
		private eMainButtonType m_Type;

		#region Properties

		/// <summary>
		/// Gets the button presses state.
		/// </summary>
		public bool Pressed
		{
			get { return m_Pressed; }
			private set
			{
				if (value == m_Pressed)
					return;

				m_Pressed = value;

				OnPressedChanged.Raise(this, m_Pressed);
			}
		}

		/// <summary>
		/// Gets/sets the button selected state.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				if (m_Parent.IsConnected)
					m_Parent.SetMainButtonSelected(m_Index, m_Selected);
			}
		}

		/// <summary>
		/// Gets/sets the button enabled state.
		/// </summary>
		public bool Enabled
		{
			get { return m_Enabled; }
			set
			{
				if (value == m_Enabled)
					return;

				m_Enabled = value;

				if (m_Parent.IsConnected)
					m_Parent.SetMainButtonEnabled(m_Index, m_Enabled);
			}
		}

		/// <summary>
		/// Gets/sets the button label.
		/// </summary>
		public string Label
		{
			get { return m_Label; }
			set
			{
				if (value == m_Label)
					return;

				m_Label = value;

				if (m_Parent.IsConnected)
					m_Parent.SetMainButtonLabel(m_Index, m_Label);
			}
		}

		/// <summary>
		/// Gets/sets the button icon.
		/// </summary>
		public eMainButtonIcon Icon
		{
			get { return m_Icon; }
			set
			{
				if (value == m_Icon)
					return;

				m_Icon = value;

				if (m_Parent.IsConnected)
					m_Parent.SetMainButtonIcon(m_Index, m_Icon);
			}
		}

		/// <summary>
		/// Gets/sets the button type.
		/// </summary>
		public eMainButtonType Type
		{
			get { return m_Type; }
			set
			{
				if (value == m_Type)
					return;

				m_Type = value;

				if (m_Parent.IsConnected)
					m_Parent.SetMainButtonType(m_Index, m_Type);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public UnifyBarMainButton([NotNull] UnifyBarDevice parent, int index)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			m_Parent = parent;
			m_Index = index;

			Subscribe(m_Parent);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnPressedChanged = null;

			Unsubscribe(m_Parent);
		}

		#region Parent Callbacks

		/// <summary>
		/// Subscribe to the parent device events.
		/// </summary>
		/// <param name="parent"></param>
		private void Subscribe(UnifyBarDevice parent)
		{
			parent.OnIsConnectedChanged += ParentOnIsConnectedChanged;
			parent.OnMainButtonPressed += ParentOnMainButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the parent device events.
		/// </summary>
		/// <param name="parent"></param>
		private void Unsubscribe(UnifyBarDevice parent)
		{
			parent.OnIsConnectedChanged -= ParentOnIsConnectedChanged;
			parent.OnMainButtonPressed -= ParentOnMainButtonPressed;
		}

		/// <summary>
		/// Called when the parent device connects/disconnects.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ParentOnIsConnectedChanged(object sender, BoolEventArgs eventArgs)
		{
			// Stop ramping
			Pressed = false;

			if (!m_Parent.IsConnected)
				return;

			m_Parent.SetMainButtonSelected(m_Index, m_Selected);
			m_Parent.SetMainButtonEnabled(m_Index, m_Enabled);
			m_Parent.SetMainButtonLabel(m_Index, m_Label);
			m_Parent.SetMainButtonIcon(m_Index, m_Icon);
			m_Parent.SetMainButtonType(m_Index, m_Type);
		}

		/// <summary>
		/// Called when the user presses or releases a main button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ParentOnMainButtonPressed(object sender, MainButtonPressedEventArgs eventArgs)
		{
			Pressed = eventArgs.Pressed;
		}

		#endregion
	}
}
