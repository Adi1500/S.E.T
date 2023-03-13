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
using Microsoft.Office.Interop.Excel;
using System.IO;
using OfficeOpenXml;
using System.Configuration;
using System.Globalization;


namespace Project
{
    public partial class Form1 : Form
    {
        myDatabase con = new myDatabase();
        MySqlCommand command;
        MySqlDataAdapter adapter;
        System.Data.DataTable dataTable;
        int br = 0;
        DateTime pocetak = new DateTime();
        DateTime kraj = new DateTime();
        private XlColorIndex color;
        bool promjena = true;
        public Form1()
        {
            InitializeComponent();
            con.Connect();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is System.Windows.Forms.TextBox && c != null)
                {
                    if (c.Tag == "EST")
                    {
                        c.Visible = false;
                    }
                    
                }
            }
            foreach (Control c in this.Controls)
            {
                if (c is System.Windows.Forms.TextBox && c != null)
                {
                    if (c.Tag == "Opis")
                    {
                        c.Visible = false;
                    }

                }
            }

            DateTime d = new DateTime();
            d = DateTime.Now;

            monthCalendar2.SetDate(d);
            monthCalendar1.SetDate(d);

            con.cn.Open();

            Refresh3();
            Refresh2();
            Refresh4();

            con.cn.Close();
        }

        public void Refresh2()
        {

            comboBox1.Items.Clear();
            List<string> imena = new List<string>();

            MySqlCommand cmd2 = new MySqlCommand();
            cmd2.CommandText = "SELECT * FROM users";
            cmd2.Connection = con.cn;
            MySqlDataReader sdr = cmd2.ExecuteReader();
            int bg = 0;
            while (sdr.Read())
            {
                imena.Add(sdr["Ime"].ToString());
            }
            sdr.Close();

            for (int i = 0; i < imena.Count; i++)
            {
                comboBox1.Items.Add(imena[i]);
            }

            comboBox1.Items.Add("Svi");
            comboBox1.Items.Add("Nista");
        }

        public void Refresh3()
        {

            comboBox2.Items.Clear();
            List<string> imena = new List<string>();

            MySqlCommand cmd2 = new MySqlCommand();
            cmd2.CommandText = "SELECT * FROM tasks WHERE broj_naloga ='"+comboBox3.Text+"'";
            cmd2.Connection = con.cn;
            MySqlDataReader sdr = cmd2.ExecuteReader();
            int bg = 0;
            while (sdr.Read())
            {
                if(!imena.Contains(sdr["serijski_broj"].ToString()))
                {
                    imena.Add(sdr["serijski_broj"].ToString());
                }
                    
            }
            sdr.Close();

            for (int i = 0; i < imena.Count; i++)
            {
                comboBox2.Items.Add(imena[i]);
            }

            comboBox2.Items.Add("Svi");
            comboBox2.Items.Add("Nista");
        }

        public void Refresh4()
        {

            comboBox3.Items.Clear();
            List<string> imena = new List<string>();

            MySqlCommand cmd2 = new MySqlCommand();
            cmd2.CommandText = "SELECT * FROM tasks";
            cmd2.Connection = con.cn;
            MySqlDataReader sdr = cmd2.ExecuteReader();
            int bg = 0;
            while (sdr.Read())
            {
                if (!imena.Contains(sdr["broj_naloga"].ToString()))
                {
                    imena.Add(sdr["broj_naloga"].ToString());
                }

            }
            sdr.Close();

            for (int i = 0; i < imena.Count; i++)
            {
                comboBox3.Items.Add(imena[i]);
            }

            comboBox3.Items.Add("Svi");
            comboBox3.Items.Add("Nista");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.cn.Open();
                Refresh2();
                Refresh3();
                Refresh4();
                UpdateData();
               
                con.cn.Close();

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                con.cn.Open();
                string serial = textBox17.Text;
                string kolicina = KOLBOX.Text;
                string brojNaloga = BRN.Text;
                string imeKupca = IMEK.Text;
                int barkod = 0;
                //br = 0;
                int br2 = 0;
                int value = 0;
                bool jasko = false;
                foreach (Control c in this.Controls)
                {
                    if (c is System.Windows.Forms.CheckBox && c != null)
                    {
                        if (((System.Windows.Forms.CheckBox)c).Checked && c.Tag == "Zadaci")
                        {
                            using (MySqlConnection connection = new MySqlConnection("Datasource = 0.0.0.0;username=Remote;password=; database=project"))
                            {
                                connection.Open();
                                using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM tasks WHERE serijski_broj = '" + serial + "'", connection))
                                {
                                    value = Convert.ToInt32(command.ExecuteScalar());
                                    br = value;

                                }
                                connection.Close();
                            }
                            Random random = new Random();
                            int barCode = random.Next(10000000, 99999999);
                            using (MySqlConnection connection = new MySqlConnection("Datasource =0.0.0.0;username=Remote;password=; database=project"))
                            {
                                connection.Open();
                                MySqlCommand cmd3 = new MySqlCommand();
                                cmd3.CommandText = "SELECT BarKod FROM tasks";
                                cmd3.Connection = connection;
                                MySqlDataReader sdr2 = cmd3.ExecuteReader();
                                
                                while (sdr2.Read())
                                {
                                    barkod = int.Parse(sdr2["BarKod"].ToString());
                                    if (barkod == barCode)
                                    {
                                        barCode = random.Next(10000000, 99999999);
                                    }
                                   
                                }
                                connection.Close();
                            }
                           
                            DateTime time;
                            time = DateTime.Now;
                            MySqlCommand cmd = new MySqlCommand("INSERT INTO tasks (task_name, broj, serijski_broj, ime_kupca, broj_naloga, kolicina, BarKod, datum) VALUES('" + c.Text + "', '" + br + "', '"+serial+"', '"+imeKupca+"', '"+brojNaloga+"', '"+kolicina+"', '"+barCode+"', '"+ time.Date.ToString("dd/MM/yyyy") + "')", con.cn);
                            cmd.ExecuteNonQuery();
                            textBox17.Text = String.Empty;
                            BRN.Text = String.Empty;
                            IMEK.Text = String.Empty;
                            KOLBOX.Text = String.Empty;
                            br++;
                            br2++;
                        }
                    }
                }
                br = br-br2;
                foreach (Control c in this.Controls)
                {
                    if (c is System.Windows.Forms.TextBox && c != null)
                    {
                        if (c.Tag == "EST")
                        {
                            if (c.Text != string.Empty)
                            {

                                TimeSpan choad = TimeSpan.FromMinutes(Convert.ToInt32(c.Text));
                                string chucy = choad.ToString(@"hh\:mm\:ss");
                                MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET EST = '" + chucy + "' WHERE broj ='" + br + "' AND serijski_broj = '" + serial + "'", con.cn);
                                cmd.ExecuteNonQuery();

                                c.Text = String.Empty;
                                br++;

                            }



                        }

                    }
                }
                br = br-br2;
                foreach (Control c in this.Controls)
                {
                    if (c is System.Windows.Forms.TextBox && c != null)
                    {
                        if (c.Tag == "Opis")
                        {
                            if (c.Text != string.Empty)
                            {
                                MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET opis = '" + c.Text + "' WHERE broj ='" + br + "' AND serijski_broj = '" + serial + "'", con.cn);
                                cmd.ExecuteNonQuery();

                                c.Text = String.Empty;
                                br++;

                            }



                        }

                    }
                }
                foreach (Control c in this.Controls)
                {
                    if (c is System.Windows.Forms.CheckBox && c != null)
                    {
                        if (((System.Windows.Forms.CheckBox)c).Checked && c.Tag == "Zadaci")
                        {
                            
                            ((System.Windows.Forms.CheckBox)c).Checked = false;
                            
                        }
                    }
                }
                con.cn.Close();
                //this.Controls.Clear();
                //this.InitializeComponent();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            EST1.Visible = true;
            DES1.Visible = true;
            DES1.Text = "Nema opisa";
            if (checkBox1.Checked == false)
            {
                EST1.Visible = false;

                DES1.Visible = false;

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            EST2.Visible = true;
            DES2.Visible = true;
            DES2.Text = "Nema opisa";

            if (checkBox2.Checked == false)
            {
                EST2.Visible = false;
                DES2.Visible = false;

            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            EST3.Visible = true;
            DES3.Visible = true;
            DES3.Text = "Nema opisa";

            if (checkBox4.Checked == false)
            {
                EST3.Visible = false;
                DES3.Visible = false;

            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            EST4.Visible = true;
            DES4.Visible = true;
            DES4.Text = "Nema opisa";

            if (checkBox3.Checked == false)
            {
                EST4.Visible = false;
                DES4.Visible = false;

            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            EST5.Visible = true;
            DES5.Visible = true;
            DES5.Text = "Nema opisa";

            if (checkBox6.Checked == false)
            {
                EST5.Visible = false;
                DES5.Visible = false;

            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            EST6.Visible = true;
            DES6.Visible = true;
            DES6.Text = "Nema opisa";

            if (checkBox5.Checked == false)
            {
                EST6.Visible = false;
                DES6.Visible = false;

            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            EST7.Visible = true;
            DES7.Visible = true;
            DES7.Text = "Nema opisa";

            if (checkBox8.Checked == false)
            {
                EST7.Visible = false;
                DES7.Visible = false;

            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            EST8.Visible = true;
            DES8.Visible = true;
            DES8.Text = "Nema opisa";

            if (checkBox7.Checked == false)
            {
                EST8.Visible = false;
                DES8.Visible = false;

            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            EST9.Visible = true;
            DES9.Visible = true;
            DES9.Text = "Nema opisa";

            if (checkBox10.Checked == false)
            {
                EST9.Visible = false;
                DES9.Visible = false;

            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            EST10.Visible = true;
            DES10.Visible = true;
            DES10.Text = "Nema opisa";

            if (checkBox9.Checked == false)
            {
                EST10.Visible = false;
                DES10.Visible = false;

            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            EST11.Visible = true;
            DES11.Visible = true;
            DES11.Text = "Nema opisa";

            if (checkBox12.Checked == false)
            {
                EST11.Visible = false;
                DES11.Visible = false;

            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            EST12.Visible = true;
            DES12.Visible = true;
            DES12.Text = "Nema opisa";

            if (checkBox11.Checked == false)
            {
                EST12.Visible = false;
                DES12.Visible = false;

            }
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            EST13.Visible = true;
            DES13.Visible = true;
            DES13.Text = "Nema opisa";

            if (checkBox14.Checked == false)
            {
                EST13.Visible = false;
                DES13.Visible = false;

            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            EST14.Visible = true;
            DES14.Visible = true;
            DES14.Text = "Nema opisa";

            if (checkBox13.Checked == false)
            {
                EST14.Visible = false;
                DES14.Visible = false;

            }
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            EST15.Visible = true;
            DES15.Visible = true;
            DES15.Text = "Nema opisa";

            if (checkBox16.Checked == false)
            {
                EST15.Visible = false;
                DES15.Visible = false;
            }
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            EST16.Visible = true;
            DES16.Visible = true;
            DES16.Text = "Nema opisa";

            if (checkBox15.Checked == false)
            {
                EST16.Visible = false;
                DES16.Visible = false;

            }
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            EST17.Visible = true;
            DES17.Visible = true;
            DES17.Text = "Nema opisa";

            if (checkBox18.Checked == false)
            {
                EST17.Visible = false;
                DES17.Visible = false;

            }
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            EST18.Visible = true;
            DES18.Visible = true;
            DES18.Text = "Nema opisa";

            if (checkBox17.Checked == false)
            {
                EST18.Visible = false;
                DES18.Visible = false;

            }
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            EST19.Visible = true;
            DES19.Visible = true;
            DES19.Text = "Nema opisa";

            if (checkBox20.Checked == false)
            {
                EST19.Visible = false;
                DES19.Visible = false;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;

            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;
            xlWorkSheet.Cells[1, 2] = "Ime zadatka";
            xlWorkSheet.Cells[1, 3] = "Serijski broj";
            xlWorkSheet.Cells[1, 4] = "Predviđeno vrijeme rada";
            xlWorkSheet.Cells[1, 5] = "Ime";
            xlWorkSheet.Cells[1, 6] = "Vrijeme početka";
            xlWorkSheet.Cells[1, 7] = "Vrijeme kraja";
            xlWorkSheet.Cells[1, 8] = "Ukupno vrijeme rada";
            xlWorkSheet.Cells[1, 10] = "Urađeno";
            xlWorkSheet.Cells[1, 11] = "Podbačaj";
            xlWorkSheet.Cells[1, 12] = "Prebačaj";
            xlWorkSheet.Cells[1, 13] = "Datum";
            xlWorkSheet.Cells[1, 14] = "Opis Zadatka";
            xlWorkSheet.Cells[1, 15] = "Ime Kupca";
            xlWorkSheet.Cells[1, 16] = "Broj Naloga";
            xlWorkSheet.Cells[1, 17] = "Kolicina";
            xlWorkSheet.Cells[1, 18] = "Bar Kod";
            xlWorkSheet.Cells[1, 21] = "Ukupna Pauza";
            xlWorkSheet.Cells[1, 22] = "Datum Pocetka";


            xlWorkSheet.Cells[1, 2].Font.Bold = true;
            xlWorkSheet.Cells[1, 3].Font.Bold = true;
            xlWorkSheet.Cells[1, 4].Font.Bold = true;
            xlWorkSheet.Cells[1, 5].Font.Bold = true;
            xlWorkSheet.Cells[1, 6].Font.Bold = true;
            xlWorkSheet.Cells[1, 7].Font.Bold = true;
            xlWorkSheet.Cells[1, 8].Font.Bold = true;
            xlWorkSheet.Cells[1, 10].Font.Bold = true;
            xlWorkSheet.Cells[1, 11].Font.Bold = true;
            xlWorkSheet.Cells[1, 12].Font.Bold = true;
            xlWorkSheet.Cells[1, 13].Font.Bold = true;
            xlWorkSheet.Cells[1, 14].Font.Bold = true;
            xlWorkSheet.Cells[1, 15].Font.Bold = true;
            xlWorkSheet.Cells[1, 16].Font.Bold = true;
            xlWorkSheet.Cells[1, 17].Font.Bold = true;
            xlWorkSheet.Cells[1, 18].Font.Bold = true;
            xlWorkSheet.Cells[1, 19].Font.Bold = true;
            xlWorkSheet.Cells[1, 21].Font.Bold = true;
            xlWorkSheet.Cells[1, 22].Font.Bold = true;

            Range cells = xlWorkSheet.Cells[1, 3];
            Range cell3 = xlWorkSheet.Cells[dataGridView1.Rows.Count, 3];
            Range range5 = xlWorkSheet.get_Range(cells, cell3);
            range5.NumberFormat = "@";


            // storing Each row and column value to excel sheet  
            for ( i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for ( j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (j == 0 || j == 8 || j == 19)
                    {
                        continue;
                    }

                    if (j == dataGridView1.Columns.Count - 1)
                    {
                        xlWorkSheet.Cells[i + 2, j + 2] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {

                        xlWorkSheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        xlWorkSheet.Cells[i + 2, 19].Formula = "*&" + xlWorkSheet.Cells[i + 2, 18].Address + "&*";


                        xlWorkSheet.Cells[i + 2, 19].Font.Name = "3 of 9 Barcode";
                    }

                }
            }
            string quote = "\"";


            xlWorkSheet.Cells[dataGridView1.Rows.Count + 3, 8].Formula = "=TEXT(SUM(" + xlWorkSheet.Cells[2, 8].Address +
        ":" + xlWorkSheet.Cells[dataGridView1.Rows.Count, 8].Address + "), "+quote+"[h]:mm:ss"+quote+")";

            xlWorkSheet.Cells[dataGridView1.Rows.Count+2, 8] = "Vrijeme Provedeno na zadacima";
            xlWorkSheet.Cells[dataGridView1.Rows.Count + 2, 8].Font.Bold = true;

            

            xlWorkSheet.Cells[dataGridView1.Rows.Count + 3, 4].Formula = "=SUM(" + xlWorkSheet.Cells[2, 4].Address +
     ":" + xlWorkSheet.Cells[dataGridView1.Rows.Count, 4].Address + ")";

            xlWorkSheet.Cells[dataGridView1.Rows.Count + 2, 4] = "Ukupno predvidjeno vrijeme";
            xlWorkSheet.Cells[dataGridView1.Rows.Count + 2, 4].Font.Bold = true;


            xlWorkSheet.Cells[dataGridView1.Rows.Count + 5, 4].Formula = "=TEXT(SUMIF("+xlWorkSheet.Cells[2, 10].Address + ":" + xlWorkSheet.Cells[dataGridView1.Rows.Count, 10].Address +", "+quote+ "NO" + quote+", "+ xlWorkSheet.Cells[2, 4].Address +
     ":" + xlWorkSheet.Cells[dataGridView1.Rows.Count, 4].Address + "), "+quote+ "[h]:mm:ss"+ quote+")";

            xlWorkSheet.Cells[dataGridView1.Rows.Count + 4, 4] = "Ukupno predvidjeno vrijeme neuradjenih zadataka";
            xlWorkSheet.Cells[dataGridView1.Rows.Count + 4, 4].Font.Bold = true;

            xlWorkSheet.Columns.AutoFit();

            xlApp.DisplayAlerts = false;

            xlWorkBook.SaveAs("Arhiva/izvjestaj.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();


            MessageBox.Show("Izvjestaj napravljen mozete ga naci u c:\\izvjestaj.xls");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void UpdateData()
        {
            if (comboBox1.Text == String.Empty || comboBox1.Text == "Svi" && (comboBox2.Text == String.Empty || comboBox2.Text == "Nista") && (comboBox3.Text == String.Empty || comboBox3.Text == "Nista"))
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks", con.cn);
                    command.ExecuteNonQuery();

                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE datum_pocetka  >= CAST('"+pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();
                    MessageBox.Show(pocetak.Date.ToString());
                    MessageBox.Show(kraj.Date.ToString());
                }

            }
            else if ((comboBox2.Text == String.Empty || comboBox2.Text == "Nista") && (comboBox3.Text == "Nista" || comboBox3.Text == String.Empty))
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE ime  = '" + comboBox1.Text + "'", con.cn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE ime = '" + comboBox1.Text + "' AND datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();
                   

                }
            }
            
            else if(comboBox2.Text == "Svi" && (comboBox1.Text == String.Empty || comboBox1.Text == "Nista") && (comboBox3.Text == String.Empty || comboBox3.Text == "Nista"))
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks", con.cn);
                    command.ExecuteNonQuery();

                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME);", con.cn);
                    command.ExecuteNonQuery();
                }
            }
            else if(comboBox3.Text == "Svi" && (comboBox1.Text == String.Empty || comboBox1.Text == "Nista") && (comboBox2.Text == String.Empty || comboBox2.Text == "Nista"))
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks", con.cn);
                    command.ExecuteNonQuery();

                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();
                }
            }
            else if (comboBox1.Text == "Nista" && comboBox2.Text == "Nista")
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE broj_naloga  = '" + comboBox3.Text + "'", con.cn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE broj_naloga = '" + comboBox3.Text + "' AND datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();


                }
            }
            else if (comboBox2.Text == "Nista"){
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE ime  = '" + comboBox1.Text + "' AND broj_naloga  = '" + comboBox3.Text + "'", con.cn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE ime = '" + comboBox1.Text + "' AND broj_naloga  = '" + comboBox3.Text + "' AND datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();


                }
            }
            else if (comboBox1.Text == "Nista")
            {
                if (checkBox19.Checked)
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE serijski_broj  = '" + comboBox2.Text + "'", con.cn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new MySqlCommand("SELECT * FROM tasks WHERE serijski_broj = '" + comboBox2.Text + "' AND datum_pocetka  >= CAST('" + pocetak.ToString("yyyy-MM-dd") + "' AS DATETIME) AND  datum_pocetka < CAST('" + kraj.AddDays(1).ToString("yyyy-MM-dd") + "' AS DATETIME); ", con.cn);
                    command.ExecuteNonQuery();


                }
            }
            dataTable = new System.Data.DataTable();
            adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable.DefaultView;
        }

        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(MessageBox.Show("Are You Sure You Want To Delete?", "Delete Record", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].FormattedValue.ToString());
                con.cn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM tasks WHERE id='" + id + "'", con.cn);
                cmd.ExecuteNonQuery();
                UpdateData();
                con.cn.Close();
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Login main = new Login();
            this.Hide();
            main.ShowDialog();
        }


        bool shouldTrigger = true;
       

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void monthCalendar2_DateChanged(object sender, DateRangeEventArgs e)
        {
            pocetak = monthCalendar2.SelectionEnd;
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            kraj = monthCalendar1.SelectionEnd;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Radnici radnici = new Radnici();
            radnici.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;

            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;
            xlWorkSheet.Cells[1, 2] = "Ime zadatka";
            xlWorkSheet.Cells[1, 3] = "Serijski broj";
            xlWorkSheet.Cells[1, 4] = "Planirano vrijeme";
            //xlWorkSheet.Cells[1, 5] = "Ime";
            //xlWorkSheet.Cells[1, 6] = "Vrijeme početka";
            //xlWorkSheet.Cells[1, 7] = "Vrijeme kraja";
            //xlWorkSheet.Cells[1, 8] = "Ukupno vrijeme rada";
            //xlWorkSheet.Cells[1, 10] = "Urađeno";
            //xlWorkSheet.Cells[1, 11] = "Podbačaj";
            //xlWorkSheet.Cells[1, 12] = "Prebačaj";
            xlWorkSheet.Cells[1, 13] = "Datum";
            xlWorkSheet.Cells[1, 14] = "Opis Zadatka";
            //xlWorkSheet.Cells[1, 15] = "Ime Kupca";
            //xlWorkSheet.Cells[1, 16] = "Broj Naloga";
            xlWorkSheet.Cells[1, 17] = "Kolicina";
            xlWorkSheet.Cells[1, 18] = "Bar Kod";
            xlWorkSheet.Cells[1, 19] = "Broj Naloga: ";

            xlWorkSheet.Cells[1, 20] = "Potpis";

           
            xlWorkSheet.Cells[1, 2].Font.Bold = true;
            xlWorkSheet.Cells[1, 3].Font.Bold = true;
            xlWorkSheet.Cells[1, 4].Font.Bold = true;
            xlWorkSheet.Cells[1, 5].Font.Bold = true;
            xlWorkSheet.Cells[1, 6].Font.Bold = true;
            xlWorkSheet.Cells[1, 7].Font.Bold = true;
            xlWorkSheet.Cells[1, 8].Font.Bold = true;
            xlWorkSheet.Cells[1, 10].Font.Bold = true;
            xlWorkSheet.Cells[1, 11].Font.Bold = true;
            xlWorkSheet.Cells[1, 12].Font.Bold = true;
            xlWorkSheet.Cells[1, 13].Font.Bold = true;
            xlWorkSheet.Cells[1, 14].Font.Bold = true;
            xlWorkSheet.Cells[1, 15].Font.Bold = true;
            xlWorkSheet.Cells[1, 16].Font.Bold = true;
            xlWorkSheet.Cells[1, 17].Font.Bold = true;
            xlWorkSheet.Cells[1, 18].Font.Bold = true;
            xlWorkSheet.Cells[1, 19].Font.Bold = true;
            xlWorkSheet.Cells[1, 19].WrapText = true;
            xlWorkSheet.Cells[1, 19].Font.Size =20; 

            xlWorkSheet.Cells[1, 20].Font.Bold = true;

            Range cells = xlWorkSheet.Cells[1, 3];
            Range cell3 = xlWorkSheet.Cells[dataGridView1.Rows.Count, 3];
            Range range5 = xlWorkSheet.get_Range(cells, cell3);
            range5.NumberFormat = "@";
            range5.WrapText = true;

            Range cells2 = xlWorkSheet.Cells[1, 14];
            Range cell33 = xlWorkSheet.Cells[dataGridView1.Rows.Count, 14];
            Range range55 = xlWorkSheet.get_Range(cells2, cell33);
            range55.WrapText = true;

            Range cell1 = xlWorkSheet.Cells[1, 2];
            Range cell2 = xlWorkSheet.Cells[dataGridView1.Rows.Count, dataGridView1.Columns.Count + 2];
            Range range3 = xlWorkSheet.get_Range(cell1, cell2);
            //range3.Cells.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, (XlColorIndex)color, Type.Missing);


            Borders border = range3.Borders;
            border.LineStyle = XlLineStyle.xlContinuous;

            range3.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            range3.Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            int br = 0;
            br = dataGridView1.Rows.Count;
            // storing Each row and column value to excel sheet  
            for (i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (j == 0 || j == 8 || j == 5 || j == 6 || j == 7 || j == 9 || j == 10 || j == 11 || j == 14 || j==4 || j==20 || j==19)
                    {
                       
                        continue;
                    }
                   
                    xlWorkSheet.Cells[br, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    xlWorkSheet.Cells[br, 19].Formula = "=\"(\"&" + xlWorkSheet.Cells[br, 18].Address + "&\")\"";


                    xlWorkSheet.Cells[br, 19].Font.Name = "IDAutomationHC39M Free Version";
                    
                }
                br--;
            }

            xlWorkSheet.Cells[1, 19] = "Broj Naloga: "+ dataGridView1.Rows[0].Cells[15].Value.ToString() + "";


            /*Microsoft.Office.Interop.Excel.Range cel = (Range)xlApp.Cells[1, 5];
            cel.Delete();*/

            Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.get_Range("A" + 2, "A49");
            Microsoft.Office.Interop.Excel.Range entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);

            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("D" + 2, "D49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("F" + 2, "F49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("F" + 2, "F49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("J" + 2, "J49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
            range = xlWorkSheet.get_Range("J" + 2, "J49");
            entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);



            for (int x = 1; x <= dataGridView1.Rows.Count; x++) // this will apply it from col 1 to 10
            {
                if (x == 5) xlWorkSheet.Rows[x].AutoFit();

                xlWorkSheet.Rows[x].RowHeight = 63;

            }


            for (int x = 1; x <= dataGridView1.Columns.Count; x++) // this will apply it from col 1 to 10
            {
                if (x == 3) xlWorkSheet.Columns[x].ColumnWidth = 16;
                else if (x == 8)
                {
                    xlWorkSheet.Columns[x].ColumnWidth = 20;
                    xlWorkSheet.Columns[x].Font.Size = 10;
                    xlWorkSheet.Columns[x].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignBottom;
                }
                else xlWorkSheet.Columns[x].ColumnWidth = 12;



            }

            xlApp.DisplayAlerts = false;

            xlWorkBook.SaveAs("Arhiva/OperacioniList.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();


            MessageBox.Show("Operacioni list napravljen mozete ga naci u c:\\OperacioniList.xls");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
           
           
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
               
                con.cn.Open();
                Refresh3();
                UpdateData();
                con.cn.Close();
                comboBox1.SelectedItem = "Nista";
                comboBox2.SelectedItem = "Nista";
            }
            catch(Exception err)
            {
            }
           

        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            con.cn.Open();
            UpdateData();
            con.cn.Close();
            comboBox1.SelectedItem = "Nista";
            if(comboBox2.SelectedItem == "Svi")
            {
                comboBox3.SelectedItem = "Nista";
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            con.cn.Open();
            UpdateData();
            con.cn.Close();
            comboBox2.SelectedItem = "Nista";
            if (comboBox1.SelectedItem == "Svi")
            {
                comboBox3.SelectedItem = "Nista";

            }
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            EST21.Visible = true;
            DES21.Visible = true;
            DES21.Text = "Nema opisa";

            if (checkBox21.Checked == false)
            {
                EST21.Visible = false;
                DES21.Visible = false;

            }
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            EST23.Visible = true;
            DES23.Visible = true;
            DES23.Text = "Nema opisa";

            if (checkBox23.Checked == false)
            {
                EST23.Visible = false;
                DES23.Visible = false;

            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {

            EST22.Visible = true;
            DES22.Visible = true;
            DES22.Text = "Nema opisa";

            if (checkBox22.Checked == false)
            {
                EST22.Visible = false;
                DES22.Visible = false;

            }
        }
    }
}
