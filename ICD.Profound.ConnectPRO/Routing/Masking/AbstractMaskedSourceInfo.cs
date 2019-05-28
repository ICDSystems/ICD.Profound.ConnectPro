using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public abstract class AbstractMaskedSourceInfo : IMaskedSourceInfo
	{
		private ISource m_Source;
		private bool m_Mask;
		private bool? m_MaskOverride;

		#region Properties

		public ISource Source
		{
			get { return m_Source; }
			set
			{
				if (m_Source == value)
					return;
				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);
			}
		}

		public bool Mask 
		{ 
			get { return m_Mask; }
			protected set
			{
				if (m_Mask == value)
					return;

				m_Mask = value;
				CheckMaskState();
			}
		}

		public bool? MaskOverride
		{
			get { return m_MaskOverride; }
			set
			{
				if (m_MaskOverride == value)
					return;

				m_MaskOverride = value;
				CheckMaskState();
			}
		}

		#endregion

		#region Methods

		protected abstract void PerformMask();

		protected abstract void PerformUnmask();

		protected virtual void Subscribe(ISource source)
		{
		}

		protected virtual void Unsubscribe(ISource source)
		{
		}

		#endregion

		#region Private Methods

		private void CheckMaskState()
		{
			if (MaskOverride ?? Mask)
				PerformMask();
			else
				PerformUnmask();
		}

		#endregion

		public void Dispose()
		{
			DisposeFinal();
		}

		protected virtual void DisposeFinal()
		{
			Source = null;
		}
	}
}
