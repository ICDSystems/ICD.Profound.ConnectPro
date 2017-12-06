using System;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoomSettings : AbstractRoomSettings
	{
		private const string FACTORY_NAME = "ConnectProRoom";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ConnectProRoom); } }

		/// <summary>
		/// Instantiates room settings from an xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[XmlFactoryMethod(FACTORY_NAME)]
		public static ConnectProRoomSettings FromXml(string xml)
		{
			ConnectProRoomSettings output = new ConnectProRoomSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}