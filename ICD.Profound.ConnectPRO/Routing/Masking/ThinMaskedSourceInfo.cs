namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public delegate void ThinMaskedSourceInfoMaskCallback(ThinMaskedSourceInfo sender);
	public delegate void ThinMaskedSourceInfoUnmaskCallback(ThinMaskedSourceInfo sender);

	public class ThinMaskedSourceInfo : AbstractMaskedSourceInfo 
	{
		public ThinMaskedSourceInfoMaskCallback MaskCallback { get; set; }

		public ThinMaskedSourceInfoUnmaskCallback UnmaskCallback { get; set; }

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
