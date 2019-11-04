using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header
{
	public delegate void HeaderButtonPressedCallback();

	public sealed class HeaderButtonModel : IComparable<HeaderButtonModel>
	{
		[PublicAPI]
		public event EventHandler OnPropertyChanged;

		private readonly HeaderButtonPressedCallback m_Callback;

		// Menu level of the buttons. e.g. 0 is top level buttons, 1 is buttons that appear when you click that button, etc.
		private int m_Level;
		// Button order within a level. Lower number appears in list first.
		private int m_Order;
		
		private bool m_Enabled;
		private bool m_Selected;
		private string m_LabelText;
		private string m_Icon;
		private eHeaderButtonMode m_Mode;

		#region Properties
		
		public HeaderButtonPressedCallback Callback { get { return m_Callback; } }

		public bool Enabled
		{
			get { return m_Enabled; }
			set
			{
				if (m_Enabled == value)
					return;

				m_Enabled = value;
				OnPropertyChanged.Raise(this);
			}
		}

		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (m_Selected == value)
					return;

				m_Selected = value;
				OnPropertyChanged.Raise(this);
			}
		}

		public eHeaderButtonMode Mode
		{
			get { return m_Mode; }
			set
			{
				if (m_Mode == value)
					return;

				m_Mode = value;
				OnPropertyChanged.Raise(this);
			}
		}

		public string LabelText
		{
			get { return m_LabelText; }
			set
			{
				if (m_LabelText == value)
					return;

				m_LabelText = value;
				OnPropertyChanged.Raise(this);
			}
		}

		public string Icon
		{
			get { return m_Icon; }
			set
			{
				if (m_Icon == value)
					return;

				m_Icon = value;
				OnPropertyChanged.Raise(this);
			}
		}

		#endregion

		public HeaderButtonModel(int level, int order, HeaderButtonPressedCallback callback)
		{
			m_Level = level;
			m_Order = order;
			m_Callback = callback;
			m_Enabled = true; //default enabled to true so I don't have to put it in every initializer
		}


		public int CompareTo(HeaderButtonModel other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (ReferenceEquals(null, other)) return 1;
			var levelComparison = m_Level.CompareTo(other.m_Level);
			if (levelComparison != 0) return levelComparison;
			return m_Order.CompareTo(other.m_Order);
		}
	}
}