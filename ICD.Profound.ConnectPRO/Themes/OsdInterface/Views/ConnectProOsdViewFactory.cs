using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Welcome;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class ConnectProOsdViewFactory : AbstractViewFactory, IOsdViewFactory
	{
		private delegate IOsdView FactoryMethod(ISigInputOutput panel, ConnectProTheme theme);

		private delegate IOsdView ComponentFactoryMethod(
			ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index);

		private readonly Dictionary<Type, ComponentFactoryMethod> m_ComponentViewFactories = new Dictionary
			<Type, ComponentFactoryMethod>
		{
		    {typeof(IReferencedScheduleView), (panel, theme, parent, index) => new ReferencedScheduleView(panel, theme, parent, index)}
		};

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			{typeof(IOsdHeaderView), (panel, theme) => new OsdHeaderView(panel, theme)},
			{typeof(IOsdWelcomeView), (panel, theme) => new OsdWelcomeView(panel, theme)},
			{typeof(IOsdSourcesView), (panel, theme) => new OsdSourcesView(panel, theme)},
			
			{typeof(IOsdHelloView), (panel, theme) => new OsdHelloView(panel, theme)},
			{typeof(IOsdIncomingCallView), (panel, theme) => new OsdIncomingCallView(panel, theme)},
			{typeof(IOsdMuteView), (panel, theme) => new OsdMuteView(panel, theme)},

			{typeof(IReferencedScheduleView), (panel, theme) => new ReferencedScheduleView(panel, theme)}
		};

		private readonly ConnectProTheme m_Theme;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProOsdViewFactory(IPanelDevice panel, ConnectProTheme theme)
			: base(panel)
		{
			m_Theme = theme;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected override T InstantiateView<T>()
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IOsdView output = factory(Panel, m_Theme);

			if (output as T == null)
				throw new InvalidCastException(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Instantiates a view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="smartObject"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override T InstantiateView<T>(ISmartObject smartObject, IVtProParent parent, ushort index)
		{
			if (!m_ComponentViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			ComponentFactoryMethod factory = m_ComponentViewFactories[typeof(T)];
			IOsdView output = factory(smartObject, m_Theme, parent, index);

			if (output as T == null)
				throw new InvalidCastException(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		#endregion
	}
}
