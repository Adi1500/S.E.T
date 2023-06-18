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
            cn = new MySqlConnection("Datasource =  192.168.0.20;username=Remote;password=admin; database=project;Convert Zero Datetime=True");

        }
    }
}
