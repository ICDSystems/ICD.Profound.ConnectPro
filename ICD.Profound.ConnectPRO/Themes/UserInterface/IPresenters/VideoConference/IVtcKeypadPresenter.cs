using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcKeypadPresenter : IPresenter
	{
		event EventHandler OnKeyboardButtonPressed;
	}
}