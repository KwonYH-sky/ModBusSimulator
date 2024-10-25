using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusMaster
{
    internal class ModbusRTU
    {
        private byte[] _frame;
        private byte _slaveAddr;
        private byte _functionCode;
        private byte[] _data;
        private byte[] _crc;

        public ModbusRTU(byte slaveAddr, byte functionCode, byte[] data)
        {
            _slaveAddr = slaveAddr;
            _functionCode = functionCode;
            _data = data;
        }

        public byte[] GetFrame()
        {
            byte[] frame = new byte[1 + 1 + 1 + _data.Length + 2];

            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            Array.Copy(_data, 0, frame, 2, _data.Length);

            ushort crc = CalcCRC(frame, 0, frame.Length - 2);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)(crc >> 8);

            return frame;
        }

        private static ushort CalcCRC(byte[] data, int offset, int count)
        {
            ushort crc = 0xFFFF;

            for (int i = offset; i < offset + count; i++)
            {
                crc ^= data[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) == 1)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }


    }
}
