using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BMDSwitcherAPI;
using LibAtem.Commands;
using LibAtem.Commands.Settings.Multiview;
using LibAtem.Common;
using LibAtem.ComparisonTests2.State;
using LibAtem.ComparisonTests2.Util;
using LibAtem.DeviceProfile;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.ComparisonTests2.Settings
{
    [Collection("Client")]
    public class TestMultiView
    {
        private readonly ITestOutputHelper _output;
        private readonly AtemClientWrapper _client;

        public TestMultiView(ITestOutputHelper output, AtemClientWrapper client)
        {
            _client = client;
            _output = output;
        }

        private List<Tuple<uint, IBMDSwitcherMultiView>> GetMultiviewers()
        {
            Guid itId = typeof(IBMDSwitcherMultiViewIterator).GUID;
            _client.SdkSwitcher.CreateIterator(ref itId, out var itPtr);
            IBMDSwitcherMultiViewIterator
                iterator = (IBMDSwitcherMultiViewIterator) Marshal.GetObjectForIUnknown(itPtr);

            var result = new List<Tuple<uint, IBMDSwitcherMultiView>>();
            uint index = 0;
            for (iterator.Next(out IBMDSwitcherMultiView r); r != null; iterator.Next(out r))
            {
                result.Add(Tuple.Create(index, r));
                index++;
            }

            return result;
        }

        [Fact]
        public void TestMultiviewCount()
        {
            List<Tuple<uint, IBMDSwitcherMultiView>> multiviewers = GetMultiviewers();
            Assert.Equal((int) _client.Profile.MultiView.Count, multiviewers.Count);
        }

        [Fact]
        public void TestMultiviewWindowCount()
        {
            foreach (Tuple<uint, IBMDSwitcherMultiView> sdkProps in GetMultiviewers())
            {
                sdkProps.Item2.GetWindowCount(out uint count);
                Assert.Equal(Constants.MultiViewWindowCount, count);
            }
        }

        private abstract class MultiviewTestDefinition<T> : TestDefinitionBase<MultiviewPropertiesSetCommand, T>
        {
            protected readonly uint _id;
            protected readonly IBMDSwitcherMultiView _sdk;

            public MultiviewTestDefinition(AtemComparisonHelper helper, Tuple<uint,  IBMDSwitcherMultiView> mv) : base(helper)
            {
                _id = mv.Item1;
                _sdk = mv.Item2;
            }

            public override void SetupCommand(MultiviewPropertiesSetCommand cmd)
            {
                cmd.MultiviewIndex = _id;
            }

            public abstract T MangleBadValue(T v);

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, T v)
            {
                ComparisonSettingsMultiViewState obj = state.Settings.MultiViews[_id];
                SetCommandProperty(obj, PropertyName, goodValue ? v : MangleBadValue(v));
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, T v)
            {
                yield return new CommandQueueKey(new MultiviewPropertiesGetCommand() { MultiviewIndex = _id });
            }
        }

        private class MultiviewLayoutTestDefinition : MultiviewTestDefinition<MultiViewLayout>
        {
            public MultiviewLayoutTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv) : base(helper, mv)
            {
            }

            // Ensure the first value will have a change
            public override void Prepare() => _sdk.SetLayout(_BMDSwitcherMultiViewLayout.bmdSwitcherMultiViewLayoutProgramRight);

            public override string PropertyName => "Layout";
            public override MultiViewLayout MangleBadValue(MultiViewLayout v) => v;

            public override MultiViewLayout[] GoodValues => Enum.GetValues(typeof(MultiViewLayout)).OfType<MultiViewLayout>().ToArray();
        }

        [Fact]
        public void TestLayout()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
                GetMultiviewers().ForEach(k => new MultiviewLayoutTestDefinition(helper, k).Run());
        }

        [Fact]
        public void TestSupportsProgramPreviewSwap()
        {
            foreach (Tuple<uint, IBMDSwitcherMultiView> sdkProps in GetMultiviewers())
            {
                sdkProps.Item2.SupportsProgramPreviewSwap(out int canToggle);
                Assert.Equal(_client.Profile.MultiView.CanSwapPreviewProgram, canToggle == 1);
            }
        }

        private class MultiviewProgramPreviewSwappedTestDefinition : MultiviewTestDefinition<bool>
        {
            public MultiviewProgramPreviewSwappedTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv) : base(helper, mv)
            {
                mv.Item2.SupportsProgramPreviewSwap(out int canTest);
                Skip.If(canTest == 0, "Model does not support Multiview.ProgramPreviewSwapped");
            }

            // Ensure the first value will have a change
            public override void Prepare() => _sdk.SetProgramPreviewSwapped(0);

            public override string PropertyName => "ProgramPreviewSwapped";
            public override bool MangleBadValue(bool v) => v;

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, bool v)
            {
                var props = state.Settings.MultiViews[_id];
                props.ProgramPreviewSwapped = v;

                var tmp = props.Windows[0].Source;
                props.Windows[0].Source = props.Windows[1].Source;
                props.Windows[1].Source = tmp;

                var tmp2 = props.Windows[0].SupportsVuMeter;
                props.Windows[0].SupportsVuMeter = props.Windows[1].SupportsVuMeter;
                props.Windows[1].SupportsVuMeter = tmp2;
            }
        }

        [SkippableFact]
        public void TestProgramPreviewSwapped()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
                GetMultiviewers().ForEach(k => new MultiviewProgramPreviewSwappedTestDefinition(helper, k).Run());
        }

        [Fact]
        public void TestCanToggleSafeArea()
        {
            foreach (Tuple<uint, IBMDSwitcherMultiView> sdkProps in GetMultiviewers())
            {
                sdkProps.Item2.CanToggleSafeAreaEnabled(out int canToggle);
                Assert.Equal(_client.Profile.MultiView.CanToggleSafeArea, canToggle == 1);
            }
        }

        /*
        private class MultiviewToggleSafeAreaTestDefinition : MultiviewTestDefinition<bool>
        {
            public MultiviewToggleSafeAreaTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv) : base(helper, mv)
            {
                mv.Item2.SupportsProgramPreviewSwap(out int canTest);
                Skip.If(canTest == 0, "Model does not support Multiview.ProgramPreviewSwapped");
            }

            // Ensure the first value will have a change
            public override void Prepare() => _sdk.SetSafeAreaEnabled(0);

            public override string PropertyName => "SafeAreaEnabled";
            public override bool MangleBadValue(bool v) => v;
        }

        [SkippableFact]
        public void TestToggleSafeArea()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
                GetMultiviewers().ForEach(k => new MultiviewToggleSafeAreaTestDefinition(helper, k).Run());
        }*/

        private class MultiviewUnroutableWindowSourcesTestDefinition : TestDefinitionBase<MultiviewWindowInputSetCommand, VideoSource>
        {
            private readonly uint _id;
            private readonly uint _window;
            private readonly IBMDSwitcherMultiView _sdk;
            private readonly bool _quick;

            public MultiviewUnroutableWindowSourcesTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv, uint window, bool quick) : base(helper)
            {
                _id = mv.Item1;
                _window = window;
                _sdk = mv.Item2;
                _quick = quick;
            }

            public override void Prepare()
            {
            }

            public override string PropertyName => "Source";

            public override void SetupCommand(MultiviewWindowInputSetCommand cmd)
            {
                cmd.MultiviewIndex = _id;
                cmd.WindowIndex = _window;
            }

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, VideoSource v)
            {
            }

            public override VideoSource[] GoodValues => new VideoSource[0];
            public override VideoSource[] BadValues
            {
                get
                {
                    if (_quick)
                        return new VideoSource[] { VideoSource.ColorBars, VideoSource.Input1 };
                    return VideoSourceLists.All.ToArray();
                }
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, VideoSource v)
            {
                return new CommandQueueKey[0];
            }
        }

        private class MultiviewPvwPgmSetVuMeterTestDefinition : TestDefinitionBase<MultiviewWindowVuMeterSetCommand, bool>
        {
            private readonly uint _id;
            private readonly uint _window;
            private readonly IBMDSwitcherMultiView _sdk;

            public MultiviewPvwPgmSetVuMeterTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv, uint window) : base(helper)
            {
                _id = mv.Item1;
                _window = window;
                _sdk = mv.Item2;
            }

            public override void Prepare()
            {
            }

            public override string PropertyName => "VuEnabled";

            public override void SetupCommand(MultiviewWindowVuMeterSetCommand cmd)
            {
                cmd.MultiviewIndex = _id;
                cmd.WindowIndex = _window;
            }

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, bool v)
            {
                _sdk.SupportsVuMeters(out int supportVuMeters);
                if (supportVuMeters != 0)
                {
                    var mv = state.Settings.MultiViews[_id];
                    mv.Windows[(int)_window].VuMeter = mv.ProgramPreviewSwapped ? (_window == 0) && v : (_window == 1) && v;
                }
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, bool v)
            {
                yield break;
            }
        }
        private class MultiviewRoutableWindowSourcesTestDefinition : TestDefinitionBase<MultiviewWindowInputSetCommand, VideoSource>
        {
            private readonly uint _id;
            private readonly uint _window;
            private readonly IBMDSwitcherMultiView _sdk;
            private readonly bool _quick;

            public MultiviewRoutableWindowSourcesTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv, uint window, bool quick) : base(helper)
            {
                _id = mv.Item1;
                _window = window;
                _sdk = mv.Item2;
                _quick = quick;
            }

            public override string PropertyName => "Source";

            public override void SetupCommand(MultiviewWindowInputSetCommand cmd)
            {
                cmd.MultiviewIndex = _id;
                cmd.WindowIndex = _window;
            }

            public override void Prepare() => _sdk.SetWindowInput(_window, (long)VideoSource.ColorBars);

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, VideoSource v)
            {
                if (goodValue)
                    state.Settings.MultiViews[_id].Windows[(int)_window].Source = v;
            }

            public override VideoSource[] GoodValues
            {
                get
                {
                    var ignorePortTypes = new List<InternalPortType>();
                    ignorePortTypes.Add(InternalPortType.Mask); // TODO - dynamic based on model

                    if (_quick)
                        return new[] { VideoSource.Black, VideoSource.ColorBars };
                    return VideoSourceLists.All.Where(s => s.IsAvailable(_helper.Profile, ignorePortTypes.ToArray()) && s.IsAvailable(SourceAvailability.Multiviewer)).ToArray();
                }
            }
            public override VideoSource[] BadValues
            {
                get
                {
                    if (_quick)
                        return new VideoSource[0];
                    return base.BadValues;
                }
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, VideoSource v)
            {
                if (goodValue)
                    yield return new CommandQueueKey(new MultiviewWindowInputGetCommand() { MultiviewIndex = _id, WindowIndex = _window });
            }
        }
        private class MultiviewRoutableWindowVuMeterTestDefinition : TestDefinitionBase<MultiviewWindowVuMeterSetCommand, bool>
        {
            private readonly uint _id;
            private readonly uint _window;
            private readonly IBMDSwitcherMultiView _sdk;

            public MultiviewRoutableWindowVuMeterTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv, uint window) : base(helper)
            {
                _id = mv.Item1;
                _window = window;
                _sdk = mv.Item2;
            }

            public override void Prepare() => _sdk.SetWindowInput(_window, (long)VideoSource.Input1);

            public override string PropertyName => "VuEnabled";

            public override void SetupCommand(MultiviewWindowVuMeterSetCommand cmd)
            {
                cmd.MultiviewIndex = _id;
                cmd.WindowIndex = _window;
            }

            public override void UpdateExpectedState(ComparisonState state, bool goodValue, bool v)
            {
                _sdk.SupportsVuMeters(out int supportsVuMeter);
                if (supportsVuMeter != 0) _sdk.CurrentInputSupportsVuMeter(_window, out supportsVuMeter);

                state.Settings.MultiViews[_id].Windows[(int)_window].VuMeter = supportsVuMeter != 0 ? v : false;
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, bool v)
            {
                yield break;
            }
        }

        [Fact]
        public void TestMultiviewSources()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
            {
                uint unroutableWindows = _client.Profile.MultiView.CanRouteInputs ? 2 : Constants.MultiViewWindowCount;

                var multiViewers = GetMultiviewers();
                {
                    Tuple<uint, IBMDSwitcherMultiView> mv = multiViewers.First();

                    mv.Item2.CanRouteInputs(out int canRouteInputs);
                    Assert.Equal(canRouteInputs != 0, _client.Profile.MultiView.CanRouteInputs);                        
                    
                    // Pvw/Pgm/unroutable windows
                    for (uint i = 0; i < unroutableWindows; i++)
                    {
                        new MultiviewUnroutableWindowSourcesTestDefinition(helper, mv, i, false).Run();
                        new MultiviewPvwPgmSetVuMeterTestDefinition(helper, mv, i).Run();
                    }

                    // Quick test for every routable window (we assume they are all equal)
                    for (uint i = unroutableWindows; i < Constants.MultiViewWindowCount; i++)
                    {
                        new MultiviewRoutableWindowSourcesTestDefinition(helper, mv, i, true).Run();
                        new MultiviewRoutableWindowVuMeterTestDefinition(helper, mv, i).Run();
                    }

                    // Run full test for one window
                    const uint SampleWindow = 5;
                    new MultiviewRoutableWindowSourcesTestDefinition(helper, mv, SampleWindow, false).Run();
                    // TODO - CurrentInputSupportsVuMeter 
                }

                // Now quickly check everything else
                foreach (Tuple<uint, IBMDSwitcherMultiView> mv in multiViewers.Skip(1))
                {
                    VideoSource[] badValuesPvwPgm = new[] { VideoSource.Input1 };
                    
                    // Pvw/Pgm/unroutable windows
                    for (uint i = 0; i < unroutableWindows; i++)
                    {
                        new MultiviewUnroutableWindowSourcesTestDefinition(helper, mv, i, true).Run();
                        new MultiviewPvwPgmSetVuMeterTestDefinition(helper, mv, i).Run();
                    }
                    
                    // Quick test for every routable window (we assume they are all equal)
                    for (uint i = unroutableWindows; i < Constants.MultiViewWindowCount; i++)
                    {
                        new MultiviewRoutableWindowSourcesTestDefinition(helper, mv, i, true).Run();
                        new MultiviewRoutableWindowVuMeterTestDefinition(helper, mv, i).Run();
                    }
                }
            }
        }

        [Fact]
        public void TestSupportsVuMeters()
        {
            foreach (Tuple<uint, IBMDSwitcherMultiView> sdkProps in GetMultiviewers())
            {
                sdkProps.Item2.SupportsVuMeters(out int supports);
                Assert.Equal(_client.Profile.MultiView.VuMeters, supports == 1);
            }
        }

        private class MultiviewVuOpacityTestDefinition : TestDefinitionBase<MultiviewVuOpacityCommand, double>
        {
            private readonly uint _id;
            private readonly IBMDSwitcherMultiView _sdk;

            public MultiviewVuOpacityTestDefinition(AtemComparisonHelper helper, Tuple<uint, IBMDSwitcherMultiView> mv) : base(helper)
            {
                _id = mv.Item1;
                _sdk = mv.Item2;

                mv.Item2.SupportsVuMeters(out int canTest);
                Skip.If(canTest == 0, "Model does not support Multiview.SupportsVuMeters");
            }

            public override void Prepare()
            {
            }

            public override string PropertyName => "Opacity";

            public override void SetupCommand(MultiviewVuOpacityCommand cmd)
            {
                cmd.MultiviewIndex = _id;
            }
            
            public override void UpdateExpectedState(ComparisonState state, bool goodValue, double v)
            {
                if (goodValue)
                    state.Settings.MultiViews[_id].VuMeterOpacity = v;
                else
                    state.Settings.MultiViews[_id].VuMeterOpacity = v <= 10 && v >= -0.1 ? 10 : 100;
            }

            public override IEnumerable<CommandQueueKey> ExpectedCommands(bool goodValue, double v)
            {
                yield return new CommandQueueKey(new MultiviewVuOpacityCommand() { MultiviewIndex = _id});
            }

            public override double[] GoodValues => new double[] { 10, 87, 14, 99, 100, 11 };
            public override double[] BadValues => new double[] { 100.1, 110, 101, -1, -10, 9 };
        }
        
        [SkippableFact]
        public void TestMultiviewVuMeterOpacity()
        {
            using (var helper = new AtemComparisonHelper(_client, _output))
                GetMultiviewers().ForEach(k => new MultiviewVuOpacityTestDefinition(helper, k).Run());
        }
    }
}