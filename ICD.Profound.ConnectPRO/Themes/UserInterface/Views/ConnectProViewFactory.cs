using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class ConnectProViewFactory : IViewFactory
	{
		private delegate IView FactoryMethod(ISigInputOutput panel, ConnectProTheme theme);

		private delegate IView ComponentFactoryMethod(
			ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index);

		/// <summary>
		/// Gets the panel for this view factory.
		/// </summary>
		public IPanelDevice Panel { get { return m_Panel; } }

		private readonly Dictionary<Type, ComponentFactoryMethod> m_ComponentViewFactories = new Dictionary
			<Type, ComponentFactoryMethod>
		{
			// Sources
			{typeof(IReferencedSourceSelectView), (nav, views, theme, index) => new ReferencedSourceSelectView(nav, views, theme, index)},

			// Displays
			{typeof(IReferencedDisplaysView), (nav, views, theme, index) => new ReferencedDisplaysView(nav, views, theme, index)},
		};

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			// Popups
			{typeof(IAppleTvView), (panel, theme) => new AppleTvView(panel, theme)},

			// Displays
			{typeof(IDisplaysView), (panel, theme) => new DisplaysView(panel, theme)},

			// Options
			{typeof(IOptionContactsView), (panel, theme) => new OptionContactsView(panel, theme)},
			{typeof(IOptionMasterMicView), (panel, theme) => new OptionMasterMicView(panel, theme)},
			{typeof(IOptionPrivacyMuteView), (panel, theme) => new OptionPrivacyMuteView(panel, theme)},
			{typeof(IOptionVolumeView), (panel, theme) => new OptionVolumeView(panel, theme)},

			// Sources
			{typeof(ISourceSelectDualView), (panel, theme) => new SourceSelectDualView(panel, theme)},
			{typeof(ISourceSelectSingleView), (panel, theme) => new SourceSelectSingleView(panel, theme)},

			// Common
			{typeof(IEndMeetingView), (panel, theme) => new EndMeetingView(panel, theme)},
			{typeof(IHeaderView), (panel, theme) => new HeaderView(panel, theme)},
			{typeof(IStartMeetingView), (panel, theme) => new StartMeetingView(panel, theme)},

			// Panel
			{typeof(IHardButtonsView), (panel, theme) => new HardButtonsView(panel, theme)}
		};

		private readonly IPanelDevice m_Panel;
		private readonly ConnectProTheme m_Theme;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProViewFactory(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			m_Theme = theme;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetNewView<T>()
			where T : class, IView
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IView output = factory(m_Panel, m_Theme);

			if (output as T == null)
				throw new Exception(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Instantiates a view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private T GetNewView<T>(ISigInputOutput panel, IVtProParent parent, ushort index)
			where T : class, IView
		{
			if (!m_ComponentViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			ComponentFactoryMethod factory = m_ComponentViewFactories[typeof(T)];
			IView output = factory(panel, m_Theme, parent, index);

			if (output as T == null)
				throw new Exception(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Creates views for the given subpage reference list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="childViews"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<T> GetNewSrlViews<T>(VtProSubpageReferenceList list, List<T> childViews, ushort count)
			where T : class, IView
		{
			count = Math.Min(count, list.MaxSize);
			list.SetNumberOfItems(count);

			ISmartObject smartObject = list.SmartObject;

			for (ushort index = (ushort)childViews.Count; index < count; index++)
			{
				T view = GetNewView<T>(smartObject, list, index);
				childViews.Add(view);
			}

			return childViews.Take(count);
		}

		#endregion
	}
}
