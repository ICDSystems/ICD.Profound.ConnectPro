using System;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	public sealed class MainButtonPressedEventArgs : EventArgs
	{
		private readonly int m_Index;
		private readonly bool m_Pressed;

		/// <summary>
		/// Gets the index of the main button.
		/// </summary>
		public int Index { get { return m_Index; } }

		/// <summary>
		/// Gets the pressed state of the button.
		/// </summary>
		public bool Pressed { get { return m_Pressed; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="pressed"></param>
		public MainButtonPressedEventArgs(int index, bool pressed)
		{
			m_Index = index;
			m_Pressed = pressed;
		}
	}
}
