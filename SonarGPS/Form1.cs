using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;


namespace SonarGPS
{
    public partial class Form1 : Form
    {
        
        int pointX = 0, pointY = 0, pointSizeX = 20, pointSizeY = 20;
        int boardSize_XY = 340;
        int line1_XY = 0, line2_XY = 0, line3_XY = 0;

        private SerialPort SerialPort = new SerialPort();  //시리얼 포트 생성

        public Form1()
        {          
            InitializeComponent();
            SerialPort.PortName = "COM3";
            SerialPort.BaudRate = 115200;
            SerialPort.DtrEnable = true;
            SerialPort.Open();

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            int Sensor1_x = 0, Sensor1_y = 0, Sensor_sizeX = 20, Sensor_sizeY = 20;
            int Sensor2_x = 0, Sensor2_y = boardSize_XY - Sensor_sizeY;
            int Sensor3_x = boardSize_XY - Sensor_sizeX, Sensor3_y = boardSize_XY - Sensor_sizeY;

            // Sensor png 출력
            Image Sensor1 = Bitmap.FromFile("C:\\Sensor.png");
            e.Graphics.DrawImage(Sensor1, Sensor1_x, Sensor1_y, Sensor_sizeX, Sensor_sizeY);

            Image Sensor2 = Bitmap.FromFile("C:\\Sensor.png");
            e.Graphics.DrawImage(Sensor2, Sensor2_x, Sensor2_y, Sensor_sizeX, Sensor_sizeY);

            Image Sensor3 = Bitmap.FromFile("C:\\Sensor.png");
            e.Graphics.DrawImage(Sensor3, Sensor3_x, Sensor3_y, Sensor_sizeX, Sensor_sizeY);


            // x,y좌표로 각 센서에서 해당 좌표까지 선 그리기 
            Pen pen1 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(pen1, 20, 20, line1_XY, line1_XY);

            Pen pen2 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(pen2, 20, boardSize_XY - 20, line2_XY, boardSize_XY - line2_XY);

            Pen pen3 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(pen3, boardSize_XY - 20, boardSize_XY - 20, boardSize_XY - line3_XY, boardSize_XY - line3_XY);

            // draw point
            SolidBrush Point = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.FillEllipse(Point, pointX - pointSizeX / 2, pointY - pointSizeY / 2, pointSizeX, pointSizeY);

            // draw angle
            Pen angle1 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(angle1, line1_XY, line1_XY, line2_XY, boardSize_XY - line2_XY);
            Pen angle2 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(angle2, line2_XY, boardSize_XY - line2_XY, boardSize_XY - line3_XY, boardSize_XY - line3_XY);
            Pen angle3 = new Pen(Color.FromArgb(255, 0, 0, 0));
            e.Graphics.DrawLine(angle3, boardSize_XY - line3_XY, boardSize_XY - line3_XY, line1_XY, line1_XY);

        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {

            // 문자열 잘라서 Sensor 값 출력
            string str = SerialPort.ReadLine();
            string[] sensor = str.Split(',');

            // senser data strint to double
            double Intsensor1 = Convert.ToDouble(sensor[0]);
            double Intsensor2 = Convert.ToDouble(sensor[1]);
            double Intsensor3 = Convert.ToDouble(sensor[2]);

            // Sensor data limit 0 ~ 150
            if ((Intsensor1 > 150) & (Intsensor1 < 10000)) { sensor[0] = "150"; Intsensor1 = 150; }
            else if (Intsensor1 >= 10000) { sensor[0] = "0"; Intsensor1 = 0; }
            if ((Intsensor2 > 150) & (Intsensor2 < 10000)) { sensor[1] = "150"; Intsensor2 = 150; }
            else if (Intsensor2 >= 10000) { sensor[1] = "0"; Intsensor2 = 0; }
            if ((Intsensor3 > 150) & (Intsensor3 < 10000)) { sensor[2] = "150"; Intsensor3 = 150; }
            else if (Intsensor3 >= 10000) { sensor[2] = "0"; Intsensor3 = 0; }

            // testbox sensor 값 출력

            textBox13.Text = sensor[0];
            textBox14.Text = sensor[1];
            textBox15.Text = sensor[2];

            // sensor값으로 x,y좌표 구하기
            // int data = Convert.ToInt32((boardSize_XY / 2 + 10) / 1.4142135623731); //max 150일 때의 좌표
            line1_XY = Convert.ToInt32(Intsensor1 + 20 / 1.4142135623731);

            if (line1_XY <= 7)
            {
                line1_XY = (Convert.ToInt32(Intsensor1) + 20);
            }

            line2_XY = Convert.ToInt32(Intsensor2 + 20 / 1.4142135623731);
            if (line2_XY <= 7)
            {
                line2_XY = Convert.ToInt32(Intsensor2);
            }

            line3_XY = Convert.ToInt32(Intsensor3 + 20 / 1.4142135623731);
            if (line3_XY <= 7)
            {
                line3_XY = Convert.ToInt32(Intsensor3);
            }
            // Point x,y 좌표 data
            textBox1.Text = Convert.ToString((pointX - pointSizeX) + 2);
            textBox2.Text = Convert.ToString(300 - ((pointY - pointSizeY) - 2));

            // Point 좌표
            pointX = (line1_XY + line2_XY + (boardSize_XY - line3_XY)) / 3;
            pointY = (line1_XY + (boardSize_XY - line2_XY) + (boardSize_XY - line3_XY)) / 3;
            pictureBox1.Invalidate(); // 화면 갱신
            
        }
    }
}
