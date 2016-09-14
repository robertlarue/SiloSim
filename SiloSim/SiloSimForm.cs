using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace SiloSim
{
    public partial class SiloSimForm : Form
    {
        delegate void SetTextCallback(string text);
        delegate void SetInputsCallback(BitArray inputs);
        delegate void SetOutputsCallback(BitArray outputs);
        delegate void SetBinInventoryCallback(int[] binInventory);
        delegate void SetScaleMotionCallback(bool scaleInMotion);
        delegate void SetClientLightsCallback(bool scaleClientsConnected, bool plcClientsConnected);

        private static int fillRate = 300;
        public static string label = "Silo Loadout Simulator";
        public Thread weighbridge;

        public SiloSimForm()
        {
            InitializeComponent();
        }
        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.scaleText.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                try {
                    this.Invoke(d, new object[] { text });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                this.scaleText.Text = text;
            }
        }
        public void SetInputs(BitArray inputs)
        {
            if (this.inputLight0.InvokeRequired)
            {
                SetInputsCallback d = new SetInputsCallback(SetInputs);
                try {
                    this.Invoke(d, new object[] { inputs });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                ConvertBitToColor(ref this.inputLight0, inputs[0]);
                ConvertBitToColor(ref this.inputLight1, inputs[1]);
                ConvertBitToColor(ref this.inputLight2, inputs[2]);
                ConvertBitToColor(ref this.inputLight3, inputs[3]);
                ConvertBitToColor(ref this.inputLight4, inputs[4]);
                ConvertBitToColor(ref this.inputLight5, inputs[5]);
                ConvertBitToColor(ref this.inputLight6, inputs[6]);
                ConvertBitToColor(ref this.inputLight7, inputs[7]);
                ConvertBitToColor(ref this.inputLight8, inputs[8]);
                ConvertBitToColor(ref this.inputLight9, inputs[9]);
                ConvertBitToColor(ref this.inputLight10, inputs[10]);
                ConvertBitToColor(ref this.inputLight11, inputs[11]);
                ConvertBitToColor(ref this.inputLight12, inputs[12]);
                ConvertBitToColor(ref this.inputLight13, inputs[13]);
                ConvertBitToColor(ref this.inputLight14, inputs[14]);
                ConvertBitToColor(ref this.inputLight15, inputs[15]);
                ConvertBitToColor(ref this.inputLight16, inputs[16]);
                ConvertBitToColor(ref this.inputLight17, inputs[17]);
                ConvertBitToColor(ref this.inputLight18, inputs[18]);
                ConvertBitToColor(ref this.inputLight19, inputs[19]);
                ConvertBitToColor(ref this.inputLight20, inputs[20]);
                ConvertBitToColor(ref this.inputLight21, inputs[21]);
                ConvertBitToColor(ref this.inputLight22, inputs[22]);
                ConvertBitToColor(ref this.inputLight23, inputs[23]);
                ConvertBitToColor(ref this.inputLight24, inputs[24]);
                ConvertBitToColor(ref this.inputLight25, inputs[25]);

            }
        }

        public void SetOutputs(BitArray outputs)
        {
            if (this.outputLight0.InvokeRequired)
            {
                SetOutputsCallback d = new SetOutputsCallback(SetOutputs);
                try {
                    this.Invoke(d, new object[] { outputs });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                ConvertBitToColor(ref this.outputLight0, outputs[0]);
                ConvertBitToColor(ref this.outputLight1, outputs[1]);
                ConvertBitToColor(ref this.outputLight2, outputs[2]);
                ConvertBitToColor(ref this.outputLight3, outputs[3]);
                ConvertBitToColor(ref this.outputLight4, outputs[4]);
                ConvertBitToColor(ref this.outputLight5, outputs[5]);
                ConvertBitToColor(ref this.outputLight6, outputs[6]);
                ConvertBitToColor(ref this.outputLight7, outputs[7]);
                ConvertBitToColor(ref this.outputLight8, outputs[8]);
                ConvertBitToColor(ref this.outputLight9, outputs[9]);
                ConvertBitToColor(ref this.outputLight10, outputs[10]);
                ConvertBitToColor(ref this.outputLight11, outputs[11]);
                ConvertBitToColor(ref this.outputLight12, outputs[12]);
                ConvertBitToColor(ref this.outputLight13, outputs[13]);
            }
        }
        public void SetBinInventory(int[] binInventory)
        {
            if (this.binInv1.InvokeRequired)
            {
                SetBinInventoryCallback d = new SetBinInventoryCallback(SetBinInventory);
                try
                {
                    this.Invoke(d, new object[] { binInventory });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                if(!this.binInv1.Focused)
                    this.binInv1.Text = binInventory[0].ToString();
                if (!this.binInv2.Focused)
                    this.binInv2.Text = binInventory[1].ToString();
                if (!this.binInv3.Focused)
                    this.binInv3.Text = binInventory[2].ToString();
                if (!this.binInv4.Focused)
                    this.binInv4.Text = binInventory[3].ToString();
                if (!this.binInv5.Focused)
                    this.binInv5.Text = binInventory[4].ToString();
            }
        }
        public void SetScaleMotion(bool scaleInMotion)
        {
            if (this.scaleMovementBtn.InvokeRequired)
            {
                SetScaleMotionCallback d = new SetScaleMotionCallback(SetScaleMotion);
                try
                {
                    this.Invoke(d, new object[] { scaleInMotion });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                if (scaleInMotion)
                {
                    this.scaleMovementBtn.BackColor = Color.Red;
                }
                else
                {
                    this.scaleMovementBtn.BackColor = SystemColors.Control;
                }
            }
        }

        public void SetClientLights(bool scaleClientsConnected, bool plcClientsConnected)
        {
            if (this.scaleClientLight.InvokeRequired)
            {
                SetClientLightsCallback d = new SetClientLightsCallback(SetClientLights);
                try
                {
                    this.Invoke(d, new object[] { scaleClientsConnected, plcClientsConnected });
                }
                catch
                {
                    Application.Exit();
                }
            }
            else
            {
                if (scaleClientsConnected)
                {
                    this.scaleClientLight.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.scaleClientLight.BackColor = Color.Silver;
                }
                if (plcClientsConnected)
                {
                    this.plcClientLight.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.plcClientLight.BackColor = Color.Silver;
                }
            }
        }
        void WeighBridgeThread()
        {
            while (true)
            {

                if (commonPulseRb.Checked)
                {
                    PLC.binInventory[0] = PLC.binInventory[0] + Convert.ToInt32(PLC.inputs[15]);
                    PLC.binInventory[1] = PLC.binInventory[1] + Convert.ToInt32(PLC.inputs[16]);
                    PLC.binInventory[2] = PLC.binInventory[2] + Convert.ToInt32(PLC.inputs[17]);
                    PLC.binInventory[3] = PLC.binInventory[3] + Convert.ToInt32(PLC.inputs[18]);
                    PLC.binInventory[4] = PLC.binInventory[4] + Convert.ToInt32(PLC.inputs[19]);
                    PLC.inputs[14] = true;
                }
                else
                {
                    PLC.inputs[15] = silo1FillRb.Checked;
                    PLC.inputs[16] = silo2FillRb.Checked;
                    PLC.inputs[17] = silo3FillRb.Checked;
                    PLC.inputs[18] = silo4FillRb.Checked;
                    PLC.inputs[19] = silo5FillRb.Checked;
                    PLC.binInventory[0] = PLC.binInventory[0] + Convert.ToInt32(PLC.inputs[15]);
                    PLC.binInventory[1] = PLC.binInventory[1] + Convert.ToInt32(PLC.inputs[16]);
                    PLC.binInventory[2] = PLC.binInventory[2] + Convert.ToInt32(PLC.inputs[17]);
                    PLC.binInventory[3] = PLC.binInventory[3] + Convert.ToInt32(PLC.inputs[18]);
                    PLC.binInventory[4] = PLC.binInventory[4] + Convert.ToInt32(PLC.inputs[19]);
                }
                Thread.Sleep(100*3600 / fillRate);
                if (commonPulseRb.Checked)
                {
                    PLC.inputs[14] = false;
                }
                else
                {
                    PLC.inputs[15] = false;
                    PLC.inputs[16] = false;
                    PLC.inputs[17] = false;
                    PLC.inputs[18] = false;
                    PLC.inputs[19] = false;
                }
                Thread.Sleep(900 * 3600 / fillRate);
            }
        }
        private void ConvertBitToColor(ref Panel panel, bool input)
        {
            if (input)
            {
                if (panel.BackColor != Color.Chartreuse)
                {
                    panel.BackColor = Color.Chartreuse;
                }
            }
            else {
                if (panel.BackColor != Color.Silver)
                {
                    panel.BackColor = Color.Silver;
                }
            }
        }

        private void hornResetCb_CheckedChanged(object sender, EventArgs e)
        {
            Core.hornResetEnabled = hornResetCb.Checked;
        }

        private void scaleTareBtn_Click(object sender, EventArgs e)
        {
            if (validateWeight(scaleTareBox.Text))
            {
                SiloSim.Scale.scaleWeight = int.Parse(scaleTareBox.Text);
            }
        }
        private bool validateWeight(string weightString)
        {
            int parsedWeight = 0;
            if (int.TryParse(weightString, out parsedWeight))
            {
                if(parsedWeight >= -120000 && parsedWeight <= 120000)
                {
                    return true;
                }
            }
            return false;
        }
        private bool validateRate(string rateString)
        {
            int parsedRate = 0;
            if (int.TryParse(rateString, out parsedRate))
            {
                if (parsedRate > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool validatePort(string portString)
        {
            int parsedPort = 0;
            if (int.TryParse(portString, out parsedPort))
            {
                if (parsedPort > 0 && parsedPort < 65537)
                {
                    return true;
                }
            }
            return false;
        }
        private void scaleGrossBtn_Click(object sender, EventArgs e)
        {
            if (validateWeight(scaleGrossBox.Text))
            {
                SiloSim.Scale.scaleWeight = int.Parse(scaleGrossBox.Text);
            }
        }

        private void scaleNetBtn_Click(object sender, EventArgs e)
        {
            if (validateWeight(scaleNetBox.Text))
            {
                SiloSim.Scale.scaleWeight = int.Parse(scaleNetBox.Text);
            }
        }

        private void scaleZeroBtn_Click(object sender, EventArgs e)
        {
            SiloSim.Scale.scaleWeight = 0;
        }

        private void scaleIncBtn_MouseDown(object sender, MouseEventArgs e)
        {
            SiloSim.Scale.scaleWeight += SiloSim.Scale.fillIncrement;
        }

        private void scaleDecBtn_MouseDown(object sender, MouseEventArgs e)
        {
            SiloSim.Scale.scaleWeight -= SiloSim.Scale.fillIncrement;
        }

        private void fillDeselectRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = false;
                PLC.inputs[16] = false;
                PLC.inputs[17] = false;
                PLC.inputs[18] = false;
                PLC.inputs[19] = false;
            }
        }

        private void silo5FillRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = false;
                PLC.inputs[16] = false;
                PLC.inputs[17] = false;
                PLC.inputs[18] = false;
                PLC.inputs[19] = true;
            }
        }

        private void silo4FillRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = false;
                PLC.inputs[16] = false;
                PLC.inputs[17] = false;
                PLC.inputs[18] = true;
                PLC.inputs[19] = false;
            }
        }

        private void silo3FillRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = false;
                PLC.inputs[16] = false;
                PLC.inputs[17] = true;
                PLC.inputs[18] = false;
                PLC.inputs[19] = false;
            }
        }

        private void silo2FillRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = false;
                PLC.inputs[16] = true;
                PLC.inputs[17] = false;
                PLC.inputs[18] = false;
                PLC.inputs[19] = false;
            }
        }

        private void silo1FillRb_CheckedChanged(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[15] = true;
                PLC.inputs[16] = false;
                PLC.inputs[17] = false;
                PLC.inputs[18] = false;
                PLC.inputs[19] = false;
            }
        }

        private void loadDeselectRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = false;
            PLC.inputs[5] = false;
            PLC.inputs[6] = false;
            PLC.inputs[7] = false;
            PLC.inputs[8] = false;
        }

        private void silo5LoadRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = false;
            PLC.inputs[5] = false;
            PLC.inputs[6] = false;
            PLC.inputs[7] = false;
            PLC.inputs[8] = true;
        }

        private void silo4LoadRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = false;
            PLC.inputs[5] = false;
            PLC.inputs[6] = false;
            PLC.inputs[7] = true;
            PLC.inputs[8] = false;
        }

        private void silo3LoadRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = false;
            PLC.inputs[5] = false;
            PLC.inputs[6] = true;
            PLC.inputs[7] = false;
            PLC.inputs[8] = false;
        }

        private void silo2LoadRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = false;
            PLC.inputs[5] = true;
            PLC.inputs[6] = false;
            PLC.inputs[7] = false;
            PLC.inputs[8] = false;
        }

        private void silo1LoadRb_CheckedChanged(object sender, EventArgs e)
        {
            PLC.inputs[4] = true;
            PLC.inputs[5] = false;
            PLC.inputs[6] = false;
            PLC.inputs[7] = false;
            PLC.inputs[8] = false;
        }

        private void fillControlBtn_Click(object sender, EventArgs e)
        {
            if (fillControlBtn.Text == "Start Filling")
            {
                fillControlBtn.Text = "Stop Filling";
                fillRateBox.Enabled = false;
                weighbridge = new Thread(WeighBridgeThread);
                weighbridge.Start();
            }
            else
            {
                weighbridge.Abort();
                fillControlBtn.Text = "Start Filling";
                fillRateBox.Enabled = true;
            }
        }

        private void binInv1_KeyDown(object sender, KeyEventArgs e)
        {
            PLC.binInventory[0] = int.Parse(binInv1.Text);
        }

        private void binInv2_KeyDown(object sender, KeyEventArgs e)
        {
            PLC.binInventory[1] = int.Parse(binInv2.Text);
        }

        private void binInv3_KeyDown(object sender, KeyEventArgs e)
        {
            PLC.binInventory[2] = int.Parse(binInv3.Text);
        }

        private void binInv4_KeyDown(object sender, KeyEventArgs e)
        {
            PLC.binInventory[3] = int.Parse(binInv4.Text);
        }

        private void binInv5_KeyDown(object sender, KeyEventArgs e)
        {
            PLC.binInventory[4] = int.Parse(binInv5.Text);
        }

        private void scaleTareBox_TextChanged(object sender, EventArgs e)
        {
            if (validateWeight(scaleTareBox.Text))
            {
                SiloSim.Scale.tareWeight = int.Parse(scaleTareBox.Text);
            }
            else
            {
                scaleTareBox.Text = SiloSim.Scale.tareWeight.ToString();
            }
        }

        private void scaleGrossBox_TextChanged(object sender, EventArgs e)
        {
            if (validateWeight(scaleGrossBox.Text))
            {
                SiloSim.Scale.grossWeight = int.Parse(scaleGrossBox.Text);
            }
            else
            {
                scaleGrossBox.Text = SiloSim.Scale.grossWeight.ToString();
            }
        }

        private void scaleNetBox_TextChanged(object sender, EventArgs e)
        {
            if (validateWeight(scaleNetBox.Text))
            {
                SiloSim.Scale.netWeight = int.Parse(scaleNetBox.Text);
            }
            else
            {
                scaleNetBox.Text = SiloSim.Scale.netWeight.ToString();
            }
        }

        private void onePerSiloRb_Click(object sender, EventArgs e)
        {
            if (onePerSiloRb.Checked)
            {
                PLC.inputs[13] = false;
                PLC.inputs[14] = false;
                PLC.inputs[15] = false;
                PLC.inputs[16] = false;
                PLC.inputs[17] = false;
                PLC.inputs[18] = false;
                PLC.inputs[19] = false;
                lblInput14.Text = "";
            }
            else
            {
                PLC.inputs[13] = true;
                lblInput14.Text = "Tons\nPulse";
            }
        }

        private void commonPulseRb_Click(object sender, EventArgs e)
        {
            if (commonPulseRb.Checked)
            {
                PLC.inputs[13] = true;
                PLC.inputs[15] = silo1FillRb.Checked;
                PLC.inputs[16] = silo2FillRb.Checked;
                PLC.inputs[17] = silo3FillRb.Checked;
                PLC.inputs[18] = silo4FillRb.Checked;
                PLC.inputs[19] = silo5FillRb.Checked;
                lblInput14.Text = "Tons\nPulse";
            }
            else
            {
                PLC.inputs[13] = false;
                lblInput14.Text = "";
            }
        }

        private void loadoutRateBox_TextChanged(object sender, EventArgs e)
        {
            if (validateRate(loadoutRateBox.Text))
            {
                SiloSim.Scale.fillIncrement = int.Parse(loadoutRateBox.Text) / 100;
            }
        }

        private void fillRateBox_TextChanged(object sender, EventArgs e)
        {
            if (validateRate(fillRateBox.Text))
            {
                fillRate = int.Parse(fillRateBox.Text);
            }
            else
            {
                fillRateBox.Text = fillRate.ToString();
            }
        }

        private void siloSimFormLabelBox_TextChanged(object sender, EventArgs e)
        {
            this.Text = siloSimFormLabelBox.Text;
        }

        private void startStopSimBtn_Click(object sender, EventArgs e)
        {
            if(startStopSimBtn.Text == "Stop Simulation")
            {
                startStopSimBtn.Text = "Start Simulation";
                scalePortBox.Enabled = true;
                plcPortBox.Enabled = true;
                PLC.plcTcpServer.Stop();
                SiloSim.Scale.scaleTcpServer.Stop();
                foreach (System.Net.Sockets.TcpClient client in PLC.plcClients)
                {
                    client.Close();
                }
                foreach (System.Net.Sockets.TcpClient client in SiloSim.Scale.scaleClients)
                {
                    client.Close();
                }
            }
            else
            {
                startStopSimBtn.Text = "Stop Simulation";
                scalePortBox.Enabled = false;
                plcPortBox.Enabled = false;
                Thread scaleThread = new Thread(SiloSim.Scale.ListenClientThread);
                scaleThread.Start(SiloSim.Scale.port);
                Thread plcThread = new Thread(PLC.ListenClientThread);
                plcThread.Start(PLC.port);
            }
        }

        private void scalePortBox_TextChanged(object sender, EventArgs e)
        {
            if (validatePort(scalePortBox.Text))
            {
                SiloSim.Scale.port = int.Parse(scalePortBox.Text);
            }
            else
            {
                scalePortBox.Text = SiloSim.Scale.port.ToString();
            }
        }

        private void plcPortBox_TextChanged(object sender, EventArgs e)
        {
            if (validatePort(plcPortBox.Text))
            {
                PLC.port = int.Parse(plcPortBox.Text);
            }
            else
            {
                plcPortBox.Text = PLC.port.ToString();
            }
        }

        private void SiloSimForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void SiloSimForm_Load(object sender, EventArgs e)
        {
            this.clientsList.SetToolTip(this.scaleClientLight, "Scale Clients:");
            this.clientsList.SetToolTip(this.plcClientLight, "PLC Clients:");
            this.scalePortBox.Text = SiloSim.Scale.port.ToString();
            this.plcPortBox.Text = SiloSim.PLC.port.ToString();
            this.Text = label;
        }

        private void plcClientLight_MouseEnter(object sender, EventArgs e)
        {
            generateClientLists();
        }
        private void scaleClientLight_MouseEnter(object sender, EventArgs e)
        {
            generateClientLists();
        }
        private void generateClientLists()
        {
            string plcClientList = "PLC Clients:";
            foreach (System.Net.Sockets.TcpClient client in PLC.plcClients)
            {
                plcClientList = plcClientList + "\n" + ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            }
            this.clientsList.SetToolTip(this.plcClientLight, plcClientList);

            string scaleClientList = "Scale Clients:";
            foreach (System.Net.Sockets.TcpClient client in SiloSim.Scale.scaleClients)
            {
                scaleClientList = scaleClientList + "\n" + ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            }
            this.clientsList.SetToolTip(this.scaleClientLight, scaleClientList);
        }

        private void inputLight0_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[0] == true)
            {
                PLC.inputs[0] = false;
                silo1LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[0] = true;
                silo1LowCb.Checked = true;
            }
        }

        private void inputLight1_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[1] == true)
            {
                PLC.inputs[1] = false;
                silo2LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[1] = true;
                silo2LowCb.Checked = true;
            }
        }

        private void inputLight2_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[2] == true)
            {
                PLC.inputs[2] = false;
                silo3LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[2] = true;
                silo3LowCb.Checked = true;
            }
        }

        private void inputLight3_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[3] == true)
            {
                PLC.inputs[3] = false;
                silo4LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[3] = true;
                silo4LowCb.Checked = true;
            }
        }

        private void inputLight9_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[9] == true)
            {
                PLC.inputs[9] = false;
                silo5LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[9] = true;
                silo5LowCb.Checked = true;
            }
        }

        private void silo1LowCb_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[0] == true)
            {
                PLC.inputs[0] = false;
                silo1LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[0] = true;
                silo1LowCb.Checked = true;
            }
        }

        private void silo2LowCb_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[1] == true)
            {
                PLC.inputs[1] = false;
                silo2LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[1] = true;
                silo2LowCb.Checked = true;
            }
        }

        private void silo3LowCb_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[2] == true)
            {
                PLC.inputs[2] = false;
                silo3LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[2] = true;
                silo3LowCb.Checked = true;
            }
        }

        private void silo4LowCb_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[3] == true)
            {
                PLC.inputs[3] = false;
                silo4LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[3] = true;
                silo4LowCb.Checked = true;
            }
        }

        private void silo5LowCb_Click(object sender, EventArgs e)
        {
            if (PLC.inputs[9] == true)
            {
                PLC.inputs[9] = false;
                silo5LowCb.Checked = false;
            }
            else
            {
                PLC.inputs[9] = true;
                silo5LowCb.Checked = true;
            }
        }

        private void outputLight0_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[0] = true;
        }

        private void outputLight0_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[0] = false;
        }

        private void outputLight1_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[1] = true;
        }

        private void outputLight1_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[1] = false;
        }

        private void outputLight2_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[2] = true;
        }

        private void outputLight2_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[2] = false;
        }

        private void outputLight3_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[3] = true;
        }

        private void outputLight3_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[3] = false;
        }

        private void outputLight4_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[4] = true;
        }

        private void outputLight4_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[4] = false;
        }

        private void outputLight5_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[5] = true;
        }

        private void outputLight5_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[5] = false;
        }

        private void outputLight6_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[6] = true;
        }

        private void outputLight6_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[6] = false;
        }

        private void outputLight7_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[7] = true;
        }

        private void outputLight7_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[7] = false;
        }

        private void outputLight8_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[8] = true;
        }

        private void outputLight8_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[8] = false;
        }

        private void outputLight9_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[9] = true;
        }

        private void outputLight9_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[9] = false;
        }

        private void outputLight10_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[10] = true;
        }

        private void outputLight10_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[10] = false;
        }

        private void outputLight11_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[11] = true;
        }

        private void outputLight11_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[11] = false;
        }

        private void outputLight12_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[12] = true;
        }

        private void outputLight12_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[12] = false;
        }

        private void outputLight13_MouseDown(object sender, MouseEventArgs e)
        {
            PLC.outputs[13] = true;
        }

        private void outputLight13_MouseUp(object sender, MouseEventArgs e)
        {
            PLC.outputs[13] = false;
        }
    }
}
