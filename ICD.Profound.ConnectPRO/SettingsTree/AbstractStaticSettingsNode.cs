using System.Collections.Generic;
using System.Linq;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractStaticSettingsNode : AbstractSettingsNode
	{
		private List<ISettingsNodeBase> m_Children; 

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			foreach (ISettingsNodeBase child in m_Children)
				child.Dispose();

			m_Children.Clear();
		}

		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <returns></returns>
		public sealed override IEnumerable<ISettingsNodeBase> GetChildren()
		{
			return m_Children ?? (m_Children = BuildChildren().ToList());
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<ISettingsNodeBase> BuildChildren();
	}
}
