using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class ConnectProOsdViewFactory : IOsdViewFactory
	{
		private delegate IOsdView FactoryMethod(ISigInputOutput panel, ConnectProTheme theme);

		private delegate IOsdView ComponentFactoryMethod(
			ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index);

		/// <summary>
		/// Gets the panel for this view factory.
		/// </summary>
		public IPanelDevice Panel { get { return m_Panel; } }

		private readonly Dictionary<Type, ComponentFactoryMethod> m_ComponentViewFactories = new Dictionary
			<Type, ComponentFactoryMethod>
		{
		};

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			{typeof(IOsdSourcesView), (panel, theme) => new OsdSourcesView(panel, theme)},
			{typeof(IOsdWelcomeView), (panel, theme) => new OsdWelcomeView(panel, theme)}
		};

		private readonly IPanelDevice m_Panel;
		private readonly ConnectProTheme m_Theme;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProOsdViewFactory(IPanelDevice panel, ConnectProTheme theme)
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
			where T : class, IOsdView
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IOsdView output = factory(m_Panel, m_Theme);

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
			IOsdView output = factory(panel, m_Theme, parent, index);

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
