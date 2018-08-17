using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO.WebConferencing
{
	public sealed class WebConferencingAppInstructions : ICollection<WebConferencingStepInstructions>
	{
		private readonly List<WebConferencingStepInstructions> m_Steps;
		private readonly SafeCriticalSection m_StepsSection;

		#region Properties

		/// <summary>
		/// Gets the number of apps.
		/// </summary>
		public int Count { get { return m_Steps.Count; } }

		public bool IsReadOnly { get { return false; } }

		public string Name { get; set; }
		public string Icon { get; set; }

		/// <summary>
		/// Gets the step at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public WebConferencingStepInstructions this[int index] { get { return m_StepsSection.Execute(() => m_Steps[index]); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public WebConferencingAppInstructions()
		{
			m_Steps = new List<WebConferencingStepInstructions>();
			m_StepsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Instantiates a WebConferencingAppInstructions instance from an xml document.
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static WebConferencingAppInstructions FromXml(string baseUrl, string xml)
		{
			WebConferencingAppInstructions output = new WebConferencingAppInstructions();
			output.Parse(baseUrl, xml);
			return output;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parses the presets xml document and adds the steps to the collection.
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="xml"></param>
		[PublicAPI]
		public void Parse(string baseUrl, string xml)
		{
			Clear();

			Name = XmlUtils.TryReadChildElementContentAsString(xml, "Name");
			Icon = (baseUrl ?? string.Empty) + XmlUtils.TryReadChildElementContentAsString(xml, "Icon");

			IEnumerable<WebConferencingStepInstructions> apps = ParseSteps(baseUrl, xml);
			m_StepsSection.Execute(() => m_Steps.AddRange(apps));
		}

		/// <summary>
		/// Returns the steps from the given xml document.
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<WebConferencingStepInstructions> ParseSteps(string baseUrl, string xml)
		{
			string steps = XmlUtils.GetChildElementAsString(xml, "Steps");

			return XmlUtils.GetChildElementsAsString(steps, "Step")
						   .Select(s => WebConferencingStepInstructions.FromXml(baseUrl, s));
		}

		public IEnumerator<WebConferencingStepInstructions> GetEnumerator()
		{
			// Copy the list and return THAT enumerator for thread safety.
			List<WebConferencingStepInstructions> output = m_StepsSection.Execute(() => m_Steps.ToList());
			return output.GetEnumerator();
		}

		public void Add(WebConferencingStepInstructions item)
		{
			m_StepsSection.Execute(() => m_Steps.Add(item));
		}

		public void Clear()
		{
			m_StepsSection.Execute(() => m_Steps.Clear());
		}

		public bool Contains(WebConferencingStepInstructions item)
		{
			return m_StepsSection.Execute(() => m_Steps.Contains(item));
		}

		public void CopyTo(WebConferencingStepInstructions[] array, int arrayIndex)
		{
			m_StepsSection.Execute(() => m_Steps.CopyTo(array, arrayIndex));
		}

		public bool Remove(WebConferencingStepInstructions item)
		{
			return m_StepsSection.Execute(() => m_Steps.Remove(item));
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
