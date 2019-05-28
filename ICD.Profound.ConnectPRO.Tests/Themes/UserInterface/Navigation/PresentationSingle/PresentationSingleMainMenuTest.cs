using System.Linq;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.PresentationSingle
{
	[TestFixture]
    class PresentationSingleMainMenuTest : AbstractMainMenuTest<PresentationSingleRoomType>
    {
	    protected override PresentationSingleRoomType InstantiateRoomType()
	    {
		    return new PresentationSingleRoomType();
	    }

		[Test]
		public void VisibleSourcesTest()
		{
			using (PresentationSingleRoomType roomType = InstantiateRoomType())
			{
				// Simulate "start meeting" button press
				NavigationHelpers.PressButton(91, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);

				var displayCount = roomType.Room.Routing.Destinations.GetDisplayDestinations().Count();
				var sources = roomType.Room.Routing.Sources.GetCoreSources().ToList();

				// Only show the displays subpage if there are multiple displays
				uint displaysSubpage = 112;
				bool displaysSubpageVisible = NavigationHelpers.CheckVisibilty(displaysSubpage, roomType.Panel);

				Assert.AreEqual(displaysSubpageVisible, displayCount > 1);

				// Ensure the correct sources subpage is visible
				// Depends on number of sources and number of destinations

				uint subpage = 112;

				if (displayCount < 2)
				{
					subpage = 101;

					if (sources.Count <= 4)
						subpage = 102;

					if (sources.Count <= 3)
						subpage = 103;

					if (sources.Count <= 2)
						subpage = 104;
				}

				if (subpage != 112)
				{
					Assert.IsFalse(NavigationHelpers.CheckVisibilty(1, roomType.Panel));
				}
				
				Assert.IsTrue(NavigationHelpers.CheckVisibilty(subpage, roomType.Panel));

				// Ensure sources are visible in the list
				for (uint i = 0; i < sources.Count; i++)
				{
					Assert.IsTrue(NavigationHelpers.SmartObjectCheckVisibilty(2011 + i, 1, roomType.Panel));
					SourcePressTest(roomType, sources[(int)i]);
				}

			}
		}
	}
}
