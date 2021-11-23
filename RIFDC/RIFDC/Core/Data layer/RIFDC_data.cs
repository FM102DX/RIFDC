using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using CommonFunctions;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.MySqlClient;
using ObjectParameterEngine;
using RIFDC;




namespace RIFDC
{
    public class DataRoom : IDataRoom
    {
        //класс, который отвечает за хранение объектов бизнес-слоя и операции с ними
        //объекты бизнес-слоя технически хранятся в датаруме, т.е. объекту ничего не надо знать про то, как именно он будет храниться в базе.
        //таким образом, бизнес-логика абстрагирована от БД

        //по аналогии - это серверная, где стоят кластеры серверов. В одном датаруме может быть только 1 текущий кластер. Кластеры можно менять в рантайме.

        //DataRoom выполняет задачи:

        // 1) хранение объектов типа KeepableClass (чтение, инсерт, апдейт, делит). Он конструирует и выполняет все запросы исходя из fieldsInfo.
        // причем запросы конструируются для каждой БД своим способом, что реализовано по паттерну Стратегия в интерфейсе IDataCluster

        // 2) все виды проверок, связанных с БД - например, проверка существования таблицы под конкретный тип и нужных колонок в ней

        // 3) инкапсулирует все, что связано с БД. Напрмиер, объект Connection.

        IDataCluster _actualCluster;
        public IDataCluster actualCluster
        {
            get
            {
                return _actualCluster;
            }
            set
            {
                _actualCluster = value;
            }
        }

       public  Lib.DbOperationResult deleteFiteredPackege(IKeepable sample, Lib.Filter filter)
        {
            return actualCluster.deleteFiteredPackege(sample, filter);
        }

        public Lib.DbOperationResult saveObject(IKeepable t)
        {
            return _actualCluster.saveObject(t);
        }

        public Lib.DbOperationResult checkObjectTable(IKeepable sampleObject, bool drop = false)
        {
            return _actualCluster.checkObjectTable(sampleObject, drop);
        }

        public List<IUniversalRowDataContainer> makeGroupQuery(IKeeper keeper, Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField)
        {
             return  actualCluster.makeGroupQuery (keeper, groupQueryType, targetRelation, targetField);
        }

        public Lib.DbOperationResult deleteItem(IKeepable t)
        {
            return _actualCluster.deleteItem(t);
        }

        public List<IUniversalRowDataContainer> readItems(IKeepable t, Lib.Filter filter)
        {
            return _actualCluster.readItems(t, filter);
        }

        public Lib.DbOperationResult connect()
        {
            return _actualCluster.connect();
        }
        public void disconnect()
        {
            _actualCluster.disconnect();
        }

        public bool isConnected
        {
            get
            {
                return _actualCluster.isNowConnected;
            }
        }
     }

    public enum StorageType
    {
        MSAccessDatabase = 1,
        MySqlDatabase = 2,
        MSSqlServerDatabase = 3,
        LocalFileSystem = 4,
        ExcelFile = 5

    }


}



