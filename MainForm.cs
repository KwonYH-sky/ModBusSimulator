using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
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
            CreatePacket(out RequestPacket packet);
            byte[] frame = packet.Frame;
            serialPort.Write(frame, 0, frame.Length);
        }

        private void CreatePacket(out RequestPacket packet)
        {
            byte slaveAddr = Convert.ToByte(slaveTextBox.Text, 16);
            byte functionCode = SelFuncCodeToByte();
            byte[] address = BitConverter.GetBytes(Convert.ToInt16(addressTextBox.Text, 16))
                .Reverse()
                .ToArray();

            byte[] quantity = string.IsNullOrEmpty(quantityTxBox.Text) ? [] :
                BitConverter.GetBytes(Convert.ToInt16(quantityTxBox.Text, 16))
                    .Reverse()
                    .ToArray();

            byte[] writeData = string.IsNullOrEmpty(dataTextBox.Text) ? [] :
                Enumerable.Range(0, dataTextBox.Text.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(dataTextBox.Text.Substring(x, 2), 16))
                    .ToArray();

            // 0x01, 0x02, 0x03, 0x04: 수량
            // 0x05, 0x06: 데이터
            // 0x0F, 0x10: 수량, 데이터
            byte[] data = SelFuncCodeToByte() switch
            {
                0x01 or 0x02 or 0x03 or 0x04 or 0x0F or 0x10 => [.. address, .. quantity],
                0x05 or 0x06 => [.. address, .. writeData],
                _ => [],
            };

            RequestPacket.RequestPacketBuilder builder = new RequestPacket.RequestPacketBuilder()
                .SetSlaveAddr(slaveAddr)
                .SetFunctionCode(functionCode)
                .SetData(data);
            if (functionCode == 0x0F || functionCode == 0x10) builder.SetWriteData(writeData);

            packet = builder.Build();
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
        }

        private void dataTextBox_TextChanged(object sender, EventArgs e)
        {

            byte funcCode = SelFuncCodeToByte();
            byte byteCount = 4;

            if (funcCode is 0x0F or 0x10)
            {
                ushort quantity = Convert.ToUInt16(quantityTxBox.Text, 16);
                byteCount = (byte)(funcCode == 0x0F ?
                    (quantity / 8 + (quantity % 8 == 0 ? 0 : 1)) :
                    quantity * 2);
            }

            if (dataTextBox.Text.Length > byteCount)
            {
                dataTextBox.Text = new string('F', byteCount);
            }
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

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateHexInput(e);
        }

        private static void ValidateHexInput(KeyPressEventArgs e)
        {
            string pattern = @"\b[0-9a-fA-F]+\b";
            if (!Regex.IsMatch(e.KeyChar.ToString(), pattern) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
