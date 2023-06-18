using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using IronBarCode;

namespace Project
{
    public partial class Login : Form
    {
        myDatabase con = new myDatabase();
        public string name;
        public string BarCode;
        public string IME;
        public string TextData { get; set; }
        public string ImeZ { get; set; }
        public string EST1 { get; set; }
        public string Opis { get; set; }
        MySqlDataAdapter adapter;
        DataTable dataTable;
        string tesT = "";
        bool pauza = false;
        string Pv;
        string Pk;
        string UkupnaPS;
        TimeSpan sabrano = TimeSpan.Zero;

        DateTime time;
        public Login()
        {
            InitializeComponent();
            con.Connect();
        }
        private void Login_Load(object sender, EventArgs e)
        {
            tesT = TextData;
            UpdateTasks();
        }





        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.cn.Open();

                login();

                con.cn.Close();
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                con.cn.Close();
            }


        }


        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();

        }

        private void login()
        {
            using (MySqlConnection connection3 = new MySqlConnection("Datasource =192.168.0.20;username=Remote;password=admin; database=project"))
            {

                connection3.Open();
                MySqlCommand cmd3 = new MySqlCommand();
                cmd3.CommandText = "SELECT vrijeme_pocetka, pocetak_pauze, kraj_pauze, ukupna_pauza FROM tasks WHERE BarKod = '" + textBox1.Text + "'";
                cmd3.Connection = connection3;
                MySqlDataReader sdr2 = cmd3.ExecuteReader();

                while (sdr2.Read())
                {
                    using (MySqlConnection connection2 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                    {
                        connection2.Open();

                        name = sdr2["vrijeme_pocetka"].ToString();
                        Pv = sdr2["pocetak_pauze"].ToString();
                        Pk = sdr2["kraj_pauze"].ToString();
                        UkupnaPS = sdr2["ukupna_pauza"].ToString();
                        DateTime PP;
                        DateTime KP;
                        TimeSpan UP = TimeSpan.Zero;

                        if (name == string.Empty && !pauza)
                        {
                            sdr2.Close();
                            time = DateTime.Now;
                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET vrijeme_pocetka = '" + time.ToString() + "', datum_pocetka = '"+time.ToString("yyyy-MM-dd HH-mm-ss")+"' WHERE BarKod ='" + textBox1.Text + "'", connection2);
                            cmd.ExecuteNonQuery();
                            RadniciBarKod radniciBarKod = new RadniciBarKod();
                            radniciBarKod.neezDuts = textBox1.Text;
                            radniciBarKod.ShowDialog();
                           
                            AutoClosingMessageBox.Show("Zadatak je započet!", "Zadatak", 1500);
                            textBox1.Text = String.Empty;

                            UpdateTasks();

                        }
                        else if (pauza && Pv == String.Empty)
                        {
                            using (MySqlConnection connection6 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                            {
                                connection6.Open();
                                time = DateTime.Now;

                                MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET pocetak_pauze = '" + time.ToString() + "' WHERE BarKod ='" + textBox1.Text + "'", connection6);
                                cmd.ExecuteNonQuery();
                                AutoClosingMessageBox.Show("Pauza je započeta!", "Pauza", 1500);
                                textBox1.Text = String.Empty;
                                pauza = false;
                                label4.Text = "Ne";
                                UpdateTasks();

                                connection6.Close();
                            }

                           
                        }
                        else if(Pv != String.Empty && Pk == String.Empty)
                        {
                            using (MySqlConnection connection7 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                            {
                                connection7.Open();
                                time = DateTime.Now;
                                TimeSpan UkupnaP = TimeSpan.Parse(UkupnaPS);

                                MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET pocetak_pauze = '" + "" + "' WHERE BarKod ='" + textBox1.Text + "'", connection7);
                                cmd.ExecuteNonQuery();
                                PP = Convert.ToDateTime(Pv);
                                UP = time.Subtract(PP);
                                UkupnaP += UP;

                                double totalHoursUK = UkupnaP.TotalHours;

                                // Create a new TimeSpan with the total hours
                                TimeSpan UKHours = TimeSpan.FromHours(totalHoursUK);
                               
                                // Convert the TimeSpan to a string with days as hours
                                string UKP = $"{UkupnaP.Days * 24 + UKHours.Hours:D2}:{UKHours.Minutes:D2}:{UKHours.Seconds:D2}";


                                AutoClosingMessageBox.Show("Pauza je završena!", "Pauza", 1500);

                                using (MySqlConnection connection8 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                                {
                                    connection8.Open();
                                    MySqlCommand cmd2 = new MySqlCommand("UPDATE tasks SET ukupna_pauza = '" + UKP + "' WHERE BarKod ='" + textBox1.Text + "'", connection8);
                                    cmd2.ExecuteNonQuery();
                                    connection8.Close();
                                }


                                textBox1.Text = String.Empty;
                                
                                
                                pauza = false;
                                label4.Text = "Ne";
                                UpdateTasks();

                                connection7.Close();
                            }
                        }
                        else
                        {
                            using (MySqlConnection connection4 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                            {
                                sdr2.Close();
                                connection4.Open();
                                time = DateTime.Now;
                                MySqlCommand cmd2 = new MySqlCommand();
                                cmd2.CommandText = "SELECT vrijeme_pocetka, EST, pocetak_pauze, kraj_pauze, ukupna_pauza FROM tasks WHERE BarKod = '" + textBox1.Text + "'";
                                cmd2.Connection = con.cn;
                                MySqlDataReader sdr = cmd2.ExecuteReader();
                                while (sdr.Read())
                                {

                                    string TimeP = sdr["vrijeme_pocetka"].ToString();
                                    string est = sdr["EST"].ToString();

                                    string pocP = sdr["pocetak_pauze"].ToString();
                                    string krP = sdr["kraj_pauze"].ToString();

                                    string ukp = sdr["ukupna_pauza"].ToString();

                                    // convert vrijeme pocetka u datetime
                                    DateTime TimeL = Convert.ToDateTime(TimeP);
                                    time = DateTime.Now;
                                    // izracunaj razliku izmedju trenutnog vremena i pocetka
                                    TimeSpan dateU = time.Subtract(TimeL);

                                    // dodaj razliku na pocetno vrijeme da dobijes krajnje vrijeme rada na zadatku
                                    DateTime DateU = DateTime.Today + dateU;
                                   
                                    string UKstring = "";
                                    bool imaPreiliPod = false;
                                    // provjeri da li je zadnje vrijeme kraja veci od trenutnog vremena
                                    TimeSpan TimePo = new TimeSpan(int.Parse(est.Split(':')[0]),    // hours
                                                                int.Parse(est.Split(':')[1]),    // minutes
                                                                int.Parse(est.Split(':')[2]));
                                    TimeSpan TimePo2 = TimeSpan.FromHours(TimePo.TotalHours);
                                    int PODays = Convert.ToInt32(TimePo.Days);
                                    int POHours = Convert.ToInt32(TimePo.Hours);
                                    int POMinutes = Convert.ToInt32(TimePo.Minutes);
                                    int POSeconds = Convert.ToInt32(TimePo.Seconds);
                                    TimeSpan offset = new TimeSpan(PODays, POHours, POMinutes, POSeconds);
                                    DateTime TimePoBase = new DateTime(1, 1, 1);
                                    DateTime RealTimePO = TimePoBase.Add(offset);
                                    
                                    /*DateTime TimePO = Convert.ToDateTime(est);
                                    DateTime TIMEPOU = TimePO.Subtract(dateU);
                                    long ticks = 864000000000 - TIMEPOU.TimeOfDay.Ticks;
                                    TimeSpan timepou = new TimeSpan(ticks);
                                    string PrePO = new DateTime(timepou.Ticks).ToString("HH:mm:ss");*/

                                    int totalHours = dateU.Days * 24;
                                    TimeSpan ukp2 = new TimeSpan(int.Parse(ukp.Split(':')[0]),    // hours
                                                                int.Parse(ukp.Split(':')[1]),    // minutes
                                                                int.Parse(ukp.Split(':')[2]));
                                    TimeSpan dateU2 = new TimeSpan(days: 0, hours: totalHours+dateU.Hours, minutes: dateU.Minutes, seconds: dateU.Seconds);
                                    TimeSpan ukp3 = TimeSpan.FromHours(ukp2.TotalHours);
                                    TimeSpan UK = dateU2 - ukp3;

                                    double totalHoursUK = UK.TotalHours;

                                    // Create a new TimeSpan with the total hours
                                    TimeSpan UKHours = TimeSpan.FromHours(totalHoursUK);
                                    

                                    // Convert the TimeSpan to a string with days as hours

                                    UKstring = $"{UK.Days * 24 + UKHours.Hours:D2}:{UKHours.Minutes:D2}:{UKHours.Seconds:D2}";
                                   
                                    int UKDays = Convert.ToInt32(UK.Days);
                                    int UKH =Convert.ToInt32(UKHours.Hours) ;
                                    int UKMinutes =Convert.ToInt32(UKHours.Minutes) ;
                                    int UKSeconds =Convert.ToInt32(UKHours.Seconds);

                                    TimeSpan timeOffset = new TimeSpan(UKDays, UKH, UKMinutes, UKSeconds);
                                    DateTime baseDateTime = new DateTime(1, 1, 1);
                                    DateTime UKVrijeme = baseDateTime.Add(timeOffset);

                                    imaPreiliPod = true;
                                    TimeSpan najukupnije = RealTimePO - UKVrijeme;
                                    najukupnije = TimeSpan.FromTicks(Math.Abs(najukupnije.Ticks));
                                    string PrePO = new DateTime(najukupnije.Ticks).ToString("HH:mm:ss");


                                    if (UKVrijeme > RealTimePO)
                                    {

                                        sdr.Close();
                                        if (!imaPreiliPod)
                                        {
                                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET vrijeme_kraja ='" + time.ToString("HH:mm:ss") + "', ukupno_vrijeme_rada = '" + UKstring + "', uradjeno = 'YES', podbacaj = '" + PrePO + "' WHERE BarKod ='" + textBox1.Text + "'", connection4);
                                            cmd.ExecuteNonQuery();
                                            AutoClosingMessageBox.Show("Zadatak je urađen!", "Zadatak", 1500);
                                            UpdateTasks();

                                            textBox1.Text = String.Empty;
                                        }
                                        else
                                        {
                                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET vrijeme_kraja ='" + time.ToString("HH:mm:ss") + "', ukupno_vrijeme_rada = '" + UKstring + "', uradjeno = 'YES', podbacaj = '" + PrePO + "' WHERE BarKod ='" + textBox1.Text + "'", connection4);
                                            cmd.ExecuteNonQuery();
                                            AutoClosingMessageBox.Show("Zadatak je urađen!", "Zadatak", 1500);
                                            UpdateTasks();

                                            textBox1.Text = String.Empty;
                                        }
                                        
                                    }
                                    else
                                    {

                                        sdr.Close();
                                        if(!imaPreiliPod)
                                        {
                                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET vrijeme_kraja ='" + time.ToString("HH:mm:ss") + "', ukupno_vrijeme_rada = '" + UKstring + "', uradjeno = 'YES', prebacaj = '" + PrePO + "' WHERE BarKod ='" + textBox1.Text + "'", connection4);
                                            cmd.ExecuteNonQuery();
                                            AutoClosingMessageBox.Show("Zadatak je urađen!", "Zadatak", 1500);
                                            UpdateTasks();

                                            textBox1.Text = String.Empty;
                                        }
                                        else
                                        {
                                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET vrijeme_kraja ='" + time.ToString("HH:mm:ss") + "', ukupno_vrijeme_rada = '" + UKstring + "', uradjeno = 'YES', prebacaj = '" + PrePO + "' WHERE BarKod ='" + textBox1.Text + "'", connection4);
                                            cmd.ExecuteNonQuery();
                                            AutoClosingMessageBox.Show("Zadatak je urađen!", "Zadatak", 1500);
                                            UpdateTasks();

                                            textBox1.Text = String.Empty;
                                        }
                                        
                                        

                                    }

                                }
                                connection4.Close();
                            }
                        }
                        connection2.Close();

                    }
                    connection3.Close();

                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminLogin admin = new AdminLogin();
            this.Hide();
            admin.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!pauza)
            {
                pauza = true;
                label4.Text = "Da";
            }
            else
            {
                pauza = false;
                label4.Text = "Ne";
            }
            textBox1.Focus();
        }


        private void UpdateTasks()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                {
                    connection.Open();

                    using (MySqlCommand command2 = new MySqlCommand())
                    {
                        command2.CommandText = "SELECT serijski_broj FROM tasks WHERE uradjeno = 'NO'";
                        command2.Connection = connection;


                        using (MySqlDataReader sdr = command2.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                
                                using (MySqlConnection connection2 = new MySqlConnection("Datasource = 192.168.0.20;username=Remote;password=admin; database=project"))
                                {

                                    connection2.Open();
                                    using (MySqlCommand command = new MySqlCommand("SELECT task_name, ime, vrijeme_pocetka FROM tasks WHERE uradjeno = 'NO'", connection))
                                    {
                                        sdr.Close();
                                        command.ExecuteNonQuery();
                                        dataTable = new DataTable();
                                        adapter = new MySqlDataAdapter(command);
                                        adapter.Fill(dataTable);
                                        dataGridView1.DataSource = dataTable.DefaultView;
                                    }



                                    connection2.Close();
                                }

                            }
                        }



                    }
                    connection.Close();

                }

            }
            catch (Exception ex)
            {
            }


        }

        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                using (_timeoutTimer)
                    MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

     
    }
}
