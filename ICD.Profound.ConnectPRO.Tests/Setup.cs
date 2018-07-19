﻿using ICD.Common.Logging.Console;
using ICD.Common.Logging.Console.Loggers;
using ICD.Common.Permissions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests
{
	[SetUpFixture]
    public sealed class Setup
	{
		private LoggingCore m_Logger;

		[OneTimeSetUp]
		public void Init()
		{
			m_Logger = new LoggingCore();
			m_Logger.AddLogger(new IcdErrorLogger());
			m_Logger.SeverityLevel = eSeverity.Debug;

			ServiceProvider.AddService<ILoggerService>(m_Logger);

			ServiceProvider.AddService(new PermissionsManager());
		}

		[OneTimeTearDown]
		public void Deinit()
		{
			ServiceProvider.RemoveAllServices();
		}
	}
}
