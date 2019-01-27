﻿using BMDSwitcherAPI;
using LibAtem.Commands;
using LibAtem.Commands.Audio;
using LibAtem.Common;
using LibAtem.ComparisonTests2.MixEffects;
using LibAtem.ComparisonTests2.State;
using LibAtem.ComparisonTests2.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.ComparisonTests2.Audio
{
    [Collection("Client")]
    public class TestAudioTalkback
    {
        private readonly ITestOutputHelper _output;
        private readonly AtemClientWrapper _client;

        public TestAudioTalkback(ITestOutputHelper output, AtemClientWrapper client)
        {
            _client = client;
            _output = output;
        }
        
        protected IBMDSwitcherTalkback GetTalkback()
        {
            try
            {
                return (IBMDSwitcherTalkback)_client.SdkSwitcher;
            } catch (InvalidCastException e)
            {
                return null;
            }
        }

        private class AudioMixerTalkbackMuteSDITestDefinition : TestDefinitionBase<bool>
        {
            private readonly IBMDSwitcherTalkback _sdk;

            public AudioMixerTalkbackMuteSDITestDefinition(AtemComparisonHelper helper, IBMDSwitcherTalkback sdk) : base(helper)
            {
                _sdk = sdk;
            }

            public override void Prepare()
            {
                // Ensure the first value will have a change
                _sdk.SetMuteSDI(0);
            }

            public override ICommand GenerateCommand(bool v)
            {
                return new AudioMixerTalkbackPropertiesSetCommand
                {
                    Mask = AudioMixerTalkbackPropertiesSetCommand.MaskFlags.MuteSDI,
                    MuteSDI = v,
                };
            }

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, bool v)
            {
                state.Audio.Talkback.MuteSDI = v;
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, bool v)
            {
                yield return new CommandQueueKey(new AudioMixerTalkbackPropertiesGetCommand());
            }
        }

        [Fact]
        public void TestSupported()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
            {
                IBMDSwitcherTalkback talkback = GetTalkback();
                Assert.Equal(helper.Profile.TalkbackOverSDI, talkback != null);
            }
        }

        [SkippableFact]
        public void TestMuteSDI()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
            {
                IBMDSwitcherTalkback talkback = GetTalkback();
                Skip.If(talkback == null, "Model does not support talkback");

                new AudioMixerTalkbackMuteSDITestDefinition(helper, talkback).Run();
            }
        }

        // TODO - test mutesdi for inputs
    }
}