using System;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProThemeSettings : AbstractThemeSettings
	{
		private const string FACTORY_NAME = "ConnectProTheme";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ConnectProTheme); } }

		/// <summary>
		/// Instantiates room settings from an xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[XmlFactoryMethod(FACTORY_NAME)]
		public static ConnectProThemeSettings FromXml(string xml)
		{
			ConnectProThemeSettings output = new ConnectProThemeSettings();
			output.ParseXml(xml);
			return output;
		}
	}
}
