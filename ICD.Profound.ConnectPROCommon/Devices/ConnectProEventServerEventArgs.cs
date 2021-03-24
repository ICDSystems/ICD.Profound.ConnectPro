using System;

namespace ICD.Profound.ConnectPROCommon.Devices
{
	public sealed class ConnectProEventServerEventArgs : EventArgs
	{
		private readonly string m_Key;
		private readonly string m_Value;

		/// <summary>
		/// Gets the key.
		/// </summary>
		public string Key { get { return m_Key; } }

		/// <summary>
		/// Gets the value.
		/// </summary>
		public string Value { get { return m_Value; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public ConnectProEventServerEventArgs(string key, string value)
		{
			m_Key = key;
			m_Value = value;
		}
	}
}