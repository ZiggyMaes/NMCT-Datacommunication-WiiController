using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WII.HID.Lib;

namespace Testtool
{
    public partial class frmMain : Form
    {
        HIDDevice _device;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            init_controller();
            HIDReport report = _device.CreateReport();
            report.ReportID = 0x12;
            report.Data[1] = (byte)0x30;
            _device.WriteReport(report);
            _device.ReadReport(OnReadReport);
        }

        private void init_controller()
        {
            _device = HIDDevice.GetHIDDevice(0x57E, 0x306);
        }

        private void OnReadReport(HIDReport report)
        {
            if (this.InvokeRequired) { this.Invoke(new ReadReportCallback(OnReadReport), report); }
            else
            {
                int button_value1 = report.Data[0];
                int button_value2 = report.Data[1];

                switch (button_value1)
                {
                    case 1: //Left
                        btnDirectionLeft.BackColor = Color.Red;
                        break;
                    case 2: // Right
                        btnDirectionRight.BackColor = Color.Red;
                        break;
                    case 4: //Down
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 5: //left-down
                        btnDirectionLeft.BackColor = Color.Red;
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 6: //right-down
                        btnDirectionRight.BackColor = Color.Red;
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 8: //Up
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 9: //left-up
                        btnDirectionLeft.BackColor = Color.Red;
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 10: //right-up
                        btnDirectionRight.BackColor = Color.Red;
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 16: // PLUS
                        btnPlus.BackColor = Color.Red;
                        break;
                    default:
                        foreach (Button buttonElement in grpButtons.Controls) {  buttonElement.BackColor = Color.White; }
                        break;                 
                }

                switch(button_value2)
                {
                    case 1:
                        btn2.BackColor = Color.Red;
                        break;
                    case 2:
                        btn1.BackColor = Color.Red;
                        break;
                    case 4:
                        btnB.BackColor = Color.Red;
                        break;
                    case 8:
                        btnA.BackColor = Color.Red;
                        break;
                    case 16:
                        btnMinus.BackColor = Color.Red;
                        break;
                    case 128:
                        btnHome.BackColor = Color.Red;
                        break;
                }

                _device.ReadReport(OnReadReport);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

}
