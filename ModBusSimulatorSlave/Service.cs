using ModBusSimSlave.Data;

namespace ModBusSimSlave
{
    class Service
    {
        private VirtualDevice VirtualDevice { get; set; }

        public Service(VirtualDevice device)
        {
            VirtualDevice = device;
        }

        public ResponsePacket? Response(RequestPacket requestPacket)
        {
            if (requestPacket.SlaveAddr != VirtualDevice.SlaveID) return null;

            Console.WriteLine("수신 데이터");
            Console.WriteLine($"SlaveAddr: {requestPacket.SlaveAddr} FunctioanCode: {requestPacket.FunctionCode}");
            Console.Write("Data: ");
            requestPacket.Data.ToList().ForEach(e => Console.Write($"{e} "));
            Console.WriteLine("");

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
                _ => null,
            };
        }

        private ResponsePacket ReadCoils(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

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
                .SetDataLength(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadDiscreteInputs(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

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
                .SetDataLength(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadHoldingRegisters(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

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
                .SetDataLength(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket ReadInputRegisters(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort quantity = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

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
                .SetDataLength(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket WriteSingleCoil(RequestPacket packet)
        {
            ushort address = (ushort)((packet.Data[0] << 8) | packet.Data[1] & 0xFF);
            ushort data = (ushort)((packet.Data[2] << 8) | packet.Data[3] & 0xFF);

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
            byte byteCount = packet.Data[4];
            byte[] writeData = packet.Data.Skip(5).ToArray();

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

            byte byteCount = packet.Data[4];
            byte[] writeData = packet.Data.Skip(5).ToArray();

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

    }
}
