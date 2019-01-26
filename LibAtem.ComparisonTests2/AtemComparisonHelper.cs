﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BMDSwitcherAPI;
using LibAtem.Commands;
using LibAtem.Common;
using LibAtem.ComparisonTests2.State;
using LibAtem.Util;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.ComparisonTests2
{
    public sealed class AtemComparisonHelper : IDisposable
    {
        public const int CommandWaitTime = 80;

        private readonly AtemClientWrapper _client;
        public AtemClientWrapper Client => _client;

        private readonly List<ICommand> _receivedCommands;

        private AutoResetEvent responseWait;
        private List<CommandQueueKey> responseTarget;

        public bool TestResult { get; set; } = true;

        public AtemComparisonHelper(AtemClientWrapper client, ITestOutputHelper output = null)
        {
            _client = client;
            Output = output;
            _receivedCommands = new List<ICommand>();

            _client.Client.OnReceive += OnReceive;

            AssertStatesMatch();
        }

        public void AssertStatesMatch()
        {
            List<string> before = ComparisonStateComparer.AreEqual(SdkState, LibState);
            if (before.Count != 0 && Output != null)
            {
                Output.WriteLine("state mismatch:");
                before.ForEach(Output.WriteLine);
            }
            Assert.Empty(before);
        }

        private void OnReceive(object sender, IReadOnlyList<ICommand> commands)
        {
            lock (_receivedCommands)
            {
                _receivedCommands.AddRange(commands);
            }
        }

        public ITestOutputHelper Output { get; }

        public ComparisonState SdkState => _client.SdkState;
        public ComparisonState LibState => _client.LibState;

        public IBMDSwitcher SdkSwitcher => _client.SdkSwitcher;

        public LibAtem.DeviceProfile.DeviceProfile Profile => _client.Profile;

        public T FindWithMatching<T>(T srcId) where T : ICommand
        {
            return _client.FindWithMatching(srcId);
        }

        public List<T> FindAllOfType<T>() where T : ICommand
        {
            return _client.FindAllOfType<T>();
        }

        public void ClearReceivedCommands()
        {
            lock (_receivedCommands)
                _receivedCommands.Clear();
        }

        public List<T> GetReceivedCommands<T>() where T : ICommand
        {
            lock (_receivedCommands)
                return _receivedCommands.OfType<T>().ToList();
        }

        public T GetSingleReceivedCommands<T>() where T : ICommand
        {
            lock (_receivedCommands)
                return _receivedCommands.OfType<T>().Single();
        }

        public int CountAndClearReceivedCommands<T>() where T : ICommand
        {
            lock (_receivedCommands)
            {
                int count = _receivedCommands.OfType<T>().Count();
                _receivedCommands.Clear();
                return count;
            }
        }

        public void SendCommand(params ICommand[] commands)
        {
            commands.ForEach(c => _client.Client.SendCommand(c));
        }

        public void Dispose()
        {
            _client.Client.OnReceive -= OnReceive;

            Assert.True(TestResult);
        }

        public void Sleep(int sleep = -1)
        {
            Thread.Sleep(sleep == -1 ? CommandWaitTime : sleep);
        }

        public void EnsureVideoMode(VideoMode mode)
        {
            // TODO - dont do if already on this mode, as it clears some data that would be good to keep
            SdkSwitcher.SetVideoMode(AtemEnumMaps.VideoModesMap[mode]);
            Sleep();
        }

        public Dictionary<VideoSource, T> GetSdkInputsOfType<T>() where T : class
        {
            Guid itId = typeof(IBMDSwitcherInputIterator).GUID;
            SdkSwitcher.CreateIterator(ref itId, out var itPtr);
            IBMDSwitcherInputIterator iterator = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(itPtr);

            Dictionary<VideoSource, T> inputs = new Dictionary<VideoSource, T>();
            for (iterator.Next(out IBMDSwitcherInput input); input != null; iterator.Next(out input))
            {
                var colGen = input as T;
                if (colGen == null)
                    continue;

                input.GetInputId(out long id);
                inputs[(VideoSource)id] = colGen;
            }

            return inputs;
        }
        
        // Note: This doesnt quite work properly yet
        public void SendAndWaitForMatching(CommandQueueKey key, ICommand toSend, int timeout = -1)
        {
            if (responseWait != null)
                return;

            responseWait = new AutoResetEvent(false);
            responseTarget = new List<CommandQueueKey> { key };

            void Handler (object sender, CommandQueueKey queueKey){
                if (queueKey.Equals(key))
                    responseWait.Set();
            }

            _client.OnCommandKey += Handler;

            if (toSend != null)
                SendCommand(toSend);

            // Wait for the expected time. If no response, then go with last data
            responseWait.WaitOne(timeout == -1 ? CommandWaitTime : timeout);

            responseWait = null;
            _client.OnCommandKey -= Handler;
        }

        public void SendAndWaitForMatching(List<CommandQueueKey> expected, ICommand toSend, int timeout = -1)
        {
            if (responseWait != null)
                throw new Exception("a SendAndWaitForMatching is already running");

            responseWait = new AutoResetEvent(false);
            responseTarget = expected;

            void Handler(object sender, CommandQueueKey queueKey)
            {
                responseTarget.Remove(queueKey);
                if (responseTarget.Count == 0)
                    responseWait.Set();
            }

            _client.OnCommandKey += Handler;

            if (toSend != null)
                SendCommand(toSend);

            // Wait for the expected time. If no response, then go with last data
            responseWait.WaitOne(timeout == -1 ? CommandWaitTime : timeout);

            _client.OnCommandKey -= Handler;
            responseWait = null;
        }
    }

}