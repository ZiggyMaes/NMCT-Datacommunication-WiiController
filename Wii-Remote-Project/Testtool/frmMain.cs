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
            report.Data[0] = (byte)0x04;
            report.Data[1] = (byte)0x30;
            _device.WriteReport(report);
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
                int dpad_value = report.Data[0]; //Left: 1 - Right: 2 - Down: 4 - Up: 8
                switch (dpad_value)
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
                    default:
                        btnDirectionLeft.BackColor = Color.White;
                        btnDirectionRight.BackColor = Color.White;
                        btnDiretionUp.BackColor = Color.White;
                        btnDirectionDown.BackColor = Color.White;
                        break;


                    
                }

                _device.ReadReport(OnReadReport);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _device.ReadReport(OnReadReport);
        }
    }

}
