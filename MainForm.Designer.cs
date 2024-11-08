namespace ModBusSimMaster
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            selPortNm = new ComboBox();
            connectBtn = new Button();
            dataRxTextBox = new RichTextBox();
            slaveTextBox = new TextBox();
            addressTextBox = new TextBox();
            dataTextBox = new TextBox();
            txBtn = new Button();
            slaveIdLabel = new Label();
            funcCodeLabel = new Label();
            dataLabel = new Label();
            addressLabel = new Label();
            selFuncCode = new ComboBox();
            quantityLabel = new Label();
            quantityTxBox = new TextBox();
            inputGroupBox = new GroupBox();
            outPutGroupBox = new GroupBox();
            inputGroupBox.SuspendLayout();
            outPutGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // selPortNm
            // 
            selPortNm.DropDownStyle = ComboBoxStyle.DropDownList;
            selPortNm.FormattingEnabled = true;
            selPortNm.Location = new Point(12, 26);
            selPortNm.Name = "selPortNm";
            selPortNm.Size = new Size(97, 23);
            selPortNm.TabIndex = 0;
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(194, 25);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(75, 23);
            connectBtn.TabIndex = 1;
            connectBtn.Text = "열기";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // dataRxTextBox
            // 
            dataRxTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dataRxTextBox.Location = new Point(6, 22);
            dataRxTextBox.Name = "dataRxTextBox";
            dataRxTextBox.Size = new Size(347, 324);
            dataRxTextBox.TabIndex = 2;
            dataRxTextBox.Text = "";
            // 
            // slaveTextBox
            // 
            slaveTextBox.Location = new Point(126, 75);
            slaveTextBox.Name = "slaveTextBox";
            slaveTextBox.Size = new Size(143, 23);
            slaveTextBox.TabIndex = 3;
            slaveTextBox.KeyPress += TextBox_KeyPress;
            // 
            // addressTextBox
            // 
            addressTextBox.Location = new Point(126, 133);
            addressTextBox.Name = "addressTextBox";
            addressTextBox.Size = new Size(143, 23);
            addressTextBox.TabIndex = 5;
            addressTextBox.KeyPress += TextBox_KeyPress;
            // 
            // dataTextBox
            // 
            dataTextBox.Location = new Point(126, 191);
            dataTextBox.Name = "dataTextBox";
            dataTextBox.Size = new Size(143, 23);
            dataTextBox.TabIndex = 6;
            dataTextBox.TextChanged += dataTextBox_TextChanged;
            dataTextBox.KeyPress += TextBox_KeyPress;
            // 
            // txBtn
            // 
            txBtn.Location = new Point(12, 294);
            txBtn.Name = "txBtn";
            txBtn.Size = new Size(274, 52);
            txBtn.TabIndex = 7;
            txBtn.Text = "보내기";
            txBtn.UseVisualStyleBackColor = true;
            txBtn.Click += txBtn_Click;
            // 
            // slaveIdLabel
            // 
            slaveIdLabel.AutoSize = true;
            slaveIdLabel.Location = new Point(12, 75);
            slaveIdLabel.Name = "slaveIdLabel";
            slaveIdLabel.Size = new Size(97, 15);
            slaveIdLabel.TabIndex = 8;
            slaveIdLabel.Text = "슬레이브ID(HEX)";
            // 
            // funcCodeLabel
            // 
            funcCodeLabel.AutoSize = true;
            funcCodeLabel.Location = new Point(12, 107);
            funcCodeLabel.Name = "funcCodeLabel";
            funcCodeLabel.Size = new Size(61, 15);
            funcCodeLabel.TabIndex = 9;
            funcCodeLabel.Text = "Func 코드";
            // 
            // dataLabel
            // 
            dataLabel.AutoSize = true;
            dataLabel.Location = new Point(12, 194);
            dataLabel.Name = "dataLabel";
            dataLabel.Size = new Size(73, 15);
            dataLabel.TabIndex = 11;
            dataLabel.Text = "데이터(HEX)";
            // 
            // addressLabel
            // 
            addressLabel.AutoSize = true;
            addressLabel.Location = new Point(12, 136);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new Size(61, 15);
            addressLabel.TabIndex = 10;
            addressLabel.Text = "주소(HEX)";
            // 
            // selFuncCode
            // 
            selFuncCode.DropDownStyle = ComboBoxStyle.DropDownList;
            selFuncCode.FormattingEnabled = true;
            selFuncCode.Items.AddRange(new object[] { "01 - Read Coils", "02 - Read Discrete Inputs", "03 - Read Holding Registers", "04 - Read Input Registers", "05 - Write Single Coil", "06 - Write Single Register", "15 - Write Multiple Coils", "16 - Write Multiple Registers" });
            selFuncCode.Location = new Point(126, 104);
            selFuncCode.Name = "selFuncCode";
            selFuncCode.Size = new Size(143, 23);
            selFuncCode.TabIndex = 12;
            selFuncCode.SelectedIndexChanged += selFuncCode_SelectedIndexChanged;
            // 
            // quantityLabel
            // 
            quantityLabel.AutoSize = true;
            quantityLabel.Location = new Point(12, 165);
            quantityLabel.Name = "quantityLabel";
            quantityLabel.Size = new Size(61, 15);
            quantityLabel.TabIndex = 14;
            quantityLabel.Text = "수량(HEX)";
            // 
            // quantityTxBox
            // 
            quantityTxBox.Location = new Point(126, 162);
            quantityTxBox.Name = "quantityTxBox";
            quantityTxBox.Size = new Size(143, 23);
            quantityTxBox.TabIndex = 13;
            quantityTxBox.KeyPress += TextBox_KeyPress;
            // 
            // inputGroupBox
            // 
            inputGroupBox.Controls.Add(quantityLabel);
            inputGroupBox.Controls.Add(txBtn);
            inputGroupBox.Controls.Add(quantityTxBox);
            inputGroupBox.Controls.Add(dataTextBox);
            inputGroupBox.Controls.Add(selFuncCode);
            inputGroupBox.Controls.Add(selPortNm);
            inputGroupBox.Controls.Add(dataLabel);
            inputGroupBox.Controls.Add(connectBtn);
            inputGroupBox.Controls.Add(addressLabel);
            inputGroupBox.Controls.Add(slaveTextBox);
            inputGroupBox.Controls.Add(funcCodeLabel);
            inputGroupBox.Controls.Add(addressTextBox);
            inputGroupBox.Controls.Add(slaveIdLabel);
            inputGroupBox.Location = new Point(12, 0);
            inputGroupBox.Name = "inputGroupBox";
            inputGroupBox.Size = new Size(292, 352);
            inputGroupBox.TabIndex = 15;
            inputGroupBox.TabStop = false;
            inputGroupBox.Text = "입력";
            // 
            // outPutGroupBox
            // 
            outPutGroupBox.Controls.Add(dataRxTextBox);
            outPutGroupBox.Location = new Point(310, 0);
            outPutGroupBox.Name = "outPutGroupBox";
            outPutGroupBox.Size = new Size(359, 352);
            outPutGroupBox.TabIndex = 15;
            outPutGroupBox.TabStop = false;
            outPutGroupBox.Text = "출력";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 364);
            Controls.Add(inputGroupBox);
            Controls.Add(outPutGroupBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "ModbusMaster";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            inputGroupBox.ResumeLayout(false);
            inputGroupBox.PerformLayout();
            outPutGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ComboBox selPortNm;
        private Button connectBtn;
        private RichTextBox dataRxTextBox;
        private TextBox slaveTextBox;
        private TextBox addressTextBox;
        private TextBox dataTextBox;
        private Button txBtn;
        private Label slaveIdLabel;
        private Label funcCodeLabel;
        private Label dataLabel;
        private Label addressLabel;
        private ComboBox selFuncCode;
        private Label quantityLabel;
        private TextBox quantityTxBox;
        private GroupBox inputGroupBox;
        private GroupBox outPutGroupBox;
    }
}
