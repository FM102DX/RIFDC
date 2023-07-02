using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RIFDC;
using CommonFunctions;
using MySql.Data.MySqlClient;

namespace CoffeePointsDemo
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainAppFrm());

            CoffeePointsApp.init();

        }

    }
    public static class CoffeePointsApp
    {
        public static IDataRoom mainDataRoom = new DataRoom();

        public static MySqlCluster_MySqlConnectorNET cls_mysql = new MySqlCluster_MySqlConnectorNET();

        public static void init()
        {
            cls_mysql.connectionData.server = "31.31.201.152";
            cls_mysql.connectionData.port = "3306";
            cls_mysql.connectionData.dbName = "rifdcdemo";
            cls_mysql.connectionData.dbUser = "rifdcdemo_user01";
            cls_mysql.connectionData.dbPassword = "aZ3hD2eE8r";

            RIFDC_App.mainDataRoom = mainDataRoom;
            RIFDC_App.currentUserId = "user01";
            
            #region localCnn

            /*
                        cls_mysql.connectionData.server = "127.0.0.1";
                        //cls_mysql.connectionData.port = "3306";
                        cls_mysql.connectionData.dbName = "coffeepoints2";
                        cls_mysql.connectionData.dbUser = "root";
                        cls_mysql.connectionData.dbPassword = "";
                        */

            #endregion


            mainDataRoom.actualCluster = cls_mysql;

            Lib.DbOperationResult or = mainDataRoom.connect();

            if (!or.success)
            {
                ServiceFucntions.mb_info("Ошибка подключения, программа остановлена");
                return;
            }

            //сохраняем Ikeeper этого типа, поскольку тут дженерики ItemKeeper<Class> в рантайме не создашь
            RIFDC_App.iKeeperSampleHolder.registerIKeeper(ItemKeeper<CoffeePoint>.getInstance());

            MainAppFrm frm = new MainAppFrm();
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();

            mainDataRoom.disconnect();
        }


    }
}
