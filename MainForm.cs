using System.IO.Ports;
using ModBusSimMaster.Data;

namespace ModBusSimMaster
{
    public partial class MainForm : Form
    {
        private readonly SerialPort serialPort;
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
            SetControls(false);
            selFuncCode.SelectedIndex = 0;
        }

        private void SetControls(bool isOpen)
        {
            slaveTextBox.Enabled = isOpen;
            selFuncCode.Enabled = isOpen;
            addressTextBox.Enabled = isOpen;
            dataTextBox.Enabled = isOpen;
            txBtn.Enabled = isOpen;
            selPortNm.Enabled = !isOpen;
        }

        private async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            await Task.Delay(100); // 100ms 지연 추가

            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);



            var packet = new RequestPacket(buffer);
            byte[] crc = new byte[2];
            Array.Copy(buffer, buffer.Length - 2, crc, 0, 2);


            // CRC Check
            if (crc[0] != packet.Crc[0] || crc[1] != packet.Crc[1])
            {
                dataRxTextBox.Invoke(() => dataRxTextBox.AppendText("CRC Error\n"));
                return;
            }

            string str = "";

            foreach (byte b in buffer) str += b.ToString("X2") + " ";

            dataRxTextBox.Invoke(() => dataRxTextBox.AppendText( str + "\n"));
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
            ushort data = Convert.ToUInt16(dataTextBox.Text, 16);

            RequestPacket modbusRTU = new RequestPacket.RequestPacketBuilder()
                .SetSlaveAddr(slaveAddr)
                .SetFunctionCode(functionCode)
                .SetData([ 
                    (byte)(address >> 8), (byte)(address & 0xFF), (byte)(data >> 8), (byte)(data & 0xFF) 
                ])
                .Build();

            byte[] frame = modbusRTU.Frame;
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

    }
}
