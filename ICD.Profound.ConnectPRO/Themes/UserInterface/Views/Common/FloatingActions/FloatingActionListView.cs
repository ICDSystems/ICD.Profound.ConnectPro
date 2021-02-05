using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	[ViewBinding(typeof(IFloatingActionListView))]
	public sealed partial class FloatingActionListView : AbstractUiView, IFloatingActionListView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public FloatingActionListView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public FloatingActionListView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public event EventHandler<IntEventArgs> OnButtonPressed;

		public int MaxItems { get { return NUMBER_OF_ITEMS; } }

		public void ShowItem(int index, string label, string icon)
		{
			if (index >= NUMBER_OF_ITEMS)
				throw new IndexOutOfRangeException();

			if (m_Labels != null)
				m_Labels[index].SetLabelText(label);
			if (m_Icons != null)
				m_Icons[index].SetIcon(icon);
			if (m_Buttons != null)
				m_Buttons.GetValue(index).Show(true);
		}

		public void HideItem(int index)
		{
			if (index >= NUMBER_OF_ITEMS)
				throw new IndexOutOfRangeException();

			if (m_Buttons != null)
				m_Buttons.GetValue(index).Show(false);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			foreach(VtProButton button in m_Buttons.Values)
				button.OnPressed += ButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			foreach (VtProButton button in m_Buttons.Values)
				button.OnPressed -= ButtonOnPressed;
		}

		private void ButtonOnPressed(object sender, EventArgs e)
		{
			VtProButton button = sender as VtProButton;
			if (button == null)
				return;

			int index;
			if (m_Buttons.TryGetKey(button, out index))
				OnButtonPressed.Raise(this, new IntEventArgs(index));
		}
	}
}
