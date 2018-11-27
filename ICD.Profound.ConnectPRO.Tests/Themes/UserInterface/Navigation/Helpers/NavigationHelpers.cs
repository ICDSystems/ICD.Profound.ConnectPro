using System.Threading;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Protocol.Sigs;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers
{
    public static class NavigationHelpers
    {
		// TODO - Press button, check subpage visibility, etc

	    public static void PressButton(uint number, MockPanelDevice panel, ushort smartObjectId = 0, bool hold = false, ushort holdtime = 0)
		{
			panel.RaiseOutputSigChange(new SigInfo(number, smartObjectId, true));
			if (hold)
			{
				Thread.Sleep(holdtime);
			}
		    panel.RaiseOutputSigChange(new SigInfo(number, smartObjectId, false));
		}

	    public static void PressButton(uint number, MockSmartObject panel, ushort smartObjectId = 0)
	    {
		    panel.RaiseOutputSigChange(new SigInfo(number, smartObjectId, true));
		    panel.RaiseOutputSigChange(new SigInfo(number, smartObjectId, false));
	    }

	    //public static void PressButton(uint number, MockSmartObject panel, ushort value, ushort smartObjectId = 0)
	    //{
		   // panel.RaiseOutputSigChange(new SigInfo(number, smartObjectId, value));
	    //}

		public static void PressListButton(uint itemJoin, ushort smartObjectId, ushort itemClicked, MockPanelDevice panel)
		{
			var list = panel.SmartObjects[smartObjectId] as MockSmartObject;

			Assert.IsNotNull(list);

			// System power preferences is selected
			Assert.IsTrue(list.BooleanInput[11].GetBoolValue());

			// Press the second item on the menu (passcode settings)
			list.RaiseOutputSigChange(new SigInfo(itemJoin, smartObjectId, itemClicked));
		}

		public static bool CheckVisibilty(uint number, ISigInputOutput panel)
	    {
			return ((MockPanelDevice)panel).BooleanInput[number].GetBoolValue();
		}

	    public static bool SmartObjectCheckVisibilty(uint index, ushort smartObjectId, MockPanelDevice panel)
	    {
		    var list = panel.SmartObjects[smartObjectId] as MockSmartObject;

		    Assert.IsNotNull(list);

		    // System power preferences is selected
		    return list.BooleanInput[index].GetBoolValue();
		}
	}
}
