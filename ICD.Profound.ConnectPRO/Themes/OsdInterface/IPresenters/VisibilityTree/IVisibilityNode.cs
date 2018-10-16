using System.Collections.Generic;
using System.Linq;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.VisibilityTree
{
	public delegate void ChildVisibilityChangedCallback(IVisibilityNode parent, IOsdPresenter presenter, bool visibility);

	public interface IVisibilityNode
	{
		event ChildVisibilityChangedCallback OnChildVisibilityChanged;

		/// <summary>
		/// Adds the node to the tree.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		void AddNode(IVisibilityNode node);

		/// <summary>
		/// Adds the presenter to the tree.
		/// </summary>
		/// <param name="presenter"></param>
		/// <returns></returns>
		void AddPresenter(IOsdPresenter presenter);

		/// <summary>
		/// Gets the immediate child nodes.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IVisibilityNode> GetNodes();

		/// <summary>
		/// Gets the immediate child presenters.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IOsdPresenter> GetPresenters();
	}

	/// <summary>
	/// Extension methods for IVisibilityNodes.
	/// </summary>
	public static class VisibilityNodeExtensions
	{
		/// <summary>
		/// Recursively hides all children of the node.
		/// </summary>
		/// <param name="extends"></param>
		public static void Hide(this IVisibilityNode extends)
		{
			foreach (IVisibilityNode node in extends.GetNodes())
				node.Hide();
			foreach (IOsdPresenter presenter in extends.GetPresenters())
				presenter.ShowView(false);
		}

		/// <summary>
		/// Returns true if any of the child nodes in the hierarchy are visible.
		/// </summary>
		/// <returns></returns>
		/// <param name="extends"></param>
		public static bool GetIsVisible(this IVisibilityNode extends)
		{
			return extends.GetPresenters().Any(c => c.IsViewVisible) || extends.GetNodes().Any(n => n.GetIsVisible());
		}

		/// <summary>
		/// Adds the nodes as children.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="nodes"></param>
		public static void AddNodes(this IVisibilityNode extends, IEnumerable<IVisibilityNode> nodes)
		{
			foreach (IVisibilityNode node in nodes)
				extends.AddNode(node);
		}

		/// <summary>
		/// Adds the presenters as children.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="presenters"></param>
		public static void AddPresenters(this IVisibilityNode extends, IEnumerable<IOsdPresenter> presenters)
		{
			foreach (IOsdPresenter presenter in presenters)
				extends.AddPresenter(presenter);
		}
	}
}
