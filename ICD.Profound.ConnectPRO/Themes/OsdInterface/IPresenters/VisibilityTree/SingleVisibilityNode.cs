using System.Linq;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.VisibilityTree
{
	/// <summary>
	/// A SingleVisibilityNode is a collection of presenters in which only a
	/// maximum of one child/node may be visible at a given time.
	/// </summary>
	public sealed class SingleVisibilityNode : AbstractVisibilityNode
	{
		#region Private Methods

		/// <summary>
		/// Called when a descendant presenter changes visibility.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="presenter"></param>
		/// <param name="visibility"></param>
		protected override void NodeOnChildVisibilityChanged(IVisibilityNode parent, IOsdPresenter presenter, bool visibility)
		{
			if (presenter.IsViewVisible)
			{
				HideExcept(null as IOsdPresenter);
				HideExcept(parent);
			}

			base.NodeOnChildVisibilityChanged(parent, presenter, visibility);
		}

		/// <summary>
		/// Called when a child presenter visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void PresenterOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			IOsdPresenter presenter = sender as IOsdPresenter;

			if (args.Data)
			{
				HideExcept(presenter);
				HideExcept(null as IVisibilityNode);
			}

			base.PresenterOnVisibilityChanged(sender, args);
		}

		/// <summary>
		/// Hides child presenters except the given presenter.
		/// </summary>
		/// <param name="ignoreControl"></param>
		private void HideExcept(IOsdPresenter ignoreControl)
		{
			foreach (IOsdPresenter presenter in GetPresenters().Where(c => c != ignoreControl))
				presenter.ShowView(false);
		}

		/// <summary>
		/// Hides child nodes except the given node.
		/// </summary>
		/// <param name="ignoreNode"></param>
		private void HideExcept(IVisibilityNode ignoreNode)
		{
			foreach (IVisibilityNode node in GetNodes().Where(n => n != ignoreNode))
				node.Hide();
		}

		#endregion
	}
}
