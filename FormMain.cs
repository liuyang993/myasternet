using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


//using AsterNET.Manager;
//using AsterNET.Manager.Event;

using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Response;
using AsterNET.FastAGI;
using AsterNET.Manager.Event;
using AsterNET.FastAGI.MappingStrategies;

using System.Diagnostics;


namespace AsterNET.WinForm
{
	public partial class FormMain : Form
	{

        class CDR 
        {
    
            public string Anumber;
            public string Bnumber;
            public DateTime answerTime;
            public DateTime hangupDate;
        }


        const string ORIGINATE_CONTEXT = "from-internel";
        const string ORIGINATE_CHANNEL = "pjsip/7001";


        const string ORIGINATE_EXTRA_CHANNEL = "SIP/101";
        const string ORIGINATE_EXTRA_EXTEN = "101";


        const string ORIGINATE_EXTEN = @"6001";
        const string ORIGINATE_CALLERID = "Asterisk.NET";
        const int ORIGINATE_TIMEOUT = 150000;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();


        Dictionary<string, CDR> dcdr = new Dictionary<string, CDR>();

        //double dDuration = 0.0;


		public FormMain()
		{
			InitializeComponent();


            dataGridView1.Columns.Add("A number", "A number");
            dataGridView1.Columns.Add("B number", "B number");
            dataGridView1.Columns.Add("B ring time", "B ring time");
            dataGridView1.Columns.Add("B answer time", "B answer time");
            dataGridView1.Columns.Add("B hangup time", "B hangup time");
            dataGridView1.Columns.Add("uniqueID", "uniqueID");


            //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 30000;
            timer.Tick += new EventHandler(timer_Tick);



		}

        void timer_Tick(object sender, EventArgs e)
        {
            dcdr.Clear();
            button5.PerformClick();
          // textBox3.Text = dDuration.ToString();
        }

		private ManagerConnection manager = null;
		private void btnConnect_Click(object sender, EventArgs e)
		{
			string address = this.tbAddress.Text;
			int port = int.Parse(this.tbPort.Text);
			string user = this.tbUser.Text;
			string password = this.tbPassword.Text;

			btnConnect.Enabled = false;
			manager = new ManagerConnection(address, port, user, password);
            
            
            
            


            //manager.DialBegin += manager_DialBegin;
            manager.DialEnd += manager_DialEnd;
            manager.Hangup += call_hangup_Events;   
            
         
       

            //manager.UnhandledEvent += new ManagerEventHandler(manager_Events);


            

			try
			{
				// Uncomment next 2 line comments to Disable timeout (debug mode)
				// manager.DefaultResponseTimeout = 0;
				// manager.DefaultEventTimeout = 0;
				manager.Login();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error connect\n" + ex.Message);
				manager.Logoff();
				this.Close();
			}
			btnDisconnect.Enabled = true;
		}

        void manager_DialEnd(object sender, DialEndEvent e)
        {
           
            DateTime dtAnswerTime = e.DateReceived;

            string Srcnumber = e.Attributes["destcalleridnum"];

            if (dcdr.ContainsKey(Srcnumber))
            {
                dcdr[Srcnumber].answerTime = dtAnswerTime;


                //this.Invoke((MethodInvoker)delegate
                //{
                //    textBox4.AppendText("answer time match " + "\r\n");
                //});

            }
            
            
            
            
            //string sUnique = e.DestUniqueId;

            //this.Invoke((MethodInvoker)delegate
            //{

            //    while (true)
            //    {
            //        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //        {
            //            if (this.dataGridView1.Rows[i].Cells["uniqueID"].Value.ToString() == sUnique)
            //            {
            //                this.dataGridView1.Rows[i].Cells["B answer time"].Value = dtAnswerTime;
            //                return;
            //            }

            //        }
            //    }
            //});

            //if(dcdr.ContainsKey(



            return;

        }

