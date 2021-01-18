using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

using System.Runtime.InteropServices;

namespace keyboardToXInput
{
    public partial class keyboardToXInput : Form
    {
        #region variables
        //DLL's for hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //hotkey variables
        private bool hotkeysRegistered = false;
        private int[] hotKeyCodes = new int[25];

        //Vigembus Xbox 360 controller variables
        ViGEmClient client;
        IXbox360Controller controller;

        //button state arrays
        private bool[] keyPressBools = new bool[16];
        private bool[] triggerBools = new bool[2];


        //maximum analog value is the max size of a short
        private decimal maximumAnalogValue = 32767;
        public decimal[] currentAxisValue = new decimal[4];
        #endregion

        #region initilization functions
        public keyboardToXInput()
        {
            InitializeComponent();
            //set the default values for every input
            for (int i = 0; i < keyPressBools.Length; i++)
            {
                keyPressBools[i] = false;
            }
            for (int i = 0; i < triggerBools.Length; i++)
            {
                triggerBools[i] = true;
            }
            for (int i = 0; i < currentAxisValue.Length; i++)
            {
                currentAxisValue[i] = 0;
            }
        }

        private void registerAllHotKeys()
        {
            //taken from https://ourcodeworld.com/articles/read/573/how-to-register-a-single-or-multiple-global-hotkeys-for-a-single-key-in-winforms
            //hardcoding these hotKeyCodes
            //TODO: Create a button config
            hotKeyCodes[0] = (int)Keys.I;
            hotKeyCodes[1] = (int)Keys.K;
            hotKeyCodes[2] = (int)Keys.J;
            hotKeyCodes[3] = (int)Keys.L;
            hotKeyCodes[4] = (int)Keys.Enter;
            hotKeyCodes[5] = (int)Keys.OemQuotes;
            hotKeyCodes[6] = (int)Keys.Y;
            hotKeyCodes[7] = (int)Keys.U;
            hotKeyCodes[8] = (int)Keys.Q;
            hotKeyCodes[9] = (int)Keys.E;
            hotKeyCodes[10] = (int)Keys.Escape;
            hotKeyCodes[11] = (int)Keys.Z;
            hotKeyCodes[12] = (int)Keys.X;
            hotKeyCodes[13] = (int)Keys.C;
            hotKeyCodes[14] = (int)Keys.V;
            hotKeyCodes[15] = (int)Keys.D1;
            hotKeyCodes[16] = (int)Keys.D3;
            hotKeyCodes[17] = (int)Keys.Left;
            hotKeyCodes[18] = (int)Keys.Right;
            hotKeyCodes[19] = (int)Keys.Up;
            hotKeyCodes[20] = (int)Keys.Down;
            hotKeyCodes[21] = (int)Keys.A;
            hotKeyCodes[22] = (int)Keys.D;
            hotKeyCodes[23] = (int)Keys.S;
            hotKeyCodes[24] = (int)Keys.W;

            Boolean registered = false;
            Boolean regFail = false;
            for (int i = 0; i < hotKeyCodes.Length; i++)
            {
                registered = RegisterHotKey(this.Handle, i, 0x0000, hotKeyCodes[i]);
                if (!registered)
                {
                    regFail = true;
                }
            }
            if (regFail)
            {
               MessageBox.Show("key registration failure");
            }
        }

        private void unregisterAllHotkeys()
        {
            Boolean unregistered = false;
            Boolean regFail = false;
            for (int i = 0; i < hotKeyCodes.Length; i++)
            {
                unregistered = UnregisterHotKey(this.Handle, i);
                if (!unregistered)
                {
                    regFail = true;
                }
            }
            if (regFail)
            {
                 MessageBox.Show("key unregistration failure");
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Catch when a HotKey is pressed 
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                if (id > -1 && id < 15)
                {
                    buttonToggle(id);
                }
                else if(id >= 15 && id < 25)
                {
                    setSlider(id);
                }
            }

            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ViGEmClient();
            controller = client.CreateXbox360Controller();
            controller.Connect();
        }
        #endregion

        #region button toggles
        //press or unpress indicated button
        private void buttonToggle(int buttonNum)
        {
            keyPressBools[buttonNum] = !keyPressBools[buttonNum];
            controller.SetButtonState(buttonNum, keyPressBools[buttonNum]);
        }

        private void dUpButton_Click(object sender, EventArgs e)
        {
            buttonToggle(0);
        }

        private void dDownButton_Click(object sender, EventArgs e)
        {
            buttonToggle(1);
        }

        private void dLeftButton_Click(object sender, EventArgs e)
        {
            buttonToggle(2);
        }

        private void dRightbutton_Click(object sender, EventArgs e)
        {
            buttonToggle(3);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            buttonToggle(4);
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            buttonToggle(5);
        }

        private void leftThumbButton_Click(object sender, EventArgs e)
        {
            buttonToggle(6);
        }

        private void rightThumbButton_Click(object sender, EventArgs e)
        {
            buttonToggle(7);
        }

        private void leftBumperButton_Click(object sender, EventArgs e)
        {
            buttonToggle(8);
        }

