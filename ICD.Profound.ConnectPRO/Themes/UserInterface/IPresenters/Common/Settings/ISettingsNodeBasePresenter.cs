using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.SettingsTree;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings
{
	public interface ISettingsNodeBasePresenter : IUiPresenter
	{
		/// <summary>
		/// Gets/sets the settings node.
		/// </summary>
		ISettingsNodeBase Node { get; set; }

		/// <summary>
		/// Returns the supported node type.
		/// </summary>
		Type NodeType { get; }
	}

	public interface ISettingsNodeBasePresenter<TView, TNode> : ISettingsNodeBasePresenter, IUiPresenter<TView>
		where TView : IUiView
		where TNode : ISettingsNodeBase
	{
		/// <summary>
		/// Gets/sets the settings node.
		/// </summary>
		new TNode Node { get; set; }
	}
}
