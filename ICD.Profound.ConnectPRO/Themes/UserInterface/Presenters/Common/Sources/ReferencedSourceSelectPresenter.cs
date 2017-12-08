using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	public sealed class ReferencedSourceSelectPresenter : AbstractComponentPresenter<IReferencedSourceSelectView>,
	                                                      IReferencedSourceSelectPresenter
	{
		private const int MAX_LINE_WIDTH = 10;

		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		
		private ISource m_Source;
		private bool m_Selected;

		#region Properties

		public ISource Source
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				m_Source = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the selected state of the presenter.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSourceSelectPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IReferencedSourceSelectView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool combine = Room != null && Room.IsCombineRoom();
				IRoom room = Room == null ? null : Room.Routing.GetRoomForSource(m_Source);

				string sourceName =
					m_Source == null
						? string.Empty
						: m_Source.GetName(combine) ?? string.Empty;

				string text = sourceName.ToUpper();

				string line1 = string.Empty;
				string line2 = string.Empty;

				if (text.Length <= MAX_LINE_WIDTH)
					line1 = text;
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = text.Length / 2;
					int splitIndex = text.FindIndices(c => char.IsWhiteSpace(c)).GetClosest(i => i - middleIndex);

					line1 = text.Substring(0, splitIndex).Trim();
					line2 = text.Substring(splitIndex + 1).Trim();
				}

				view.SetColor(m_Selected ? eSourceColor.Yellow : eSourceColor.White);
				view.SetFeedbackText(room == null ? string.Empty : room.GetName(combine));
				view.SetLine1Text(line1);
				view.SetLine2Text(line2);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedSourceSelectView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedSourceSelectView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
