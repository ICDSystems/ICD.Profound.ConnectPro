using System;
using ICD.Common.Properties;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public abstract class AbstractSettingsNodeBasePresenter<TView, TNode> : AbstractUiPresenter<TView>,
	                                                                        ISettingsNodeBasePresenter<TView, TNode>
		where TView : class, IUiView
		where TNode : class, ISettingsNodeBase
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
		[CanBeNull]
		public TNode Node
		{
			get { return m_Node; }
			set
			{
				if (value == m_Node)
					return;

				Unsubscribe(m_Node);
				m_Node = value;
				Subscribe(m_Node);

				NodeChanged(m_Node);
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
		protected AbstractSettingsNodeBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Node = null;
		}

		/// <summary>
		/// Called when the wrapped node changes.
		/// </summary>
		/// <param name="node"></param>
		protected virtual void NodeChanged(TNode node)
		{
			RefreshIfVisible();
		}

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected virtual void Subscribe(TNode node)
		{
			if (node == null)
				return;

			ISettingsLeaf leaf = node as ISettingsLeaf;
			if (leaf != null)
				leaf.OnSettingsChanged += LeafOnSettingsChanged;
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected virtual void Unsubscribe(TNode node)
		{
			if (node == null)
				return;

			ISettingsLeaf leaf = node as ISettingsLeaf;
			if (leaf != null)
				leaf.OnSettingsChanged -= LeafOnSettingsChanged;
		}

		/// <summary>
		/// Called when the leaf settings change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void LeafOnSettingsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
