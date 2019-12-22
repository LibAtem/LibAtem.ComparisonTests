﻿using BMDSwitcherAPI;
using LibAtem.Commands.Audio.Fairlight;
using LibAtem.ComparisonTests.State.SDK;
using LibAtem.MockTests.Util;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.Fairlight
{
    [Collection("ServerClientPool")]
    public class TestFairlightProgramOutEqualizer
    {
        private readonly ITestOutputHelper _output;
        private readonly AtemServerClientPool _pool;

        public TestFairlightProgramOutEqualizer(ITestOutputHelper output, AtemServerClientPool pool)
        {
            _output = output;
            _pool = pool;
        }

        private static IBMDSwitcherFairlightAudioEqualizer GetEqualizer(AtemMockServerWrapper helper)
        {
            IBMDSwitcherFairlightAudioMixer mixer = TestFairlightProgramOut.GetFairlightMixer(helper);
            var equalizer = AtemSDKConverter.CastSdk<IBMDSwitcherFairlightAudioEqualizer>(mixer.GetMasterOutEffect);
            Assert.NotNull(equalizer);
            return equalizer;
        }

        [Fact]
        public void TestEnabled()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<FairlightMixerMasterSetCommand, FairlightMixerMasterGetCommand>("EqualizerEnabled");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioEqualizer equalizer = GetEqualizer(helper);
                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    stateBefore.Fairlight.ProgramOut.Equalizer.Enabled= i % 2 > 0;
                    helper.SendAndWaitForChange(stateBefore, () => { equalizer.SetEnabled(i % 2); });
                }
            });
        }

        [Fact]
        public void TestGain()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<FairlightMixerMasterSetCommand, FairlightMixerMasterGetCommand>("EqualizerGain");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioEqualizer equalizer = GetEqualizer(helper);
                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    double target = Randomiser.Range(-20, 20);
                    stateBefore.Fairlight.ProgramOut.Equalizer.Gain = target;
                    helper.SendAndWaitForChange(stateBefore, () => { equalizer.SetGain(target); });
                }
            });
        }

        /*
        [Fact]
        public void TestReset()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<FairlightMixerMasterLimiterSetCommand, FairlightMixerMasterLimiterGetCommand>("Release");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioLimiter limiter = GetLimiter(helper);

                uint timeBefore = helper.Server.CurrentTime;

                helper.SendAndWaitForChange(null, () => { limiter.Reset(); });

                // It should have sent a response, but we dont expect any comparable data
                Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
            });
        }
        */
    }
}