﻿using System;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public interface IMaskedSourceInfo : IDisposable
	{
		ISource Source { get; }

		bool Mask { get; }

		bool? MaskOverride { get; set; }
	}
}
