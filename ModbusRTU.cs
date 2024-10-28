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

        public ModbusRTU(byte[] frame)
        {
            _frame = frame;
            _slaveAddr = frame[0];
            _functionCode = frame[1];
            _data = new byte[frame.Length - 5];
            Array.Copy(frame, 2, _data, 0, _data.Length);
            _crc = new byte[2];
            Array.Copy(frame, frame.Length - 2, _crc, 0, 2);
        }

        public byte[] GetFrame()
        {
            byte[] frame = new byte[1 + 1 + _data.Length + 2]; // SlaveAddr + FunctionCode + Data + CRC

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

        public byte[] Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        public byte SlaveAddr
        {
            get { return _slaveAddr; }
            set { _slaveAddr = value; }
        }

        public byte FunctionCode
        {
            get { return _functionCode; }
            set { _functionCode = value; }
        }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public byte[] Crc
        {
            get { return _crc; }
            set { _crc = value; }
        }

    }
}
