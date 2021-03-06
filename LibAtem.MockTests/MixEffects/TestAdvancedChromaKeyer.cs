﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BMDSwitcherAPI;
using LibAtem.Commands;
using LibAtem.Commands.MixEffects.Key;
using LibAtem.Common;
using LibAtem.MockTests.Util;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.MixEffects
{
    [Collection("ServerClientPool")]
    public class TestAdvancedChromaKeyer : MixEffectsTestBase
    {
        public TestAdvancedChromaKeyer(ITestOutputHelper output, AtemServerClientPool pool) : base(output, pool)
        {
        }

        [Fact]
        public void TestForegroundLevel()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("ForegroundLevel");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.ForegroundLevel = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetForegroundLevel(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestBackgroundLevel()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("BackgroundLevel");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.BackgroundLevel = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetBackgroundLevel(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestKeyEdge()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("KeyEdge");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.KeyEdge = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetKeyEdge(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestSpillSuppression()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("SpillSuppression");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.SpillSuppression = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetSpillSuppress(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestFlareSuppression()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("FlareSuppression");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.FlareSuppression = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetFlareSuppress(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestBrightness()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Brightness");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-100, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.Brightness = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetBrightness(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestContrast()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Contrast");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-100, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.Contrast = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetContrast(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestSaturation()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Saturation");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(0, 200, 10);
                    keyerBefore.AdvancedChroma.Properties.Saturation = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetSaturation(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestRed()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Red");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-100, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.Red = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetRed(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestGreen()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Green");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-100, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.Green = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetGreen(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestBlue()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaPropertiesSetCommand, MixEffectKeyAdvancedChromaPropertiesGetCommand>("Blue");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-100, 100, 10);
                    keyerBefore.AdvancedChroma.Properties.Blue = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetBlue(target / 100); });
                });
            });
            Assert.True(tested);
        }

        /**
         * Sample
         */

        [Fact]
        public void TestEnableCursor()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaSampleSetCommand, MixEffectKeyAdvancedChromaSampleGetCommand>("EnableCursor");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    keyerBefore.AdvancedChroma.Sample.EnableCursor = i % 2 == 1;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetSamplingModeEnabled(i % 2); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestPreview()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaSampleSetCommand, MixEffectKeyAdvancedChromaSampleGetCommand>("Preview");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    keyerBefore.AdvancedChroma.Sample.Preview = i % 2 == 1;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetPreviewEnabled(i % 2); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestCursorX()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaSampleSetCommand, MixEffectKeyAdvancedChromaSampleGetCommand>("CursorX");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-15.3, 15.3, 1000);
                    keyerBefore.AdvancedChroma.Sample.CursorX = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetCursorXPosition(target); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestCursorY()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaSampleSetCommand, MixEffectKeyAdvancedChromaSampleGetCommand>("CursorY");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(-8.3, 8.3, 1000);
                    keyerBefore.AdvancedChroma.Sample.CursorY = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetCursorYPosition(target); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestCursorSize()
        {
            bool tested = false;
            var handler = CommandGenerator.CreateAutoCommandHandler<MixEffectKeyAdvancedChromaSampleSetCommand, MixEffectKeyAdvancedChromaSampleGetCommand>("CursorSize");
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var target = Randomiser.Range(6.25, 99.25, 100);
                    keyerBefore.AdvancedChroma.Sample.CursorSize = target;
                    helper.SendAndWaitForChange(stateBefore, () => { sdkKeyer.SetCursorSize(target / 100); });
                });
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestSampledColor()
        {
            bool tested = false;
            AtemMockServerWrapper.Each(Output, Pool, SampledColorHandler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    var targetY = Randomiser.Range(0, 1, 10000);
                    var targetCb = Randomiser.Range(0, 1, 10000);
                    var targetCr = Randomiser.Range(0, 1, 10000);
                    keyerBefore.AdvancedChroma.Sample.SampledY = targetY;
                    keyerBefore.AdvancedChroma.Sample.SampledCb = targetCb;
                    keyerBefore.AdvancedChroma.Sample.SampledCr = targetCr;
                    helper.SendAndWaitForChange(stateBefore,
                        () => { sdkKeyer.SetSampledColor(targetY, targetCb, targetCr); });
                });
            });
            Assert.True(tested);
        }
        private static IEnumerable<ICommand> SampledColorHandler(Lazy<ImmutableList<ICommand>> previousCommands, ICommand cmd)
        {
            if (cmd is MixEffectKeyAdvancedChromaSampleSetCommand sampleCmd)
            {
                var expectedMask = MixEffectKeyAdvancedChromaSampleSetCommand.MaskFlags.SampledY |
                                   MixEffectKeyAdvancedChromaSampleSetCommand.MaskFlags.SampledCb |
                                   MixEffectKeyAdvancedChromaSampleSetCommand.MaskFlags.SampledCr;
                Assert.Equal(expectedMask, sampleCmd.Mask);

                var setKey = CommandQueueKey.ForGetter<MixEffectKeyAdvancedChromaSampleGetCommand>(sampleCmd);
                var previousCmd = previousCommands.Value.OfType<MixEffectKeyAdvancedChromaSampleGetCommand>()
                    .Last(c => setKey.Equals(new CommandQueueKey(c)));
                Assert.NotNull(previousCmd);

                previousCmd.SampledY = sampleCmd.SampledY;
                previousCmd.SampledCb = sampleCmd.SampledCb;
                previousCmd.SampledCr = sampleCmd.SampledCr;
                yield return previousCmd;
            }
        }
        
        [Fact]
        public void TestResetChromaCorrection()
        {
            MixEffectKeyAdvancedChromaResetCommand keyerTarget = new MixEffectKeyAdvancedChromaResetCommand { ChromaCorrection = true };
            var handler = CommandGenerator.MatchCommand(keyerTarget);

            bool tested = false;
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    keyerTarget.MixEffectIndex = meId;
                    keyerTarget.KeyerIndex = keyId;

                    uint timeBefore = helper.Server.CurrentTime;

                    helper.SendAndWaitForChange(null, () => { sdkKeyer.ResetChromaCorrection(); });

                    // It should have sent a response, but we dont expect any comparable data
                    Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
                }, 1);
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestResetKeyAdjustments()
        {
            MixEffectKeyAdvancedChromaResetCommand keyerTarget = new MixEffectKeyAdvancedChromaResetCommand { KeyAdjustments = true };
            var handler = CommandGenerator.MatchCommand(keyerTarget);

            bool tested = false;
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    keyerTarget.MixEffectIndex = meId;
                    keyerTarget.KeyerIndex = keyId;

                    uint timeBefore = helper.Server.CurrentTime;

                    helper.SendAndWaitForChange(null, () => { sdkKeyer.ResetKeyAdjustments(); });

                    // It should have sent a response, but we dont expect any comparable data
                    Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
                }, 1);
            });
            Assert.True(tested);
        }

        [Fact]
        public void TestResetColorAdjustments()
        {
            MixEffectKeyAdvancedChromaResetCommand keyerTarget = new MixEffectKeyAdvancedChromaResetCommand { ColorAdjustments = true };
            var handler = CommandGenerator.MatchCommand(keyerTarget);

            bool tested = false;
            AtemMockServerWrapper.Each(Output, Pool, handler, DeviceTestCases.AdvancedChromaKeyer, helper =>
            {
                SelectionOfKeyers<IBMDSwitcherKeyAdvancedChromaParameters>(helper, (stateBefore, keyerBefore, sdkKeyer, meId, keyId, i) =>
                {
                    tested = true;
                    Assert.NotNull(keyerBefore.AdvancedChroma);

                    keyerTarget.MixEffectIndex = meId;
                    keyerTarget.KeyerIndex = keyId;

                    uint timeBefore = helper.Server.CurrentTime;

                    helper.SendAndWaitForChange(null, () => { sdkKeyer.ResetColorAdjustments(); });

                    // It should have sent a response, but we dont expect any comparable data
                    Assert.NotEqual(timeBefore, helper.Server.CurrentTime);
                }, 1);
            });
            Assert.True(tested);
        }

    }
}