using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.EventArguments
{
	public sealed class SourceRoutedEventArgs : EventArgs
	{
		private readonly ISource[] m_Routed;
		private readonly ISource[] m_Unrouted;

		/// <summary>
		/// Gets the newly routed sources.
		/// </summary>
		public IEnumerable<ISource> Routed { get { return m_Routed; } }
 
		/// <summary>
		/// Gets the newly unrouted sources.
		/// </summary>
		public IEnumerable<ISource> Unrouted { get { return m_Unrouted; } } 

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="routed"></param>
		/// <param name="unrouted"></param>
		public SourceRoutedEventArgs([NotNull] IEnumerable<ISource> routed, [NotNull] IEnumerable<ISource> unrouted)
		{
			if (routed == null)
				throw new ArgumentNullException("routed");

			if (unrouted == null)
				throw new ArgumentNullException("unrouted");

			m_Routed = routed.ToArray();
			m_Unrouted = unrouted.ToArray();
		}
	}
}
