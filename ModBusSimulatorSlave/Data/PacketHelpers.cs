namespace ModBusSimSlave.Data
{
    internal static class PacketHelpers
    {
        public static ushort CalcCRC(byte[] data, int offset, int count)
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

        public static bool CheckCRC(byte[] buffer)
        {
            ushort crc = CalcCRC(buffer, 0, buffer.Length - 2);
            return crc == (ushort)(buffer[buffer.Length - 2] | (buffer[buffer.Length - 1] << 8));
        }

        public static int GetExpectedRequestPKLength(byte[] buffer)
        {
            int functionCode = buffer[1];
            return functionCode switch
            {
                0x01 or 0x02 or 0x03 or 0x04 or 0x05 or 0x06 => 8,
                0x0F or 0x10 => 9 + buffer[6], // Write Multiple 패킷 길이
                _ => 8

            };
        }

        public static int GetExpectedResponsePKLength(byte[] buffer)
        {
            int functionCode = buffer[1];
            return functionCode switch
            {
                0x01 or 0x02 or 0x03 or 0x04 => 5 + buffer[2], // Read 패킷 길이
                0x05 or 0x06 or 0x0F or 0x10 => 8, // Write 패킷 길이
                _ => 5,
            };
        }
    }
}