        private void rightBumperButton_Click(object sender, EventArgs e)
        {
            buttonToggle(9);
        }

        private void centerButton_Click(object sender, EventArgs e)
        { 
            buttonToggle(10);
        }

        private void aButton_Click(object sender, EventArgs e)
        {
            buttonToggle(11);
        }

        private void bButton_Click(object sender, EventArgs e)
        {
            buttonToggle(12);
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            buttonToggle(13);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            buttonToggle(14);
        }

        #endregion

        #region axes and sliders
        private void setSlider(int id)
        {
            //15-16 are triggers
            //17-20 are right analog stick
            //21-25 are left analog stick
            switch (id)
            {
                case 15:
                    setTrigger(0);
                    break;
                case 16:
                    setTrigger(1);
                    break;
                case 17:
                    setAxis(2, -rXUpDown.Value,rXValue);
                    break;
                case 18:
                    setAxis(2, rXUpDown.Value, rXValue);
                    break;
                case 19:
                    setAxis(3, rYUpDown.Value,rYValue);
                    break;
                case 20:
                    setAxis(3, -rYUpDown.Value,rYValue);
                    break;
                case 21:
                    setAxis(0, -lXUpDown.Value,lXValue);
                    break;
                case 22:
                    setAxis(0, lXUpDown.Value, lXValue);
                    break;
                case 23:
                    setAxis(1, -lYUpDown.Value,lYValue);
                    break;
                case 24:
                    setAxis(1, lYUpDown.Value, lYValue);
                    break;
            }
        }

        private void setAxis(int axisNum, decimal axisPercentage, Label valueLabel)
        {
            Decimal setVal = maximumAnalogValue * (axisPercentage / 100);
            setVal += currentAxisValue[axisNum];
            //if the value being added goes past the max, set it to max
            if (Math.Abs(setVal) > maximumAnalogValue)
            {
                if (setVal < 0)
                {
                    setVal = -maximumAnalogValue;
                }
                else
                {
                    setVal = maximumAnalogValue;
                }
            }
            controller.SetAxisValue(axisNum, (short)setVal);
            currentAxisValue[axisNum] = (short)setVal;
            valueLabel.Text = "" + currentAxisValue[axisNum];
        }

        private void setTrigger(int triggerNum)
        {
            //flip indicated trigger state
            triggerBools[triggerNum] = !triggerBools[triggerNum];
            if (!triggerBools[triggerNum])
            {
                controller.SetSliderValue(triggerNum, (byte) 1);
            }
            else
            {
                controller.SetSliderValue(triggerNum, (byte) 0);
            }
        }
        #endregion

        #region axes and sliders buttons
        private void LtButton_Click(object sender, EventArgs e)
        {
            setTrigger(0);
        }

        private void RtButton_Click(object sender, EventArgs e)
        {
            setTrigger(1);
        }

        private void LeftAnalogZero_Click(object sender, EventArgs e)
        {
            //set axis 0 and 1 to zero
            controller.SetAxisValue(0, 0);
            controller.SetAxisValue(1, 0);
            currentAxisValue[0] = 0;
            currentAxisValue[1] = 1;
            lXValue.Text = "0";
            lYValue.Text = "0";
        }

        private void RightAnalogZero_Click(object sender, EventArgs e)
        {
            //set axis 2 and 3 to zero/
            controller.SetAxisValue(2, 0);
            controller.SetAxisValue(3, 0);
            currentAxisValue[2] = 0;
            currentAxisValue[3] = 1;
            rXValue.Text = "0";
            rYValue.Text = "0";
        }

        private void LXLeftButton_Click(object sender, EventArgs e)
        {
            setAxis(0, (short)-lXUpDown.Value, lXValue);
        }
        private void LXRightButton_Click(object sender, EventArgs e)
        {
            setAxis(0, (short)lXUpDown.Value, lXValue);
        }

        private void LYUpButton_Click(object sender, EventArgs e)
        {
            setAxis(1, (short)lYUpDown.Value, lYValue);
        }

        private void LYDownButton_Click(object sender, EventArgs e)
        {
            setAxis(1, (short)-lYUpDown.Value, lYValue);
        }

        private void RXLeftButton_Click(object sender, EventArgs e)
        {
            setAxis(2, (short)-rXUpDown.Value, rXValue);
        }

        private void RXRightButton_Click(object sender, EventArgs e)
        {
            setAxis(2, (short)rXUpDown.Value, rXValue);
        }

        private void RYUpButton_Click(object sender, EventArgs e)
        {
            setAxis(3, (short)rYUpDown.Value, rYValue);
        }

        private void RYDownButton_Click(object sender, EventArgs e)
        {
            setAxis(3, (short)-rYUpDown.Value, rYValue);
        }
        #endregion

        #region Miscellaneous Modifiers 
        private void topCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }

        private void HotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!hotkeysRegistered)
            {
                registerAllHotKeys();
            }
            else
            {
                unregisterAllHotkeys();
            }
            hotkeysRegistered = !hotkeysRegistered;
        }

        #endregion

    }
}
