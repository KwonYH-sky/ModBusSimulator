namespace ModBusSimSlave.Data
{
    /**
     * Request Packet 요청 패킷
     * Function Code 15, 16을 제외한 모든 코드에서는 총 8바이트 고정 크기를 가짐
     * 15, 16 코드에서는 수량과 코일 혹은 레지스터에 따라 Byte Count와 Write Data의 길이가 달라짐
     * 수량 및 바이트 카운터에 따라 패킷 검수 과정
     */
    class RequestPacket
    {
        private byte[] _frame;
        private byte _slaveAddr;
        private byte _functionCode;
        private byte[] _data;
        private byte _byteCount;
        private byte[] _multiWirteData;
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

        // 멀티 코일 & 레지스터 쓰기를 위한 생성자
        private RequestPacket(byte slaveAddr, byte functionCode, byte[] data, byte[] writeData)
        {
            _slaveAddr = slaveAddr;
            _functionCode = functionCode;
            _data = data;

            ushort quantity = (ushort)(data[2] << 8 | data[3] & 0xFF);

            _byteCount = _functionCode switch
            {
                0x0F => (byte)(quantity / 8 + (quantity % 8 == 0 ? 0 : 1)),
                0x10 => (byte)(quantity * 2),
                _ => 0
            };
            _multiWirteData = writeData;

            if (_multiWirteData.Length != _byteCount)
                throw new ArgumentException($"바이트 크기에 맞지 않는 데이터 들어옴\n바이크 크기: {_byteCount}, 들어온 데이터 크기: {_multiWirteData.Length}");

            _frame = GetMultiWriteFrame();
            _crc = new byte[2];
            Array.Copy(_frame, _frame.Length - 2, _crc, 0, 2);

        }

        /**
         * 외부에서 받아온 패킷을 가공하기 위한 생성자
         */
        public RequestPacket(byte[] frame)
        {
            _frame = frame;
            // 멀티 코일 & 레지스터 쓰기
            if (frame[1] == 0x0F || frame[1] == 0x10)
            {
                _slaveAddr = frame[0];
                _functionCode = frame[1];
                _data = frame.Skip(2).Take(4).ToArray();
                _byteCount = frame[6];
                _multiWirteData = frame.Skip(7).Take(_byteCount).ToArray();
                _crc = new byte[2];
                Array.Copy(frame, frame.Length - 2, _crc, 0, 2);
            }
            else
            {
                _slaveAddr = frame[0];
                _functionCode = frame[1];
                _data = new byte[frame.Length - 4];
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
            byte[] frame = new byte[1 + 1 + _data.Length + 1 + _multiWirteData.Length + 2]; // SlaveAddr + FunctionCode + Data + CRC

            frame[0] = _slaveAddr;
            frame[1] = _functionCode;
            Array.Copy(_data, 0, frame, 2, _data.Length);

            frame[2 + _data.Length] = _byteCount;
            Array.Copy(_multiWirteData, 0, frame, 3 + _data.Length, _multiWirteData.Length);

            ushort crc = PacketHelpers.CalcCRC(frame, 0, frame.Length - 2);
            frame[frame.Length - 2] = (byte)(crc & 0xFF);
            frame[frame.Length - 1] = (byte)(crc >> 8);

            return frame;
        }


        /**
         * Getter & Setter
         */
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

        public byte ByteCount
        {
            get { return _byteCount; }
            set { _byteCount = value; }
        }

        public byte[] MultiWriteData
        {
            get { return _multiWirteData; }
            set { _multiWirteData = value; }
        }

        public byte[] Crc
        {
            get { return _crc; }
            set { _crc = value; }
        }


        /**
         * 패킷 빌더
         */
        public class RequestPacketBuilder
        {
            private byte _slaveAddr;
            private byte _functionCode;
            private byte[] _data;
            private byte _byteCount;
            private byte[] _multiWriteData;

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

            public RequestPacketBuilder SetWriteData(byte[] multiWriteData)
            {
                _multiWriteData = multiWriteData;
                return this;
            }

            public RequestPacket Build()
            {
                return _functionCode == 0x0F || _functionCode == 0x10 ?
                    new RequestPacket(_slaveAddr, _functionCode, _data, _multiWriteData) :
                    new RequestPacket(_slaveAddr, _functionCode, _data);
            }
        }
    }
}
