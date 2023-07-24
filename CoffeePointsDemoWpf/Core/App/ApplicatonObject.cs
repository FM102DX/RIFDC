using CoffeePointsDemo;
using CoffeePointsDemo.Service;
using RIFDC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace CoffeePointsDemoWpf
{
    public class ApplicatonObject
    {
        public String BaseDirectory { get => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }

        public String? LogsDirectory
        {
            get
            {
                var directory = System.IO.Path.Combine(BaseDirectory, "Logs");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public IDataRoom MainDataRoom = new DataRoom();

        public MySqlCluster_MySqlConnectorNET MySqlDataCluster = new MySqlCluster_MySqlConnectorNET();

        public String? DataDirectory
        {
            get
            {
                var directory = System.IO.Path.Combine(BaseDirectory, "Data");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public String? OutputDirectory
        {
            get
            {
                var directory = System.IO.Path.Combine(BaseDirectory, "Output");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public CommonOperationResult Connect()
        {
            MySqlDataCluster.connectionData.server = "31.31.201.152";
            MySqlDataCluster.connectionData.port = "3306";
            MySqlDataCluster.connectionData.dbName = "rifdcdemo";
            MySqlDataCluster.connectionData.dbUser = "rifdcdemo_user01";
            MySqlDataCluster.connectionData.dbPassword = "aZ3hD2eE8r";

            RIFDC_App.mainDataRoom = MainDataRoom;

            RIFDC_App.currentUserId = "user01";

            MainDataRoom.actualCluster = MySqlDataCluster;

            Lib.DbOperationResult connectOperationResult;

            string exitFailMsg = $"Error occured while trying to connect to {MySqlDataCluster.connectionData.server}:{MySqlDataCluster.connectionData.port}";

            try
            {
                connectOperationResult = MainDataRoom.connect();
                if (!connectOperationResult.Success)
                {
                    return CommonOperationResult.SayFail($"{exitFailMsg}");
                }
            }
            catch (Exception ex)
            {
                string exitMsg = $"{exitFailMsg}. Error is {ex.Message}";
                return CommonOperationResult.SayFail(exitMsg);
            }

            //saving Ikeeper of this type, because we cant ItemKeeper<Class> at runtime
            RIFDC_App.iKeeperSampleHolder.registerIKeeper(ItemKeeper<CoffeePoint>.getInstance());

            // MainDataRoom.disconnect();
            return CommonOperationResult.SayOk();
        }

    }
}
