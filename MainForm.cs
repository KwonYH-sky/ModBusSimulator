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
            funcCodeTextBox.Enabled = isOpen;
            addressTextBox.Enabled = isOpen;
            dataTextBox.Enabled = isOpen;
            txBtn.Enabled = isOpen;
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
            
        }
    }
}
