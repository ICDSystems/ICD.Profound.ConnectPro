using System;
using ICD.Common.Properties;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IReferencedRouteListItemPresenter : IUiPresenter<IReferencedRouteListItemView>
	{
		RouteListItem Model { set; }
	}

	public struct RouteListItem : IEquatable<RouteListItem>
	{
		private readonly IRoom m_Room;
		private readonly IDestinationBase m_Destination;
		private readonly ISource m_Source;

		[NotNull]
		public IRoom Room { get { return m_Room; } }

		[NotNull]
		public IDestinationBase Destination { get { return m_Destination; } }

		[NotNull]
		public ISource Source { get { return m_Source; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		public RouteListItem(IRoom room, IDestinationBase destination, ISource source)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			if (destination == null)
				throw new ArgumentNullException("destination");

			if (source == null)
				throw new ArgumentNullException("source");

			m_Room = room;
			m_Destination = destination;
			m_Source = source;
		}

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(RouteListItem other)
		{
			return Equals(m_Room, other.m_Room) &&
			       Equals(m_Destination, other.m_Destination) &&
				   Equals(m_Source, other.m_Source);
		}

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is RouteListItem && Equals((RouteListItem)obj);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = m_Room != null ? m_Room.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (m_Destination != null ? m_Destination.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (m_Source != null ? m_Source.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}
