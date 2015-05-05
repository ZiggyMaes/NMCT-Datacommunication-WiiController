using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
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

            System.Timers.Timer statusUpdateTimer = new System.Timers.Timer();
            statusUpdateTimer.Elapsed += new ElapsedEventHandler(requestStatusUpdate);
            statusUpdateTimer.Interval = 1000;
            statusUpdateTimer.Enabled = true;
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

        private void requestStatusUpdate(object source, ElapsedEventArgs e)
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = 0x15;
            _device.WriteReport(report);
            _device.ReadReport(OnReadReport);
        }

        private void OnReadReport(HIDReport report)
        {
            if (this.InvokeRequired) { this.Invoke(new ReadReportCallback(OnReadReport), report); }
            else
            {
                switch (report.ReportID)
                {
                    case 0x20:
                        processStatus(report);
                        break;
                    case 0x37:
                        getButtonData(report);
                        getAccelerometerData(report);
                        break;
                }
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
                        foreach (Control ctrl in grpControllerFront.Controls) if(ctrl is Button) ctrl.BackColor = Color.White; 
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
                        foreach (Control ctrl in grpControllerRear.Controls) if (ctrl is Button) ctrl.BackColor = Color.White;
                        break;
                }
        }

        private void processStatus(HIDReport report)
        {
            bool[] activeLeds = new bool[4];
            int batteryLevel = Convert.ToInt32(report.Data[5]);
            byte ledStatus = report.Data[2];
            
            for(int i=0;i<4;i++)
            {
                int mask = 1 << 4 + i;

                if((ledStatus & mask) > 0) activeLeds[i] = true;
                else activeLeds[i] = false;                
            }
        }

        private void getAccelerometerData(HIDReport report)
        {

        }

        private void chkLedHandler(object sender, EventArgs e)
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = 0x11;

            int i = 0;
            byte ledsHexSum = 0;
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
                                ledsHexSum |= 0x10;
                                break;
                            case 2:
                                ledsHexSum |= 0x20;
                                break;
                            case 3:
                                ledsHexSum |= 0x40;
                                break;
                            case 4:
                                ledsHexSum |= 0x80;
                                break;
                        }
                    }
                }
            }
            report.Data[0] = ledsHexSum;
            _device.WriteReport(report);
        }

        private void pgbBattery_Click(object sender, EventArgs e)
        {
            //getBatteryLevel(report);
        }
    }

}
