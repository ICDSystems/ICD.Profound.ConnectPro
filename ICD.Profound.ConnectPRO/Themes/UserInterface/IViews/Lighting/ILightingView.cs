using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Lighting
{
	public interface ILightingView : IUiView
	{

		event EventHandler OnClosePressed;

		event EventHandler<UShortEventArgs> OnPresetPressed;

		int MaxPresets { get; }

		void SetPresetLabels(IEnumerable<string> labels);

		void SetPresetActive(ushort index, bool state);
	}
}
