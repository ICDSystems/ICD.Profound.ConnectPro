using System;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings
{
	public abstract class AbstractSettingsNodeBasePresenter<TView, TNode> : AbstractTouchDisplayPresenter<TView>,
	                                                                        ISettingsNodeBasePresenter<TView, TNode>
		where TView : class, ITouchDisplayView
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
		protected AbstractSettingsNodeBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
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
		}

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		private void Subscribe(TNode node)
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
		private void Unsubscribe(TNode node)
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
