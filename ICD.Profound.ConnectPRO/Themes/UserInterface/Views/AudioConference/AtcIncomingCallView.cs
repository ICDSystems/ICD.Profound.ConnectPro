using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.AudioConference
{
	public sealed partial class AtcIncomingCallView : AbstractUiView, IAtcIncomingCallView
	{
		/// <summary>
		/// Raised when the user presses the answer button.
		/// </summary>
		public event EventHandler OnAnswerButtonPressed;

		/// <summary>
		/// Raised when the user presses the ignore button.
		/// </summary>
		public event EventHandler OnIgnoreButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public AtcIncomingCallView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAnswerButtonPressed = null;
			OnIgnoreButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the incoming caller number.
		/// </summary>
		/// <param name="number"></param>
		public void SetCallerInfo(string number)
		{
			m_CallerLabel.SetLabelTextAtJoin(m_CallerLabel.SerialLabelJoins.First(), number);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AnswerButton.OnPressed += AnswerButtonOnPressed;
			m_RejectButton.OnPressed += RejectButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AnswerButton.OnPressed -= AnswerButtonOnPressed;
			m_RejectButton.OnPressed -= RejectButtonOnPressed;
		}

		private void RejectButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnIgnoreButtonPressed.Raise(this);
		}

		private void AnswerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAnswerButtonPressed.Raise(this);
		}

		#endregion
	}
}
