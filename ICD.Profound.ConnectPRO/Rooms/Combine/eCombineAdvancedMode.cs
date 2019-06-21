using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Rooms.Combine
{
	public enum eCombineAdvancedMode
	{
		Simple,
		Advanced
	}

	public sealed class CombineAdvancedModeEventArgs : GenericEventArgs<eCombineAdvancedMode>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public CombineAdvancedModeEventArgs(eCombineAdvancedMode data)
			: base(data)
		{
		}
	}
}
