﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LibAtem.Commands;
using LibAtem.Commands.DataTransfer;
using LibAtem.State;
using Xunit;
using Xunit.Abstractions;

namespace LibAtem.MockTests.Util
{
    internal class DownloadJobWorker
    {
        private readonly uint _chunkSize = 1396; // 1100 + Randomiser.RangeInt(290);
        private readonly uint _chunkCount = 20 + Randomiser.RangeInt(15);
        private readonly ITestOutputHelper _output;
        private readonly uint _bank;
        private readonly uint _index;
        private readonly byte[] _bytes;

        private bool _locked;
        private uint _transferId;
        private uint _pendingAck;
        private bool _isComplete;
        private uint _offset = 0;

        public DownloadJobWorker(ITestOutputHelper output, uint bank, uint index, byte[] bytes)
        {
            _output = output;
            _bank = bank;
            _index = index;
            _bytes = bytes;
        }

        public IEnumerable<ICommand> HandleCommand(Lazy<ImmutableList<ICommand>> previousCommands, ICommand cmd)
        {
            var lockRes = UploadJobWorker.LockCommandHandler(previousCommands, cmd).ToList();
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
            if (cmd is DataTransferDownloadRequestCommand startCmd)
            {
                Assert.Equal(_bank != 0xffff, _locked);
                Assert.False(_isComplete);
                Assert.Equal(_index, startCmd.TransferIndex);
                Assert.Equal(_bank, startCmd.TransferStoreId);

                _transferId = startCmd.TransferId;
                _pendingAck = 0;

                res.AddRange(SendData());
            }
            else if (cmd is DataTransferAckCommand ackCmd)
            {
                // Assert.False(_isComplete);

                if (_offset >= _bytes.Length)
                {
                    res.Add(new DataTransferCompleteCommand
                    {
                        TransferId = _transferId
                    });
                    _isComplete = true;
                }
                else
                {
                    res.AddRange(SendData());
                }
            }

            return res;
        }


        private IEnumerable<ICommand> SendData()
        {
            for (int i = 0; i < 5; i++)
            {
                long size = Math.Min(_chunkSize, _bytes.Length - _offset);
                if (size > 0)
                {
                    byte[] body = new byte[size];
                    Array.Copy(_bytes, (int) _offset, body, 0, (int) size);
                    yield return new DataTransferDataCommand
                    {
                        TransferId = _transferId,
                        Body = body
                    };
                    _offset += _chunkSize;
                    _pendingAck += 1;
                }
            }
        }
    }
}