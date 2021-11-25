using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIFDC.DbDrivers.MySql
{
    public class MySqlDataCluster : MySqlClusterPattern, IDataCluster
    {
        public new ConnectionData connectionData { get; set; }
        public override string connectionString
        {
            //здесь для каждого типа сервера бд (aceess, mysql и др. указывается свой способ получения connectionString) 
            get
            {
                string server = Fn.sfn(connectionData.server, "server=", ";");
                string port = Fn.sfn(connectionData.port, "port=", ";");
                string dbName = Fn.sfn(connectionData.dbName, "database=", ";");
                string dbUser = Fn.sfn(connectionData.dbUser, "user=", ";");
                string dbPassword = Fn.sfn(connectionData.dbPassword, "password=", ";");
                string persistSecurityInfo = Fn.sfn(connectionData.persistSecurityInfo, "persist Security Info=", ";");
                string pooling = Fn.sfn(connectionData.pooling, "pooling=", ";");
                string useCompression = Fn.sfn(connectionData.useCompression, "use Compression=", ";");
                string charSet = "CHARSET = utf8;";
                return server + port + dbName + dbUser + dbPassword + persistSecurityInfo + pooling + useCompression + charSet;
            }
        }

        public new string dbCommonTitle
        {
            get { return Fn.sfn(connectionData.server, "server=", ";") + Fn.sfn(connectionData.dbName, "dbName=", ";"); }
        }

        public MySqlCluster_MySqlConnectorNET()
        {
            //TODO громоздко, и надо чтобы, может быть, разрешить только 1 экземпляр
            //ну пока пусть так
            //_filePath = __filePath;
            connectionData = new ConnectionData();
        }


        public new MySqlConnection activeConnection
        {
            get { return activeConnection; }
        }

        public new class ConnectionData
        {
            //сведения о подключении - сервер, имя базы, пароли и др.

            public string server = "";
            public string port = "";
            public string dbName = "";
            public string dbUser = "";
            public string dbPassword = "";
            public string persistSecurityInfo = "";
            public string pooling = "";
            public string useCompression = "";

        }
    }
}
