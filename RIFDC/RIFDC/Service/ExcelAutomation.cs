using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using CommonFunctions;
using RIFDC;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace RIFDC
{

    //инфраструктура для автоматизации операций с экслеевскими таблицами
    //например, чтобы открыть Excel-файл и прочитать его содержимое
    public class ExcelApp
    {
        //это запущенное приложение Excel
        public Excel.Application excelPhysicalApp;
        public ExcelApp(Excel.Application _excelPhysicalApp)
        {
            excelPhysicalApp = _excelPhysicalApp;
        }
        public Excel.Workbook getOpenedWorkBookByName(string name)
        {
            try
            {
                foreach (Excel.Workbook wb in excelPhysicalApp.Workbooks)
                {
                    if (wb.Name == name) return wb;
                }
            }
            catch
            {

            }
            return null;
        }
        public Excel.Worksheet getOpenedWorkSheetByName(Excel.Workbook _wb, string name)
        {
            try
            {
                foreach (Excel.Worksheet ws in _wb.Worksheets)
                {
                    fn.Dp(ws.Name.ToLower());
                    if (ws.Name.ToLower() == name.ToLower())
                    {
                          return ws;
                    }

                }
            }
            catch
            {

            }
            return null;
        }

        public List<string> getOpenedWorkBooksNames()
        {
            List<string> s = new List<string>();
            try
            {
                foreach (Excel.Workbook wb in excelPhysicalApp.Workbooks)
                {
                    s.Add(wb.Name);
                }
            }
            catch
            {

            }
            return s;
        }

        public bool isSelectionSimple(Excel.Range r)
        {
            //является ли выделение простым, т.е. когда выделена 1 яейка
            int areaCount = r.Areas.Count;
            int rowCount = r.Rows.Count;
            int colCount = r.Columns.Count;
            if (areaCount == 1 && rowCount == 1 && colCount == 1) { return true; } else return false;
        }
        public string getActiveCellText()
        {
            //взять текст того файла того листа той ячейки, где сейчас курсор
            return excelPhysicalApp.ActiveCell.Text;
        }
        public static class addressData
        {
            public static string excelColumnLetter(long iCol)
            {
                // буква колонки по номеру
                long a;
                long b;

                a = iCol;
                string ConvertToLetter = "";

                while (iCol > 0)
                {
                    a = (int)((iCol - 1) / 26);
                    b = (iCol - 1) % 26;
                    ConvertToLetter = ((char)(b + 65)) + ConvertToLetter;
                    iCol = a;
                }
                return ConvertToLetter;
            }
            static long getExelColLetterNumber(char c)
            {
                c = Convert.ToChar(c.ToString().ToUpper());
                return (int)c - 65 + 1;
            }

            public static long getXCoordFromA1TypeAddress(string address)
            {
                //берет X координуту из адреса типа "AB100"
                return getCoordFromA1TypeAddress(address, true);
            }
            public static long getYCoordFromA1TypeAddress(string address)
            {
                //берет X координуту из адреса типа "AB100"
                return getCoordFromA1TypeAddress(address, false);
            }
            static long getCoordFromA1TypeAddress(string address, bool getXCoord)
            {
                char c;
                string s = "";
                long x;
                for (int i = 0; i < address.Length; i++)
                {
                    c = address[i];
                    if (getXCoord)
                    {
                        if (Char.IsNumber(c)) { s += address[i]; }
                    }
                    else
                    {
                        if (Char.IsLetter(c)) { s += address[i]; }
                    }
                }

                if (!getXCoord)
                {
                    x = excelColumnNumberFromLetter(s);
                }
                else
                {
                    try { x = Convert.ToInt64(s); } catch { x = -1; }
                }
                return x;
            }
            public static long excelColumnNumberFromLetter(string columnLetter)
            {
                //номер колонки по буквам
                double sum = 0;
                char c;
                int i, j;

                for (i = 1; i <= columnLetter.Length; i++)
                {
                    j = Math.Abs(columnLetter.Length - i) + 1;
                    c = (char)columnLetter[i - 1];
                    sum = sum + Math.Pow(26, j - 1) * getExelColLetterNumber(c);
                }
                return (long)sum;
            }
        }
        public static ExcelApp getMyInstance()
        {
            Excel.Application excelPhysicalApp = getExcelPhysicalApp();
            if (excelPhysicalApp == null)
            {
                excelPhysicalApp = new Excel.Application();
            }

            ExcelApp excelApp = new ExcelApp(excelPhysicalApp);
            return excelApp;
        }

        private static Excel.Application getExcelPhysicalApp()
        {
            Excel.Application excelPhysicalApp;
            try
            {
                // excelPhysicalApp = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                excelPhysicalApp = (Microsoft.Office.Interop.Excel.Application)Marshal2.GetActiveObject("Excel.Application");
            }
            catch
            {
                excelPhysicalApp = null;
            }
            return excelPhysicalApp;

        }


    }
    
    public class ExcelDataTable
    {
        //таблица, содержащаяся на 1 листе и представляющая собой 1 неразрывный квадратный range c заголовком в 1й строке этого range
        //используется чтобы прочитать эту таблицу в память и всячески ей оперировать, напр, через linq

        //для начаал я хочу ее прочитать из записать

        public ExcelApp excelApp;
        public Excel._Workbook myFile;
        public Excel._Worksheet mySheet;
        public long x1;
        public long x2;
        public long y1;
        public long y2;

        string startAddress, endAddress;

        public static object[,] lines2TableArray(List<ExcelDataTableRow> lines)
        {
            //собирает массив из рядов таблицы

            long rowCount = 0;
            long width = 0;

            if (lines.Count == 0) return null;

            width = lines[0].items.Count;
            rowCount = lines.Count;

            object[,] _dataArray = new object[rowCount, width];
            int i, j;


            //теперь dataRows
            for (i = 0; i < rowCount; i++)
            {
                for (j = 0; j < width; j++)
                {
                    _dataArray[i, j] = lines[i].items[j].value;
                }
            }
            return _dataArray;
        }

        public long dataRangeRowShift = 0; //на случай, если диапазон данных начинается не от headerRow, а через пару строк ниже
        public object[,] tableArray
        {
            get
            {
                //собирает  var arrData = (object[,]) из текущего массива items
                object[,] _dataArray = new object[1 + dataRangeRowShift + dataRows.Count, headerRow.items.Count];
                int i, j;

                //сначала haederRow
                for (j = 0; j < headerRow.items.Count; j++)
                {
                    _dataArray[0, j] = headerRow.items[j].value;
                }

                //теперь dataRows
                for (i = 0; i < dataRows.Count; i++)
                {
                    for (j = 0; j < dataRows[i].items.Count; j++)
                    {
                        _dataArray[i + 1 + dataRangeRowShift, j] = dataRows[i].items[j].value;
                    }
                }
                return _dataArray;
            }
        }

        public object[,] dataArray
        {
            get
            {
                return lines2TableArray(dataRows);
            }
        }

        public string status { get { return (string.Format("Table rows: {0} cols: {1}", rowCount, columnCount)); } }

        public long rowCount { get { return (x2 - x1 + 1); } }
        public long dataRowCount { get { return (x2 - x1 + 1 - 1 - dataRangeRowShift); } }
        public long columnCount { get { return (y2 - y1 + 1); } }

        public long rowCount_1 { get { return (1 + dataRangeRowShift + dataRows.Count); } }
        public long dataRowCount_1 { get { return (dataRows.Count); } }
        public long columnCount_1 { get { return (headerRow.items.Count); } }


        public ExcelDataTableRow headerRow;

        public List<ExcelDataTableRow> dataRows = new List<ExcelDataTableRow>();

        public ExcelDataTable(Excel._Worksheet _mySheet, string _startAddress, string _endAddress)
        {
            mySheet = _mySheet;
            headerRow = new ExcelDataTableRow(this);
            startAddress = _startAddress;
            endAddress = _endAddress;
            recalcXYCoord();
        }

        void recalcXYCoord()
        {
            x1 = ExcelApp.addressData.getXCoordFromA1TypeAddress(startAddress);
            x2 = ExcelApp.addressData.getXCoordFromA1TypeAddress(endAddress);
            y1 = ExcelApp.addressData.getYCoordFromA1TypeAddress(startAddress);
            y2 = ExcelApp.addressData.getYCoordFromA1TypeAddress(endAddress);
        }

        public Excel.Range myRange
        {
            get
            {
                // return mySheet.get_Range(mySheet.Cells[rangeBegin.row, rangeBegin.column], mySheet.Cells[rangeEnd.row, rangeEnd.column]);
                return mySheet.get_Range(startAddress, endAddress);
            }
        }
        public Excel.Range myDataRange
        {
            get
            {
                // return mySheet.get_Range(mySheet.Cells[rangeBegin.row, rangeBegin.column], mySheet.Cells[rangeEnd.row, rangeEnd.column]);
                return mySheet.get_Range(ExcelApp.addressData.excelColumnLetter(y1) + (x1 + 1 + dataRangeRowShift).ToString(), endAddress);
            }
        }


        public void clearTheTable()
        {
            headerRow.clear();
            //foreach (ExcelDataTableRow r in dataRows) { r.clear(); }
            dataRows.Clear();
        }

        public void readTheTableFromExcelSheet()
        {

            ExcelDataTableRow rw;


            //обнулить эту таблицу и прочитать ее снова
            clearTheTable();


            var arrData = (object[,])mySheet.Range[startAddress + ":" + endAddress].Value;
            // mySheet.Range[startAddress + ":" + endAddress].Value = arrData;



            //сначала читаем hearer row
            for (long i = 1; i <= columnCount; i++)
            {
                //r = (Excel.Range)mySheet.Cells[1, i];
                ExcelDataTableCell es = new ExcelDataTableCell();
                es.value = arrData[1, i];
                headerRow.items.Add(es);
            }

            long arrLen = rowCount;
            for (long currentRow = 2 + dataRangeRowShift; currentRow <= arrLen; currentRow++)
            {
                rw = new ExcelDataTableRow(this);
                for (long currentCol = 1; currentCol <= columnCount; currentCol++)
                {
                    //r = (Excel.Range)mySheet.Cells[currentRow, currentCol];

                    ExcelDataTableCell es = new ExcelDataTableCell();

                    //es.value = rData.Cells[currentRow, currentCol];

                    es.value = arrData[currentRow, currentCol];
                    rw.items.Add(es);
                }
                dataRows.Add(rw);
            }
        }

        public ExcelDataTableRow createRow() { return new ExcelDataTableRow(this); }

        public class ExcelDataTableRow
        {
            public ExcelDataTable parent;
            public List<ExcelDataTableCell> items = new List<ExcelDataTableCell>();
            public long rowNo; //номер строки таблицы, где находится запись
            public ExcelDataTableRow(ExcelDataTable _parent)
            {
                parent = _parent;
            }
            public void clear()
            {
                items = new List<ExcelDataTableCell>();
            }

        }


        public class ExcelDataTableCell
        {
            //ячейка таблицы. это не то же самое, что excelapp.cell, т.к. тот объект тяжелее
            public object value;

        }

        public void makeTableDump()
        {
            // делает дамп таблицы в дебаг
            object[,] _dataArray = tableArray;
            int i, j;
            string s;
            for (i = 0; i < rowCount_1; i++)
            {
                s = "";
                for (j = 0; j < columnCount_1; j++)
                {
                    s += fn.ConvertObjectToString(_dataArray[i, j]);
                }
                fn.Dp(s);
            }
        }

    }

    public class ExcelOpenedFileSelector
    {
        //интерфейс выбора открытого эксель-файла и рабочего листа в нем
        ExcelApp excelApp;
        public ComboBox MyWbCbx;
        public ComboBox MyWsCbx;
        public TextBox tbStart;
        public TextBox tbEnd;
        public TextBox drrShiftTb;
        public Label notifyLabel;
        public Button prepareFileButton;
        public Button reloadButton;

        public Excel.Workbook myWorkBook
        {
            get
            {
                if (!setExcelApp()) return null;
                return excelApp.getOpenedWorkBookByName(myWorkBookName);
            }
        }

        public Excel.Worksheet myWorkSheet
        {
            get
            {
                if (!setExcelApp()) return null;
                return excelApp.getOpenedWorkSheetByName(myWorkBook, myWorkSheetName);
            }
        }

        //public string myFormulaName { get { return "[" + myWorkBook.Name + "]" + myWorkSheet.Name; ; } }

        //public string name { get { getMyWorkBook(); if (myWorkBook == null) return ""; else return myWorkBook.Name; } }


        public OpenExcelFile getTheFile ()
        {
            OpenExcelFile of = new OpenExcelFile(myWorkBookName, myWorkSheetName, startAddress, endAddress, dataRangeRowShift);
            of.open();
            if (of.isOpen) return of; else return null;
        }
        public string status { set { notifyLabel.Text = value; } }
        public string startAddress { get { return tbStart.Text; } }
        public string endAddress { get { return tbEnd.Text; } }
        public string myWorkBookName { get { return MyWbCbx.Text; } }
        public string myWorkSheetName { get { return MyWsCbx.Text; } }
        public new long dataRangeRowShift
        {
            get
            {
                long l = 0;
                try
                {
                    l = Convert.ToInt64(drrShiftTb.Text);
                }
                catch
                {

                }
                return l;
            }
        }
        public ExcelOpenedFileSelector(ComboBox _MyWbCbx, ComboBox _MyWsCbx, TextBox _tbStart, TextBox _tbEnd, Button _reloadButton, Label _notifyLabel, TextBox _drrShiftTb)
        {
            MyWbCbx = _MyWbCbx;
            MyWsCbx = _MyWsCbx;
            tbStart = _tbStart;
            tbEnd = _tbEnd;
            notifyLabel = _notifyLabel;
            reloadButton = _reloadButton;
            drrShiftTb = _drrShiftTb;
            MyWbCbx.SelectedValueChanged += new EventHandler(_process_workBookSelected);
            MyWsCbx.SelectedValueChanged += new EventHandler(_process_workSheetSelected);
            reloadButton.Click += new EventHandler(_process_reload);
        }

        public void _process_reload(object sender, EventArgs e)
        {
            reFillControls();
        }
        public bool setExcelApp()
        {
            if (excelApp == null)
            {
                excelApp = ExcelApp.getMyInstance();
                if (excelApp == null)
                {
                    status = "Приложение Excel не запущено";
                    return false;
                }
            }
            return true;
        }
        public void reFillControls()
        {
            if (!setExcelApp()) return;
            //перезалить имена книг
            resetControls();
            fillWbNames();
        }

        public void _process_workBookSelected(object sender, EventArgs e)
        {
            if (!setExcelApp()) return;
            //myWorkBookName = MyWbCbx.Text;
            fillWsNames();
        }
        public void _process_workSheetSelected(object sender, EventArgs e)
        {
            //myWorkSheetName = MyWsCbx.Text;
        }
        public void fillWbNames()
        {
            MyWbCbx.Items.Clear();

            foreach (string s in excelApp.getOpenedWorkBooksNames())
            {
                MyWbCbx.Items.Add(s);
            }
        }
        public void fillWsNames()
        {

            Excel.Workbook wb = myWorkBook;
            if (wb==null)
            {
                status = "ERROR: workbook is not open";
            }
            MyWsCbx.Items.Clear();

            foreach (Excel.Worksheet ws in myWorkBook.Worksheets)
            {
                MyWsCbx.Items.Add(ws.Name);
            }
        }
        public void resetControls()
        {
            MyWbCbx.Items.Clear();
            MyWsCbx.Items.Clear();
            MyWbCbx.Text = "";
            MyWsCbx.Text = "";
            tbStart.Text = "";
            tbEnd.Text = "";
        }
    }

    public class OpenExcelFile : ExcelFile
    {
        //файл, который уже открыт в приложении excel, его просто надо найти среди открытых по _myWorkBookName 
        //
        public OpenExcelFile(string _myWorkBookName, string _myWorkSheetName, string _startAddress, string _endAddress, long _dataRangeRowShift, ExcelApp _excelApp = null) : base("", _myWorkBookName, _myWorkSheetName, _startAddress, _endAddress, _dataRangeRowShift, _excelApp)
        {

        }

        //закрытый файл, который лежит на диске и его надо найти, открыть, прочитать, закрыть

        public override Lib.CommonOperationResult open()
        {
            Lib.CommonOperationResult r = new Lib.CommonOperationResult();
            r.success = false;

            excelApp = ExcelApp.getMyInstance();
            if (excelApp == null) { r.msg = "Приложение Excel не запущено"; return r; }
            if (myWorkBook == null) { r.msg = "WorkBook not found: name = " + myWorkBookName; return r; }
            if (myWorkSheet == null) { r.msg = "WorkSheet not found: name = " + myWorkSheetName; return r; }

            r.success = true;
            return r;


        }

    }

    public class StaticExcelFile : ExcelFile
    {
        //закрытый файл, который лежит на диске и его надо найти, открыть, прочитать, закрыть

        private Excel.Application _excelPhysicalAppCurrentInctance;
        private Excel.Workbook _excelOpenedWorkBookCurrentInctance;
        bool useExistingExcelAppInstance;
        bool closeExelWhenJobDone;

        public StaticExcelFile(string _filePath, string _myWorkBookName, string _myWorkSheetName, string _startAddress, string _endAddress, long _dataRangeRowShift, bool _useExistingExcelAppInstance = false, bool _closeExelWhenJobDone = true) : base(_filePath, _myWorkBookName, _myWorkSheetName, _startAddress, _endAddress, _dataRangeRowShift)
        {
            useExistingExcelAppInstance= _useExistingExcelAppInstance;
            closeExelWhenJobDone= _closeExelWhenJobDone;
        }

        public override Lib.CommonOperationResult open()
        {
            Lib.CommonOperationResult r = new Lib.CommonOperationResult();
            try
            {
                //возможность использовать уже открытый инстанс экселя
                if (useExistingExcelAppInstance)
                {
                    excelApp = ExcelApp.getMyInstance();
                    _excelPhysicalAppCurrentInctance = excelApp.excelPhysicalApp;
                }
                else
                {
                    _excelPhysicalAppCurrentInctance = new Excel.Application(); //открыть эксель
                    excelApp = new ExcelApp(_excelPhysicalAppCurrentInctance);
                }
               // _excelPhysicalAppCurrentInctance.Visible = true;
                _excelOpenedWorkBookCurrentInctance = _excelPhysicalAppCurrentInctance.Workbooks.Open(@"" + filePath + myWorkBookName);
                r.success = true;
            }
            catch (Exception e)
            {
                r.success = false;
                r.msg = e.Message;
            }
            return r;
        }
        public override Lib.CommonOperationResult close()
        {
            Lib.CommonOperationResult r = new Lib.CommonOperationResult();

            try
            {

                _excelOpenedWorkBookCurrentInctance.Close(false);
                _excelOpenedWorkBookCurrentInctance = null;


                if (closeExelWhenJobDone)
                {
                    _excelPhysicalAppCurrentInctance.Quit();
                    _excelPhysicalAppCurrentInctance = null;
                    excelApp = null;
                }
               
            }
            catch (Exception e)
            {
                r.success = false;
                r.msg = e.Message;
            }
            r.success = true;
            return r;
        }

    }

    public abstract class ExcelFile : IExcelFile
    {
        //эксельный файл
        public string myWorkBookName;
        public string myWorkSheetName;
        public long dataRangeRowShift { get; }
        public string startAddress { get; }
        public string endAddress { get; }

        
        public ExcelApp excelApp;
        public string filePath { get; set; }
        public Excel.Workbook myWorkBook
        {
            get
            {
                return excelApp.getOpenedWorkBookByName(myWorkBookName);
            }
        }

        public Excel.Worksheet myWorkSheet
        {
            get
            {
                return excelApp.getOpenedWorkSheetByName(myWorkBook, myWorkSheetName);
            }
        }
        public ExcelFile(string _filePath, string _myWorkBookName, string _myWorkSheetName, string _startAddress, string _endAddress, long _dataRangeRowShift, ExcelApp _excelApp = null)
        {
            excelApp = _excelApp;
            //т.е. это вот не обязательно, тут может быть ситуация а) когда создаешь файл и надо его открыть б) когда уже есть открытый эксельапп
            //если эксельапп не передан, он просто создается под каждое нужное действие

            filePath = _filePath;
            myWorkBookName = _myWorkBookName;
            myWorkSheetName = _myWorkSheetName;
            startAddress = _startAddress;
            endAddress = _endAddress;
            dataRangeRowShift = _dataRangeRowShift;
        }


        public virtual Lib.CommonOperationResult open()
        {
            return null;
        }
        public virtual Lib.CommonOperationResult close()
        {
            return null;
        }
        public virtual Lib.CommonOperationResult save()
        {
            try
            {
                myWorkBook.Save();
            }
            catch (Exception e)
            {
                fn.Dp("ERROR EXCEL " + e.Message);
            }
            return null;

        }

        public bool isOpen
        {
            get
            {
                //считаем, что если книга не null, то значит файл открыт
                Excel.Worksheet ws = myWorkSheet;
                if (ws == null) return false; else return true;
            }
        }



    }

    public static class Marshal2
    {
        // взято тут, не тестировано
        // https://ru.stackoverflow.com/questions/1224211/Подключения-к-запущенному-com-объекту-в-net-5

        internal const String OLEAUT32 = "oleaut32.dll";
        internal const String OLE32 = "ole32.dll";

        [System.Security.SecurityCritical]  // auto-generated_required
        public static Object GetActiveObject(String progID)
        {
            Object obj = null;
            Guid clsid;

            // Call CLSIDFromProgIDEx first then fall back on CLSIDFromProgID if
            // CLSIDFromProgIDEx doesn't exist.
            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            //            catch
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, IntPtr.Zero, out obj);
            return obj;
        }

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport(OLEAUT32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out Object ppunk);

    }
}
