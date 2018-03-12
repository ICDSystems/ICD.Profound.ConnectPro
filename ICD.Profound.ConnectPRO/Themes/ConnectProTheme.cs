using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Connect.Themes;
using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Themes.MicrophoneInterface;
using ICD.Profound.ConnectPRO.Themes.OsdInterface;
using ICD.Profound.ConnectPRO.Themes.UserInterface;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProTheme : AbstractTheme<ConnectProThemeSettings>
	{
		private readonly IcdHashSet<IConnectProUserInterfaceFactory> m_UiFactories;
		private readonly SafeCriticalSection m_UiFactoriesSection;

		// Used with settings.
		private string m_TvPresetsPath;
		private readonly XmlTvPresets m_TvPresets;

		#region Properties

		public ICore Core { get { return ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Gets the tv presets.
		/// </summary>
		public ITvPresets TvPresets { get { return m_TvPresets; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProTheme()
		{
			m_TvPresets = new XmlTvPresets();

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
		/// Sets the tv presets from the given xml document path.
		/// </summary>
		/// <param name="path"></param>
		public void SetTvPresetsFromPath(string path)
		{
			m_TvPresetsPath = path;

			string tvPresetsPath = string.IsNullOrEmpty(path)
									   ? null
									   : PathUtils.GetDefaultConfigPath("TvPresets", m_TvPresetsPath);

			if (string.IsNullOrEmpty(tvPresetsPath))
				return;

			try
			{
				string tvPresetsXml = IcdFile.ReadToEnd(tvPresetsPath, Encoding.ASCII);
				m_TvPresets.Clear();
				m_TvPresets.Parse(tvPresetsXml);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, "Failed to load TV Presets {0} - {1}", m_TvPresetsPath, e.Message);
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

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_TvPresetsPath = null;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProThemeSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.TvPresets = m_TvPresetsPath;
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

			// Tv Presets
			SetTvPresetsFromPath(settings.TvPresets);

			base.ApplySettingsFinal(settings, factory);
		}

		#endregion
	}
}
