using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcContactsView : AbstractView, IVtcContactsView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnDirectoryButtonPressed;
		public event EventHandler OnFavoritesButtonPressed;
		public event EventHandler OnRecentButtonPressed;
		public event EventHandler OnCallButtonPressed;
		public event EventHandler OnHangupButtonPressed;

		private readonly List<IVtcReferencedContactsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcContactsView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
			m_ChildList = new List<IVtcReferencedContactsView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ChildList, count);
		}
	}
}
