using ICD.Profound.ConnectPRO.Tests.RoomTypes.Common;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation
{
    public abstract class AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
    {
		protected abstract TRoomType InstantiateRoomType();
	}
}
