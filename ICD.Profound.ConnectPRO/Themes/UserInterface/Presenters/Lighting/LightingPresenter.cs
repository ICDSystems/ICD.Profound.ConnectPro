using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Lighting;
using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Lighting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Lighting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Lighting
{
	[PresenterBinding(typeof(ILightingPresenter))]
	public sealed class LightingPresenter : AbstractUiPresenter<ILightingView> , ILightingPresenter
	{

		private readonly SafeCriticalSection m_RefreshSection;

		private ILightingRoomInterfaceDevice m_LightingInterface;

		private ushort? m_PresetIndex;

		private readonly Dictionary<int, ushort> m_PresetsIdToIndex;
		private readonly List<LightingProcessorControl> m_PresetsOrdered;

		private ILightingRoomInterfaceDevice LightingInterface
		{
			get { return m_LightingInterface; }
			set
			{
				if (m_LightingInterface == value)
					return;

				Unsubscribe(m_LightingInterface);
				m_LightingInterface = value;
				Subscribe(value);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public LightingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresetsIdToIndex = new Dictionary<int, ushort>();
			m_PresetsOrdered = new List<LightingProcessorControl>();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ILightingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{

				if (LightingInterface == null)
				{
					view.SetPresetLabels(Enumerable.Empty<string>());
					UpdatePresetFeedback(view, null);
					return;
				}

				m_PresetsOrdered.Clear();
				m_PresetsOrdered.AddRange(LightingInterface.GetPresets().Take(view.MaxPresets));

				m_PresetsIdToIndex.Clear();
				for (ushort i = 0; i < m_PresetsOrdered.Count; i++)
				{
					m_PresetsIdToIndex.Add(m_PresetsOrdered[i].Id, i);
				}

				view.SetPresetLabels(m_PresetsOrdered.Select(p => p.Name));
				UpdatePresetFeedback(view, m_LightingInterface.GetPreset());
			}

			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void UpdatePresetFeedback(int? presetId)
		{
			UpdatePresetFeedback(GetView(), presetId);
		}

		private void UpdatePresetFeedback(ILightingView view, int? presetId)
		{
			ushort? presetIndex = PresetIdToIndex(presetId);

			if (presetIndex == m_PresetIndex)
				return;

			if (m_PresetIndex.HasValue)
				view.SetPresetActive(m_PresetIndex.Value, false);

			m_PresetIndex = presetIndex;

			if (m_PresetIndex.HasValue)
				view.SetPresetActive(m_PresetIndex.Value, true);

		}

		private ushort? PresetIdToIndex(int? presetId)
		{
			ushort presetIndexTemp;
			if (presetId.HasValue && m_PresetsIdToIndex.TryGetValue(presetId.Value, out presetIndexTemp))
				return presetIndexTemp;
			return null;
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ILightingView view)
		{
			base.Subscribe(view);

			if (view == null)
				return;

			view.OnClosePressed += ViewOnClosePressed;
			view.OnPresetPressed += ViewOnPresetPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ILightingView view)
		{
			base.Unsubscribe(view);

			if (view == null)
				return;

			view.OnClosePressed -= ViewOnClosePressed;
			view.OnPresetPressed -= ViewOnPresetPressed;
		}

		private void ViewOnClosePressed(object sender, EventArgs args)
		{
			ShowView(false);
		}

		private void ViewOnPresetPressed(object sender, UShortEventArgs args)
		{
			if (LightingInterface == null || args.Data >= m_PresetsOrdered.Count)
				return;
			
			LightingInterface.SetPreset(m_PresetsOrdered[args.Data].Id);
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;


			LightingInterface = room.Originators.GetInstanceRecursive<ILightingRoomInterfaceDevice>();

		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			LightingInterface = null;
		}

		#endregion

		#region LightingRoomInterface Callbacks

		private void Subscribe(ILightingRoomInterfaceDevice device)
		{
			if (device == null)
				return;

			device.OnControlsChanged += DeviceOnControlsChanged;
			device.OnPresetChanged += DeviceOnPresetChanged;
		}

		private void Unsubscribe(ILightingRoomInterfaceDevice device)
		{
			if (device == null)
				return;

			device.OnControlsChanged -= DeviceOnControlsChanged;
			device.OnPresetChanged -= DeviceOnPresetChanged;
		}

		private void DeviceOnControlsChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		private void DeviceOnPresetChanged(object sender, GenericEventArgs<int?> args)
		{
			UpdatePresetFeedback(args.Data);
		}

		#endregion
	}
}
