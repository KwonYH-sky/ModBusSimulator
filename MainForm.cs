using System.IO.Ports;
using System.Text;
using ModBusSimMaster.Data;

namespace ModBusSimMaster
{
    public partial class MainForm : Form
    {
        private readonly SerialPort serialPort;

        private readonly object packetBufferLock = new();
        private readonly List<byte> packetBuffer = [];

        public MainForm()
        {
            InitializeComponent();
            serialPort = new SerialPort();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            selPortNm.Items.AddRange(ports);

            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
            serialPort.DataReceived += DataReceivedHandler;

            if (ports.Length > 0)
            {
                selPortNm.SelectedIndex = 0;
            }

            connectBtn.Text = "열기";
            selFuncCode.SelectedIndex = 0;
            SetControls(false);
        }

        private void SetControls(bool isOpen)
        {
            slaveTextBox.Enabled = isOpen;
            selFuncCode.Enabled = isOpen;
            addressTextBox.Enabled = isOpen;
            txBtn.Enabled = isOpen;
            selPortNm.Enabled = !isOpen;

            if (isOpen)
                ToggleInputFields();
            else
            {
                dataTextBox.Enabled = false;
                quantityTxBox.Enabled = false;
            }
        }

        private async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            await Task.Delay(100); // 100ms 지연 추가

            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);

            ProcessPacketBuffer(buffer);

        }

        private void ProcessPacketBuffer(byte[] bytes)
        {
            lock (packetBufferLock)
            {
                packetBuffer.AddRange(bytes);

                while (packetBuffer.Count >= 5)
                {
                    int expectedLength = PacketHelpers.GetExpectedResponsePKLength(packetBuffer.ToArray());

                    if (packetBuffer.Count < expectedLength) break;

                    byte[] packetBytes = packetBuffer.GetRange(0, expectedLength).ToArray();

                    if (PacketHelpers.CheckCRC(packetBytes))
                    {
                        var resPacket = new ResponsePacket(packetBytes);
                        StringBuilder sb = new();
                        sb.Append($"SlaveID: {resPacket.SlaveAddr} FunctionCode: {resPacket.FunctionCode}\n");
                        sb.Append("Data: ");
                        resPacket.Data.ToList().ForEach(e => sb.Append($"{e:X2} "));
                        dataRxTextBox.Invoke(() => dataRxTextBox.AppendText($"{sb}\n"));
                        packetBuffer.RemoveRange(0, expectedLength);
                    }
                    else
                    {
                        dataRxTextBox.Invoke(() => dataRxTextBox.AppendText("CRC Error\n"));
                        packetBuffer.RemoveAt(0);
                    }

                }
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                SetControls(false);
                connectBtn.Text = "열기";
            }
            else
            {
                serialPort.PortName = selPortNm.Text;
                serialPort.Open();
                SetControls(true);
                connectBtn.Text = "닫기";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void txBtn_Click(object sender, EventArgs e)
        {
            byte slaveAddr = Convert.ToByte(slaveTextBox.Text, 16);
            byte functionCode = SelFuncCodeToByte();
            ushort address = Convert.ToUInt16(addressTextBox.Text, 16);

            // TODO: 수량, 데이터 functionCode에 따라 다르게 처리
            // 0x01, 0x02, 0x03, 0x04: 수량
            // 0x05, 0x06: 데이터
            // 0x0F, 0x10: 수량, 데이터
            ushort quantity = Convert.ToUInt16(quantityTxBox.Text, 16);
            ushort data = dataTextBox.Enabled ? Convert.ToUInt16(dataTextBox.Text, 16) : quantity;

            // TODO: RequestPacket FunctionCode에 따라 다르게 생성
            var packet = new RequestPacket.RequestPacketBuilder()
                .SetSlaveAddr(slaveAddr)
                .SetFunctionCode(functionCode)
                .SetData([
                    (byte)(address >> 8), (byte)(address & 0xFF), (byte)(data >> 8), (byte)(data & 0xFF)
                ])
                .Build();

            byte[] frame = packet.Frame;
            serialPort.Write(frame, 0, frame.Length);
        }

        private byte SelFuncCodeToByte()
        {
            return selFuncCode.SelectedIndex switch
            {
                0 => 0x01,
                1 => 0x02,
                2 => 0x03,
                3 => 0x04,
                4 => 0x05,
                5 => 0x06,
                6 => 0x0F,
                7 => 0x10,
                _ => 0x01,
            };
            ;
        }

        private void dataTextBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: dataTextBox의 값이 0xFFFF를 넘어가면 0xFFFF로 설정
            // 멀티 쓰기 일 경우 byteCount를 넘어가면 byteCount로 설정
        }

        private void selFuncCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleInputFields();
        }

        private void ToggleInputFields()
        {
            if (SelFuncCodeToByte() == 0x01 || SelFuncCodeToByte() == 0x02 || SelFuncCodeToByte() == 0x03 || SelFuncCodeToByte() == 0x04)
            {
                quantityTxBox.Enabled = true;
                dataTextBox.Enabled = false;
            }
            else if (SelFuncCodeToByte() == 0x05 || SelFuncCodeToByte() == 0x06)
            {
                quantityTxBox.Enabled = false;
                dataTextBox.Enabled = true;
            }
            else if (SelFuncCodeToByte() == 0x0F || SelFuncCodeToByte() == 0x10)
            {
                quantityTxBox.Enabled = true;
                dataTextBox.Enabled = true;
            }
        }
    }
}
