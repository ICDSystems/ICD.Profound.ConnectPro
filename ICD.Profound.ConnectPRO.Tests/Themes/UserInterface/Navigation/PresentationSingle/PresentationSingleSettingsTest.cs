using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.PresentationSingle
{
	[TestFixture]
	public sealed class PresentationSingleSettingsTest : AbstractSettingsTest<PresentationSingleRoomType>
	{
		protected override PresentationSingleRoomType InstantiateRoomType()
		{
			return new PresentationSingleRoomType();
		}
	}
}
