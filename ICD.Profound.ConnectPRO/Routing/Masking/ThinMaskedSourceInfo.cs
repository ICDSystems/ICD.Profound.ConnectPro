using ICD.Common.Properties;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public delegate void ThinMaskedSourceInfoMaskCallback(ThinMaskedSourceInfo sender);
	public delegate void ThinMaskedSourceInfoUnmaskCallback(ThinMaskedSourceInfo sender);

	public sealed class ThinMaskedSourceInfo : AbstractMaskedSourceInfo 
	{
		public ThinMaskedSourceInfoMaskCallback MaskCallback { get; set; }

		public ThinMaskedSourceInfoUnmaskCallback UnmaskCallback { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ThinMaskedSourceInfo([NotNull] IConnectProRoom room)
			: base(room)
		{
		}

		protected override void PerformMask()
		{
			var handler = MaskCallback;
			if (handler != null)
				handler(this);
		}

		protected override void PerformUnmask()
		{
			var handler = UnmaskCallback;
			if (handler != null)
				handler(this);
		}
	}
}
