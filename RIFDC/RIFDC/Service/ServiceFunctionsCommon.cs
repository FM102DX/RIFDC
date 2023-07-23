using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace RIFDC.RIFDC.Service
{

    public static class Fn
    {

        public static string GetRandomWord(string[] input)
        {
            if (input.Length == 0) return "";

            Random random = new Random();
            int no = random.Next(0, input.Length + 1);
            if (no > input.Length - 1) no = input.Length - 1;
            return input[no];
        }

        public static double GetRandomDouble(double min, double max)
        {
            Random random = new Random();
            double no = min + random.NextDouble() * (max - min);
            return no;
        }

        public class CommonOperationResult
        {
            //результат, возвращаемый после операций в объектном слое
            public bool success;
            public string msg;
            public object returningValue;

            public static CommonOperationResult GetInstance(bool _success, string _msg, object _returningValue = null)
            {
                CommonOperationResult c = new CommonOperationResult();
                c.success = _success;
                c.msg = _msg;
                c.returningValue = _returningValue;
                return c;
            }

            public static CommonOperationResult ReturnValue(object _returningValue = null) { return GetInstance(true, "", _returningValue); }
            public static CommonOperationResult SayFail(string _msg = "") { return GetInstance(false, _msg, null); }
            public static CommonOperationResult SayOk(string _msg = "") { return GetInstance(true, _msg, null); }
            public static CommonOperationResult SayItsNull(string _msg = "") { return GetInstance(true, _msg, null); }
        }

        public static string SubstrBeginsFromLtrNo(string source, int begin)
        {
            //чтобы уж точно не было ошибок 
            string s = "";
            try
            {
                s = source.Substring(begin);
            }
            catch
            {
                s = "";
            }
            return s;
        }
        public class FilePathAnalyzer
        {
            string source = "";

            public FilePathAnalyzer(string _source)
            {
                source = _source;
            }

            private int FileNameLength
            {
                get
                {
                    return source.Length - SepIndex - 1;
                }
            }
            private int FilePathLength
            {
                get
                {
                    return SepIndex;
                }
            }
            private int SepIndex
            {
                get
                {

                    if (source == "") return -1;

                    string s = "";

                    int i;

                    for (i = source.Length - 1; i >= 0; i--)
                    {
                        s = source.Substring(i, 1);

                        if (s == @"\")
                        {
                            return i;
                        }
                    }
                    return -1;
                }
            }
            public string GetFileName
            {
                get
                {
                    if (source == "") return "";

                    return source.Substring(SepIndex + 1, FileNameLength);
                }
            }
            public string GetFilePath
            {
                get
                {
                    if (source == "") return "";

                    return source.Substring(0, FilePathLength) + "\\";
                }
            }

        }

        public static bool IsItSimpleNumber(object _s)
        {
            //является ли s простым номером, т.е. состоящим только из цифр 1,2,3 и т.д., без точек, запятых и прочего
            string s = ConvertObjectToString(_s);

            if (s == "") return false;

            bool hasNotDigits = false;

            s.ToCharArray().ToList().ForEach(x => { if (!char.IsDigit(x)) hasNotDigits = true; });

            if (hasNotDigits) return false;

            return true;

        }
        public class OpenFileDialogConstructor
        {
            private TextBox TbTargetFilePath { get; set; }
            private Button BtnOpenFile { get; set; }
            public OpenFileDialogConstructor(TextBox _tbTargetFilePath, Button _btnOpenFile)
            {
                TbTargetFilePath = _tbTargetFilePath;
                BtnOpenFile = _btnOpenFile;
                BtnOpenFile.Click += BtnOpenFile_Click;
            }

            private void BtnOpenFile_Click(object sender, EventArgs e)
            {
                OpenFile();
            }

            private void OpenFile()
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = "*.xls;*.xlsx";
                ofd.Filter = "Microsoft Excel (*.xls*)|*.xls*";
                ofd.Title = "Выберите документ Excel";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    TbTargetFilePath.Text = ofd.FileName;
                    return;
                }
            }
        }


        public static string Chr10 { get { return Convert.ToChar(10).ToString(); } }
        public static string Chr13 { get { return Convert.ToChar(13).ToString(); } }
        public class StringByExpressionMerger
        {
            //класс, который соединяет элементы строкового массива выражением
            private string expr;
            private List<string> elements = new List<string>();
            public StringByExpressionMerger(string _expr)
            {
                expr = _expr;
            }

            public void AddElement(string element)
            {
                if (ConvertObjectToString(element) != "") elements.Add(element);
            }

            public string Result
            {
                get
                {
                    string rez = "";
                    bool eos;
                    int counter = 0;
                    if (elements.Count == 0) return "";
                    foreach (string s in elements)
                    {
                        eos = counter == elements.Count - 1;
                        rez = rez + s + (eos ? "" : expr);
                    }
                    return rez;
                }
            }

        }


        public class ParamStringManager
        {

            private string ParamStr = "";


            public ParamStringManager(string _paramStr)
            {
                ParamStr = _paramStr;

            }

            public string GetParamValue(string paramName)
            {
                if (ParamStr == "") return "";
                string[] x = ParamStr.Split(';');
                string[] z;
                for (int i = 0; i < x.Length; i++)
                {
                    z = x[i].Split('=');
                    if (z[0] == paramName) return z[1];
                }
                return "";
            }



        }

        public static string RetStringArrayDump(string[] arr)
        {
            return string.Join(";", arr);
        }




        public static string GenerateFourBlockString()
        {
            //return "AAAA-AAAA-AAAA-AAAA";

            return RandomString(4).ToUpper() + "-"
                + RandomString(4).ToUpper() + "-"
                + RandomString(4).ToUpper() + "-"
                + RandomString(4).ToUpper();
        }

        public static string GenerateValueThatIsNotInList(List<string> lst, int length)
        {
            if (lst == null) return "";

            string newValue = "";

            string oldValuesList = Lst2str(lst.Cast<object>().ToList());

            do
            {
                newValue = RandomString(length);
            }
            while (oldValuesList.Contains(newValue));

            return newValue;
        }
        public static string Lst2str(List<object> arr)
        {
            string s = "";
            if (arr == null) return "";
            if (arr.Count == 0) return "";
            foreach (object x in arr)
            {
                string comma = arr.IndexOf(x) == arr.Count - 1 ? "" : ",";
                s += ConvertObjectToString(x) + comma;
            }
            return s;
        }
        public static bool ListIsNullOrEmpty(System.Collections.IList list)
        {
            if (list == null)
            {
                return true;
            }
            else
            {
                if (list.Count == 0) return true; else return false;
            }
        }
        public static CommonOperationResult ConvertedObject(string typeStr, object value)
        {
            //возвращает object - обертку исходя из того, какой тип передан в typeStr

            if (value == null) return CommonOperationResult.SayItsNull();

            if (value.GetType().ToString() == typeStr) { return CommonOperationResult.ReturnValue(value); }

            object rez = null;
            try
            {
                switch (typeStr)
                {
                    case "System.String":
                        rez = Convert.ToString(value);
                        break;


                    case "System.Double":
                        rez = Convert.ToDouble(value);
                        break;

                    case "System.Int32":
                    case "System.Int16":
                    case "System.Int":
                        rez = Convert.ToInt32(value);
                        break;

                    case "System.Boolean":
                        rez = Convert.ToBoolean(value);
                        break;

                    case "System.DateTime":
                        rez = Convert.ToDateTime(value);
                        break;

                    default:
                        //Fn.MessageBoxInfo("Fn.ConvertedObject обнаружила неизвестный тип: "+ typeStr);
                        rez = value;
                        break;

                }
            }
            catch
            {
                //тут надо отдельно отследить, когда конвертация неуспешна
                return CommonOperationResult.SayFail("Error converting formats in Fn.ConvertedObject");
                //rez = null;
            }
            return CommonOperationResult.ReturnValue(rez);
        }
        //здесь вспомогательные статические функции
        public static void H(int x) { Debug.WriteLine("marker " + x.ToString()); }

        public static string StringListDuplicates(List<string> s)
        {
            //возвращает дублирующиеся значения, если таковые есть в s
            string rez = "";

            var query = s.GroupBy(x => x).Where(g => g.Count() > 1).Select(n => n.Key).ToList();

            for (int i = 0; i < query.Count; i++)
            {
                rez += query[i].ToString() + (i == query.Count - 1 ? "" : ",");
            }
            return rez;
        }

        public static void Dp(string s) { Debug.WriteLine(s); }

        public static void Dp(int x) { Debug.WriteLine(x.ToString()); }

        public static string ConvertObjectToString(object value)
        {
            string _value;

            if (value == null)
            {
                return "";
            }
            else
            {
                try
                {
                    _value = Convert.ToString(value);
                }
                catch
                {
                    _value = "";
                }
            }
            return _value;
        }
        public static int GetPageIdFromTreeNode(TreeNode n)
        {
            if (n != null)
            {
                return n.Name.Length > 3 ? Convert.ToInt32(n.Name.Substring(3, n.Name.Length - 3)) : 0;
            }
            return 0;
        }
        public static TreeNode GetTreeNodeByName(TreeView tv, string name)
        {
            //получить узел по name, обойдя treeview
            TreeNode n1;
            foreach (TreeNode n in tv.Nodes)
            {
                if (n.Name == name) { return n; }
                n1 = FindNodeByName(n, name);
                if (n1 != null) { return n1; }
            }
            return null;
        }
        public static TreeNode GetTreeNodeById(TreeView tv, int id)
        {
            return GetTreeNodeByName(tv, "key" + id.ToString());
        }

        private static TreeNode FindNodeByName(TreeNode n, string name)
        {
            //получить узел по name, обойдя treeview
            TreeNode n1;
            foreach (TreeNode m in n.Nodes)
            {
                if (m.Name == name) { return m; }
                n1 = FindNodeByName(m, name);
                if (n1 != null) { return n1; }
            }
            return null;
        }

        public static string GetEntityTypeFromFullTypeNameString(string s)
        {
            //приходит строка вида ****.****.***. --- ***.xxx - вот нам нужен этот последний ххх
            string[] s0 = s.Split('.');
            if (s.Length <= 0) { return ""; }
            return s0[s0.Length - 1];

        }

        public static double GetRndDouble(double min, double max, int digits = 2)
        {
            double d = min + Random.NextDouble() * (max - min);

            d = Math.Round(d, digits);

            return d;
        }

        public static int GetRndInt(int min, int max)
        {
            return Random.Next(min, max);
        }


        private static Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string SubstituteStringBeginEnd(string source, string b, string e = "")
        {
            return string.IsNullOrEmpty(source) ? "" : b + source + e;
        }


        public static string RepeatString(string ptn, int q)
        {
            //повторяет строку q раз
            if (q <= 0) return "";

            string s = "";
            for (int i = 1; i <= q; i++)
            {
                s += ptn;
            }
            return s;
        }

        public static string RemoveArraySymbolsFromString(string source, string symbols)
        {
            int i;
            string rez = "";
            for (i = 0; i < source.Length; i++)
            {
                if (!symbols.Contains(source[i]))
                {
                    rez += source[i];
                }
            }
            return rez;
        }

        public class TabObjectWrapper
        {
            private TabControl _tabControl;
            private List<TabPage> _tabList = new List<TabPage>();
            public TabObjectWrapper(TabControl _tabControl)
            {
                this._tabControl = _tabControl;

            }
            private void loadPages()
            {
                _tabList.Clear();
                foreach (TabPage tab in _tabControl.TabPages)
                {
                    _tabList.Add(tab);
                }
            }

            public void ShowOnlyTab(string tabName)
            {
                loadPages();
                _tabControl.TabPages[tabName].Show();
                _tabList.Where(x => x.Name != tabName).ToList().ForEach(y => y.Hide());
            }
        }
    }
}