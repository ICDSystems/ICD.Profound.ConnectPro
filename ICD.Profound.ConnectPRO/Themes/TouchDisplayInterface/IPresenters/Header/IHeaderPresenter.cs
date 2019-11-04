namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header
{
	public interface IHeaderPresenter : ITouchDisplayPresenter
	{
		void AddLeftButton(HeaderButtonModel button);
		void RemoveLeftButton(HeaderButtonModel button);
		bool ContainsLeftButton(HeaderButtonModel button);

		void AddRightButton(HeaderButtonModel button);
		void RemoveRightButton(HeaderButtonModel button);
		bool ContainsRightButton(HeaderButtonModel button);
	}
}