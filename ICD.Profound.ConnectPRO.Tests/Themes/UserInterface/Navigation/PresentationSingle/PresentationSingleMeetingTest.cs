using NUnit.Framework;
using ICD.Profound.ConnectPRO.Tests.RoomTypes.Common;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes.PresentationSingle
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
