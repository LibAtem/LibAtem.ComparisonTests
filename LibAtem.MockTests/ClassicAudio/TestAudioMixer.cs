﻿using System;
using System.Collections.Generic;
using BMDSwitcherAPI;
using LibAtem.Commands.Audio;
using LibAtem.Common;
using LibAtem.MockTests.Util;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.ClassicAudio
{
    [Collection("ServerClientPool")]
    public class TestAudioMixer
    {
        private readonly ITestOutputHelper _output;
        private readonly AtemServerClientPool _pool;

        public TestAudioMixer(ITestOutputHelper output, AtemServerClientPool pool)
        {
            _output = output;
            _pool = pool;
        }

        public static IBMDSwitcherAudioMixer GetAudioMixer(AtemMockServerWrapper helper)
        {
            var mixer = helper.Helper.SdkSwitcher as IBMDSwitcherAudioMixer;
            Assert.NotNull(mixer);
            return mixer;
        }

        [Fact]
        public void TestSendLevelsCommand()
        {
            var expected = new AudioMixerSendLevelsCommand();
            var handler = CommandGenerator.MatchCommand(expected);
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.ClassicAudioMain, helper =>
            {
                IBMDSwitcherAudioMixer mixer = GetAudioMixer(helper);
                AtemState stateBefore = helper.Helper.BuildLibState();

                for (int i = 0; i < 5; i++)
                {
                    uint timeBefore = helper.Server.CurrentTime;

                    expected.SendLevels = i % 2 == 1;

                    helper.SendAndWaitForChange(stateBefore, () => { mixer.SetAllLevelNotificationsEnable(i % 2); });

                    // It should have sent a response, but we dont expect any comparable data
                    Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
                }
            });
        }


        [Fact]
        public void TestResetProgramOutPeaks()
        {
            var expected = new AudioMixerResetPeaksCommand {Mask = AudioMixerResetPeaksCommand.MaskFlags.Master};
            var handler = CommandGenerator.MatchCommand(expected, true);
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.ClassicAudioMain, helper =>
            {
                IBMDSwitcherAudioMixer mixer = GetAudioMixer(helper);
                AtemState stateBefore = helper.Helper.BuildLibState();

                uint timeBefore = helper.Server.CurrentTime;

                helper.SendAndWaitForChange(stateBefore, () => { mixer.ResetProgramOutLevelNotificationPeaks(); });

                // It should have sent a response, but we dont expect any comparable data
                Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
            });
        }

        [Fact]
        public void TestResetAllPeaks()
        {
            var expected = new AudioMixerResetPeaksCommand { Mask = AudioMixerResetPeaksCommand.MaskFlags.All };
            var handler = CommandGenerator.MatchCommand(expected, true);
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.ClassicAudioMain, helper =>
            {
                IBMDSwitcherAudioMixer mixer = GetAudioMixer(helper);
                AtemState stateBefore = helper.Helper.BuildLibState();

                uint timeBefore = helper.Server.CurrentTime;

                helper.SendAndWaitForChange(stateBefore, () => { mixer.ResetAllLevelNotificationPeaks(); });

                // It should have sent a response, but we dont expect any comparable data
                Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
            });
        }

        [Fact]
        public void TestTally()
        {
            AtemMockServerWrapper.Each(_output, _pool, null, DeviceTestCases.ClassicAudioMain, helper =>
            {
                AtemState stateBefore = helper.Helper.BuildLibState();

                for (int i = 0; i < 5; i++)
                {
                    var cmd = new AudioMixerTallyCommand
                    {
                        Inputs = new Dictionary<AudioSource, bool>()
                    };

                    Assert.NotEmpty(stateBefore.Audio.Tally);

                    // the sdk is a bit picky about ids, so best to go with what it expects
                    foreach (KeyValuePair<AudioSource, bool> k in stateBefore.Audio.Tally)
                    {
                        bool isMixedIn = Randomiser.Range(0, 1) > 0.7;
                        cmd.Inputs[k.Key] = isMixedIn;
                    }

                    stateBefore.Audio.Tally = cmd.Inputs;
                    helper.SendAndWaitForChange(stateBefore, () => { helper.Server.SendCommands(cmd); });
                }
            });
        }

    }
}