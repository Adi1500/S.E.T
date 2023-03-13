using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Project
{
    class myDatabase
    {
        public MySqlConnection cn;
        public void Connect()
        {
            cn = new MySqlConnection("Datasource =  0.0.0.0;username=Remote;password=; database=project;Convert Zero Datetime=True");

        }
    }
}
