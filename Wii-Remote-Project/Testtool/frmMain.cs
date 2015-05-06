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

        //Accelerometer rectangles
        bool firstGraphicDrawn = false;
        System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
        System.Drawing.Graphics acceleroGraphics;


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
            _device = HIDDevice.GetHIDDevice(0x57E, 0x306);
            createReport(0x12, new byte[2] { 0, 0x37 }); // start data stream via report 0x37
        }

        private void createReport(byte ReportID, byte[] data = null)
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = ReportID;
            if(data != null)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    report.Data[i] = data[i];
                }
            }
            _device.WriteReport(report);
            _device.ReadReport(OnReadReport);
        }

        private void requestStatusUpdate(object source, ElapsedEventArgs e)
        {
            if (chkRumble.Checked) createReport(0x15, new byte[] { 1 });
            else createReport(0x15);          
        }

        private void OnReadReport(HIDReport report)
        {
            if (this.InvokeRequired) { this.Invoke(new ReadReportCallback(OnReadReport), report); }
            else
            {
                switch (report.ReportID)
                {
                    case 0x20:
                        updateLeds(report);
                        pgbBattery.Value = Convert.ToInt32(report.Data[5]); // update battery
                        break;
                    case 0x37:
                        processButtonData(report);
                        processAccelerometerData(report);
                        break;
                }
                _device.ReadReport(OnReadReport);
            }
        }

        private void processButtonData(HIDReport report)
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

        private void updateLeds(HIDReport report)
        {
            bool[] activeLeds = new bool[4];
            byte ledStatus = report.Data[2];
            byte mask = 1 << 4;

            for(int i=0;i<4;++i)
            {
                if((mask & ledStatus) > 0)
                {
                    switch (i)
                    {
                        case 0:
                            chkLed1.Checked = true;
                            break;
                        case 1:
                            chkLed2.Checked = true;
                            break;
                        case 2:
                            chkLed3.Checked = true;
                            break;
                        case 3:
                            chkLed4.Checked = true;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            chkLed1.Checked = false;
                            break;
                        case 1:
                            chkLed2.Checked = false;
                            break;
                        case 2:
                            chkLed3.Checked = false;
                            break;
                        case 3:
                            chkLed4.Checked = false;
                            break;
                    }
                }
                mask <<= 1;
            }
        }

        private void processAccelerometerData(HIDReport report)
        {
            float[] acceleroData = new float[3] { report.Data[2] / (float)255.0, report.Data[3] / (float)255.0, report.Data[4] / (float)255.0 }; //X,Y,Z

            drawAcceleroRectangles(acceleroData);
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

        private void chkRumble_CheckedChanged(object sender, EventArgs e)
        {
            if(chkRumble.Checked) createReport(0x11, new byte[] { 1 });
            else createReport(0x11, new byte[] { 0 });
        }

        private void drawAcceleroRectangles(float[] acceleroData)
        {
            if (firstGraphicDrawn) destroyGraphics(); //destroy all previously drawn rectangles

            redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            acceleroGraphics = grpControllerRear.CreateGraphics();

            if (acceleroData[0] < .5) acceleroGraphics.FillRectangle(redBrush, new Rectangle(5, 440, (int)(acceleroData[0] * 150), 50));//X-
            else acceleroGraphics.FillRectangle(redBrush, new Rectangle(150, 440, (int)(acceleroData[0] * 150), 50));//X+*/

            if (acceleroData[1] < .5) acceleroGraphics.FillRectangle(redBrush, new Rectangle(125, 490, 50, (int)(acceleroData[2] * 150)));//Y-
            else  acceleroGraphics.FillRectangle(redBrush, new Rectangle(125, 290, 50, (int)(acceleroData[1] * 150)));//Y+          

            if (acceleroData[2] < .5) acceleroGraphics.FillRectangle(redBrush, new Rectangle(75, 650, (int)(acceleroData[2]*150), 50));//Z-
            else acceleroGraphics.FillRectangle(redBrush, new Rectangle(75, 720, (int)(acceleroData[2] * 150), 50));//Z+*/

            firstGraphicDrawn = true;
        }

        private void destroyGraphics()
        {
            redBrush.Dispose();
            acceleroGraphics.Dispose();
        }
    }

}
