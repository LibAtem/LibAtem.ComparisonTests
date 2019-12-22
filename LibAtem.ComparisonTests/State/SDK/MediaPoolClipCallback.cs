﻿using System;
using BMDSwitcherAPI;
using LibAtem.State;

namespace LibAtem.ComparisonTests.State.SDK
{
    public sealed class MediaPoolClipCallback : SdkCallbackBaseNotify<IBMDSwitcherClip, _BMDSwitcherMediaPoolEventType>, IBMDSwitcherClipCallback
    {
        private readonly MediaPoolState.ClipState _state;

        public MediaPoolClipCallback(MediaPoolState.ClipState state, IBMDSwitcherClip props, Action<string> onChange) : base(props, onChange)
        {
            _state = state;

            //_props.GetMaxFrameCount(out uint frames);
            //for (uint o = 0; o < frames; o++)
            //_state.Frames[o] = new ComparisonMediaPoolFrameState();
        }

        public void Notify(_BMDSwitcherMediaPoolEventType eventType, IBMDSwitcherFrame frame, int frameIndex, IBMDSwitcherAudio audio, int clipIndex)
        {

            switch (eventType)
            {
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeValidChanged:
                    //_props.is
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeNameChanged:
                    Props.GetName(out string name);
                    _state.Name = name;
                    OnChange(null);
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeHashChanged:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeAudioValidChanged:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeAudioNameChanged:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeAudioHashChanged:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockBusy:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockIdle:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCompleted:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCancelled:
                    break;
                case _BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferFailed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        public override void Notify(_BMDSwitcherMediaPoolEventType eventType)
        {
            // TODO - does this need to be for each frame or just clip?
            /*
            _props.GetMaxFrameCount(out uint count);
            for (int i = 0; i < _state.Clips.; i++)
                Notify(eventType, null, i);
                */
        }
    }
}