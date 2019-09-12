using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Header
{
    [ViewBinding(typeof(IHeaderView))]
    public sealed partial class HeaderView : AbstractTouchDisplayView, IHeaderView
    {
        public HeaderView(ISigInputOutput panel, ConnectProTheme theme)
            : base(panel, theme)
        {
        }

        public event EventHandler OnStartEndMeetingPressed;

        public void SetRoomName(string name)
        {
            m_RoomName.SetLabelText(name);
        }

        public void SetTimeLabel(string time)
        {
            m_TimeLabel.SetLabelText(time);
        }

        public void SetStartEndMeetingButtonMode(eStartEndMeetingMode mode)
        {
            m_StartEndMeetingButton.SetMode((ushort) mode);
        }

        #region Control Callbacks

        protected override void SubscribeControls()
        {
            base.SubscribeControls();

            m_StartEndMeetingButton.OnPressed += StartEndMeetingButtonOnPressed;
        }

        protected override void UnsubscribeControls()
        {
            base.UnsubscribeControls();

            m_StartEndMeetingButton.OnPressed -= StartEndMeetingButtonOnPressed;
        }

        private void StartEndMeetingButtonOnPressed(object sender, EventArgs e)
        {
            OnStartEndMeetingPressed.Raise(this);
        }

        #endregion
    }
}