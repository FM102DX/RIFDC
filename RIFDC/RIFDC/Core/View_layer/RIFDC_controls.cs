using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using StateMachineNamespace;
using ObjectParameterEngine;
using CommonFunctions;
using RIFDC;
using System.Drawing;

namespace RIFDC
{

    /*
     * здесь храним классы контролов
     * идея в том, чтобы под каждый тип контрола сделать свой класс, который знает про этот контрол все, а именно:
     * 1) как устанавливать и брать значения
     * 1.1) как заполнять контрол 
     * 2) переметры инициализации
     * 3) какие обработчики вешать на события
     * 4) какие типы проверок он поддерживает
     * ...
     * 
     * 
     * */


    

    public interface IDFCSearchControl
    {

    }


    public interface IRIFDCButton
    {
        //это все, что считается кнопкой и на что можно нажать
        FormBtnTypeEnum btnType { get; }
        bool locked { get; set; }

        string name { get; }

        void addEventHandler(EventHandler x);
    }

    // интерфейсы
    public interface IRecordBasedControl
    {
        EventHandlerCollection_RecordBasedControl eventHandlers_RecordBC { get; set; }
        void setValue(object value);
        object getValue();
        void focus();
        Control targetControl { get; set; }
        void addEventHandlers();
        object Tag { get; set; }
        string Text { get; set; }
        int selectionStart { get; set; }
        void Focus();
        bool locked { get; set; } // lock-unlock

        object emptyValue { get; set; }

        event Lib.methodContainer gotFocus;
    }

    public interface IGridBasedControl
    {
        EventHandlerCollection_GridBasedControl eventHandlers_GridBC { get; set; }
        Control targetControl { get; set; }

        event Lib.Sorter.ImSorted_EventHandler ImSorted;
        bool multiSelectMode { get; set; }

        List<string> selectedItemsIds { get; }

        void reReadItem(IKeepable T);
        void setCurrentId(string id);
        void setCurrentIndex(int index);
        void fillMe();
        object getValue();
        void selectAll();
        bool isMultiSelectRepresentable { get; set; }

        Lib.LinePaintingRuleHolder paint { get; set; }

        void selectNone();
        void addEventHandlers();
        void expandAll();

        void collapseAll();

        Lib.Filter internalFilter { get; set; }

    }

    #region //основные классы

    public class RIFDC_TextBox : RIFDC_TextBoxBasedControl, IRecordBasedControl
    {
        //представляет textBox
        public string ctrlTypeName { get { return "System.Windows.Forms.TextBox"; } }
        TextBox tb;
        public string nullText;

