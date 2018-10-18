using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	/// <summary>
	/// Base class for all of the views.
	/// </summary>
	public abstract class AbstractOsdView : IOsdView
	{
		public event EventHandler<BoolEventArgs> OnVisibilityChanged;
		public event EventHandler<BoolEventArgs> OnEnabledChanged;

		private readonly ConnectProTheme m_Theme;

		private IVtProControl m_CachedPage;

		/// <summary>
		/// Returns true if the view is visible.
		/// </summary>
		public bool IsVisible { get { return Page.IsVisible; } }

		/// <summary>
		/// Returns true if the view is enabled.
		/// </summary>
		public bool IsEnabled { get { return Page.IsEnabled; } }

		/// <summary>
		/// Gets the page control.
		/// </summary>
		public IVtProControl Page
		{
			get
			{
				return m_CachedPage ?? (m_CachedPage = GetChildren().FirstOrDefault(c => c is VtProSubpage || c is VtProPage));
			}
		}

		public ConnectProTheme Theme { get { return m_Theme; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractOsdView(ISigInputOutput panel, ConnectProTheme theme)
			: this(panel, theme, null, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractOsdView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
		{
			m_Theme = theme;

			InstantiateControls(panel, parent, index);
			SubscribeControls();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			OnVisibilityChanged = null;
			OnEnabledChanged = null;

			UnsubscribeControls();

			foreach (IVtProControl control in GetChildren())
				control.Dispose();
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public virtual void Show(bool visible)
		{
			if (visible == IsVisible)
				return;

			try
			{
				Page.Show(visible);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, e, string.Format("Unable to show {0}", GetType().Name));
				throw;
			}

			OnVisibilityChanged.Raise(this, new BoolEventArgs(visible));
		}

		/// <summary>
		/// Sets the enabled state of the view.
		/// </summary>
		/// <param name="enabled"></param>
		public void Enable(bool enabled)
		{
			if (enabled == IsEnabled)
				return;

			try
			{
				Page.Enable(enabled);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, e, string.Format("Unable to enable {0}", GetType().Name));
				throw;
			}

			OnEnabledChanged.Raise(this, new BoolEventArgs(enabled));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected abstract void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index);

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<IVtProControl> GetChildren();

		/// <summary>
		/// Generates child views for the given subpage reference list. Sets the number of items in the SRL.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory"></param>
		/// <param name="subpageReferenceList"></param>
		/// <param name="viewList"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected static IEnumerable<T> GetChildViews<T>(IOsdViewFactory factory, VtProSubpageReferenceList subpageReferenceList,
		                                                 List<T> viewList, ushort count)
			where T : class, IOsdView
		{
			if (factory as ConnectProOsdViewFactory == null)
				throw new ArgumentException("View factory must support SRLs", "factory");

			return (factory as ConnectProOsdViewFactory).GetNewSrlViews(subpageReferenceList, viewList, count);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected virtual void SubscribeControls()
		{
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected virtual void UnsubscribeControls()
		{
		}

		protected void Log(eSeverity severity, string message, params string[] args)
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger != null)
				logger.AddEntry(severity, message, args);
		}

		protected void Log(eSeverity severity, Exception ex, string message, params string[] args)
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger != null)
				logger.AddEntry(severity, ex, message, args);
		}

		#endregion
	}
}
