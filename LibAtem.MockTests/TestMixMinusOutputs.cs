﻿using System.Collections.Generic;
using System.Linq;
using BMDSwitcherAPI;
using LibAtem.Commands.Settings;
using LibAtem.Common;
using LibAtem.MockTests.Util;
using LibAtem.MockTests.SdkState;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests
{
    [Collection("ServerClientPool")]
    public class TestMixMinusOutputs
    {
        private readonly ITestOutputHelper _output;
        private readonly AtemServerClientPool _pool;

        public TestMixMinusOutputs(ITestOutputHelper output, AtemServerClientPool pool)
        {
            _output = output;
            _pool = pool;
        }

        private List<IBMDSwitcherMixMinusOutput> GetMixMinusOutputs(AtemMockServerWrapper helper)
        {
            var iterator = AtemSDKConverter.CastSdk<IBMDSwitcherMixMinusOutputIterator>(helper.SdkClient.SdkSwitcher.CreateIterator);

            var result = new List<IBMDSwitcherMixMinusOutput>();
            for (iterator.Next(out IBMDSwitcherMixMinusOutput r); r != null; iterator.Next(out r))
            {
                result.Add(r);
            }

            return result;
        }

        [Fact]
        public void TestAvailableAudioModes()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<MixMinusOutputSetCommand, MixMinusOutputGetCommand>("Mode");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.MixMinusOutputs, helper =>
            {
                var previousCommands = helper.Server.GetParsedDataDump().OfType<MixMinusOutputGetCommand>().ToList();

                for (int i = 0; i < 5; i++)
                {
                    AtemState stateBefore = helper.Helper.BuildLibState();
                    uint id = Randomiser.RangeInt((uint)stateBefore.Settings.MixMinusOutputs.Count - 1);
                    SettingsState.MixMinusOutputState mixMinusState = stateBefore.Settings.MixMinusOutputs[(int) id];

                    MixMinusOutputGetCommand cmd = previousCommands.Single(c => c.Id == id);
                    MixMinusMode newValue = Randomiser.EnumValue<MixMinusMode>();
                    cmd.SupportedModes = newValue;

                    mixMinusState.SupportedModes = newValue;

                    helper.SendFromServerAndWaitForChange(stateBefore, cmd);
                }
            });
        }

        [Fact]
        public void TestMode()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<MixMinusOutputSetCommand, MixMinusOutputGetCommand>("Mode");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.MixMinusOutputs, helper =>
            {
                bool tested = false;

                List<IBMDSwitcherMixMinusOutput> outputs = GetMixMinusOutputs(helper);
                for (int id = 0; id < outputs.Count; id++)
                {
                    IBMDSwitcherMixMinusOutput mixMinus = outputs[id];
                    tested = true;

                    AtemState stateBefore = helper.Helper.BuildLibState();
                    SettingsState.MixMinusOutputState mixMinusState = stateBefore.Settings.MixMinusOutputs[id];

                    for (int i = 0; i < 5; i++)
                    {
                        MixMinusMode newValue = Randomiser.EnumValue<MixMinusMode>();
                        mixMinusState.Mode = newValue;

                        helper.SendAndWaitForChange(stateBefore,
                            () => { mixMinus.SetAudioMode(AtemEnumMaps.MixMinusModeMap[newValue]); });
                    }

                }
                Assert.True(tested);
            });
        }


        [Fact]
        public void TestAudioInputId()
        {
            var handler = CommandGenerator.CreateAutoCommandHandler<MixMinusOutputSetCommand, MixMinusOutputGetCommand>("Mode");
            AtemMockServerWrapper.Each(_output, _pool, handler, DeviceTestCases.MixMinusOutputs, helper =>
            {
                var previousCommands = helper.Server.GetParsedDataDump().OfType<MixMinusOutputGetCommand>().ToList();

                for (int i = 0; i < 10; i++)
                {
                    AtemState stateBefore = helper.Helper.BuildLibState();
                    uint id = Randomiser.RangeInt((uint)stateBefore.Settings.MixMinusOutputs.Count - 1);
                    SettingsState.MixMinusOutputState mixMinusState = stateBefore.Settings.MixMinusOutputs[(int)id];

                    MixMinusOutputGetCommand cmd = previousCommands.Single(c => c.Id == id);
                    uint newValue = Randomiser.RangeInt(9) + 1;
                    cmd.HasAudioInputId = i % 2 == 0;
                    cmd.AudioInputId = cmd.HasAudioInputId ? (AudioSource) newValue : 0;

                    mixMinusState.HasAudioInputId = cmd.HasAudioInputId;
                    mixMinusState.AudioInputId = cmd.AudioInputId;

                    helper.SendFromServerAndWaitForChange(stateBefore, cmd);
                }
            });
        }
    }
}