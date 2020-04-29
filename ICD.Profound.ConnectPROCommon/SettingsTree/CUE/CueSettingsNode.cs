﻿using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.CUE
{
	public sealed class CueSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public CueSettingsNode()
		{
			Name = "CUE";
			Icon = eSettingsIcon.Cue;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new BackgroundSettingsLeaf();
		}
	}
}
