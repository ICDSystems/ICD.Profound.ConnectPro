#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.About
{
	public interface IReferencedSettingsPluginsPresenter : IUiPresenter<IReferencedSettingsPluginsView>
	{
		/// <summary>
		/// Gets/sets the wrapped assembly.
		/// </summary>
		Assembly Assembly { get; set; }
	}
}