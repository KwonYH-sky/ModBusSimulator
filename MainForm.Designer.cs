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
            SuspendLayout();
            // 
            // selPortNm
            // 
            selPortNm.DropDownStyle = ComboBoxStyle.DropDownList;
            selPortNm.FormattingEnabled = true;
            selPortNm.Location = new Point(12, 12);
            selPortNm.Name = "selPortNm";
            selPortNm.Size = new Size(121, 23);
            selPortNm.TabIndex = 0;
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(139, 11);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(75, 23);
            connectBtn.TabIndex = 1;
            connectBtn.Text = "열기";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // dataRxTextBox
            // 
            dataRxTextBox.Location = new Point(232, 12);
            dataRxTextBox.Name = "dataRxTextBox";
            dataRxTextBox.Size = new Size(264, 260);
            dataRxTextBox.TabIndex = 2;
            dataRxTextBox.Text = "";
            // 
            // slaveTextBox
            // 
            slaveTextBox.Location = new Point(97, 62);
            slaveTextBox.Name = "slaveTextBox";
            slaveTextBox.Size = new Size(117, 23);
            slaveTextBox.TabIndex = 3;
            slaveTextBox.KeyPress += TextBox_KeyPress;
            // 
            // addressTextBox
            // 
            addressTextBox.Location = new Point(97, 120);
            addressTextBox.Name = "addressTextBox";
            addressTextBox.Size = new Size(117, 23);
            addressTextBox.TabIndex = 5;
            addressTextBox.KeyPress += TextBox_KeyPress;
            // 
            // dataTextBox
            // 
            dataTextBox.Location = new Point(97, 178);
            dataTextBox.Name = "dataTextBox";
            dataTextBox.Size = new Size(117, 23);
            dataTextBox.TabIndex = 6;
            dataTextBox.TextChanged += dataTextBox_TextChanged;
            dataTextBox.KeyPress += TextBox_KeyPress;
            // 
            // txBtn
            // 
            txBtn.Location = new Point(12, 228);
            txBtn.Name = "txBtn";
            txBtn.Size = new Size(202, 44);
            txBtn.TabIndex = 7;
            txBtn.Text = "보내기";
            txBtn.UseVisualStyleBackColor = true;
            txBtn.Click += txBtn_Click;
            // 
            // slaveIdLabel
            // 
            slaveIdLabel.AutoSize = true;
            slaveIdLabel.Location = new Point(12, 62);
            slaveIdLabel.Name = "slaveIdLabel";
            slaveIdLabel.Size = new Size(67, 15);
            slaveIdLabel.TabIndex = 8;
            slaveIdLabel.Text = "슬레이브ID";
            // 
            // funcCodeLabel
            // 
            funcCodeLabel.AutoSize = true;
            funcCodeLabel.Location = new Point(12, 94);
            funcCodeLabel.Name = "funcCodeLabel";
            funcCodeLabel.Size = new Size(61, 15);
            funcCodeLabel.TabIndex = 9;
            funcCodeLabel.Text = "Func 코드";
            // 
            // dataLabel
            // 
            dataLabel.AutoSize = true;
            dataLabel.Location = new Point(12, 181);
            dataLabel.Name = "dataLabel";
            dataLabel.Size = new Size(43, 15);
            dataLabel.TabIndex = 11;
            dataLabel.Text = "데이터";
            // 
            // addressLabel
            // 
            addressLabel.AutoSize = true;
            addressLabel.Location = new Point(12, 123);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new Size(31, 15);
            addressLabel.TabIndex = 10;
            addressLabel.Text = "주소";
            // 
            // selFuncCode
            // 
            selFuncCode.DropDownStyle = ComboBoxStyle.DropDownList;
            selFuncCode.FormattingEnabled = true;
            selFuncCode.Items.AddRange(new object[] { "01 - Read Coils", "02 - Read Discrete Inputs", "03 - Read Holding Registers", "04 - Read Input Registers", "05 - Write Single Coil", "06 - Write Single Register", "15 - Write Multiple Coils", "16 - Write Multiple Registers" });
            selFuncCode.Location = new Point(97, 91);
            selFuncCode.Name = "selFuncCode";
            selFuncCode.Size = new Size(117, 23);
            selFuncCode.TabIndex = 12;
            selFuncCode.SelectedIndexChanged += selFuncCode_SelectedIndexChanged;
            // 
            // quantityLabel
            // 
            quantityLabel.AutoSize = true;
            quantityLabel.Location = new Point(12, 152);
            quantityLabel.Name = "quantityLabel";
            quantityLabel.Size = new Size(31, 15);
            quantityLabel.TabIndex = 14;
            quantityLabel.Text = "수량";
            // 
            // quantityTxBox
            // 
            quantityTxBox.Location = new Point(97, 149);
            quantityTxBox.Name = "quantityTxBox";
            quantityTxBox.Size = new Size(117, 23);
            quantityTxBox.TabIndex = 13;
            quantityTxBox.KeyPress += TextBox_KeyPress;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(508, 284);
            Controls.Add(quantityLabel);
            Controls.Add(quantityTxBox);
            Controls.Add(selFuncCode);
            Controls.Add(dataLabel);
            Controls.Add(addressLabel);
            Controls.Add(funcCodeLabel);
            Controls.Add(slaveIdLabel);
            Controls.Add(txBtn);
            Controls.Add(dataTextBox);
            Controls.Add(addressTextBox);
            Controls.Add(slaveTextBox);
            Controls.Add(dataRxTextBox);
            Controls.Add(connectBtn);
            Controls.Add(selPortNm);
            Name = "MainForm";
            Text = "ModbusMaster";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
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
    }
}
