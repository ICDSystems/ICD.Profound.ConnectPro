using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Connect.Themes;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.MicrophoneInterface;
using ICD.Profound.ConnectPRO.Themes.OsdInterface;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProTheme : AbstractTheme<ConnectProThemeSettings>
	{
		public const string LOGO_DEFAULT = "Logo.png";

		private readonly IcdHashSet<IConnectProUserInterfaceFactory> m_UiFactories;
		private readonly SafeCriticalSection m_UiFactoriesSection;

		private readonly XmlTvPresets m_TvPresets;
		private readonly WebConferencingInstructions m_WebConferencingInstructions;

		// Used with settings.
		private string m_TvPresetsPath;
		private string m_WebConferencingInstructionsPath;
		private string m_Logo;

		#region Properties

		public ICore Core { get { return ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Gets/sets the url to the logo image for the splash screen.
		/// </summary>
		public string Logo
		{
			get { return m_Logo; }
			set
			{
				Uri baseUri = GenerateBaseUri();
				Uri absolute = new Uri(baseUri, value);

				m_Logo = absolute.ToString();
			}
		}

		/// <summary>
		/// Gets the tv presets.
		/// </summary>
		public ITvPresets TvPresets { get { return m_TvPresets; } }

		/// <summary>
		/// Gets the web conferencing instructions.
		/// </summary>
		public WebConferencingInstructions WebConferencingInstructions { get { return m_WebConferencingInstructions; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProTheme()
		{
			Logo = LOGO_DEFAULT;

			m_TvPresets = new XmlTvPresets();
			m_WebConferencingInstructions = new WebConferencingInstructions();

			m_UiFactories = new IcdHashSet<IConnectProUserInterfaceFactory>
			{
				new ConnectProUserInterfaceFactory(this),
				new ConnectProMicrophoneInterfaceFactory(this),
				new ConnectProOsdInterfaceFactory(this)
			};

			m_UiFactoriesSection = new SafeCriticalSection();
		}

		#region Public Methods

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public IEnumerable<IConnectProUserInterfaceFactory> GetUiFactories()
		{
			m_UiFactoriesSection.Enter();

			try
			{
				return m_UiFactories.ToArray(m_UiFactories.Count);
			}
			finally
			{
				m_UiFactoriesSection.Leave();
			}
		}

		/// <summary>
		/// Sets the tv presets from the given xml document path.
		/// </summary>
		/// <param name="path"></param>
		public void SetTvPresetsFromPath(string path)
		{
			m_TvPresetsPath = path;

			string tvPresetsPath =
				string.IsNullOrEmpty(path)
					? null
					: PathUtils.GetDefaultConfigPath("TvPresets", m_TvPresetsPath);

			if (string.IsNullOrEmpty(tvPresetsPath))
				return;

			try
			{
				string xml = IcdFile.ReadToEnd(tvPresetsPath, new UTF8Encoding(false));
				xml = EncodingUtils.StripUtf8Bom(xml);

				m_TvPresets.Clear();
				m_TvPresets.Parse(xml);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "Failed to load TV Presets {0} - {1}", m_TvPresetsPath, e.Message);
			}
		}

		/// <summary>
		/// Sets the web conferencing instructions from the given xml document path.
		/// </summary>
		/// <param name="path"></param>
		public void SetWebConferencingInstructionsFromPath(string path)
		{
			m_WebConferencingInstructionsPath = path;

			string webConferencingInstructionsPath =
				string.IsNullOrEmpty(path)
					? null
					: PathUtils.GetDefaultConfigPath("WebConferencing", m_WebConferencingInstructionsPath);

			if (string.IsNullOrEmpty(webConferencingInstructionsPath))
				return;

			try
			{
				string xml = IcdFile.ReadToEnd(webConferencingInstructionsPath, new UTF8Encoding(false));
				xml = EncodingUtils.StripUtf8Bom(xml);

				m_WebConferencingInstructions.Clear();
				m_WebConferencingInstructions.Parse(xml);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "Failed to load Web Conferencing Instructions {0} - {1}", m_WebConferencingInstructionsPath, e.Message);
			}
		}

		/// <summary>
		/// Clears the instantiated user interfaces.
		/// </summary>
		public override void ClearUserInterfaces()
		{
			m_UiFactoriesSection.Enter();

			try
			{
				m_UiFactories.ForEach(f => f.Clear());
			}
			finally
			{
				m_UiFactoriesSection.Leave();
			}
		}

		/// <summary>
		/// Clears and rebuilds the user interfaces.
		/// </summary>
		public override void BuildUserInterfaces()
		{
			m_UiFactoriesSection.Enter();

			try
			{
				m_UiFactories.ForEach(f => f.BuildUserInterfaces());
			}
			finally
			{
				m_UiFactoriesSection.Leave();
			}
		}

		#endregion

		/// <summary>
		/// Gets the base uri for the processor.
		/// </summary>
		/// <returns></returns>
		private static Uri GenerateBaseUri()
		{
			string ip = IcdEnvironment.NetworkAddresses.FirstOrDefault();
			string url = string.Format(@"http://{0}/", ip);
			
			return new Uri(url);
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Logo = LOGO_DEFAULT;
			m_TvPresetsPath = null;
			m_WebConferencingInstructionsPath = null;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProThemeSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Logo = Logo;
			settings.TvPresets = m_TvPresetsPath;
			settings.WebConferencingInstructions = m_WebConferencingInstructionsPath;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProThemeSettings settings, IDeviceFactory factory)
		{
			// Ensure the rooms are loaded
			factory.LoadOriginators<IRoom>();

			Logo = settings.Logo;

			SetTvPresetsFromPath(settings.TvPresets);
			SetWebConferencingInstructionsFromPath(settings.WebConferencingInstructions);

			base.ApplySettingsFinal(settings, factory);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("PrintUIs", "Prints information about the current UIs", () => ConsolePrintUis());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		private string ConsolePrintUis()
		{
			TableBuilder builder = new TableBuilder("Type", "Room", "Target");

			foreach (IConnectProUserInterfaceFactory factory in GetUiFactories())
			{
				foreach (IUserInterface ui in factory.GetUserInterfaces())
				{
					Type type = ui.GetType();
					IConnectProRoom room = ui.Room;
					object target = ui.Target;

					builder.AddRow(type, room, target);
				}
			}

			return builder.ToString();
		}

		#endregion
	}
}
