﻿using BMDSwitcherAPI;
using LibAtem.Commands.Audio.Fairlight;
using LibAtem.ComparisonTests.State.SDK;
using LibAtem.MockTests.Util;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.Fairlight
{
    public class TestFairlightProgramOutCompressor
    {
        private readonly ITestOutputHelper _output;

        public TestFairlightProgramOutCompressor(ITestOutputHelper output)
        {
            _output = output;
        }

        private static IBMDSwitcherFairlightAudioCompressor GetCompressor(AtemMockServerWrapper helper)
        {
            IBMDSwitcherFairlightAudioDynamicsProcessor dynamics = TestFairlightProgramOut.GetDynamics(helper);
            var compressor = AtemSDKConverter.CastSdk<IBMDSwitcherFairlightAudioCompressor>(dynamics.GetProcessor);
            Assert.NotNull(compressor);
            return compressor;
        }

        [Fact]
        public void TestCompressorEnabled()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("CompressorEnabled");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.CompressorEnabled = i % 2 > 0;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetEnabled(i % 2); });
                }
            });
        }

        [Fact]
        public void TestThreshold()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("Threshold");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    var target = Randomiser.Range(-50, 0);
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.Threshold = target;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetThreshold(target); });
                }
            });
        }

        [Fact]
        public void TestRatio()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("Ratio");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    var target = Randomiser.Range(1.2, 20);
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.Ratio = target;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetRatio(target); });
                }
            });
        }

        [Fact]
        public void TestAttack()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("Attack");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    var target = Randomiser.Range(0.7, 100);
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.Attack = target;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetAttack(target); });
                }
            });
        }

        [Fact]
        public void TestHold()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("Hold");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    var target = Randomiser.Range(0, 4000);
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.Hold = target;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetHold(target); });
                }
            });
        }

        [Fact]
        public void TestRelease()
        {
            var handler =
                CommandGenerator
                    .CreateAutoCommandHandler<FairlightMixerMasterCompressorSetCommand,
                        FairlightMixerMasterCompressorGetCommand>("Release");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
            {
                IBMDSwitcherFairlightAudioCompressor compressor = GetCompressor(helper);

                AtemState stateBefore = helper.Helper.LibState;

                for (int i = 0; i < 5; i++)
                {
                    var target = Randomiser.Range(50, 4000);
                    stateBefore.Fairlight.ProgramOut.Dynamics.Compressor.Release = target;
                    helper.SendAndWaitForChange(stateBefore, () => { compressor.SetRelease(target); });
                }
            });
        }

        /*
        [Fact]
        public void TestReset()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<FairlightMixerMasterLimiterSetCommand, FairlightMixerMasterLimiterGetCommand>("Release");
            AtemMockServerWrapper.Each(_output, handler, DeviceTestCases.FairlightMain, helper =>
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