using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.Contacts
{
	[ViewBinding(typeof(IReferencedSelectedContactView))]
	public sealed partial class ReferencedSelectedContactView : AbstractTouchDisplayView, IReferencedSelectedContactView
	{
		public event EventHandler OnRemovePressed;
		
		public ReferencedSelectedContactView(ISigInputOutput panel, TouchCueTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnRemovePressed = null;
		}

		public void SetContactName(string name)
		{
			m_NameLabel.SetLabelText(name);
		}

		public void SetAvatarImagePath(string url)
		{
			m_AvatarImage.SetImageUrl(url);
		}

		public void SetAvatarImageVisibility(bool visible)
		{
			m_AvatarImage.Show(visible);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_RemoveButton.OnPressed += RemoveButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_RemoveButton.OnPressed -= RemoveButtonOnPressed;
		}

		private void RemoveButtonOnPressed(object sender, EventArgs e)
		{
			OnRemovePressed.Raise(this);
		}

		#endregion
	}
}