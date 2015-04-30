using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WII.HID.Lib;

namespace Testtool
{
    public partial class frmMain : Form
    {
        HIDDevice _device; //Wii controller object

        public frmMain()
        {
            InitializeComponent();

            //LED event handlers
            chkLed1.CheckedChanged += new System.EventHandler(chkLedHandler);
            chkLed2.CheckedChanged += new System.EventHandler(chkLedHandler);
            chkLed3.CheckedChanged += new System.EventHandler(chkLedHandler);
            chkLed4.CheckedChanged += new System.EventHandler(chkLedHandler);

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            init_controller();
            init_DataRetrieval();
        }

        private void init_controller()
        {
            _device = HIDDevice.GetHIDDevice(0x57E, 0x306);
        }

        private void init_DataRetrieval()
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = 0x12;
            report.Data[1] = (byte)0x37;
            _device.WriteReport(report);
            _device.ReadReport(OnReadReport);
        }

        private void OnReadReport(HIDReport report)
        {
            if (this.InvokeRequired) { this.Invoke(new ReadReportCallback(OnReadReport), report); }
            else
            {
                getButtonData(report);
                getBatteryData(report);
                getAccelerometerData(report);

                _device.ReadReport(OnReadReport);
            }
        }

        private void getButtonData(HIDReport report)
        {
                switch (report.Data[0] & 0x1F) //0x1F -> eerste 5 bits
                {
                    case 0x1: //Left
                        btnDirectionLeft.BackColor = Color.Red;
                        break;
                    case 0x2: // Right
                        btnDirectionRight.BackColor = Color.Red;
                        break;
                    case 0x4: //Down
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 0x5: //left-down
                        btnDirectionLeft.BackColor = Color.Red;
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 0x6: //right-down
                        btnDirectionRight.BackColor = Color.Red;
                        btnDirectionDown.BackColor = Color.Red;
                        break;
                    case 0x8: //Up
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 0x9: //left-up
                        btnDirectionLeft.BackColor = Color.Red;
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 0xA: //right-up
                        btnDirectionRight.BackColor = Color.Red;
                        btnDiretionUp.BackColor = Color.Red;
                        break;
                    case 0x10: // PLUS
                        btnPlus.BackColor = Color.Red;
                        break;
                    default:
                        foreach (Control ctrl in grpControllerFront.Controls) 
                        {
                            if(ctrl is Button) ctrl.BackColor = Color.White; 
                        }

                        break;                 
                }

                switch(report.Data[1] & 0x9F) //0x9F -> eerste 5 bits + laatste bit
                {
                    case 0x1: //button 2
                        btn2.BackColor = Color.Red;
                        break;
                    case 0x2: //button 1
                        btn1.BackColor = Color.Red;
                        break;
                    case 0x4: //button B
                        btnB.BackColor = Color.Red;
                        break;
                    case 0x8: //button A
                        btnA.BackColor = Color.Red;
                        break;
                    case 0x10: //button -
                        btnMinus.BackColor = Color.Red;
                        break;
                    case 0x80: //button home
                        btnHome.BackColor = Color.Red;
                        break;
                    default:
                        foreach (Control ctrl in grpControllerRear.Controls)
                        {
                            if (ctrl is Button) ctrl.BackColor = Color.White;
                        }
                        break;
                }
        }

        private void getBatteryData(HIDReport report)
        {

        }

        private void getAccelerometerData(HIDReport report)
        {

        }

        private void chkLedHandler(object sender, EventArgs e)
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = 0x11;

            int i = 0;
            int ledsHexSum = 0;
            foreach (Control ctrl in grpLeds.Controls)
            {
                if (ctrl is CheckBox)
                {
                    i++;
                    if (((CheckBox)ctrl).Checked)
                    {
                        switch (i)
                        {
                            case 1:
                                ledsHexSum += 16;
                                break;
                            case 2:
                                ledsHexSum += 32;
                                break;
                            case 3:
                                ledsHexSum += 64;
                                break;
                            case 4:
                                ledsHexSum += 128;
                                break;
                        }
                    }
                }
            }
            string hex = "0x" + ledsHexSum.ToString("X");
            report.Data[0] = Convert.ToByte(hex, 16);
            _device.WriteReport(report);
        }
    }

}
