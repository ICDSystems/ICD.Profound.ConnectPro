namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Header
{
	public interface IHeaderPresenter : ITouchDisplayPresenter
	{
		bool Collapsed { get; }

		void AddLeftButton(HeaderButtonModel button);
		void RemoveLeftButton(HeaderButtonModel button);
		bool ContainsLeftButton(HeaderButtonModel button);

		void AddRightButton(HeaderButtonModel button);
		void RemoveRightButton(HeaderButtonModel button);
		bool ContainsRightButton(HeaderButtonModel button);
	}
}