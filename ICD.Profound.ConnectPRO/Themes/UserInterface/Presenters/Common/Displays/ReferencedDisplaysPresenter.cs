﻿using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class ReferencedDisplaysPresenter : AbstractComponentPresenter<IReferencedDisplaysView>,
	                                                  IReferencedDisplaysPresenter
	{
		private const int MAX_LINE_WIDTH = 10;

		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private IDestination m_Destination;
		private ISource m_ActiveSource;

		#region Properties

		/// <summary>
		/// Gets/sets the destination for this presenter.
		/// </summary>
		public IDestination Destination
		{
			get { return m_Destination; }
			set
			{
				if (value == m_Destination)
					return;

				m_Destination = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the source that is actively selected for routing.
		/// </summary>
		public ISource ActiveSource
		{
			get { return m_ActiveSource; }
			set
			{
				if (value == m_ActiveSource)
					return;

				m_ActiveSource = value;

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
		public ReferencedDisplaysPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
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

		protected override void Refresh(IReferencedDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool combine = Room != null && Room.IsCombineRoom();
				string destinationName =
					m_Destination == null
						? string.Empty
						: m_Destination.GetName(combine) ?? string.Empty;

				eDisplayColor color = m_ActiveSource == null ? eDisplayColor.Grey : eDisplayColor.Yellow;
				string text = m_ActiveSource == null ? destinationName : string.Format("PRESS TO SHOW SELECTION ON {0}", destinationName);
				text = text.ToUpper();

				string line1;
				string line2;

				if (text.Length <= MAX_LINE_WIDTH)
				{
					line1 = text;
					line2 = string.Empty;
				}
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = text.Length / 2;
					int splitIndex = text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

					line1 = text.Substring(0, splitIndex).Trim();
					line2 = text.Substring(splitIndex + 1).Trim();
				}

				view.SetColor(color);
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
		protected override void Subscribe(IReferencedDisplaysView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedDisplaysView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the display button.
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
