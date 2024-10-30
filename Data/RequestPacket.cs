namespace ModBusSimMaster.Data
{
    class RequestPacket
    {
        private byte[] _frame;
        private byte _slaveAddr;
        private byte _functionCode;
        private byte[] _data;
        private byte _byteCount;
        private byte[] _writeData;
        private byte[] _crc;

<<<<<<< HEAD
        private RequestPacket(byte slaveAddr, byte functionCode, byte[] data)
=======
        public RequestPacket(byte slaveAddr, byte functionCode, byte[] data)
>>>>>>> 5eacb85e602b94055b4f4ae2c1f4d40e591d0c2a
        {
            _slaveAddr = slaveAddr;
            _functionCode = functionCode;
            _data = data;

            _frame = GetFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);
        }

        // 멀티 코일 & 레지스터 쓰기를 위한 생성자
        private RequestPacket(byte slaveAddr, byte functionCode, byte[] data, byte byteCnt, byte[] writeData)
        {
            _slaveAddr = slaveAddr;
            _functionCode = functionCode;
            _data = data;
            _byteCount = byteCnt;
            _writeData = writeData;

            _frame = GetMultiWriteFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);

        }

        public RequestPacket(byte[] frame)
        {
            if (frame[1] == 0x0F || frame[1] == 0x10)
            {
                _slaveAddr = frame[0];
                _functionCode = frame[1];
                _data = new byte[frame.Length - 7];
                Array.Copy(frame, 2, _data, 0, _data.Length);
                _byteCount = frame[frame.Length - 4];
                _writeData = new byte[frame.Length - 7 - 1];
                Array.Copy(frame, 3 + _data.Length, _writeData, 0, _writeData.Length);
                _crc = new byte[2];
                Array.Copy(frame, frame.Length - 2, _crc, 0, 2);
            }
            else
            {
                _slaveAddr = frame[0];
                _functionCode = frame[1];
                _data = new byte[frame.Length - 5];
                Array.Copy(frame, 2, _data, 0, _data.Length);
                _crc = new byte[2];
                Array.Copy(frame, frame.Length - 2, _crc, 0, 2);
            }
        }

        private byte[] GetFrame()
        {
            byte[] frame = new byte[1 + 1 + _data.Length + 2]; // SlaveAddr + FunctionCode + Data + CRC

            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            Array.Copy(_data, 0, frame, 2, _data.Length);

            ushort crc = PacketHelpers.CalcCRC(frame, 0, frame.Length - 2);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)(crc >> 8);

            return frame;
        }

        private byte[] GetMultiWriteFrame()
        {
            byte[] frame = new byte[1 + 1 + _data.Length + 1 + _writeData.Length + 2]; // SlaveAddr + FunctionCode + Data + CRC

            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            Array.Copy(_data, 0, frame, 2, _data.Length);

            frame[2 + _data.Length] = _byteCount;
            Array.Copy(_writeData, 0, frame, 3 + _data.Length, _writeData.Length);

            ushort crc = PacketHelpers.CalcCRC(frame, 0, frame.Length - 2);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)(crc >> 8);

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


        public class RequestPacketBuilder
        {
            private byte _slaveAddr;
            private byte _functionCode;
            private byte[] _data;
            private byte _byteCount;
            private byte[] _writeData;

            public RequestPacketBuilder SetSlaveAddr(byte slaveAddr)
            {
                _slaveAddr = slaveAddr;
                return this;
            }

            public RequestPacketBuilder SetFunctionCode(byte functionCode)
            {
                _functionCode = functionCode;
                return this;
            }

            public RequestPacketBuilder SetData(byte[] data)
            {
                _data = data;
                return this;
            }

            public RequestPacketBuilder SetByteCount(byte byteCount)
            {
                _byteCount = byteCount;
                return this;
            }

            public RequestPacketBuilder SetWriteData(byte[] writeData)
            {
                _writeData = writeData;
                return this;
            }

            public RequestPacket Build()
            {
                return _functionCode == 0x0F || _functionCode == 0x10 ?
                    new RequestPacket(_slaveAddr, _functionCode, _data, _byteCount, _writeData) :
                    new RequestPacket(_slaveAddr, _functionCode, _data);
            }
        }
    }
}
