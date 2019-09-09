using System;
using System.Collections.Generic;
using ICD.Profound.ConnectPRO.SettingsTree;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public abstract class AbstractSettingsNodeBasePresenter<TView, TNode> : AbstractUiPresenter<TView>,
	                                                                        ISettingsNodeBasePresenter<TView, TNode>
		where TView : class, IUiView
		where TNode : ISettingsNodeBase
	{
		private TNode m_Node;

		#region Properties

		/// <summary>
		/// Gets/sets the settings node.
		/// </summary>
		ISettingsNodeBase ISettingsNodeBasePresenter.Node { get { return Node; } set { Node = (TNode)value; } }

		/// <summary>
		/// Gets/sets the settings node.
		/// </summary>
		public TNode Node
		{
			get { return m_Node; }
			set
			{
				if (EqualityComparer<TNode>.Default.Equals(value, m_Node))
					return;

				m_Node = value;

				NodeChanged(m_Node);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Returns the supported node type.
		/// </summary>
		public Type NodeType { get { return typeof(TNode); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsNodeBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Called when the wrapped node changes.
		/// </summary>
		/// <param name="node"></param>
		protected virtual void NodeChanged(TNode node)
		{
		}
	}
}
