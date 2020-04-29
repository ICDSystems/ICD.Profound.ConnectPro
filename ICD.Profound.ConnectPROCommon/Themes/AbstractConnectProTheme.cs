using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.Extensions;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;
using ICD.Connect.Sources.TvTuner.TvPresets;
using ICD.Connect.Themes;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPROCommon.Rooms.Combine;
using ICD.Profound.ConnectPROCommon.Themes.Ccrm4000UserInterface;
using ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface;
using ICD.Profound.ConnectPROCommon.Themes.Mpc3201UserInterface;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface;
using ICD.Profound.ConnectPROCommon.Themes.ShureMicrophoneInterface;
using ICD.Profound.ConnectPROCommon.Themes.ShureMx396Interface;
using ICD.Profound.ConnectPROCommon.Themes.YkupSwitcherInterface;
using ICD.Profound.ConnectPROCommon.WebConferencing;

namespace ICD.Profound.ConnectPROCommon.Themes
{
	public abstract class AbstractConnectProTheme<TSettings> : AbstractTheme<TSettings>, IConnectProTheme where TSettings : IConnectProThemeSettings, new()
	{
		public event EventHandler OnCueBackgroundChanged;

		/// <summary>
		/// Raised when starting to combine rooms.
		/// </summary>
		public event StartRoomCombine OnStartRoomCombine;

		/// <summary>
		/// Raised when ending combining rooms.
		/// </summary>
		public event EventHandler<GenericEventArgs<Exception>> OnEndRoomCombine;

		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;
		private readonly SafeCriticalSection m_UiFactoriesSection;

		private readonly XmlTvPresets m_TvPresets;
		private readonly WebConferencingInstructions m_WebConferencingInstructions;

		private readonly ConnectProDateFormatting m_DateFormatting;

		// Used with settings.
		private string m_TvPresetsPath;
		private string m_WebConferencingInstructionsPath;
		private eCueBackgroundMode m_CueBackground;
		private bool m_CueMotion;

		private IPartitionManager m_SubscribedPartitionManager;

		#region Properties

		/// <summary>
		/// Gets/sets the configured relative or absolute path to the logo image for the splash screen.
		/// </summary>
		public string Logo { get; set; }

		/// <summary>
		/// Gets/sets the absolute path to the configured logo image for the splash screen.
		/// </summary>
		[CanBeNull]
		public string LogoAbsolutePath
		{
			get
			{
				string host = IcdEnvironment.NetworkAddresses.FirstOrDefault();
				if (string.IsNullOrEmpty(host))
					return null;

				try
				{
					Uri defaultHost = new IcdUriBuilder { Host = host }.Uri;
					Uri absolute = new Uri(defaultHost, Logo);
					return absolute.ToString();
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Failed to get logo path for host {0} and logo {1} - {2}", host, Logo, e.Message);
				}

				return null;
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

		public bool CueMotion
		{
			get { return m_CueMotion; }
			set
			{
				if (m_CueMotion == value)
					return;
				
				m_CueMotion = value;

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
		protected AbstractConnectProTheme()
		{
			Logo = AbstractConnectProThemeSettings.LOGO_DEFAULT;

			m_TvPresets = new XmlTvPresets();
			m_WebConferencingInstructions = new WebConferencingInstructions();
			m_DateFormatting = new ConnectProDateFormatting(this);

			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new ConnectProCcrm4000UserInterfaceFactory(this),
				new ConnectProEventServerUserInterfaceFactory(this),
				new ConnectProMpc3201InterfaceFactory(this),
				new ConnectProOsdInterfaceFactory(this),
				new ConnectProShureMicrophoneInterfaceFactory(this),
				new ConnectProShureMx396InterfaceFactory(this),
				new ConnectProYkupSwitcherInterfaceFactory(this)
			};

			m_UiFactoriesSection = new SafeCriticalSection();
			
			Core.Originators.OnChildrenChanged += OriginatorsOnChildrenChanged;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnCueBackgroundChanged = null;
			OnStartRoomCombine = null;
			OnEndRoomCombine = null;

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
		/// Combine rooms based on the given control being open or closed.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="open"></param>
		private void CombineRooms(IPartitionDeviceControl control, bool open)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			IEnumerable<IPartition> partitions = m_SubscribedPartitionManager.Partitions.GetPartitions(control);

			if (open)
				CombineRooms(partitions, Enumerable.Empty<IPartition>());
			else
				CombineRooms(Enumerable.Empty<IPartition>(), partitions);
		}

		/// <summary>
		/// Opens and closes the given partitions to update the combine spaces.
		/// </summary>
		/// <param name="open"></param>
		/// <param name="close"></param>
		public void CombineRooms(IEnumerable<IPartition> open, IEnumerable<IPartition> close)
		{
			if (open == null)
				throw new ArgumentNullException("open");

			if (close == null)
				throw new ArgumentNullException("close");

			IList<IPartition> openList = open as IList<IPartition> ?? open.ToArray();
			IList<IPartition> closeList = close as IList<IPartition> ?? close.ToArray();

			StartRoomCombine startHandler = OnStartRoomCombine;
			if (startHandler != null)
				startHandler(this, openList.Count, closeList.Count);

			try
			{
				m_SubscribedPartitionManager.CombineRooms(openList, closeList, () => new ConnectProCombineRoom());
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, e, "Failed to combine rooms - " + e.Message);

				OnEndRoomCombine.Raise(this, new GenericEventArgs<Exception>(e));
				return;
			}

			OnEndRoomCombine.Raise(this, new GenericEventArgs<Exception>(null));
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

			Logo = AbstractConnectProThemeSettings.LOGO_DEFAULT;
			m_TvPresetsPath = null;
			m_WebConferencingInstructionsPath = null;
			CueBackground = default(eCueBackgroundMode);
			CueMotion = false;

			Unsubscribe(m_SubscribedPartitionManager);
			m_SubscribedPartitionManager = null;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Logo = Logo;
			settings.TvPresets = m_TvPresetsPath;
			settings.WebConferencingInstructions = m_WebConferencingInstructionsPath;
			settings.CueBackground = CueBackground;
			settings.CueMotion = CueMotion;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			// Ensure the rooms are loaded
			factory.LoadOriginators<IRoom>();
			
			Logo = settings.Logo;
			CueBackground = settings.CueBackground;
			CueMotion = settings.CueMotion;

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

			manager.OnPartitionControlOpenStateChange += ManagerOnPartitionControlOpenStateChange;
		}

		private void Unsubscribe(IPartitionManager manager)
		{
			if (manager == null)
				return;

			manager.OnPartitionControlOpenStateChange -= ManagerOnPartitionControlOpenStateChange;
		}

		private void ManagerOnPartitionControlOpenStateChange(IPartitionDeviceControl control, bool open)
		{
			CombineRooms(control, open);
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
			string cueMotionHelp = "SetCueMotion <true,false>";

			yield return new GenericConsoleCommand<eCueBackgroundMode>("SetCueBackground", cueBackgroundHelp,
			                                                           m => ConsoleSetCueBackground(m));
			yield return new GenericConsoleCommand<bool>("SetCueMotion", cueMotionHelp,
				m => ConsoleSetCueMotion(m));
		}

		private string ConsoleSetCueBackground(eCueBackgroundMode cueBackgroundMode)
		{
			CueBackground = cueBackgroundMode;
			return string.Format("Cue Background set to {0}", CueBackground);
		}

		private string ConsoleSetCueMotion(bool cueMotion)
		{
			CueMotion = cueMotion;
			return string.Format("Cue Motion set to {0}", CueMotion);
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
