using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ZoomSettingsNode()
		{
			Name = "Zoom";
			Icon = eSettingsIcon.ZoomRoom;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new ZoomGeneralSettingsLeaf();
			yield return new ZoomAdvancedSettingsLeaf();
			yield return new ZoomCameraSettingsLeaf();
			yield return new ZoomMicrophoneSettingsLeaf();
			yield return new ZoomSpeakerSettingsLeaf();
		}
	}
}
