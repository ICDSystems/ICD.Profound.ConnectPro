using ICD.Connect.Panels;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews
{
	/// <summary>
	/// IViewFactory provides functionality for a presenter to obtain its view.
	/// </summary>
	public interface IOsdViewFactory
	{
		/// <summary>
		/// Gets the panel for this view factory.
		/// </summary>
		IPanelDevice Panel { get; }

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetNewView<T>() where T : class, IOsdView;
	}
}
