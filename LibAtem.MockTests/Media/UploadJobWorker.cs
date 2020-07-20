﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LibAtem.Commands;
using LibAtem.Commands.DataTransfer;
using LibAtem.Commands.Media;
using LibAtem.Common;
using LibAtem.MockTests.Util;
using LibAtem.Util.Media;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.Media
{
    internal class UploadJobWorker
    {
        private readonly uint _chunkSize = 1396; // 1100 + Randomiser.RangeInt(290);
        private readonly uint _chunkCount = 20 + Randomiser.RangeInt(15);
        private readonly uint _targetBytes;
        private readonly ITestOutputHelper _output;
        private readonly uint _index;

        private bool _locked;
        private uint _transferId;
        private DataTransferFileDescriptionCommand _description;
        private uint _pendingAck;
        private bool _isComplete;

        public List<byte> Buffer { get; } = new List<byte>();

        public UploadJobWorker(uint targetBytes, ITestOutputHelper output, uint index)
        {
            _targetBytes = targetBytes;
            _output = output;
            _index = index;
        }

        public IEnumerable<ICommand> HandleCommand(Lazy<ImmutableList<ICommand>> previousCommands, ICommand cmd)
        {
            var lockRes = LockCommandHandler(previousCommands, cmd).ToList();
            if (lockRes.Any())
            {
                foreach (LockStateChangedCommand lockCmd in lockRes.OfType<LockStateChangedCommand>())
                {
                    _locked = lockCmd.Locked;
                    // _output.WriteLine($"Locked {lockCmd.Locked}");
                }

                return lockRes;
            }

            // _output.WriteLine($"Got cmd: {cmd.GetType().FullName}");

            var res = new List<ICommand>();
            if (cmd is DataTransferUploadRequestCommand startCmd)
            {
                Assert.True(_locked);
                Assert.False(_isComplete);
                Assert.Equal(DataTransferUploadRequestCommand.TransferMode.Write, startCmd.Mode);
                Assert.Equal((int)_targetBytes, startCmd.Size);
                Assert.Equal(_index, startCmd.TransferIndex);
                Assert.Equal((uint)MediaPoolFileType.Still, startCmd.TransferStoreId);

                _transferId = startCmd.TransferId;
                _pendingAck = 0;

                res.Add(new DataTransferUploadContinueCommand
                {
                    TransferId = startCmd.TransferId,
                    ChunkCount = _chunkCount,
                    ChunkSize = _chunkSize
                });
            }
            else if (cmd is DataTransferFileDescriptionCommand descCmd)
            {
                Assert.NotEqual(0u, _transferId);
                Assert.Null(_description);
                Assert.False(_isComplete);

                Assert.Equal(_transferId, descCmd.TransferId);
                _description = descCmd;

                if (Buffer.Count >= _targetBytes)
                {
                    _isComplete = true;

                    res.AddRange(Complete());
                }
                else
                {
                    res.Add(null);
                }

            }
            else if (cmd is DataTransferDataCommand dataCmd)
            {
                Assert.NotEqual(0u, _transferId);
                Assert.False(_isComplete);

                Assert.Equal(_transferId, dataCmd.TransferId);
                Assert.True(dataCmd.Body.Length <= _chunkSize);
                Tuple<int, byte[]> decoded = FrameEncodingUtil.DecodeRLESegment(_targetBytes, dataCmd.Body);
                Buffer.AddRange(decoded.Item2.Take(decoded.Item1));

                _pendingAck++;
                if (_pendingAck >= _chunkCount)
                {
                    //res.Add(new DataTransferAckCommand {TransferId = _transferId});
                    res.Add(new DataTransferUploadContinueCommand
                    {
                        TransferId = _transferId,
                        ChunkCount = _chunkCount,
                        ChunkSize = _chunkSize
                    });
                    _pendingAck = 0;
                }

                // _output.WriteLine($"Now have {_buffer.Count} bytes of {_targetBytes}");

                if (Buffer.Count >= _targetBytes && _description != null)
                {
                    _isComplete = true;
                    res.AddRange(Complete());
                }

                if (res.Count == 0)
                {
                    res.Add(null);
                }
            }

            return res;
        }

        private IEnumerable<ICommand> Complete()
        {
            // _output.WriteLine("Complete");

            _isComplete = true;

            Assert.NotNull(_description);
            yield return new DataTransferCompleteCommand { TransferId = _transferId };
            yield return new MediaPoolFrameDescriptionCommand
            {
                Bank = MediaPoolFileType.Still,
                Index = _index,
                IsUsed = true,
                Filename = _description.Name,
                Hash = _description.FileHash
            };
        }

        public static IEnumerable<ICommand> LockCommandHandler(Lazy<ImmutableList<ICommand>> previousCommands, ICommand cmd)
        {
            if (cmd is LockStateSetCommand lockCmd)
            {
                //Assert.True(lockCmd.Locked);

                if (lockCmd.Locked)
                {
                    yield return new LockObtainedCommand
                    {
                        Index = lockCmd.Index
                    };
                }

                yield return new LockStateChangedCommand
                {
                    Index = lockCmd.Index,
                    Locked = lockCmd.Locked
                };
            }
        }
    }

    internal class AbortedUploadJobWorker
    {
        private readonly uint _chunkSize = 1396; // 1100 + Randomiser.RangeInt(290);
        private readonly uint _chunkCount = 20 + Randomiser.RangeInt(15);
        private readonly ITestOutputHelper _output;

        private uint _transferId;
        private bool _isAborted;

        public AbortedUploadJobWorker(ITestOutputHelper output)
        {
            _output = output;
        }

        public IEnumerable<ICommand> HandleCommand(Lazy<ImmutableList<ICommand>> previousCommands, ICommand cmd)
        {
            var lockRes = UploadJobWorker.LockCommandHandler(previousCommands, cmd).ToList();
            if (lockRes.Any())
                return lockRes;

            // _output.WriteLine($"Got cmd: {cmd.GetType().FullName}");

            var res = new List<ICommand>();
            if (cmd is DataTransferUploadRequestCommand startCmd)
            {
                Assert.False(_isAborted);

                _transferId = startCmd.TransferId;

                res.Add(new DataTransferUploadContinueCommand
                {
                    TransferId = startCmd.TransferId,
                    ChunkCount = _chunkCount,
                    ChunkSize = _chunkSize
                });
            }
            else if (cmd is DataTransferFileDescriptionCommand descCmd)
            {
                res.Add(null);
            }
            else if (cmd is DataTransferDataCommand dataCmd)
            {
                res.Add(null);
            }
            else if (cmd is DataTransferAbortCommand abortCmd)
            {
                Assert.Equal(_transferId, abortCmd.TransferId);
                _isAborted = true;
                res.Add(null);
            }

            return res;
        }

    }
}