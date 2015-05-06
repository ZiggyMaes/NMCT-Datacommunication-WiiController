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
        System.Drawing.SolidBrush redBrush;
        System.Drawing.Graphics acceleroGraphics;

        //Drawing canvas
        private System.Drawing.Graphics g;
        private System.Drawing.Pen pen = new System.Drawing.Pen(Color.Blue, 2F);


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
            enableIRCamera();
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
                        processIRData(report);
                        break;
                }
                _device.ReadReport(OnReadReport);
            }
        }

        private void processIRData(HIDReport report)
        {
            int[,] IRPositions = getIRPositions(report);

            if((report.Data[1] & 0x9F) == 0x4)
            {
                g = pcbDrawCanvas.CreateGraphics();
                g.FillRectangle(new System.Drawing.SolidBrush(Color.Red), new Rectangle(1023-IRPositions[0, 0], IRPositions[0, 1], 10, 10));
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

            Color[] randomColor = new System.Drawing.Color[2] {Color.Red, Color.Green};
            int random;
            Random rnd = new Random();
            random = rnd.Next(0,2);


            redBrush = new System.Drawing.SolidBrush(randomColor[random]);
            acceleroGraphics = grpControllerRear.CreateGraphics();

            
            if (acceleroData[0] < .5) acceleroGraphics.FillRectangle(redBrush, new Rectangle(5, 440, (int)(acceleroData[0] * 150), 50));//X-
            else acceleroGraphics.FillRectangle(redBrush, new Rectangle(150, 440, (int)(acceleroData[0] * 150), 50));//X+

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

        private void enableIRCamera()
        {
            createReport(0x13, new byte[1] { 0x04 });
            createReport(0x1a, new byte[1] { 0x04 });
            writeDataToRegister(0xB00030, new byte[1] { 0x08 });

            //Gevoeligheid Wii level 3
            writeDataToRegister(0xB00000, new byte[9] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xaa, 0x00, 0x64 });
            writeDataToRegister(0xb0001a, new byte[2] { 0x63, 0x03 });

            writeDataToRegister(0xB00033, new byte[1] { 0x1 });
            writeDataToRegister(0xB00030, new byte[1] { 0x8 });

        }
        private void writeDataToRegister(int address, byte[] data)
        {
            if ((_device != null))
            {
                int index = 0;
                while (index < data.Length)
                { 
                    // Bepaal hoeveel bytes er nog moeten verzonden worden
                    int leftOver = data.Length - index;

                    // We kunnen maximaal 16 bytes per keer verzenden dus moeten we het aantal te verzenden bytes daarop limiteren
                    int count = (leftOver > 16 ? 16 : leftOver);
                    int tempAddress = address + index;
                    HIDReport report = _device.CreateReport();
                    report.ReportID = 0x16;
                    report.Data[0] = (byte)((tempAddress & 0x4000000) >> 0x18); 
                    report.Data[1] = (byte)((tempAddress & 0xff0000) >> 0x10); 
                    report.Data[2] = (byte)((tempAddress & 0xff00) >> 0x8); 
                    report.Data[3] = (byte)((tempAddress & 0xff)); 
                    report.Data[4] = (byte)count;
                    Buffer.BlockCopy(data, index, report.Data, 5, count); 
                    _device.WriteReport(report); 
                    index += 16;
                }
            }
        }

        private int[,] getIRPositions(HIDReport report)
        {
            //5 - 9 / 10- 14
            int x1 = report.Data[5] | (report.Data[7] & 3 << 4) << 4;
            int x2 = report.Data[8] | (report.Data[7] & 3) << 8;
            int x3 = report.Data[10] | (report.Data[12] & 3 << 4) << 4;
            int x4 = report.Data[13] | (report.Data[12] & 3) << 8;

            int y1 = report.Data[6] | (report.Data[7] & 3 << 6) << 2;
            int y2 = report.Data[9] | (report.Data[7] & 3 << 2) << 6;
            int y3 = report.Data[11] | (report.Data[12] & 3 << 6) << 2;
            int y4 = report.Data[14] | (report.Data[12] & 3 << 2) << 6;

            Console.Write("X1: " + x1 + " || Y1: " + y1 + "\n");

            return new int[4,2] {{x1,y1}, {x2,y2},{x3,y3},{x4,y4}};
        }

    }

}
