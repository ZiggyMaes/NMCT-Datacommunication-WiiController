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

namespace PresentationTool
{
    public partial class frmPresentationTool : Form
    {
        HIDDevice _device;
        private System.Drawing.Graphics graphics;

        bool blnbuttonReleased = true;

        public frmPresentationTool()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            _device = HIDDevice.GetHIDDevice(0x57E, 0x306);
        }

        private void frmPresentationTool_Load(object sender, EventArgs e)
        {
            enableIRCamera();
            createReport(0x12, new byte[2] { 0, 0x37 }); // start data stream via report 0x37
        }
        private void createReport(byte ReportID, byte[] data = null)
        {
            HIDReport report = _device.CreateReport();
            report.ReportID = ReportID;
            if (data != null)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    report.Data[i] = data[i];
                }
            }
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
                    case 0x37:
                        processButtonData(report);
                        processIRData(report);
                        break;
                }
                _device.ReadReport(OnReadReport);
            }
        }

        private void processIRData(HIDReport report)
        {
            int[,] IRPositions = getIRPositions(report);
            graphics = this.CreateGraphics();


            if ((report.Data[1] & 0x9F) == 0x4)
            {
                graphics.FillRectangle(new System.Drawing.SolidBrush(Color.Red), new Rectangle(this.Width - (Convert.ToInt16(IRPositions[0,0] / 1023.0 * this.Width)), Convert.ToInt16(IRPositions[0, 1] / 767.0 * this.Height), 10, 10));
                /*Array.Resize(ref drawnPointsX, drawnPointsX.Length + 1);
                Array.Resize(ref drawnPointsY, drawnPointsY.Length + 1);
                drawnPointsX[drawnPointsX.GetUpperBound(0)] = 1023 - IRPositions[0, 0];
                drawnPointsY[drawnPointsY.GetUpperBound(0)] = IRPositions[0, 1];*/

            }
            else
            {
                graphics.Clear(this.BackColor);
                graphics.FillRectangle(new System.Drawing.SolidBrush(Color.Red), new Rectangle(this.Width - (Convert.ToInt16(IRPositions[0, 0] / 1023.0 * this.Width)), Convert.ToInt16(IRPositions[0, 1] / 767.0 * this.Height), 10, 10));


                /*for(int i=0;i<drawnPointsX.Length;++i)
                {
                    graphics.FillRectangle(new System.Drawing.SolidBrush(Color.Red), new Rectangle(drawnPointsX[i], drawnPointsY[i], 10, 10));
                }*/
            }

        }

        private void processButtonData(HIDReport report)
        {
            if (((report.Data[0] & 0x1) == 0x1) && blnbuttonReleased == true) //Left
            {
                blnbuttonReleased = false;
                SendKeys.Send("{LEFT}");         
            }
            else if (((report.Data[0] & 0x2) == 0x2) && blnbuttonReleased == true) //Right
            {
                blnbuttonReleased = false;
                SendKeys.Send("{RIGHT}");
            }
            else if ((report.Data[0] & 0x2) == 0) blnbuttonReleased = true;

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

            return new int[4, 2] { { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 } };
        }

    }
}
