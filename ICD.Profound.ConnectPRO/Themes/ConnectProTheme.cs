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
using ICD.Connect.API.Nodes;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.Extensions;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Themes;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Rooms.Combine;
using ICD.Profound.ConnectPRO.Themes.Ccrm4000UserInterface;
using ICD.Profound.ConnectPRO.Themes.Mpc3201UserInterface;
using ICD.Profound.ConnectPRO.Themes.OsdInterface;
using ICD.Profound.ConnectPRO.Themes.ShureMicrophoneInterface;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProTheme : AbstractTheme<ConnectProThemeSettings>
	{
		public event EventHandler OnCueBackgroundChanged;

		public const string LOGO_DEFAULT = "Logo.png";

		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;
		private readonly SafeCriticalSection m_UiFactoriesSection;

		private readonly XmlTvPresets m_TvPresets;
		private readonly WebConferencingInstructions m_WebConferencingInstructions;

		private readonly ConnectProDateFormatting m_DateFormatting;

		// Used with settings.
		private string m_TvPresetsPath;
		private string m_WebConferencingInstructionsPath;
		private eCueBackgroundMode m_CueBackground;

		private IPartitionManager m_SubscribedPartitionManager;

		#region Properties

		public ICore Core { get { return ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Gets/sets the configured relative or absolute path to the logo image for the splash screen.
		/// </summary>
		public string Logo { get; set; }

		/// <summary>
		/// Gets/sets the absolute path to the configured logo image for the splash screen.
		/// </summary>
		public string LogoAbsolutePath
		{
			get
			{
				Uri defaultHost = new IcdUriBuilder {Host = IcdEnvironment.NetworkAddresses.FirstOrDefault()}.Uri;
				Uri absolute = new Uri(defaultHost, Logo);

				return absolute.ToString();
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

		/// <summary>
		/// Gets/sets the CUE background mode.
		/// </summary>
		public eCueBackgroundMode CueBackground
		{
			get { return m_CueBackground; }
			set
			{
				if (m_CueBackground == value)
					return;

				m_CueBackground = value;

				OnCueBackgroundChanged.Raise(this);
			}
		}

		/// <summary>
		/// Gets the date formatting rules.
		/// </summary>
		public ConnectProDateFormatting DateFormatting { get { return m_DateFormatting; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProTheme()
		{
			Logo = LOGO_DEFAULT;

			m_TvPresets = new XmlTvPresets();
			m_WebConferencingInstructions = new WebConferencingInstructions();
			m_DateFormatting = new ConnectProDateFormatting(this);

			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new ConnectProCcrm4000UserInterfaceFactory(this),
				new ConnectProMpc3201InterfaceFactory(this),
				new ConnectProOsdInterfaceFactory(this),
				new ConnectProShureMicrophoneInterfaceFactory(this),
				new ConnectProUserInterfaceFactory(this)
			};

			m_UiFactoriesSection = new SafeCriticalSection();
			
			Core.Originators.OnChildrenChanged += OriginatorsOnChildrenChanged;
		}

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Core.Originators.OnChildrenChanged -= OriginatorsOnChildrenChanged;
		}
		
		private void OriginatorsOnChildrenChanged(object sender, EventArgs args)
		{
			ReassignRooms();
		}

		#region Public Methods

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public override IEnumerable<IUserInterfaceFactory> GetUiFactories()
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
		
		/// <summary>
		/// Reassigns rooms to the existing user interfaces.
		/// </summary>
		private void ReassignRooms()
		{
			m_UiFactoriesSection.Enter();

			try
			{
				m_UiFactories.ForEach(f => f.ReassignUserInterfaces());
			}
			finally
			{
				m_UiFactoriesSection.Leave();
			}
		}

		#endregion

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
			CueBackground = default(eCueBackgroundMode);

			Unsubscribe(m_SubscribedPartitionManager);
			m_SubscribedPartitionManager = null;
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
			settings.CueBackground = CueBackground;
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
			CueBackground = settings.CueBackground;

			SetTvPresetsFromPath(settings.TvPresets);
			SetWebConferencingInstructionsFromPath(settings.WebConferencingInstructions);

			Unsubscribe(m_SubscribedPartitionManager);
			Core.TryGetPartitionManager(out m_SubscribedPartitionManager);
			Subscribe(m_SubscribedPartitionManager);

			base.ApplySettingsFinal(settings, factory);
		}

		#endregion
		
		#region Partitioning

		private void Subscribe(IPartitionManager manager)
		{
			if (manager == null)
				return;

			manager.OnPartitionOpenStateChange += ManagerOnPartitionOpenStateChange;
		}

		private void Unsubscribe(IPartitionManager manager)
		{
			if (manager == null)
				return;

			manager.OnPartitionOpenStateChange -= ManagerOnPartitionOpenStateChange;
		}

		private void ManagerOnPartitionOpenStateChange(IPartitionDeviceControl control, bool open)
		{
			m_SubscribedPartitionManager.SetPartition<ConnectProCombineRoom>(control, open);
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Cue Background", CueBackground);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			string cueBackgroundHelp = string.Format("SetCueBackground <{0}>",
			                                         StringUtils.ArrayFormat(EnumUtils.GetValues<eCueBackgroundMode>()));

			yield return new GenericConsoleCommand<eCueBackgroundMode>("SetCueBackground", cueBackgroundHelp,
			                                                           m => ConsoleSetCueBackground(m));
		}

		private string ConsoleSetCueBackground(eCueBackgroundMode cueBackgroundMode)
		{
			CueBackground = cueBackgroundMode;
			return string.Format("Cue Background set to {0}", CueBackground);
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
