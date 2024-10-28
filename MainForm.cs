using System.IO.Ports;

namespace ModBusMaster
{
    public partial class MainForm : Form
    {
        SerialPort serialPort;

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
        }

        private void SetControls(bool isOpen)
        {
            slaveTextBox.Enabled = isOpen;
            selFuncCode.Enabled = isOpen;
            addressTextBox.Enabled = isOpen;
            dataTextBox.Enabled = isOpen;
            txBtn.Enabled = isOpen;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);

            ModbusRTU modbusRTU = new ModbusRTU(buffer);

            byte slaveAddr = modbusRTU.SlaveAddr;
            byte functionCode = modbusRTU.FunctionCode;
            byte[] data = modbusRTU.Data;

            string str = $"SlaveAddr: {slaveAddr}, FunctionCode: {functionCode}, Data: ";
            foreach (byte b in data)
            {
                str += $"{b:X2} ";
            }

            dataRxTextBox.Invoke(new Action(() => dataRxTextBox.AppendText(str + "\n")));
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
            byte slaveAddr = byte.Parse(slaveTextBox.Text);
            byte functionCode = SelFuncCodeToByte();
            short address = Convert.ToInt16(addressTextBox.Text);
            short data = Convert.ToInt16(dataTextBox.Text);

            ModbusRTU modbusRTU = new ModbusRTUBuilder()
                .SetSlaveAddr(slaveAddr)
                .SetFunctionCode(functionCode)
                .SetData([ 
                    (byte)(address >> 8), (byte)(address & 0xFF), (byte)(data >> 8), (byte)(data & 0xFF) 
                ])
                .Build();

            byte[] frame = modbusRTU.GetFrame();
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
