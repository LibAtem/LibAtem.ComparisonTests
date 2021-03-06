﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BMDSwitcherAPI;
using log4net;
using log4net.Config;
using Xunit;
using LibAtem.State;
using LibAtem.State.Builder;
using LibAtem.MockTests.SdkState;

namespace LibAtem.MockTests.Util
{
    public sealed class AtemSdkClientWrapper : IDisposable
    {
        private readonly IBMDSwitcherDiscovery _switcherDiscovery;
        private readonly IBMDSwitcher _sdkSwitcher;
        private readonly AtemSDKStateMonitor _sdkState;
        private readonly AtemStateBuilderSettings _updateSettings;

        public IBMDSwitcher SdkSwitcher => _sdkSwitcher;
        
        public delegate void StateChangeHandler(object sender);
        public event StateChangeHandler OnSdkStateChange;

        public int Id { get; }

        public AtemSdkClientWrapper(string address, AtemStateBuilderSettings updateSettings, int id)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            if (!logRepository.Configured) // Default to all on the console
                BasicConfigurator.Configure(logRepository);

            Id = id;

            _updateSettings = updateSettings;

            _switcherDiscovery = new CBMDSwitcherDiscovery();
            Assert.NotNull(_switcherDiscovery);

            _BMDSwitcherConnectToFailure failReason = 0;
            try
            {
                _switcherDiscovery.ConnectTo(address, out _sdkSwitcher, out failReason);
            }
            catch (COMException)
            {
                throw new Exception($"SDK Connection failure: {failReason}");
            }

            //_sdkSwitcher.AddCallback(new SwitcherConnectionMonitor()); // TODO - make this monitor work better!

            _sdkState = new AtemSDKStateMonitor(_sdkSwitcher);
            _sdkState.OnStateChange += (s) => OnSdkStateChange?.Invoke(s);
        }

        public AtemState BuildState() => SdkStateBuilder.Build(SdkSwitcher, _updateSettings);
        
        public void Dispose()
        {
            _sdkState.Dispose();

            //Thread.Sleep(500);
        }
        
    }

}