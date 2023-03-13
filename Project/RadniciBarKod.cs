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

namespace Project
{
    public partial class RadniciBarKod : Form
    {
        myDatabase con = new myDatabase();
        public string neezDuts { get; set; }
        public RadniciBarKod()
        {
            InitializeComponent();
            con.Connect();
        }

        private void sextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //con.cn.Open();
                using (MySqlConnection connection2 = new MySqlConnection("Datasource = 0.0.0.0;username=Remote;password=; database=project"))
                {
                    connection2.Open();
                    MySqlCommand cmd2 = new MySqlCommand("SELECT Ime FROM users WHERE BarCode = '" + sextBox1.Text + "'", connection2);
                    MySqlDataReader sdr3 = cmd2.ExecuteReader();
                    while (sdr3.Read())
                    {
                        sextBox1.Text = sdr3["Ime"].ToString();
                        sdr3.Close();
                        using (MySqlConnection connection3 = new MySqlConnection("Datasource = 0.0.0.0;username=Remote;password=; database=project"))
                        {
                            connection3.Open();
                            MySqlCommand cmd = new MySqlCommand("UPDATE tasks SET ime = '" + sextBox1.Text + "' WHERE BarKod ='" + neezDuts + "'", connection3);
                            cmd.ExecuteNonQuery();
                            this.Hide();
                            connection3.Close();
                        }
                    }
                    connection2.Close();
                }


                //con.cn.Close();
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                con.cn.Close();
            }
        }
    }
}
