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

		/// <summary>
		/// Raised when the button selection state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnSelectedChanged;

		/// <summary>
		/// Raised when the enabled state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnEnabledStateChanged;

		/// <summary>
		/// Raised when the label changes.
		/// </summary>
		public event EventHandler<StringEventArgs> OnLabelChanged;

		/// <summary>
		/// Raised when the icon changes.
		/// </summary>
		public event EventHandler<GenericEventArgs<eMainButtonIcon>> OnIconChanged;

		/// <summary>
		/// Raised when the type changes.
		/// </summary>
		public event EventHandler<GenericEventArgs<eMainButtonType>> OnTypeChanged;

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
		/// Gets the index for the button.
		/// </summary>
		public int Index { get { return m_Index; } }

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

				OnSelectedChanged.Raise(this, m_Selected);
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

				OnEnabledStateChanged.Raise(this, m_Enabled);
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

				OnLabelChanged.Raise(this, m_Label);
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

				OnIconChanged.Raise(this, m_Icon);
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

				OnTypeChanged.Raise(this, m_Type);
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
			OnSelectedChanged = null;
			OnEnabledStateChanged = null;
			OnLabelChanged = null;
			OnIconChanged = null;
			OnTypeChanged = null;

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
		}

		/// <summary>
		/// Called when the user presses or releases a main button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ParentOnMainButtonPressed(object sender, MainButtonPressedEventArgs eventArgs)
		{
			if (eventArgs.Index == m_Index)
				Pressed = eventArgs.Pressed;
		}

		#endregion
	}
}
