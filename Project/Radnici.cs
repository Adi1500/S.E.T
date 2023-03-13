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

namespace Project
{
    public partial class Radnici : Form
    {
        myDatabase con = new myDatabase();
        MySqlCommand command;
        MySqlDataAdapter adapter;
        System.Data.DataTable dataTable;

        public Radnici()
        {
            InitializeComponent();
            con.Connect();

        }

        private void Radnici_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                Random random = new Random();
                int barCode = random.Next(10000000, 99999999);
                con.cn.Open();
                MySqlCommand command = new MySqlCommand("INSERT INTO users (Ime, BarCode) VALUES('" + textBox1.Text + "', '" + barCode + "')", con.cn);
                command.ExecuteNonQuery();
                textBox1.Text = String.Empty;

                
                con.cn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                con.cn.Open();
                UpdateData();
                con.cn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MessageBox.Show("Are You Sure You Want To Delete?", "Delete Record", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].FormattedValue.ToString());
                con.cn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM users WHERE id='" + id + "'", con.cn);
                cmd.ExecuteNonQuery();
                UpdateData();
                con.cn.Close();
            }
        }

        private void UpdateData()
        {
            command = new MySqlCommand("SELECT * FROM users", con.cn);
            command.ExecuteNonQuery();
            dataTable = new System.Data.DataTable();
            adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable.DefaultView;
        }

        private void Radnici_FormClosed(object sender, FormClosedEventArgs e)
        {
            
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
            xlWorkSheet.Cells[1, 2] = "Ime radnika";
            xlWorkSheet.Cells[1, 3] = "Bar Kod";

            xlWorkSheet.Cells[1, 2].Font.Bold = true;
            xlWorkSheet.Cells[1, 3].Font.Bold = true;

            Range cell1 = xlWorkSheet.Cells[1, 2];
            Range cell2 = xlWorkSheet.Cells[dataGridView1.Rows.Count, dataGridView1.Columns.Count + 1];
            Range range3 = xlWorkSheet.get_Range(cell1, cell2);
            //range3.Cells.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, (XlColorIndex)color, Type.Missing);


            Borders border = range3.Borders;
            border.LineStyle = XlLineStyle.xlContinuous;

            range3.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            range3.Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            // storing Each row and column value to excel sheet  
            for (i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {

                    xlWorkSheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    xlWorkSheet.Cells[i + 2, 4].Formula = "=\"(\"&" + xlWorkSheet.Cells[i + 2, 3].Address + "&\")\"";


                    xlWorkSheet.Cells[i + 2, 4].Font.Name = "IDAutomationHC39M Free Version";
                }
            }



            /*Microsoft.Office.Interop.Excel.Range cel = (Range)xlApp.Cells[1, 5];
            cel.Delete();*/

            Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.get_Range("A" + 2, "A49");
            Microsoft.Office.Interop.Excel.Range entireRow = range.EntireColumn;
            entireRow.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);

            



            for (int x = 1; x <= dataGridView1.Rows.Count; x++) // this will apply it from col 1 to 10
            {


                xlWorkSheet.Rows[x].RowHeight = 70;

            }


            for (int x = 1; x <= dataGridView1.Columns.Count; x++) // this will apply it from col 1 to 10
            {



                    xlWorkSheet.Columns[x].ColumnWidth = 30;
                    xlWorkSheet.Columns[x].Font.Size = 11;
                    xlWorkSheet.Columns[x].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignBottom;

    



            }

            xlApp.DisplayAlerts = false;

            xlWorkBook.SaveAs("Arhiva/Radnici.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();


            MessageBox.Show("Bar kodove radnika mozete naci u c:\\Radnici.xls");
        }
    }
}
