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
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace RIFDC
{
    /*
     * здесь драйвер для эксель файлов. 
     * 
     * отдельно будет жить дравер для открытых файлов, отдельно - для закрытых, т.е. 2 разных драйвера.
     * 
     * входные данные:
     * 
     * [расположение файла]
     * имя файла(workbook.name)
     * имя листа (worksheet.name)
     * range begin + range end , например "A1:W100"
     * 
     * поддерживаются операции:
     * 
     * 1) чтение из файла - читается сразу массивом вся таблица
     * 2) запись в режиме rewrite, когда стираются все данные и пишутся заново
     * 3) запись в режиме append,  когда данные просто добавляются в конец таблицы
     * 
     * 
     * */

    public enum ExcelReaderTypeEnum
    {
        Flat = 1,
        StandardTree = 2
    }

    public interface IExcelDataReader
    {
        //это класс, который непосредственно читает лист экселя и превращает его в объекты
        //просто это можно делать очень по разному
        List<IUniversalRowDataContainer> readItems(IExcelFile myFile, IKeepable t, Lib.Filter filter);
    }

    public interface IExcelDataTable
    {
        //общий интерфейс для открытого эксель файла, подается на вход ORM, которая будет читать / писать данные
        object[,] dataArray { get; }
    }

    public interface IExcelFile
    {
        //Excel файл, который можно открывать/закрывать
        string filePath { get; }
        Excel.Workbook myWorkBook { get; }
        Excel.Worksheet myWorkSheet { get; }

        long dataRangeRowShift { get; }
        string startAddress { get; }
        string endAddress { get; }
        string tag { get; set; }
        Lib.CommonOperationResult open();
        Lib.CommonOperationResult close();

        Lib.CommonOperationResult save();
        bool isOpen { get; }
        ExcelApp excelApp { get; }


    }

    public class ExcelStaticFilesBatchProcessor
    {

        IDataRoom dataRoom01;
        IDataCluster myDataCluster;
        IKeeper myKeeper;

        //класс обработчик статик файло в пакете
        List<BatchProcessorUnit> items = new List<BatchProcessorUnit>();

        //контролы
        System.Windows.Forms.ProgressBar myProgressBar;
        System.Windows.Forms.Label myStatusLabel;



        string status
        {
            set
            {
                myStatusLabel.Text = value;
            }
        }

        int progress
        {
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                myProgressBar.Value = value;
            }
        }

        List<long> timeArr = new List<long>(); //время обработки каждого файла

        public ExcelStaticFilesBatchProcessor(IKeeper _myKeeper, System.Windows.Forms.ProgressBar _myProgressBar, System.Windows.Forms.Label _myStatusLabel)
        {
            myKeeper = _myKeeper;
            myProgressBar = _myProgressBar;
            myStatusLabel = _myStatusLabel;

        }

        public void run()
        {
            Stopwatch stopwatch = new Stopwatch();

            dataRoom01 = myKeeper.dataRoom;

            bool useSameExcel;
            bool closeAppAfterUse;
            int count = 0;
            double _progress;

            foreach (BatchProcessorUnit bu in items)
            {
                stopwatch.Start();
                useSameExcel = (count != 0); //первый открывает инстанс экселя, остальные его используют, последний закрывает
                closeAppAfterUse = (count == items.Count - 1);

                myDataCluster = new ExcelStaticFileCluster(@"" + bu.path, @"" + bu.wbName, @"" + bu.wsName, bu.startAddRess, bu.endAddRess, bu.drrShift, useSameExcel, closeAppAfterUse);
                dataRoom01.actualCluster = myDataCluster;
                dataRoom01.connect();
                if (dataRoom01.isConnected)
                {
                    myKeeper.readItems(true);
                    dataRoom01.disconnect();
                }

                stopwatch.Stop();
                timeArr.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                count += 1;

                status = string.Format("Processed files {0} of {1}", count, items.Count);
                _progress = (100 / items.Count) * count;
                //progress = Math.Round(_progress, );
                progress = Convert.ToInt32(_progress);
            }

            status = fn.list2str(timeArr.Cast<Object>().ToList());
        }
        public void addBatchProcessorUnit(string _path, string _wbName, string _wsName, string _startAddRess, string _endAddRess, long _drrShift)
        {
            BatchProcessorUnit bu = new BatchProcessorUnit();
            bu.path = _path;
            bu.wbName = _wbName;
            bu.wsName = _wsName;
            bu.startAddRess = _startAddRess;
            bu.endAddRess = _endAddRess;
            bu.drrShift = _drrShift;
            items.Add(bu);
        }


        class BatchProcessorUnit
        {
            public string path = "";
            public string wbName = "";
            public string wsName = "";
            public string startAddRess = "";
            public string endAddRess = "";
            public long drrShift = 0;
        }
    }

    public class ExcelOpenedFileCluster : ExcelClusterPattern, IDataCluster
    {
        //это файл эксель, который открыт в приложении
        public ExcelOpenedFileCluster(string _wbName, string _wsName, string _beginAddress, string _endAddress, long _dataRangeRowShift, IExcelDataReader _customReader = null) : base("", _wbName, _wsName, _beginAddress, _endAddress, _dataRangeRowShift)
        {
            //здесь base пустой
            myFile = new OpenExcelFile(_wbName, _wsName, _beginAddress, _endAddress, _dataRangeRowShift);
        }
        public ExcelOpenedFileCluster(IExcelFile _myFile, IExcelDataReader _customReader = null) : base(_myFile)
        {

        }

    }

    public class ExcelStaticFileCluster : ExcelClusterPattern, IDataCluster
    {
        //это файл эксель, который уже открыт в приложении, и нам надо просто из него что-то взять
        public ExcelStaticFileCluster(string _filePath, string _wbName, string _wsName, string _beginAddress, string _endAddress, long _dataRangeRowShift, bool _useExistingExcelAppInstance = false, bool _closeExelWhenJobDone = true) : base(_filePath, _wbName, _wsName, _beginAddress, _endAddress, _dataRangeRowShift)
        {
            //здесь base пустой
            myFile = new StaticExcelFile(_filePath, _wbName, _wsName, _beginAddress, _endAddress, _dataRangeRowShift, _useExistingExcelAppInstance, _closeExelWhenJobDone);
        }
        public ExcelStaticFileCluster(IExcelFile _myFile, IExcelDataReader _customReader = null) : base(_myFile, _customReader)
        {

        }

    }

    public abstract class ExcelClusterPattern : IDataCluster
    {
        //это класс-паттерн для Excel кластеров, т.к. есть разные способы подключения

        public ExcelApp excelApp;
        public IExcelFile myFile;
        public string filePath;
        
        IExcelDataReader customReader;

        public MySqlConnection activeConnection;

        public Lib.DbOperationResult deleteFiteredPackege(IKeepable sample, Lib.Filter filter)
        {
            return null;
        }
        public List<IUniversalRowDataContainer> makeGroupQuery(IKeeper keeper, Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField)
        {
            return null;
        }

        public StorageType storageType
        {
            get
            {
                return StorageType.ExcelFile;
            }
        }

        public string dbCommonTitle //virtual
        {
            get { return ""; }
        }

        public ExcelClusterPattern(IExcelFile _myFile, IExcelDataReader _customReader=null)
        {
            myFile = _myFile;
            customReader = _customReader;
        }
        public ExcelClusterPattern(string _filePath, string _wbName, string _wsName, string _beginAddress, string _endAddress, long _dataRangeRowShift)
        {
            //myFile = new ExcelFile(null, _filePath, _wbName, _wsName, _beginAddress, _endAddress, _dataRangeRowShift);
        }

        public bool isNowConnected { get { return myFile.isOpen; } }

        public Lib.DbOperationResult connect()
        {
            Lib.DbOperationResult cr = new Lib.DbOperationResult();
            bool success = false;
            try
            {
                Lib.CommonOperationResult _tmp = myFile.open();
                excelApp = myFile.excelApp;
                cr.msg = _tmp.msg;
                success = _tmp.success;
            }
            catch (Exception e)
            {
                success = false;
                cr.msg = e.Message;
            }

            cr.success = success;
            return cr;
        }

        public Lib.DbOperationResult reconnect()
        {
            return null;
        }

        public void disconnect()
        {
            myFile.close();
        }

        public Lib.DbOperationResult checkObjectTable(IKeepable t, bool drop=false)
        {
            return Lib.DbOperationResult.sayOk();
        }


        public Lib.DbOperationResult deleteItem(IKeepable t)
        {
            //по отдельности объекты не удаляется
            return Lib.DbOperationResult.sayOk();
        }

        public Lib.DbOperationResult deleteItemById(IKeepable sample, string id)
        {
            //по отдельности объекты не удаляется
            return null;
        }

        public Lib.DbOperationResult saveObject(IKeepable t)
        {
            //по отдельности объекты не сохраняются
            return Lib.DbOperationResult.sayOk();
        }

        public List<IUniversalRowDataContainer> readItems(IKeepable t, Lib.Filter filter)
        {
            //читает множество объектов Т из базы

            IExcelDataReader reader=null;

            //здесь реализуем паттерн Мост, т.к. у нас есть статик и динамик файлы, и одновременно они могут быть плоскими и древовидными, 
            //причем древовидные можно еще и читать по разому

            if (customReader != null)
            {
                reader = customReader;
            }
            else
            {
                reader = new ExcelFlatDataReader();
            }
            if (reader == null) return null;

            return reader.readItems(myFile, t, filter);
        }

    }

    public class ExcelFlatDataReader : IExcelDataReader
    {
        //для чтения плоских Excel таблиц в объекты, т.е. 1 строка = 1 объект

        public List<IUniversalRowDataContainer> readItems(IExcelFile myFile, IKeepable t, Lib.Filter filter)
        {

            if (!myFile.isOpen) return null;

            List<IUniversalRowDataContainer> rez = new List<IUniversalRowDataContainer>();
            Lib.UniversalDataKeeper d;
            object tmp = null;
            ExcelDataTable dataTable;
            object[,] dataArr;
            long len;
            long i;

            dataTable = new ExcelDataTable(myFile.myWorkSheet, myFile.startAddress, myFile.endAddress);
            dataTable.dataRangeRowShift = myFile.dataRangeRowShift;

            dataTable.readTheTableFromExcelSheet();
            dataArr = dataTable.dataArray;
            len = dataTable.dataRows.Count;

            for (i = 0; i < len; i++)
            {
                //перебираем по порядку поля, кот. надо присвоить
                d = new Lib.UniversalDataKeeper(); //это строка
                foreach (Lib.FieldInfo f in t.fieldsInfo.fields) // это метаполя, котоыре надо присвоить
                {
                    if (f.excelFileBoundColumnNumber > 0) //это 1-based
                    {
                        tmp = dataArr[i, f.excelFileBoundColumnNumber - 1];
                        d.addNewElement(f.fieldDbName, tmp);
                    }
                }
                rez.Add(d);
            }
            return rez;
        }



    }

    public class ExcelTreeViewBasedDataReader : IExcelDataReader
    {
        //для чтения древовидных таблиц, где в каждой строке присутствует вся иерархия объектов

        public List<IUniversalRowDataContainer> readItems(IExcelFile myFile, IKeepable t, Lib.Filter filter)
        {

            if (!myFile.isOpen) return null;

            List<IUniversalRowDataContainer> rez = new List<IUniversalRowDataContainer>();
            Lib.UniversalDataKeeper d;
            object tmp = null;
            ExcelDataTable dataTable;
            object[,] dataArr;
            long len;
            long i;

            dataTable = new ExcelDataTable(myFile.myWorkSheet, myFile.startAddress, myFile.endAddress);
            dataTable.dataRangeRowShift = myFile.dataRangeRowShift;

            dataTable.readTheTableFromExcelSheet();
            dataArr = dataTable.dataArray;
            len = dataTable.dataRows.Count;

            for (i = 0; i < len; i++)
            {
                //перебираем по порядку поля, кот. надо присвоить
                d = new Lib.UniversalDataKeeper(); //это строка
                foreach (Lib.FieldInfo f in t.fieldsInfo.fields) // это метаполя, котоыре надо присвоить
                {
                    if (f.excelFileBoundColumnNumber > 0) //это 1-based
                    {
                        tmp = dataArr[i, f.excelFileBoundColumnNumber - 1];
                        d.addNewElement(f.fieldDbName, tmp);
                    }
                }
                rez.Add(d);
            }
            return rez;
        }



    }
}
