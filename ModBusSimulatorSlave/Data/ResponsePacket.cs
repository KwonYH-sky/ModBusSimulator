﻿namespace ModBusSimSlave.Data
{
    class ResponsePacket
    {
        private byte[] _frame;
        private byte _slaveAddr;
        private byte _functionCode;
        private byte _byteCount;
        private byte[] _data;
        private byte[] _crc;

        private ResponsePacket(byte slaveArr, byte functionCode, byte dataLength, byte[] data)
        {
            _slaveAddr = slaveArr;
            _functionCode = functionCode;
            _byteCount = dataLength;
            _data = data;

            _frame = GetReadFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);
        }

        private ResponsePacket(byte slaveArr, byte functionCode, byte[] data)
        {
            _slaveAddr = slaveArr;
            _functionCode = functionCode;
            _data = data;

            _frame = GetWriteFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);
        }

        public ResponsePacket(byte[] frame)
        {
            _frame = frame;
            _slaveAddr = frame[0];
            _functionCode = frame[1];

            if (_functionCode > 0x80)
            {
                _data = [frame[2]];
                _crc = frame.Skip(3).Take(2).ToArray();
                return;
            }

            if (_functionCode is 0x05 or 0x06 or 0x0F or 0x10)
            {
                _data = frame.Skip(2).Take(4).ToArray();
                _crc = frame.Skip(6).Take(2).ToArray();
                return;
            }

            _byteCount = frame[2];
            _data = new byte[_byteCount];
            Array.Copy(frame, 3, _data, 0, _byteCount);
            _crc = new byte[2];
            Array.Copy(frame, 3 + _byteCount, _crc, 0, 2);
        }

        private byte[] GetReadFrame()
        {

            byte[] frame = new byte[1 + 1 + 1 + _data.Length + 2]; // SlaveAddr + FunctionCode + DataLength + Data + CRC

            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            frame[2] = _byteCount;
            Array.Copy(_data, 0, frame, 3, _data.Length);

            ushort crc = PacketHelpers.CalcCRC(frame, 0, frame.Length - 2);
            frame[^2] = (byte)(crc & 0xFF);
            frame[^1] = (byte)(crc >> 8);

            return frame;
        }

        private byte[] GetWriteFrame()
        {
            byte[] frame = new byte[1 + 1 + _data.Length + 2]; // SlaveAddr + FunctionCode + Data + CRC
            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            Array.Copy(_data, 0, frame, 2, _data.Length);

            ushort crc = PacketHelpers.CalcCRC(frame, 0, frame.Length - 2);
            frame[^2] = (byte)(crc & 0xFF);
            frame[^1] = (byte)(crc >> 8);

            return frame;
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

        public byte ByteCount
        {
            get { return _byteCount; }
            set { _byteCount = value; }
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

        public class ResponsePacketBuilder
        {
            private byte _slaveAddr;
            private byte _functionCode;
            private byte _byteCount;
            private byte[] _data;

            public ResponsePacketBuilder SetSlaveAddr(byte slaveAddr)
            {
                _slaveAddr = slaveAddr;
                return this;
            }

            public ResponsePacketBuilder SetFunctionCode(byte functionCode)
            {
                _functionCode = functionCode;
                return this;
            }

            public ResponsePacketBuilder SetByteCount(byte byteCount)
            {
                _byteCount = byteCount;
                return this;
            }

            public ResponsePacketBuilder SetData(byte[] data)
            {
                _data = data;
                return this;
            }

            public ResponsePacket Build()
            {
                return _functionCode == 0x05 || _functionCode == 0x06 || _functionCode == 0x0F || _functionCode == 0x10 || _functionCode > 0x80 ?
                    new ResponsePacket(_slaveAddr, _functionCode, _data) :
                    new ResponsePacket(_slaveAddr, _functionCode, _byteCount, _data);
            }
        }
    }
}