        void manager_DialBegin(object sender, DialBeginEvent e)
        {
            string srtAnumber = e.Attributes["destcalleridnum"].ToString();
            string strBnumber = e.DialString;
            DateTime dtRingTime = e.DateReceived;
            string sUnique = e.DestUniqueId;

            this.Invoke((MethodInvoker)delegate
            {
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells["A number"].Value = srtAnumber;
                this.dataGridView1.Rows[index].Cells["B number"].Value = strBnumber;
                this.dataGridView1.Rows[index].Cells["B ring time"].Value = dtRingTime;
                this.dataGridView1.Rows[index].Cells["uniqueID"].Value = sUnique;

                this.dataGridView1.Rows[index].HeaderCell.Value = String.Format("{0}", index + 1);

                //this.dataGridView1.Refresh();
            });

            return;

        }

        void call_hangup_Events(object sender, HangupEvent e)
        {

            //textBox4.AppendText("hangup  event happen  " + "\r\n");
            DateTime dtAnswerTime = e.DateReceived;

            string Srcnumber = e.CallerIdNum;


            //this.Invoke((MethodInvoker)delegate
            //{
            //    textBox4.AppendText("hangup  time match " + "\r\n");
            //});


            if (dcdr.ContainsKey(Srcnumber))
            {




                dcdr[Srcnumber].hangupDate = dtAnswerTime;

                var secDiff = (dcdr[Srcnumber].hangupDate - dcdr[Srcnumber].answerTime).TotalSeconds;

                if (secDiff > 1)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox2.Text = (int.Parse(textBox2.Text) + 1).ToString();
                    });


                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox3.Text = (double.Parse(secDiff.ToString()) + double.Parse(textBox3.Text)).ToString();
                    });


                    
                    return;

                }
                else
                {
                    //textBox2.Text = "a";
                    //textBox3.Text = "b";
                    return;
                }

            }



            //string sUnique = e.Attributes["linkedid"].ToString();

            //this.Invoke((MethodInvoker)delegate
            //{

            //    while (true)
            //    {
            //        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //        {
            //            if (this.dataGridView1.Rows[i].Cells["uniqueID"].Value.ToString() == sUnique)
            //            {
            //                this.dataGridView1.Rows[i].Cells["B hangup time"].Value = dtAnswerTime;
            //                return; 

            //                //add total connect calls and total talk duration 

            //                //var secDiff = (dtAnswerTime - DateTime.Parse(this.dataGridView1.Rows[i].Cells["B answer time"].Value.ToString())).TotalSeconds;

            //                //if (secDiff > 2)
            //                //{

            //                //    textBox2.Text = (int.Parse(textBox2.Text) + 1).ToString();
            //                //    textBox3.Text = (int.Parse(secDiff.ToString()) + int.Parse(textBox3.Text)).ToString();
            //                //    textBox3.Refresh();
            //                //    return;

            //                //}
            //                //else
            //                //{
            //                //    return;
            //                //}

            //            }

            //        }
            //    }

            //});

            return;

        }

		void manager_Events(object sender, ManagerEvent e)
		{
           // string s1 = e.UniqueId;
            //Debug.WriteLine("Event : " + e.GetType().Name);

            //if (e.GetType().Name == "DialBegin")
            //{

            //    Debug.WriteLine("Event : " + e.GetType().Name);
            //}
                
		}





		private void btnDisconnect_Click(object sender, EventArgs e)
		{
			btnConnect.Enabled = true;
			if (this.manager != null)
			{
				manager.Logoff();
				this.manager = null;
			}
			btnDisconnect.Enabled = false;
		}

        private void button1_Click(object sender, EventArgs e)
        {


            ///****************    office asterisk
            // * 
            //channel originate sip/ast2/70019779805550000 extension 100@autodialer

            long  initBNumber = 70019779805550000;

            //long initBNumber = 80019779805550000;

            long initANumber = 97477009999; 


            for (int i = 0; i < 100; i++)
            {

                OriginateAction oc = new OriginateAction();
                oc.Async = true;
                oc.Context = @"autodialer";
                oc.Priority = "1";
                //oc.Channel = @"sip/ast2/70019779805550000";

                oc.Channel = @"sip/ast2/" + (initBNumber+i).ToString();


                //oc.CallerId = "97477009999";

                oc.CallerId = (initANumber+i).ToString();
                oc.Exten = "100";
                oc.Timeout = ORIGINATE_TIMEOUT;
                manager.SendAction(oc, null);

            }



            //********/




            // my banwagon  

            //OriginateAction oc1 = new OriginateAction();
            //oc1.Context = @"from-internel";
            //oc1.Priority = "1";
            //oc1.Channel = @"pjsip/7001";
            //oc1.CallerId = "97477009999";
            //oc1.Exten = "100";
            //oc1.Async = true;
            //oc1.Timeout = ORIGINATE_TIMEOUT;

            //manager.SendAction(oc1, null);







            //


                //OriginateAction oc1 = new OriginateAction();
                //oc1.Context = @"autodialer";
                //oc1.Priority = "1";
                //oc1.Channel = @"sip/ast2/70019779805550000";
                //oc1.CallerId = "97477009999";
                //oc1.Exten = "100";
                //oc1.Timeout = ORIGINATE_TIMEOUT;
                //manager.SendAction(oc1, null);


            //

                //OriginateAction oc2 = new OriginateAction();
                //oc2.Context = @"autodialer";
                //oc2.Priority = "1";
                //oc2.Channel = @"sip/ast2/70019779805550000";
                //oc2.CallerId = "97477009999";
                //oc2.Exten = "100";
                //oc2.Timeout = ORIGINATE_TIMEOUT;
                //manager.SendAction(oc2, null);




        }

        private void button2_Click(object sender, EventArgs e)
        {
            OriginateAction oc = new OriginateAction();
            oc.Context = @"from-internel";
            oc.Priority = "1";
            oc.Channel = @"Local/8001@from-internel";
            oc.CallerId = "999111";
            //oc.
            
            oc.Exten = "8001";
            oc.Timeout = ORIGINATE_TIMEOUT;
            oc.Async = true;

            manager.SendAction(oc, null);
            //ManagerResponse originateResponse = manager.SendAction(oc, oc.Timeout);

            //OriginateAction oc1 = new OriginateAction();
            //oc1.Context = @"from-internel";
            //oc1.Priority = "1";
            //oc1.Channel = @"Local/8001@from-internel";
            //oc1.CallerId = ORIGINATE_CALLERID;
            //oc1.Exten = "8001";
            //oc1.Timeout = ORIGINATE_TIMEOUT;

            ////manager.SendAction(oc1, null);

            ////manager.ReconnectIntervalFast

            //if (manager.IsConnected())
            //{
            //    ManagerResponse originateResponse1 = manager.SendAction(oc1, oc1.Timeout);

            //}
            //OriginateAction oc2 = new OriginateAction();
            //oc2.Context = @"from-internel";
            //oc2.Priority = "1";
            //oc2.Channel = @"Local/8001@from-internel";
            //oc2.CallerId = ORIGINATE_CALLERID;
            //oc2.Exten = "8001";
            //oc2.Timeout = ORIGINATE_TIMEOUT;

            //manager.SendAction(oc2, null);

            return;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //long initBNumber = 70019779805550000;

            long initBNumber = 80019779805550000;

            long initANumber = 97477009999;


            for (int i = 0; i < 100; i++)
            {

                OriginateAction oc = new OriginateAction();
                oc.Async = true;
                oc.Context = @"autodialer";
                oc.Priority = "1";
                //oc.Channel = @"sip/ast2/70019779805550000";

                oc.Channel = @"sip/ast2/" + (initBNumber + i).ToString();


                //oc.CallerId = "97477009999";

                oc.CallerId = (initANumber + i).ToString();
                oc.Exten = "100";
                oc.Timeout = ORIGINATE_TIMEOUT;
                manager.SendAction(oc, null);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            textBox2.Text = "";
            textBox3.Text = "";

        }

        private void button5_Click(object sender, EventArgs e)
        {

            // my bandwagon  one call

           // channel originate pjsip/6001 extension 6001@from-internel

            //OriginateAction oc1 = new OriginateAction();
            //oc1.Context = @"from-internel";
            //oc1.Priority = "1";
            //oc1.Channel = @"pjsip/6001";
            //oc1.CallerId = "97477009999";
            //oc1.Exten = "6001";
            //oc1.Async = true;
            //oc1.Timeout = ORIGINATE_TIMEOUT;


            //CDR call = new CDR();
            //call.Anumber = "97477009999";
            //call.Bnumber = "7001";

            //try
            //{
            //    dcdr.Add(call.Anumber, call);
            //    manager.SendAction(oc1, null);
            //}
            //catch (Exception ep)
            //{ }


            dcdr.Clear();

            // asterisk 190 N calls 

            long initBNumber = 500036312340000;

            long initANumber = 85222930000;


            int j = int.Parse(textBox1.Text);

            for (int i = 0; i < j; i++)
            {

                OriginateAction oc = new OriginateAction();
                oc.Async = true;


                oc.Context = @"autodialer";
                oc.Priority = "1";
                oc.Channel = @"sip/astToTB/" + (initBNumber + i).ToString();

                oc.CallerId = (initANumber + i).ToString();
                oc.Exten = "100";
                oc.Timeout = ORIGINATE_TIMEOUT;


                //oc.Context = @"from-internel";
                //oc.Priority = "1";
                //oc.Channel = @"pjsip/6001";
                //oc.CallerId = "97477009999";
                //oc.Exten = "6001";
                //oc.Async = true;
                //oc.Timeout = ORIGINATE_TIMEOUT;


                CDR call = new CDR();
                call.Anumber = oc.CallerId;
                call.Bnumber = (initBNumber + i).ToString();

                try
                {
                    dcdr.Add(call.Anumber, call);

                    manager.SendAction(oc, null);
                }
                catch (Exception ep)
                { }

            }
                


            //*************************************                end of my bandwagon 







            /********    asterisk 190

            long initBNumber = 500036312340000;


            long initANumber = 85222930000;

            
            //int j = int.Parse(textBox1.Text);

            for (int i = 0; i < 50; i++)
            {

                OriginateAction oc = new OriginateAction();
                oc.Async = true;
                oc.Context = @"autodialer";
                oc.Priority = "1";
                //oc.Channel = @"sip/ast2/70019779805550000";

                oc.Channel = @"sip/astToTB/" + (initBNumber + i).ToString();


                //oc.CallerId = "97477009999";

                oc.CallerId = (initANumber + i).ToString();
                oc.Exten = "100";
                oc.Timeout = ORIGINATE_TIMEOUT;
                manager.SendAction(oc, null);


            }    // for 

             * 
             * 
             * ******///                     ebd of asterisk 190 


            





















            //int i,k=0;
            //int j = int.Parse(textBox1.Text);


            //while(true)
            //{
            //    for ( i = k; i < j; i++)
            //    {

            //        OriginateAction oc = new OriginateAction();
            //        oc.Async = true;
            //        oc.Context = @"autodialer";
            //        oc.Priority = "1";
            //        //oc.Channel = @"sip/ast2/70019779805550000";

            //        oc.Channel = @"sip/astToTB/" + (initBNumber + i).ToString();


            //        //oc.CallerId = "97477009999";

            //        oc.CallerId = (initANumber + i).ToString();
            //        oc.Exten = "100";
            //        oc.Timeout = ORIGINATE_TIMEOUT;
            //        manager.SendAction(oc, null);

            //        if (i >= k + 50)
            //        {
            //            k = k + 51;
            //            System.Threading.Thread.Sleep(30000);
            //            break;
                        
            //        }

                  
            //      }    // for 

                

            //  }
            


        }

        private void button6_Click(object sender, EventArgs e)
        {
            //var secDiff = 5;
            //textBox3.Text = (int.Parse(secDiff.ToString()) + int.Parse(textBox3.Text)).ToString();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            timer.Start();
        }
	}
}
