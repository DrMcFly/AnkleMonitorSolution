
using Microsoft.VisualBasic.Logging;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPIP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static class Program
        {
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

            MainService();

            var Monitor1 = new MT200X();

            string[] ex_data = {
                "0031$MGV002,015136002064033,Hello;!",
                "0166$MGV002,015136002064033,064033,R,270324,185134,A,3506.02619,N,09227.75232,W,00,0.81,0.191,9.48,196.2,,319,24,B3FE,6A440B,22,0000,0000,0,,,,,,01000,048,Timer,,;!"
            };

            //Monitor1.decoder(ex_data[1]);
            //BigBox.Text += Monitor1.decoder(ex_data[1]).Length + Environment.NewLine;
            //BigBox.Text += Enum.GetValues(typeof(MT200X.DataVariables)).Length + Environment.NewLine;

            //for (int i = 0; i < Enum.GetValues(typeof(MT200X.DataVariables)).Length; i++)
            //{
            //    MT200X.DataVariables variable = (MT200X.DataVariables)i;

            //    BigBox.Text += variable.ToString() + ": ";
            //    BigBox.Text += Monitor1.dataArray[i].ToString() + Environment.NewLine;
            //}

            

        }

        class MT200X
        {

            public enum DataVariables
            {
                DataLength,
                ProtocolVersion,
                IMEI,
                DeviceName,
                GPRS_DF,
                Date,
                Time,
                GPS_FF,
                Latitude,
                NS,
                Longitude,
                WE,
                //SatBDS,
                SatGPS,
                //SatGLONASS,
                HDOP,
                Speed,
                Course,
                Altitude,
                Mileage,
                MCC,
                MNC,
                LAC,
                CellID,
                GSM_SS,
                Temp1,
                Temp2,
                R1,
                R2,
                R3,
                R4,
                R5,
                R6,
                ExterAcc,
                BAT_PERCENT,
                AlertType,
                WIFI_Data,
                MBS
            }

            public object[] dataArray = new object[]
            {
                (int)0,                  // int data_length;
                (string)"",              // string protocol_version;
                (ulong)0,                // ulong IMEI;
                (string)"",              // string device_name;
                (char)'\0',              // char GPRS_DF;
                (int)0,                  // int date;
                (int)0,                  // int time;
                (char)'\0',              // char GPS_FF;
                (float)0.0f,              // string latitude;
                (char)'\0',              // char NS;
                (float)0.0f,              // string longitude; 
                (char)'\0',              // char WE;
                //(int)0,                  // int sat_BDS;
                (int)0,                  // int sat_GPS;
                //(int)0,                  // int sat_GLONASS;
                (float)0.0f,             // float HDOP;
                (int)0,                  // int speed;
                (int)0,                  // int course;
                (int)0,                  // int altitude;
                (int)0,                  // int mileage;
                (int)0,                  // int MCC;
                (int)0,                  // int MNC;
                (int)0,                  // int LAC;
                (string)"",              // string CELL_ID;
                (int)0,                  // int GSM_SS;
                (int)0,                  // int R1;
                (int)0,                  // int R2;
                (int)0,                  // int R3;
                (int)0,                  // int R4;
                (int)0,                  // int R5;
                (float)0.0f,             // float temp1;
                (float)0.0f,             // float temp2;
                (int)0,                  // int R6;
                (int)0,                  // int exter_acc;
                (int)0,                  // int BAT_PERCENT;
                (string)"",              // string alert_type;
                (string)"",              // string WIFI_Data;
                (string)""               // string MBS;
            };

            public int num_tags;

            public string[] decoder(string input)
            {
                char cmd_head = '$';
                char cmd_tail = '!';
                char cmd_break = ',';
                char end_tag = ';';

                char[] delimiters = { cmd_head, cmd_tail, cmd_break, end_tag };

                string[] output = input.Split(delimiters/*, StringSplitOptions.RemoveEmptyEntries*/);

                num_tags = output.Length;


                try
                {
                    if (num_tags < 10)
                    {
                        throw new Exception("You fucked up...");
                    }
                    else
                    {
                        for (int i = 0; i < num_tags; i++)
                        {
                            if (i == (int)DataVariables.Latitude || i == (int)DataVariables.Longitude || i == (int)DataVariables.Temp1 || i == (int)DataVariables.Temp2)
                            {
                                dataArray[i] = double.Parse(output[i]) * 0.01;
                            }
                            else if (output[i] == "")
                            {
                                dataArray[i] = "null";
                            }
                            else
                            {
                                dataArray[i] = output[i];
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    ;
                }

                return output;

            }

        };




        static TcpListener listener;

        const int LIMIT = 5; //5 concurrent clients
        string data;
        Socket serverSocket;
        private TextBox BigBox;
        IPEndPoint iPEndPoint;


        public void MainService()
        {
            listener = new TcpListener(2055);
            listener.Start();


            for (int i = 0; i < LIMIT; i++)
            {
                Thread t = new Thread(new ThreadStart(Service));
                t.Start();
            }
        }

        public void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();

                try
                {
                    Stream s = new NetworkStream(soc);

                    string job = "";
                    byte[] RX_data = new byte[256]; // Allocate 256 byte buffer

                    s.Read(RX_data, 0, RX_data.Length);
                    data = BitConverter.ToString(RX_data); // Converts bytes to a string

                    string ASCII_RX = Encoding.ASCII.GetString(RX_data); // Converts string of hex to text

                }
                catch (Exception e)
                {
                    throw new Exception("Failed to Connect...", e);
                }
            }
        }


        







        private void InitializeComponent()
        {
            this.BigBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BigBox
            // 
            this.BigBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BigBox.Location = new System.Drawing.Point(12, 12);
            this.BigBox.Multiline = true;
            this.BigBox.Name = "BigBox";
            this.BigBox.Size = new System.Drawing.Size(412, 632);
            this.BigBox.TabIndex = 20;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(436, 656);
            this.Controls.Add(this.BigBox);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


    }
}



