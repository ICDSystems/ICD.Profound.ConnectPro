using System.Linq;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VisibilityTree
{
	/// <summary>
	/// Ensures that a presenter is visible when all other child nodes/presenters
	/// in the heirarchy are not visible.
	/// </summary>
	public sealed class DefaultVisibilityNode : AbstractVisibilityNode
	{
		private readonly IPresenter m_DefaultPresenter;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="defaultPresenter"></param>
		public DefaultVisibilityNode(IPresenter defaultPresenter)
		{
			m_DefaultPresenter = defaultPresenter;
			AddPresenter(m_DefaultPresenter);
			Subscribe(m_DefaultPresenter);
		}

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IPresenter presenter)
		{
			presenter.OnViewVisibilityChanged += DefaultPresenterOnVisibilityChanged;
		}

		/// <summary>
		/// Called when the default presenter visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DefaultPresenterOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			if (!args.Data)
				return;

			// Hide all other children when the default presenter becomes visible.
			foreach (IPresenter presenter in GetPresenters().Where(c => c != m_DefaultPresenter))
				presenter.ShowView(false);
			foreach (IVisibilityNode node in GetNodes())
				node.Hide();
		}

		#endregion

		/// <summary>
		/// Called when a child presenter visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void PresenterOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.PresenterOnVisibilityChanged(sender, args);

			if (sender == m_DefaultPresenter)
				return;

			if (!args.Data && !this.GetIsVisible())
				m_DefaultPresenter.ShowView(true);
		}

		/// <summary>
		/// Called when a descendant presenter changes visibility.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="presenter"></param>
		/// <param name="visibility"></param>
		protected override void NodeOnChildVisibilityChanged(IVisibilityNode parent, IPresenter presenter, bool visibility)
		{
			base.NodeOnChildVisibilityChanged(parent, presenter, visibility);

			if (!visibility && !this.GetIsVisible())
				m_DefaultPresenter.ShowView(true);
		}
	}
}
