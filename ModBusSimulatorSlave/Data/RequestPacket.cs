namespace ModBusMaster.Data
{
    class RequestPacket
    {
        private byte[] _frame;
        private byte _slaveAddr;
        private byte _functionCode;
        private byte[] _data;
        private byte[] _crc;

        private RequestPacket(byte slaveAddr, byte functionCode, byte[] data)
        {
            _slaveAddr = slaveAddr;
            _functionCode = functionCode;
            _data = data;

            _frame = GetFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);
        }

        public RequestPacket(byte[] frame)
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

            public RequestPacket Build()
            {
                return new RequestPacket(_slaveAddr, _functionCode, _data);
            }
        }
    }
}