        public RIFDC_TextBox(Control c) : base(c)
        {
            tb = (TextBox)targetControl;
        }
        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            tb.TextChanged += eventHandlers_RecordBC.textBox_TextChanged_Processor;
            tb.Leave += eventHandlers_RecordBC.textBox_Leave_Processor;
        }
        public new int selectionStart { get { return tb.SelectionStart; } set { tb.SelectionStart = value; } }
    }

    public class RIFDC_RichTextBox : RIFDC_TextBoxBasedControl, IRecordBasedControl
    {
        //представляет textBox
        public string ctrlTypeName { get { return "System.Windows.Forms.RIchTextBox"; } } //TODO выдавать ошибку определения если замапили не тот тип контрола
        RichTextBox tb;
        public string nullText;
        public void setValue(object value)
        {
            tb.Text = (value ?? "").ToString();
        }
        public RIFDC_RichTextBox(Control c) : base(c)
        {
            tb = (RichTextBox)targetControl;
        }

        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            tb.TextChanged += eventHandlers_RecordBC.checkBox_CheckedChanged_Processor;
            tb.Leave += eventHandlers_RecordBC.textBox_Leave_Processor;
        }

    }

    public class RIFDC_DTPicker : MappedRecordBasedControl, IRecordBasedControl
    {
        //представляет textBox

        public string ctrlTypeName { get { return "System.Windows.Forms.DateTimePicker"; } }

        DateTimePicker tb;
        public void setValue(object value)
        {
            if (value == null) value = "";
            tb.Text = value.ToString();
        }
        public object getValue()
        {
            return tb.Text;
        }
        public RIFDC_DTPicker(Control c) : base(c)
        {
            tb = (DateTimePicker)targetControl;
        }
        public override void addEventHandlers()
        {
            //беерм eventHandlers_RecordBC и обвешиваем нужные события
            tb.TextChanged += eventHandlers_RecordBC.checkBox_CheckedChanged_Processor;
            tb.Leave += eventHandlers_RecordBC.textBox_Leave_Processor;
        }
    }

    public class RIFDC_MaskedTextBox : RIFDC_TextBoxBasedControl, IRecordBasedControl
    {
        //представляет textBox

        public string ctrlTypeName { get { return "System.Windows.Forms.MaskedTextBox"; } }

        MaskedTextBox tb;
        /*
        public new object getValue()
        {
            
            return tb.Text;
        }
        */

        public string nullText = "  .  .";
        public RIFDC_MaskedTextBox(Control c) : base(c)
        {
            emptyValue = nullText;
            tb = (MaskedTextBox)targetControl;
        }
        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            tb.TextChanged += eventHandlers_RecordBC.textBox_TextChanged_Processor;
            tb.Leave += eventHandlers_RecordBC.textBox_Leave_Processor;
        }
    }


    public class RIFDC_DataDisplayLabel : MappedRecordBasedControl, IRecordBasedControl
    {
        //представляет textBox
        public string ctrlTypeName { get { return "System.Windows.Forms.Label"; } }

        Label lb;
        public void setValue(object value)
        {
            //cb = (CheckBox)targetControl;
            if (value == null) value = false;
            lb.Text = fn.toStringNullConvertion(value);
        }
        public object getValue()
        {
            return lb.Text;
        }
        public RIFDC_DataDisplayLabel(Control c) : base(c)
        {
            lb = (Label)targetControl;
        }

        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
         //   cb.CheckedChanged += eventHandlers_RecordBC.checkBox_CheckedChanged_Processor;
        }
    }


    public class RIFDC_CheckBox : MappedRecordBasedControl, IRecordBasedControl
    {
        //представляет textBox
        public string ctrlTypeName { get { return "System.Windows.Forms.CheckBox"; } }

        CheckBox cb;
        public void setValue(object value)
        {
            //cb = (CheckBox)targetControl;
            if (value == null) value = false;
            cb.Checked = Convert.ToBoolean(value);
        }
        public object getValue()
        {
            return cb.Checked;
        }
        public RIFDC_CheckBox(Control c) : base(c)
        {
            cb = (CheckBox)targetControl;
        }

        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            cb.CheckedChanged += eventHandlers_RecordBC.checkBox_CheckedChanged_Processor;
        }
    }

    public class RIFDC_MenuStripButtonObject : IRIFDCButton
    {
        //это кнопка в Menustrip, она не Control, так что пришлось сделать отдельный класс
        private System.Windows.Forms.ToolStripMenuItem targetControl;

        public string ctrlTypeName { get { return ""; } }

        public string name { get { return targetControl.Name; } }
        public FormBtnTypeEnum btnType { get; set; }

        public void addEventHandler(EventHandler x)
        {
            targetControl.Click += x;
        }

        public RIFDC_MenuStripButtonObject(ToolStripMenuItem _targetControl, FormBtnTypeEnum _btnType) 
        {
            targetControl = _targetControl;
            btnType = _btnType;
        }
        public bool locked
        {
            get
            {
                return !targetControl.Enabled;
            }
            set
            {
                targetControl.Enabled = !value;
            }
        }

    }

    public class RIFDC_Button : RIFDCControlBase, IRIFDCButton
    {
        //представляет textBox
        public string ctrlTypeName { get { return ""; } }

        public FormBtnTypeEnum btnType { get; set; }

        public string name { get { return targetControl.Name; } }

        public void addEventHandler(EventHandler x)
        {
            targetControl.Click += x;
        }
        public RIFDC_Button(Control c, FormBtnTypeEnum _btnType) : base(c)
        {
            btnType = _btnType;
        }
        public bool locked
        {
            get
            {
                return !targetControl.Enabled;
            }
            set
            {
                targetControl.Enabled = !value;
            }
        }

    }

    public class RIFDC_DataGridView : MappedGridBasedControl, IGridBasedControl
    {

        DataGridEditabilityMode editabilityMode= DataGridEditabilityMode.NotEditableAtAll;
        public RIFDC_DataGridView(
                                    Control c, 
                                    IKeeper _dataSource, 
                                    Lib.IControlFormat myFormat = null, 
                                    DataGridEditabilityMode _editabilityMode = DataGridEditabilityMode.NotEditableAtAll,
          bool _isMultiSelectRepresentable=false ) : base(c, _dataSource)
        {
            dgr = (DataGridView)targetControl;

            if (myFormat != null)
            {
                foreach (Lib.IControlFormatLine line in myFormat.lines)
                {
                    addDataColumn(line.fieldClassName, line.colWidth, line.caption);
                }
            }
            editabilityMode = _editabilityMode;
            isMultiSelectRepresentable = _isMultiSelectRepresentable;

            //форматирование грида

            dgr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // dgr.MultiSelect = false;
            // dgr.AllowUserToAddRows = false;
            // dgr.AllowUserToResizeRows = false;
            // dgr.AllowUserToDeleteRows = false;
            // dgr.RowHeadersVisible = false;



        }

        public Lib.Filter internalFilter { get; set; } = null;
        //public string myRIFDCType { get { return "RIFDC_DataGridView"; } }
        public bool isMultiSelectRepresentable { get; set; } = false;
        public void expandAll()
        {

        }

        public void collapseAll()
        {

        }
        public string ctrlTypeName { get { return "System.Windows.Forms.DataGridView"; } }

        DataGridView dgr;

        bool _multiSelectMode=false;
        public void setCurrentId(string id)
        {
            if (id == null) return;
            DataGridViewRow dgrow = getDataGridViewRowById(id.ToString());
            if (dgrow != null) dgrow.Selected = true;
        }

        public List<string> selectedItemsIds 
        {
            get
            {
                List<string> rez = new List<string>();
                bool isSelected;
                foreach (DataGridViewRow r in dgr.Rows)
                    {
                        try
                        {
                            isSelected = Convert.ToBoolean(r.Cells[0].Value);
                            if (isSelected)
                            {
                                rez.Add(fn.toStringNullConvertion(r.Cells[data.boundColumnIndex].Value));
                            }
                        }
                        catch
                        {

                        }
                    }
                return rez;
            }
        }

        public Lib.LinePaintingRuleHolder paint { get; set; } = null;
        private void setCheckedValue(bool value)
        {
            //перебрать все строки грида и присвоить 0й колонке value
            try
            {
                foreach (DataGridViewRow rw in dgr.Rows)
                {
                    rw.Cells[0].Value = value;
                }
            }
            catch
            {

            }
        }
        public void selectAll() { setCheckedValue(true); }

        public void selectNone() { setCheckedValue(false); }

        public void setValue(object value)
        {

        }

        private void setGridColumnVisibility(string colName, bool isVisible)
        {
            foreach (DataGridViewColumn c0 in dgr.Columns)
            {
                if (c0.Name == colName)
                {
                    c0.Visible = isVisible;
                    break;
                }
            }
        }

        public bool multiSelectMode 
        {
            get{ return _multiSelectMode; }
            set 
            { 
                _multiSelectMode = value;
                setGridColumnVisibility("multiSelectCheckBoxColumn", _multiSelectMode);
            }
        }

        public object getValue()
        {
            return dgr.SelectedRows[0].Cells[data.boundColumnIndex].Value;
        }

        public void setCurrentIndex(int index)
        {
            if (dgr.Rows.Count > 0 && (dgr.Rows.Count > index))
            {
                dgr.ClearSelection();
                dgr.Rows[index].Selected = true;
            }
        }

        public void reReadItem(IKeepable t)
        {
            //перечитать этот объект в контрол- только его, обновлять весь контрол не надо
            //не пользоваться тем, что он выделен
            // 1) найти в гриде объект с таким id

            DataGridViewRow dgrow = getDataGridViewRowById(t.id.ToString());

            if (dgrow == null) return; // был прецедент

            dgrow = readDataGridViewRow(t, dgr, dgrow, true);
        }

        public override void mapMe()
        {
            //мапит колонки из объекта data на физический грид
            //проверить количество колонок и выставить ширину колонок

            int i = 1;
            DataGridViewColumn c;
            DataGridView _dg = (System.Windows.Forms.DataGridView)targetControl;
            _dg.Columns.Clear();
            //добавляем кололнку с чекбоксами для выделения строк
            
            c = new DataGridViewColumn();
            c.Width = 50;
            c.Name = "multiSelectCheckBoxColumn";
            c.CellTemplate = new DataGridViewCheckBoxCell();
            _dg.Columns.Insert(0, c);
            c.Visible = false;
            //c.DisplayIndex = 1;

            foreach (DataColumn dc in data.dataColumns)
            {
                c = new DataGridViewColumn();
                c.Tag = dc.srcDataFieldName;
                c.Width = dc.colWidth;
                if (dc.colWidth == 0) c.Visible = false;
                c.HeaderText = dc.caption;
                switch (dc.srcDataFieldType)
                {
                    case Lib.FieldTypeEnum.Int:
                    case Lib.FieldTypeEnum.Double:
                        c.CellTemplate = new DataGridViewTextBoxCell();
                        break;

                    case Lib.FieldTypeEnum.Date:
                    case Lib.FieldTypeEnum.String:
                    case Lib.FieldTypeEnum.Memo:
                    case Lib.FieldTypeEnum.Time:
                        c.CellTemplate = new DataGridViewTextBoxCell();
                        break;

                    case Lib.FieldTypeEnum.Bool:
                        c.CellTemplate = new DataGridViewCheckBoxCell();
                        break;

                    default:
                        c.CellTemplate = new DataGridViewTextBoxCell();
                        break;

                }
                _dg.Columns.Insert(i, c);
                i++;
            }
        }

        private DataGridViewRow readDataGridViewRow(IKeepable t, DataGridView _dg, DataGridViewRow dgr, bool reRead=false, string colorCode = "")
        {
            //читает row из объекта
            //теперь поехали перебирать колонки
            DataGridViewCell workingCell = null;
            ObjectParameters.ObjectParameter val = null;

            string stringValue="";
            bool readOnlyCell;
            Lib.FieldInfo f=null;
            DateTime dateTime;
            bool itsNull;

            for (int i = 0; i < data.dataColumns.Count; i++)
            {

                f = t.fieldsInfo.getFieldInfoObjectByFieldClassName(data.dataColumns[i].srcDataFieldName);
                if (f == null) return dgr;

                itsNull = f.isNull;

                val = ObjectParameters.getObjectParameterByName(t, data.dataColumns[i].srcDataFieldName);

                switch (f.fieldType)
                {
                    case Lib.FieldTypeEnum.Date:
                        dateTime = Convert.ToDateTime(val.value);
                        stringValue =  (itsNull) ? "" : dateTime.ToString("dd.MM.yyyy");
                        break;

                    case Lib.FieldTypeEnum.Time:
                        dateTime = Convert.ToDateTime(val.value);
                        stringValue = (itsNull) ? "" : dateTime.ToString("hh:mm:ss");
                        break;

                    case Lib.FieldTypeEnum.DateTime:
                        dateTime = Convert.ToDateTime(val.value);
                        stringValue = (itsNull) ? "" : dateTime.ToString("dd.MM.yyyy hh:mm:ss");
                        break;

                    case Lib.FieldTypeEnum.Int:
                    case Lib.FieldTypeEnum.Double:
                        stringValue = (itsNull) ? "" : fn.toStringNullConvertion(val.value);
                        break;


                    case Lib.FieldTypeEnum.String:
                    case Lib.FieldTypeEnum.Memo:
                        stringValue = (itsNull) ? "" : fn.toStringNullConvertion(val.value);
                        break;

                    default:
                        stringValue = fn.toStringNullConvertion(val.value);
                        break;
                }

                workingCell = reRead ? dgr.Cells[i+1] : new DataGridViewTextBoxCell();
                readOnlyCell = true; // (editabilityMode== DataGridEditabilityMode.NotEditableAtAll) ? false : 

                switch (_dg.Columns[i+1].CellType.Name)
                {
                    case "DataGridViewTextBoxCell":
                        workingCell.Value = (val == null) ? "" : (val.value == null ? "" : stringValue);
                        break;

                    case "DataGridViewCheckBoxCell":
                        workingCell.Value = (val == null) ? false : (bool)val.value;
                        //fn.dp("newCell.Value=" + newCell.Value.ToString());
                        break;

                        //TODO здесь другие типы контролов
                }

               if (!reRead) dgr.Cells.Add(workingCell);
                workingCell.ReadOnly = readOnlyCell;
            }

            //раскраска 
            //если пустая строка, убрать раскраску
            
            if (colorCode=="") dgr.DefaultCellStyle.BackColor = Color.White;

            if (paint!=null)
            {
                Color c = paint.getMyColor(t);

              //  fn.dp("COLOR=" + c.ToString());
                //if (c==Color.White) fn.dp ()

                dgr.DefaultCellStyle.BackColor = paint.getMyColor(t);

            }
            return dgr;
        }

        public override void fillMe()
        {
            DataGridView _dg = (DataGridView)targetControl;
            _dg.Rows.Clear();
            //тут надо что то отключить, чтобы он не выводил событие SelectionChanged при заливке

            //fillViewControlFlag = true;
            //заливаем контрол
            //запускаем итерацию по датасорсу формы

            //object val;

            DataGridViewRow dgr;

            ObjectParameters.ObjectParameter val = null;

            DataGridViewCell newCell = null;

            foreach (IKeepable t in data.dataSource.items)
            {
                dgr = new DataGridViewRow();

                //первый столбец - чекбоксы для мультиселекта
                newCell = new DataGridViewCheckBoxCell();
                newCell.Value = false;
                newCell.ReadOnly = false;

                dgr.Cells.Add(newCell);
                
                dgr = readDataGridViewRow(t, _dg, dgr);

                _dg.Rows.Add(dgr);
            }
        }

        public DataGridViewRow getDataGridViewRowById(string id)
        {
            //DataGridView dgr = (DataGridView)targetControl;

            string value2 = id.ToString();

            foreach (DataGridViewRow r in dgr.Rows)
            {
                if (fn.toStringNullConvertion(r.Cells[data.boundColumnIndex].Value) == value2)
                {
                    return r;
                }
            }
            return null;
        }

        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            dgr.SelectionChanged += eventHandlers_GridBC.dataGridView_SelectionChanged_Processor;
            dgr.ColumnHeaderMouseClick += Dgr_ColumnHeaderMouseClick;

        }

        public event Lib.Sorter.ImSorted_EventHandler ImSorted;

        private DataGridViewColumn getColumnByClassName(string fieldClassName)
        {
            foreach (DataGridViewColumn c0 in dgr.Columns)
            {
                if (fn.toStringNullConvertion(c0.Tag).ToLower() == fieldClassName.ToLower()) return c0;
            }
            return null;
        }

        private void setGridHeadersFromSorter(Lib.Sorter sorter)
        {
            Lib.AscDescSortEnum dir;

            foreach (Lib.Sorter.SortingRule sr in sorter.sortingRuleList)
            {
                dir = sr.sortingDirection;

                //теперь поставить галочку встолбце грида
                //нужен метод, который из сортингрулов проставляет заголовки грида
                //номер столбца грида по fieldInfoObject


                DataGridViewColumn c = getColumnByClassName(sr.fieldClassName);

                if (c == null) continue;

                string s = c.HeaderText;

                string s0 = (dir == Lib.AscDescSortEnum.Asc) ? "^" : "˅";

                if (s.Length > 0)
                {
                    if (s[0].ToString() == "^" || s[0].ToString() == "˅")
                    {
                        s = s0 + " " + fn.substrBeginsFromLtrNo(s, 2);
                    }
                    else
                    {
                        s = s0 + " " + s;
                    }
                }
                c.HeaderText = s;
            }
        }

        private void Dgr_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (data.dataSource == null) return;
 
            //вот тут 
            int col = e.ColumnIndex; //если индекс = 6, то column будет +1
            
            //это имя поля
            try
            {
                string cName = data.dataColumns[col - 1].srcDataFieldName;
                Lib.FieldInfo f = data.dataSource.sampleObject.getFieldInfoByFieldClassName(cName);

                Lib.Sorter.SortingRule _x = data.dataSource.sort.myActualSorter.ruleOnTheField(f);
                Lib.Sorter myNewSorter = new Lib.Sorter();
                Lib.AscDescSortEnum dir = Lib.AscDescSortEnum.Asc;

                if (_x !=null) 
                {
                    if (_x.sortingDirection == Lib.AscDescSortEnum.NotSpecified || _x.sortingDirection == Lib.AscDescSortEnum.Desc) 
                                dir = Lib.AscDescSortEnum.Asc; 
                                    else 
                                        dir = Lib.AscDescSortEnum.Desc;
                }

                //Logger.log("GRIDSORT", f.fieldClassName +"___"+ dir.ToString());

                myNewSorter.addNewSortingRule(f, dir);

                ImSorted(myNewSorter);

                setGridHeadersFromSorter(myNewSorter);

        //data.dataSource.sort.applySorter(myNewSorter);
            }
            catch (Exception e1)
            {
                //ну просто не будет сортировать
                fn.dp(e1.Message);

            }
        }

    }

    public class RIFDC_ComboBox : MappedRecordBasedControl, IRecordBasedControl
    {
        //представляет ComboBox

        public List<ComboBoxDataLine> items = new List<ComboBoxDataLine>();

        bool allowNull = false;

        const string cbxNullText = "cbxNullText";

        public RowContainingDataObject data;
        public string ctrlTypeName { get { return "System.Windows.Forms.ComboBox"; } }
        ComboBox cbx { get { return (ComboBox)targetControl; } }
        public void setValue(object _value)
        {
            //тут приходит интовое вэлью, надо найти в комбобоксе это вэлью и сделать его текущим
            // или где-то надыбать объект, но это долго


            string value = fn.toStringNullConvertion(_value);

            string id = Convert.ToString(value);

            //надо взять тот объект по id и сделать его текущим, присвоив selectedItem
            ComboBoxDataLine line=null;

            foreach (ComboBoxDataLine _line in items)
            {
                if (_line.valueMember == value)
                {
                    line = _line;
                    break;
                }
            }

            if (line != null) { cbx.SelectedItem = line; } else { cbx.SelectedIndex = -1; }
        }
        public object getValue()
        {
            string val = fn.toStringNullConvertion(cbx.SelectedValue);
            if (val == cbxNullText)
            {
                return null;
            }
            else
            {
                return val;
            }
        }

        public RIFDC_ComboBox(Control c, IKeeper _dataSource, bool _allowNull = false) : base(c)
        {
            data = new RowContainingDataObject(_dataSource);
            allowNull = _allowNull;
        }

        public void fillMe()
        {
            if (cbx.DataSource == null) cbx.Items.Clear();

            items.Clear();

            cbx.ValueMember = "valueMember";
            cbx.DisplayMember = "displayMember";

            //cbx.DataSource = data.dataSource.actualItemList; // тут проблема, если он не реализует Icollection

            if (allowNull)
            {
                items.Add(ComboBoxDataLine.getInstance(cbxNullText, ""));
            }
            foreach (IKeepable x in data.dataSource.actualItemList)
            {
                items.Add(ComboBoxDataLine.getInstance(x.displayId, x.displayName));
            }
            cbx.DataSource = items;

            cbx.SelectedIndex = -1;
        }

        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            cbx.SelectedIndexChanged += eventHandlers_RecordBC.ComboBox_SelectedIndexChanged_Processor;
            //cbx.Leave += eventHandlers_RecordBC.textBox_Leave_Processor;
        }

        public class ComboBoxDataLine
        {
            public string valueMember { get; set; }
            public string displayMember { get; set; }
            public static ComboBoxDataLine getInstance(string _valueMember, string _displayMember)
            {
                ComboBoxDataLine line = new ComboBoxDataLine();
                line.valueMember = _valueMember;
                line.displayMember = _displayMember;
                return line;
            }
        }

    }

    public class RIFDC_TreeView : MappedGridBasedControl, IGridBasedControl
    {

        DataGridEditabilityMode editabilityMode = DataGridEditabilityMode.NotEditableAtAll;
        public RIFDC_TreeView(
                                    Control c,
                                    IKeeper _dataSource,
                                    Lib.IControlFormat myFormat = null,
                                    DataGridEditabilityMode _editabilityMode = DataGridEditabilityMode.NotEditableAtAll) : base(c, _dataSource)
        {
            tvr = (TreeView)targetControl;
            
/*
            if (myFormat != null)
            {
                foreach (Lib.IControlFormatLine line in myFormat.lines)
                {
                    addDataColumn(line.fieldClassName, line.colWidth, line.caption);
                }
            }
            */

            editabilityMode = _editabilityMode;

            //dgr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


        }

        public string ctrlTypeName { get { return "System.Windows.Forms.TreeView"; } }
        //public string myRIFDCType { get { return "RIFDC_TreeView"; } }
        public bool isMultiSelectRepresentable { get; set; } = false;
        TreeView tvr;

        bool _multiSelectMode = false;
        public void setCurrentId(string id)
        {
            /*
             * if (id == null) return;
            DataGridViewRow dgrow = getDataGridViewRowById(id.ToString());
            if (dgrow != null) dgrow.Selected = true;
            */
        }

        public List<string> selectedItemsIds
        {
            get
            {
                return null;
            }
        }

        public Lib.Filter internalFilter { get; set; } = null;
        public Lib.LinePaintingRuleHolder paint { get; set; } = null;
        public bool multiSelectMode { get; set; }

        public void setValue(object value)
        {

        }
        public object getValue()
        {
            TreeNode _tvrn = tvr.SelectedNode;
            if (_tvrn == null) return "";
            return _tvrn.Name;
        }

        public void setCurrentIndex(int index)
        {
/*            if (dgr.Rows.Count > 0 && (dgr.Rows.Count > index))
            {
                dgr.ClearSelection();
                dgr.Rows[index].Selected = true;
            }*/
        }

        public void reReadItem(IKeepable t)
        {
            //надо найти node с таким id и поменять называние
            TreeNode _tvrn = null;
            TreeNode[] _tvrns = tvr.Nodes.Find(t.id, true);
            if (_tvrns.Length > 0) _tvrn = _tvrns[0];

            if (_tvrn != null) _tvrn.Text = t.displayName;

        }

        private void processTreeViewNode(TreeNode node, IKeeper dataSource)
        {
            TreeNode _node = null;
            data.dataSource.actualItemList.Where(x => fn.toStringNullConvertion(x.parentId) == node.Name)
                                          .ToList()
                                          .ForEach(y => {
                                             _node= node.Nodes.Add(y.id, y.displayName);
                                              processTreeViewNode(_node, dataSource);
                                          });
        }

        public override void fillMe()
        {
            //тут надо зелить TreeView
            //? а какие поля там отображать? DIsplayName?
            // ну то есть такая иедология что displayName - в treeView, остальное - в record-based полях

            tvr.Nodes.Clear();

            //сначала выбираем все без предков и добавляем их
            // а там есть такие, что с предками, но эти предки не попали в фильтр
            //значит, надо изменить понятие "без предков"
            //и как это сделать? 
            //видимо, без предков - это те, кто не имеет предков в данном множестве
            //кгм. это для каждого элемента надо проверять, есть ли его родитель в этом дереве


            // надо взять parent каждого, и проверить, еслть ли он вообще в этом списке

            data.dataSource.actualItemList.Where(x => fn.toStringNullConvertion(x.parentId) == "")
                                          .ToList()
                                          .ForEach(y=> {
                                              tvr.Nodes.Add(y.id, y.displayName);
                                            });

            
            //теперь надо из них вытащить те, у которых parentId нет в списке id
            var zeroLevelUnitsWithParentIds = data.dataSource.actualItemList.Where(x => fn.toStringNullConvertion(x.parentId) != "").ToList();
            
            zeroLevelUnitsWithParentIds.ForEach(x=> {

                int cnt = zeroLevelUnitsWithParentIds.Where(y=>y.id==x.parentId).Count();
                if (cnt>0)
                {
                    tvr.Nodes.Add(x.id, x.displayName);
                }
            });

            // а если найденное где-то внутри дерева , 
            // если фильтр отсекает часть дререва?

            //далее для каждого узла делаем рекурсивный drilldown
            List<TreeNode> zeroLevelNodesList = new List<TreeNode>();

            foreach (TreeNode tvn in tvr.Nodes)
            {
                zeroLevelNodesList.Add(tvn);
            }

            zeroLevelNodesList.ForEach(x => processTreeViewNode(x, data.dataSource));

            //сколько уровней показывать + комбо уровней levels2show

            tvr.ExpandAll();

        }


        public override void addEventHandlers()
        {
            //беерм eventHandlers_GridBC и обвешиваем нужные события
            //dgr.SelectionChanged += eventHandlers_GridBC.dataGridView_SelectionChanged_Processor;
            //dgr.ColumnHeaderMouseClick += Dgr_ColumnHeaderMouseClick;
            tvr.AfterSelect += eventHandlers_GridBC.treeView_SelectionChanged_Processor;

        }

        public event Lib.Sorter.ImSorted_EventHandler ImSorted;

        public void selectAll()
        {
            
        }

        public void selectNone()
        {
            
        }

        public void expandAll()
        {
            tvr.ExpandAll();
        }

        public void collapseAll()
        {
            tvr.CollapseAll();
        }
    }




    #endregion

    // public delegate void CommonCtrlEventHandler(object sender, EventArgs e);
    // вспомогательные классы

    public class MappedGridBasedControl : RIFDCControlBase
    {
        //общий паттерн для привязанного контрола, который в той или иной мере является гридом

        public EventHandlerCollection_GridBasedControl eventHandlers_GridBC { get; set; }

        public RowContainingDataObject data;



        public MappedGridBasedControl(Control c, IKeeper _dataSource) : base(c)
        {
            data = new RowContainingDataObject(_dataSource);
        }

        public object getObjectById(string id)
        {
            IKeepable t = data.dataSource.getItemById(id);
            return t;
        }

        public virtual void setMyFormat(Lib.GridBasedControlFormat format)
        {

        }

        public virtual void mapMe()
        {

        }

        public virtual void fillMe()
        {
        }

        public virtual void clearMe()
        {
        }

        public virtual void setTargetCtrlIndex(int index)
        {
        }

        public virtual object getTargetControlValue()
        {
            return null;
        }


        public DataColumn addDataColumn(string srcFieldClassName, int cloWidth, string caption = "")
        {
            DataColumn d = data.addDataColumn(srcFieldClassName, cloWidth, caption);
            mapMe(); //TODO это невежественное решение, т.к. одна колонка мапится много раз, но пока пусть будет
            return d;
        }

        public override void addEventHandlers()
        {

        }
    }
    public class RIFDC_TextBoxBasedControl : MappedRecordBasedControl
    {
        public string nullText = "";
        public RIFDC_TextBoxBasedControl(Control c) : base(c)
        {
            emptyValue = nullText;
        }
        public object getValue()
        {
            if (emptyValue != null)
            {
                //здесь реализуем логику nullability: если в контроле содержится NullText, возвращаем Null 
                string val = fn.toStringNullConvertion(emptyValue);
                if (targetControl.Text == val) return null;
                return targetControl.Text;
            }
            else
            {
                return targetControl.Text;
            }
        }
        public void setValue(object value)
        {
            //здесь реализуем логику nullability: если в контрол приходит null, ставим nullText, и если в контроле содержится NullText, возвращаем Null 

            string s= fn.toStringNullConvertion(emptyValue);

            if (s != "")
            {
                if (value == null)
                {
                    targetControl.Text = s;
                }
                else
                {
                    targetControl.Text = fn.toStringNullConvertion(value);
                }
            }
            else
            {
                targetControl.Text = fn.toStringNullConvertion(value);
            }
        }




    }

    public class RowContainingDataObject
    {
        //любой объект, который содержит столбцы и строки
        //он содержит IKeeper в качестве датасорса и массив columns
        public IKeeper dataSource { get; set; }
        public int boundColumnIndex=1;
        public List<DataColumn> dataColumns = new List<DataColumn>();
        IKeepable t { get { return dataSource.sampleObject; } }
        public DataColumn addDataColumn(string srcFieldClassName, int colWidth, string caption = "")
        {
            Lib.FieldInfo f = t.fieldsInfo.getFieldInfoObjectByFieldClassName(srcFieldClassName);
            if (f == null)
            {
                fn.dp("Ошибка маппинга: не найден fieldInfo для поля " + srcFieldClassName);
                return null;
            }

            if (caption == "") caption = f.caption;
            if (caption == "") caption = srcFieldClassName;

            DataColumn dc = new DataColumn();
            dc.caption = caption;
            dc.srcDataFieldName = srcFieldClassName;
            dc.srcDataFieldType = f.fieldType;
            dc.colWidth = colWidth;
            dataColumns.Add(dc);
            return dc;
        }
        public RowContainingDataObject(IKeeper _dataSource)
        {
            dataSource = _dataSource;
        }

        public IKeepable getObjectById(string id)
        {
            return dataSource.getItemById(id);
        }
    }
    public class MappedRecordBasedControl : RIFDCControlBase
    {
        //общий паттерн для привязанного контрола, связанному с конкретным полем данных

        //это хранит источник строк для присоединенных табличных контроллеров в случае comboBox
        //public RowContainingDataObject<KeepableClass> data; // = new RowContainingDataObject<KeepableClass>();

        public EventHandlerCollection_RecordBasedControl eventHandlers_RecordBC { get; set; }

        public object getTargetControlValue()
        {
            return null;
        }

        public void setTargetControlValue(object value)
        {

        }

        public MappedRecordBasedControl(Control c) : base(c)
        {

        }

        public int selectionStart { get; set; }
        public string Text { get; set; }

        public bool locked
        {
            get
            {
                return !targetControl.Enabled;
            }
            set
            {
                targetControl.Enabled = !value;
            }
        }
        public object emptyValue { get; set; } = null; //значение, которое надо присвоить контролу, чтобы он отображался как пустой
    }

    public class EventHandlerCollection_RecordBasedControl
    {
        //пакет ивентхендлеров, который крудманагер передает контролу
        public EventHandler textBox_TextChanged_Processor;
        public EventHandler textBox_Leave_Processor;
        public EventHandler checkBox_CheckedChanged_Processor;
        public EventHandler selectedIndexChanged_Processor;
        public EventHandler dTPicker_ValueChanged_Processor;
        public EventHandler ComboBox_SelectedIndexChanged_Processor;
    }

    public class EventHandlerCollection_GridBasedControl
    {
        //пакет ивентхендлеров, который крудманагер передает контролу
        public TreeViewEventHandler treeView_SelectionChanged_Processor;
        public EventHandler dataGridView_SelectionChanged_Processor;
        public EventHandler listView_SelectionChanged_Processor;
    }

    public class DataColumn
    {
        public string caption;
        public string srcDataFieldName;
        public Lib.FieldTypeEnum srcDataFieldType; //от этого, в частности, зависит тип поздаваемых ячеек в гридах
        public int colWidth;
    }

    public class RIFDCControlBase
    {
        public Control targetControl { get; set; }

        public RIFDCControlBase(Control c)
        {
            targetControl = c;

            //targetControl.GotFocus += (sender, e) => gotFocus();
            targetControl.GotFocus += TargetControl_GotFocus;

        }

        private void TargetControl_GotFocus(object sender, EventArgs e)
        {
            if (gotFocus!=null) gotFocus();
        }

        public event Lib.methodContainer gotFocus;

        // public virtual FrmControlTypeEnum targetControlType {get;}
        public object Tag
        {
            get { return targetControl.Tag; }
            set { targetControl.Tag = value; }
        }

        public void focus()
        {
            targetControl.Focus();
        }
        public virtual void addEventHandlers() { }

        public void Focus() { targetControl.Focus(); }


    }

    public abstract class DFCSearchControlPattern
    {
        //одна из реализаций контрола, который занимается поиском 

        internal  Button btnSearch; //кнопка, при нажатии на которую работает поиск
        internal Button btnReset; //кнопка, при нажатии на которую вып. сброс фильтров
        internal Lib.FiltrationTypeEnum filtrationType;

        internal TextBox tbSearchStr; //поле для ввода запросов

        internal DataFormComponent parent;

        public DFCSearchControlPattern(DataFormComponent _parent, Lib.FiltrationTypeEnum _filtrationType, TextBox _tbSearchStr, Button _btnSearch, Button _btnReset)
        {
            parent = _parent;
            tbSearchStr = _tbSearchStr;
            btnSearch = _btnSearch;
            btnReset = _btnReset;
            filtrationType = _filtrationType;

            btnSearch.Click += (sender, e) => doSearch();
            btnReset.Click += (sender, e) => doReset();
            tbSearchStr.KeyDown += (object sender, KeyEventArgs e) => { if (e.KeyCode == Keys.Enter) doSearch(); };
            tbSearchStr.KeyDown += (object sender, KeyEventArgs e) => { if (e.KeyCode == Keys.Escape) doReset(); };
        }

        internal virtual void doSearch()
        { 
            //функция, которая, собственно, парсит строку и выполняет поиск
        }
        internal virtual void doReset()
        {
           
        }
    }

    public class DFCSearchControl : DFCSearchControlPattern, IDFCSearchControl
    {

        public DFCSearchControl (DataFormComponent _parent, Lib.FiltrationTypeEnum filtrationType, TextBox _tbSearchStr, Button _btnSearch, Button _btnReset) : base (_parent, filtrationType, _tbSearchStr, _btnSearch, _btnReset)
        {

        }
        internal override void doSearch()
        {
            //функция, которая, собственно, парсит строку и выполняет поиск
            if (tbSearchStr == null) return;
            if (tbSearchStr.Text.Trim() == "") return;

            string[] s = tbSearchStr.Text.Split(' ');

            //теперь надо поиск. поиск может быть по всем словам (AND) или только по одному (OR) сделаем пока по AND

            //здесь по or, т.к. смысл такой, что поле А содержит слово, или поле Б содержит слово и т.д.
            Lib.Filter filter = new Lib.Filter( Lib.RIFDC_LogicalOperators.AND);

            //надо сегенить ФЕ вида (полеА-contains-слово1 AND полеА-contains-слово1) ... 

            //List<Lib.FieldInfo> targetFields = parent.dataSource.sampleObject.fieldsInfo.getMySearchableFields();

            Lib.FieldInfo f = parent.dataSource.sampleObject.fieldsInfo.getFieldInfoObjectByFieldClassName("searchable");

            s.ToList().ForEach(piece => {
                    filter.addNewFilteringRule( f, Lib.RIFDC_DataCompareOperatorEnum.contains, piece, Lib.Filter.FilteringRuleTypeEnum.SearchFilteringRule);
            });
            parent.dataSource.filtration.resetSearshFilter(filtrationType);
            parent.applyLocalFilter(filter);
        }

        internal override void doReset()
        {
            // по логике эта штука должна ресетить фильтр, но только тот, что был тут установлен

            tbSearchStr.Text = "";
            parent.dataSource.filtration.resetSearshFilter(filtrationType);
            parent.fillTheForm();
        }
    }


        //ENUMS

        public enum DataGridEditabilityMode
    {
        NotEditableAtAll = 1
    }

    public enum FrmControlTypeEnum
    {
        TextBox = 0,
        CheckBox = 1,
        DataGrid = 2,
        List = 3,
        RichTextBox = 4,
        Button = 5,
        ComboBox = 6,
        DateTimePicker = 7,
        MaskedTextBox = 8,
        Undefined = 99

    }

    public enum FormUpdateModeEnum
    {
        updateFieldOnLeaveImmediately = 1,
        updateWholeRecordOnButtonClick = 2
    }
    public enum FormEventTypeEnum
    {
        btnClick = 1,
        fieldLeft = 2,
        recordSaveNew = 3,
        recordUpdateAttempt = 4,

        currentRecordChangeAttempt = 5,
        currentRecordChanged = 6,
        currentRecordEditCancelAttenpt = 7, //нажатие escape при редактировании записи

        keyPress = 8,
        textBoxChanged = 9,
        checkBoxCheckedChanged = 10,
        comboBoxSelectionChanged = 11,
        dtPickerSelectionChanged = 12,
        dependencyProcessionRequest = 13

    }
    public enum FormBtnTypeEnum
    {
        btnOk = 1,
        btnCancel = 2,
        btnConfirmEdit = 3,
        btnCancelEdit = 4, // это как на форме esc нажимаешь и оно возвращается в исходное состояние
        btnAddNew = 5,
        btnEdit = 6,
        btnDelete = 7,
        btnOrderUp = 8,
        btnOrderDown = 9,
        btnRecordMoveNextRecord = 10,
        btnRecordMovePrevRecord = 11,
        btnSaveRecord = 12,
        btnReloadDataFormComponent = 13,
        btnOpenHistoryForm =14,
        btnOpenExternalCrudForm=15,
        btnSortBySelectedFieldAsc = 30,
        btnSortBySelectedFieldDesc = 31,
        btnToggleMultiSelectionMode = 40,
        btnSelectAll = 41,
        btnSelectNone = 42,


        
        btnOpenGroupOperationsForm = 43,
        
        btnTreeViewAddRootElement = 50,
        btnTreeViewExpandAll = 51,
        btnTreeViewCollapseAll = 52,
       // btnTreeViewCollapseAll = 52,

        btnTestA = 90,
        btnTestB = 91
    }
}

