#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPRO.SettingsTree.About
{
	public sealed class PluginsSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PluginsSettingsLeaf()
		{
			Name = "Plugins";
		}

		public IEnumerable<Assembly> GetPluginAssemblies()
		{
			return PluginFactory.GetFactoryAssemblies().OrderBy(a => a.FullName);
		}
	}
}
