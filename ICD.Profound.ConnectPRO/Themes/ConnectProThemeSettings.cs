using System;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPRO.Themes
{
	[KrangSettings(FACTORY_NAME)]
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
	}
}
