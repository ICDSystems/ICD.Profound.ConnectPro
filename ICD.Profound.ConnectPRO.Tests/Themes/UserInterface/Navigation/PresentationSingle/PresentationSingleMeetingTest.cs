using NUnit.Framework;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.PresentationSingle
{
	[TestFixture]
	public sealed class PresentationSingleMeetingTest : AbstractMeetingTest<PresentationSingleRoomType>
	{
		protected override PresentationSingleRoomType InstantiateRoomType()
		{
			return new PresentationSingleRoomType();
		}
	}
}
