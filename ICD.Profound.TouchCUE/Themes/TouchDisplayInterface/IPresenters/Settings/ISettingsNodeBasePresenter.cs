using System;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings
{
	public interface ISettingsNodeBasePresenter : ITouchDisplayPresenter
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

	public interface ISettingsNodeBasePresenter<TView, TNode> : ISettingsNodeBasePresenter, ITouchDisplayPresenter<TView>
		where TView : ITouchDisplayView
		where TNode : ISettingsNodeBase
	{
		/// <summary>
		/// Gets/sets the settings node.
		/// </summary>
		new TNode Node { get; set; }
	}
}
