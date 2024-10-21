//
// Copyright (c) 2021 Bitcraze
// Copyright (c) 2010-2024 Antmicro
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using System;
using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.Bus;
using System.Collections.Generic;
using Antmicro.Migrant;
using Antmicro.Renode.Core.Structure.Registers;
using Antmicro.Renode.Peripherals.UART;

namespace Antmicro.Renode.Peripherals.Miscellaneous
{
    [AllowedTranslations(AllowedTranslation.WordToDoubleWord | AllowedTranslation.ByteToDoubleWord)]
    public class CF_Syslink : BasicDoubleWordPeripheral, IUART
    {
        public CF_Syslink(Machine machine, uint deck = 0, uint frequency = 8000000) : base(machine)
        {
            this.frequency = frequency;
            this.DataLength = UInt32.MaxValue - 6; // Packet lengths have data and 6 extra bytes
            // Fakes 1-wire memory read from decks.
            switch(deck) //TODO better way to do this?
            {
                case 1: // Flowdeck2
                    this.deckCount = 1;
                    this.deckData = CreateMessage(0x22, 14, new byte[]
				    {0x01,0x00,0x00,0xEB,0x04,0x05,0x06,0x07,0xBC,0x0F,0x89,0x00,0x00,0xFF});
                    break;
                default: // No deck
                    this.deckCount = 0;
                    this.deckData = new byte[]{};
                    break;
            }
        }

        public void WriteChar(byte value)
        {
            // Read entire message
            // With the queue, read byte 2 (0-indexed) to find message type
            // Sends back the correct message once the entire package has been received
            receiveFifo.Enqueue(value);
            if(receiveFifo.Count == 4)
            {
                DataLength = value;
            }
            if(receiveFifo.Count == DataLength + 6)
            {
                DataLength = UInt32.MaxValue - 6;
                SendBack();
            }
        }

        private uint DataLength;

        private void SendBack()
        {
            byte[] data = receiveFifo.ToArray();
            switch(data[2])
            {
                case 0x20: // SYSLINK_OW_SCAN
                    byte[] OwScanData = CreateMessage(0x20, 0x01, new byte[]{deckCount});
                    for(int i = 0; i < OwScanData.Length; ++i)
                    {
                        CharReceived?.Invoke((byte)OwScanData[i]);
                    }
                    receiveFifo.Clear();
                    break;
                case 0x22: // OW_READ
                    for(int i = 0; i < deckData.Length; ++i)
                    {
                        CharReceived?.Invoke((byte)deckData[i]);
                    }
                    receiveFifo.Clear();
                    break;
                default:
                    while(receiveFifo.Count > 0)
                    {
                        CharReceived?.Invoke((byte)receiveFifo.Dequeue());
                    }
                    break;
            }

            this.Log(LogLevel.Noisy, "Complete data sent back!");
        }

        // Creates the message to be sent back
        public byte[] CreateMessage(byte command, byte length, byte[] data)
        {
            byte[] result = new byte[length+6];
            result[0] = 0xBC;
            result[1] = 0xCF;
            result[2] = command;
            result[3] = length;
            for(int i = 0; i < length; i++)
            {
                result[4+i] = data[i];
            }
            for(int i = 2; i < length+4; i++)
            {
                result[length+4] += result[i]; // Checksum 1
                result[length+5] += result[length+4]; // Checksum 2
            }

            return result;
        }

        public override void Reset()
        {
            base.Reset();
            receiveFifo.Clear();
        }

        public uint BaudRate { get; }

        public Bits StopBits { get; }

        public Parity ParityBit { get; }

        public event Action<byte> CharReceived;

        private void Update(){}

        private readonly uint frequency;
        private readonly byte deckCount;
        private readonly byte[] deckData;
        private readonly Queue<byte> receiveFifo = new Queue<byte>();
    }
}
