using System.Linq;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common
{
	public abstract class AbstractMainMenuTest<TRoomType> : AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
	{
		protected const ushort DIGITAL_INCREMENT = 2;
		protected const ushort ANALOG_INCREMENT = 2;

		protected const ushort BUTTON_MODE_START = 11;
		protected const ushort FEEDBACK_MODE_START = 12;

		[Test]
		public void VolumeControlTest()
		{
			using (TRoomType roomType = InstantiateRoomType())
			{
				if (roomType.Room.Originators.GetInstances<IVolumePoint>().FirstOrDefault() == null)
					return;

				// Simulate "start meeting" button press
				NavigationHelpers.PressButton(91, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);

				// Check floating volume button visible
				Assert.IsTrue(NavigationHelpers.CheckVisibilty(40, roomType.Panel));

				// Click floating volume button
				NavigationHelpers.PressButton(45, roomType.Panel);

				// Check volume page is visible
				Assert.IsTrue(NavigationHelpers.CheckVisibilty(11, roomType.Panel));

				// Get volume point
				IVolumePoint volumePoint = roomType.Room.Originators.GetInstance<IVolumePoint>();
				Assert.NotNull(volumePoint);

				// Get volume control
				IVolumeDeviceControl volumeControl = volumePoint.Control;

				// Check that volume is at zero
				Assert.AreEqual(0, volumeControl.VolumeLevel);

				// vol up
				NavigationHelpers.PressButton(523, roomType.Panel);
				Assert.AreEqual((int)(65535 * 0.01 * volumeControl.VolumeLevel), roomType.Panel.UShortInput[500].GetUShortValue(), 655);

				// vol up
				NavigationHelpers.PressButton(523, roomType.Panel, 0, true, 2000);
				Assert.AreEqual((int)(65535 * 0.01 * volumeControl.VolumeLevel), roomType.Panel.UShortInput[500].GetUShortValue(), 655);

				// vol down
				NavigationHelpers.PressButton(522, roomType.Panel, 0, true, 2000);
				Assert.AreEqual((int)(65535 * 0.01 * volumeControl.VolumeLevel), roomType.Panel.UShortInput[500].GetUShortValue(), 655);

				// vol down
				NavigationHelpers.PressButton(522, roomType.Panel);
				Assert.AreEqual((int)(65535 * 0.01 * volumeControl.VolumeLevel), roomType.Panel.UShortInput[500].GetUShortValue(), 655);

				// mute
				NavigationHelpers.PressButton(524, roomType.Panel);
				Assert.IsTrue(volumeControl.IsMuted);
				NavigationHelpers.PressButton(524, roomType.Panel);
				Assert.IsFalse(volumeControl.IsMuted);

				//exit
				NavigationHelpers.PressButton(46, roomType.Panel);

				//Check volume page is not visible
				Assert.IsFalse(NavigationHelpers.CheckVisibilty(11, roomType.Panel));
			}
		}

		[Test]
		public void VisibleHeaderTest()
		{
			using (TRoomType roomType = InstantiateRoomType())
			{
				// Simulate "start meeting" button press
				NavigationHelpers.PressButton(91, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);

				NavigationHelpers.CheckVisibilty(0, roomType.Panel);
			}
		}

		protected void SourcePressTest(TRoomType roomType, ISource source)
		{
			//Source Override
			eControlOverride controlOverride = ConnectProRoutingSources.GetControlOverride(source);

			//Source Index
			uint sourceIndex = (uint)roomType.Room.Routing.Sources.GetCoreSources().ToList().IndexOf(source);

			// Get number of displays
			var displayCount = roomType.Room.Routing.Destinations.GetVideoDestinations().Count();

			MockSmartObject sourcesSmartObject = roomType.Panel.SmartObjects[1] as MockSmartObject;

			//Select source
			NavigationHelpers.PressButton(4011 + (DIGITAL_INCREMENT * sourceIndex), sourcesSmartObject, 1);

			// If we have one display we check routed state
			switch (controlOverride)
			{
				case eControlOverride.Vtc:
					VtcSourcePress(roomType, source, displayCount > 1
						, BUTTON_MODE_START + (ANALOG_INCREMENT * sourceIndex)
						, FEEDBACK_MODE_START + (ANALOG_INCREMENT * sourceIndex));
					break;

				case eControlOverride.Atc:
					AtcSourcePress(roomType, source
						, BUTTON_MODE_START + (ANALOG_INCREMENT * sourceIndex)
						, FEEDBACK_MODE_START + (ANALOG_INCREMENT * sourceIndex));
					break;

				case eControlOverride.CableTv:
					TvSourcePress(roomType, source, displayCount > 1
						, BUTTON_MODE_START + (ANALOG_INCREMENT * sourceIndex)
						, FEEDBACK_MODE_START + (ANALOG_INCREMENT * sourceIndex));
					break;

				case eControlOverride.WebConference:
					WebSourcePress(roomType, source, displayCount > 1
						, BUTTON_MODE_START + (ANALOG_INCREMENT * sourceIndex)
						, FEEDBACK_MODE_START + (ANALOG_INCREMENT * sourceIndex));
					break;

				case eControlOverride.Default:
					DefaultSourcePress(roomType, source, displayCount > 1
						, BUTTON_MODE_START + (ANALOG_INCREMENT * sourceIndex)
						, FEEDBACK_MODE_START + (ANALOG_INCREMENT * sourceIndex));
					break;
			}
		}

		private void DefaultSourcePress(TRoomType roomType, ISource source, bool dualDisplay, uint buttonModeId, uint feedbackModeId)
		{
			if (dualDisplay)
			{
				SelectiveMode(roomType, buttonModeId);
			}

			//Check source route/selective status
			CheckRoutingAndSelectiveStatus(roomType.Panel, buttonModeId, feedbackModeId);

			//Make sure the routes are active
			CheckActiveRoutes(roomType, source);
		}

		private void TvSourcePress(TRoomType roomType, ISource source, bool dualDisplay, uint buttonModeId, uint feedbackModeId)
		{
			if (dualDisplay)
			{
				SelectiveMode(roomType, buttonModeId);
			}

			//Check source route/selective status
			CheckRoutingAndSelectiveStatus(roomType.Panel, buttonModeId, feedbackModeId);

			//Make sure the routes are active
			CheckActiveRoutes(roomType, source);
		}

		private void VtcSourcePress(TRoomType roomType, ISource source, bool dualDisplay, uint buttonModeId, uint feedbackModeId)
		{
			if (dualDisplay)
			{
				//Check routing to second display
			}

			//Check source route/selective status
			CheckRoutingAndSelectiveStatus(roomType.Panel, buttonModeId, feedbackModeId);

			//Make sure the routes are active
			CheckActiveRoutes(roomType, source);
		}

		private void AtcSourcePress(TRoomType roomType, ISource source, uint buttonModeId, uint feedbackModeId)
		{
			//Check source route/selective status
			CheckRoutingAndSelectiveStatus(roomType.Panel, buttonModeId, feedbackModeId);

			//Make sure the routes are active
			CheckActiveRoutes(roomType, source);
		}

		private void WebSourcePress(TRoomType roomType, ISource source, bool dualDisplay, uint buttonModeId, uint feedbackModeId)
		{
			if (dualDisplay)
			{
				SelectiveMode(roomType, buttonModeId);
			}

			//Check source route/selective status
			CheckRoutingAndSelectiveStatus(roomType.Panel, buttonModeId, feedbackModeId);

			//Make sure the routes are active
			CheckActiveRoutes(roomType, source);

			//Check for visible alert
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(106, roomType.Panel));

			var webAlert = roomType.Panel.SmartObjects[15] as MockSmartObject;

			foreach (IBoolInputSig sig in webAlert.BooleanInput)
			{
				//Press button for step
				NavigationHelpers.PressButton(2000 + sig.Number, webAlert, 15);

				Assert.IsTrue(NavigationHelpers.CheckVisibilty(107, roomType.Panel));

				string graphic = roomType.Panel.StringInput[500].GetStringValue();

				//Press NEXT button
				NavigationHelpers.PressButton(543, roomType.Panel);

				//Check graphic
				Assert.AreNotEqual(graphic, roomType.Panel.StringInput[500].GetStringValue());

				//Press BACK button
				NavigationHelpers.PressButton(542, roomType.Panel);

				//Check graphic
				Assert.AreEqual(graphic, roomType.Panel.StringInput[500].GetStringValue());

				//Press Close button
				NavigationHelpers.PressButton(113, roomType.Panel);

				//Check for visible alert
				Assert.IsFalse(NavigationHelpers.CheckVisibilty(107, roomType.Panel));
			}

			//Press 'Dismiss' button
			NavigationHelpers.PressButton(140, roomType.Panel);

			//Alert should not be visible
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(106, roomType.Panel));
		}

		private void SelectiveMode(TRoomType roomType, uint buttonModeId)
		{
			MockSmartObject sourcesSmartObject = roomType.Panel.SmartObjects[1] as MockSmartObject;

			//Check source button status
			Assert.AreEqual(2, sourcesSmartObject?.UShortInput[buttonModeId].GetUShortValue());

			//Check/Click Display 1 button status
			Assert.AreEqual(2, sourcesSmartObject?.UShortInput[300].GetUShortValue());
			NavigationHelpers.PressButton(301, roomType.Panel);
			Assert.AreEqual(1, sourcesSmartObject?.UShortInput[300].GetUShortValue());

			//Check/Click Display 2 button status
			Assert.AreEqual(2, sourcesSmartObject?.UShortInput[301].GetUShortValue());
			NavigationHelpers.PressButton(304, roomType.Panel);
			Assert.AreEqual(1, sourcesSmartObject?.UShortInput[301].GetUShortValue());
		}

		private void CheckRoutingAndSelectiveStatus(MockPanelDevice panel, uint buttonModeId, uint feedbackModeId)
		{
			//Check source route status
			Assert.AreEqual(1, (panel.SmartObjects[1] as MockSmartObject)?.UShortInput[buttonModeId].GetUShortValue());

			//Check source selective status
			Assert.AreEqual(2, (panel.SmartObjects[1] as MockSmartObject)?.UShortInput[feedbackModeId].GetUShortValue());
		}

		private void CheckActiveRoutes(TRoomType roomType, ISource source)
		{
			//Get Displays
			var destinations = roomType.Room.Routing.Destinations.GetVideoDestinations();

			//Get active video routes
			var routingVideo = roomType.Room.Routing.State.GetFakeActiveVideoSources()
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			//Check active audio sources
			Assert.IsTrue(roomType.Room.Routing.State.GetCachedActiveAudioSources().Contains(source));

			//Check video routes
			foreach (var destination in destinations)
			{
				Assert.True(routingVideo.ContainsKey(destination) && routingVideo.Values.Any(v => v.Contains(source)));
			}
		}
	}
}