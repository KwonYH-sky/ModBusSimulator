namespace ModBusSimSlave
{
    public class VirtualDevice
    {
        private static readonly Random random = new Random();

        public int SlaveID { get; set; }

        public bool[] Coils { get; set; }
        public bool[] DiscreteInputs { get; set; }

        public ushort[] HoldingRegisters { get; set; }
        public ushort[] InputRegisters { get; set; }

        public DateTime LastComunication { get; set; }


        public VirtualDevice(int slaveID, int coilCnt, int registerCnt)
        {
            SlaveID = slaveID;

            Coils = new bool[coilCnt];
            DiscreteInputs = new bool[coilCnt];

            HoldingRegisters = new ushort[registerCnt];
            InputRegisters = new ushort[registerCnt];

            InputRegisters[0] = Convert.ToUInt16(DateTime.Now.Year);
            InputRegisters[1] = Convert.ToUInt16(DateTime.Now.Month);
            InputRegisters[2] = Convert.ToUInt16(DateTime.Now.Day);
            InputRegisters[3] = Convert.ToUInt16(DateTime.Now.Hour);
            InputRegisters[4] = Convert.ToUInt16(DateTime.Now.Minute);
            InputRegisters[5] = Convert.ToUInt16(DateTime.Now.Second);

            for (int i = 0; i < coilCnt; i++)
            {
                DiscreteInputs[i] = random.Next(2) == 1;
            }

            LastComunication = DateTime.Now;
        }

        public bool ReadCoil(int address)
        {
            return Coils[address];
        }

        public bool ReadDiscreteInput(int address)
        {
            return DiscreteInputs[address];
        }

        public ushort ReadHoldingRegister(int address)
        {
            return HoldingRegisters[address];
        }

        public ushort ReadInputRegister(int address)
        {
            return InputRegisters[address];
        }

        public void WriteCoil(int address, bool value)
        {
            Coils[address] = value;
        }

        public void WriteHoldingRegister(int address, ushort value)
        {
            HoldingRegisters[address] = value;
        }

        public void UpdateComunication()
        {
            LastComunication = DateTime.Now;
        }
    }
}
