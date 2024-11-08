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
            coilCnt = coilCnt < 10 ? 10 : coilCnt;
            registerCnt = registerCnt < 10 ? 10 : registerCnt;

            Coils = new bool[coilCnt];
            DiscreteInputs = new bool[coilCnt];

            HoldingRegisters = new ushort[registerCnt];
            InputRegisters = new ushort[registerCnt];

            // 가상 장치 생성된 날짜가 읽기 전용 레지스터에 순차적으로 저장
            InputRegisters[0] = Convert.ToUInt16(DateTime.Now.Year);
            InputRegisters[1] = Convert.ToUInt16(DateTime.Now.Month);
            InputRegisters[2] = Convert.ToUInt16(DateTime.Now.Day);
            InputRegisters[3] = Convert.ToUInt16(DateTime.Now.Hour);
            InputRegisters[4] = Convert.ToUInt16(DateTime.Now.Minute);
            InputRegisters[5] = Convert.ToUInt16(DateTime.Now.Second);

            // 읽기 전용 코일에 랜덤으로 ON/OFF 설정
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
