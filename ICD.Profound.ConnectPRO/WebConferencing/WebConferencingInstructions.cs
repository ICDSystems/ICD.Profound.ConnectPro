using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO.WebConferencing
{
	public sealed class WebConferencingInstructions : ICollection<WebConferencingAppInstructions>
	{
		private readonly List<WebConferencingAppInstructions> m_Apps;
		private readonly SafeCriticalSection m_AppsSection;

		#region Properties

		/// <summary>
		/// Gets the number of apps.
		/// </summary>
		public int Count { get { return m_Apps.Count; } }

		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Gets the app at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public WebConferencingAppInstructions this[int index] { get { return m_AppsSection.Execute(() => m_Apps[index]); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public WebConferencingInstructions()
		{
			m_Apps = new List<WebConferencingAppInstructions>();
			m_AppsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Instantiates a WebConferencingInstructions instance from an xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static WebConferencingInstructions FromXml(string xml)
		{
			WebConferencingInstructions output = new WebConferencingInstructions();
			output.Parse(xml);
			return output;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parses the presets xml document and adds the apps to the collection.
		/// </summary>
		/// <param name="xml"></param>
		[PublicAPI]
		public void Parse(string xml)
		{
			Clear();

			IEnumerable<WebConferencingAppInstructions> apps = ParseApps(xml);
			m_AppsSection.Execute(() => m_Apps.AddRange(apps));
		}

		/// <summary>
		/// Returns the apps from the given xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<WebConferencingAppInstructions> ParseApps(string xml)
		{
			string baseUrl = XmlUtils.TryReadChildElementContentAsString(xml, "BaseUrl") ?? BuildDefaultBaseUrl();
			string apps = XmlUtils.GetChildElementAsString(xml, "Apps");

			return XmlUtils.GetChildElementsAsString(apps, "App")
						   .Select(s => WebConferencingAppInstructions.FromXml(baseUrl, s));
		}

		private static string BuildDefaultBaseUrl()
		{
			return new IcdUriBuilder
			{
				Host = IcdEnvironment.NetworkAddresses.FirstOrDefault(),
				Path = "/WebConferencing/",
			}.ToString();
		}

		public IEnumerator<WebConferencingAppInstructions> GetEnumerator()
		{
			// Copy the list and return THAT enumerator for thread safety.
			List<WebConferencingAppInstructions> output = m_AppsSection.Execute(() => m_Apps.ToList());
			return output.GetEnumerator();
		}

		public void Add(WebConferencingAppInstructions item)
		{
			m_AppsSection.Execute(() => m_Apps.Add(item));
		}

		public void Clear()
		{
			m_AppsSection.Execute(() => m_Apps.Clear());
		}

		public bool Contains(WebConferencingAppInstructions item)
		{
			return m_AppsSection.Execute(() => m_Apps.Contains(item));
		}

		public void CopyTo(WebConferencingAppInstructions[] array, int arrayIndex)
		{
			m_AppsSection.Execute(() => m_Apps.CopyTo(array, arrayIndex));
		}

		public bool Remove(WebConferencingAppInstructions item)
		{
			return m_AppsSection.Execute(() => m_Apps.Remove(item));
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
