using ModBusSimSlave.Data;

namespace ModBusSimSlave
{
    class Service
    {
        public Dictionary<int, VirtualDevice> VitualDeviceManagement { get;}
        private VirtualDevice VirtualDevice;

        public Service(Dictionary<int, VirtualDevice> management)
        {
            VitualDeviceManagement = management;
        }

        public ResponsePacket Response(RequestPacket requestPacket)
        {
            if (!VitualDeviceManagement.ContainsKey(requestPacket.SlaveAddr)) 
                return ErrorResponse(0x02, requestPacket);

            VirtualDevice = VitualDeviceManagement[requestPacket.SlaveAddr];
            VirtualDevice.UpdateComunication();

            return requestPacket.FunctionCode switch
            {
                0x01 => ReadCoils(requestPacket),
                0x02 => ReadDiscreteInputs(requestPacket),
                0x03 => ReadHoldingRegisters(requestPacket),
                0x04 => ReadInputRegisters(requestPacket),
                0x05 => WriteSingleCoil(requestPacket),
                0x06 => WriteSingleRegister(requestPacket),
                0x0F => WriteMultipleCoils(requestPacket),
                0x10 => WriteMultipleRegisters(requestPacket),
                _ => ErrorResponse(0x01, requestPacket),
            };
        }

        private ResponsePacket ReadCoils(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            if (quantity > VirtualDevice.Coils.Length || address > VirtualDevice.Coils.Length - 1) 
                return ErrorResponse(0x03, packet);

            byte byteCount = (byte)(quantity / 8 + (quantity % 8 == 0 ? 0 : 1)); // 8개의 코일을 1바이트로 표현 코일은 1비트
            byte[] data = new byte[byteCount];

            for (int i = 0; i < quantity; i++)
            {
                bool bit = VirtualDevice.Coils[address + i];
                if (bit) data[i / 8] |= (byte)(1 << (i % 8)); // 코일이 켜져있으면 해당 비트를 1로 설정
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetByteCount(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadDiscreteInputs(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            if (quantity > VirtualDevice.DiscreteInputs.Length || address > VirtualDevice.DiscreteInputs.Length - 1)
                return ErrorResponse(0x03, packet);

            byte byteCount = (byte)(quantity / 8 + (quantity % 8 == 0 ? 0 : 1));
            byte[] data = new byte[byteCount];

            for(int i = 0; i < quantity; i++)
            {
                bool bit = VirtualDevice.DiscreteInputs[address + i];
                if (bit) data[i / 8] |= (byte)(1 << (i % 8));
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetByteCount(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadHoldingRegisters(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            if (quantity > VirtualDevice.HoldingRegisters.Length || address > VirtualDevice.HoldingRegisters.Length - 1)
                return ErrorResponse(0x03, packet);

            byte byteCount = (byte)(quantity * 2); // 레지스터 하나당 2바이트
            byte[] data = new byte[byteCount];

            for (int i = 0; i < quantity; i++)
            {
                ushort value = VirtualDevice.HoldingRegisters[address + i];
                data[i * 2] = (byte)(value >> 8);
                data[i * 2 + 1] = (byte)(value & 0xFF);
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetByteCount(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadInputRegisters(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            if (quantity > VirtualDevice.InputRegisters.Length || address > VirtualDevice.InputRegisters.Length - 1)
                return ErrorResponse(0x03, packet);

            byte byteCount = (byte)(quantity * 2); // 레지스터 하나당 2바이트
            byte[] data = new byte[byteCount];

            for (int i = 0; i < quantity; i++)
            {
                ushort value = VirtualDevice.InputRegisters[address + i];
                data[i * 2] = (byte)(value >> 8);
                data[i * 2 + 1] = (byte)(value & 0xFF);
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetByteCount(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket WriteSingleCoil(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort data = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            // 잘못된 데이터 or 잘못된 주소 일 때 ErrorPacket 생성
            // 단일 Coil 쓰기는 0xFF00 혹은 0x0000 만 사용가능
            if ((data != 0xFF00 && data != 0x0000) || address > VirtualDevice.Coils.Length - 1) 
                return ErrorResponse(0x03, packet);
            
            // 0xFF00이면 true, 0x0000이면 false
            VirtualDevice.Coils[address] = data == 0xFF00;

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetData(packet.Data)
                .Build();
        }

        private ResponsePacket WriteSingleRegister(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort data = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            if (address > VirtualDevice.HoldingRegisters.Length - 1)
                return ErrorResponse(0x03, packet);

            // 레지스터에 데이터 쓰기
            VirtualDevice.HoldingRegisters[address] = data;

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetData(packet.Data)
                .Build();
        }

        private ResponsePacket WriteMultipleCoils(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);
            byte byteCount = packet.ByteCount;
            byte[] writeData = packet.MultiWriteData;

            if (byteCount != writeData.Length || VirtualDevice.Coils.Length < quantity) 
                return ErrorResponse(0x03, packet);

            // 쓰기 데이터를 코일에 쓰기
            for (int i = 0; i < quantity; i++)
            {
                // writeData의 i번째 비트가 1이면 true, 0이면 false
                bool bit = ((writeData[i/8] >> (i % 8)) & 0x01) == 1;
                VirtualDevice.Coils[address + i] = bit;
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetData(packet.Data.Take(4).ToArray())
                .Build();
        }

        private ResponsePacket WriteMultipleRegisters(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

            byte byteCount = packet.ByteCount;
            byte[] writeData = packet.MultiWriteData;

            if (byteCount != writeData.Length || VirtualDevice.HoldingRegisters.Length < quantity)
                return ErrorResponse(0x03, packet);

            // 쓰기 데이터를 레지스터에 쓰기
            for (int i = 0; i < quantity; i++)
            {
                ushort value = (ushort)((writeData[i * 2] << 8) | writeData[i * 2 + 1] & 0xFF);
                VirtualDevice.HoldingRegisters[address + i] = value;
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode(packet.FunctionCode)
                .SetData(packet.Data.Take(4).ToArray())
                .Build();
        }

        private ResponsePacket ErrorResponse(byte errorCode, RequestPacket packet)
        {
            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(packet.SlaveAddr)
                .SetFunctionCode((byte)(packet.FunctionCode | 0x80))
                .SetData([errorCode])
                .Build();
        }

    }
}
