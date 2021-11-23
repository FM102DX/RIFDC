using CommonFunctions;
using ObjectParameterEngine;
using StateMachineNamespace;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace RIFDC
{
    public class DataForm
    {

    }
    
    public interface IRIFDCCrudForm
    {
        //это форма, которая может принимать сообщения от другой формы
        //нужно чтобы передавать в форму некие начальные значения
        Lib.InterFormMessage startMsg { get; set; }
        void Show();
        Form MdiParent { get; set; }

    }
    public interface IDataFormComponent
    {
        List<string> selectedItemsIds { get; }
        IKeeper dataSource { get; }
    }
    public class DataFormComponent : StateMachine, ObjectMessenger.IObjectMessengerParty, IDataFormComponent, DependencyManager.IDependable
    {
        //класс, упраляющий формой, в которой работает CRUD объектов типа T
        public DataFormComponent(IKeeper _dataSource, Form _myForm, Lib.FrmCrudModeEnum _frmCrudMode, Lib.InterFormMessage incomingObjectMsg = null)
        {
            dataSource = _dataSource;
            form = _myForm;

            currentRecord = new CurrentRecord(this);
            messenger = new ObjectMessenger.ObjectMessengerClient(this, readMyMail);
            dependencyManager = new DependencyManager.DependencyManagerClient(this);
            externalCrudMatrixCollection = new MappedExternalCrudMatrixCollection(this);

            switch (_frmCrudMode)
            {
                case Lib.FrmCrudModeEnum.GridOnly:
                    //crudOperator = new FormCrudOperator_GridOnly(this);
                    break;

                case Lib.FrmCrudModeEnum.StandAloneFrmEdit:
                    break;

                case Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly:

                    crudOperator = new FormCrudOperator_Fly(this);
                    break;

                case Lib.FrmCrudModeEnum.GridAndFieldsSignificant:
                    //FormCrudOperator_GridAndFieldsSignificant op4 = new FormCrudOperator_GridAndFieldsSignificant(this);
                    //crudOperator = op4;
                    break;
            }


            

            if (incomingObjectMsg!=null)
            {
                incomingParentFormFilter = dataSource.sampleObject.getIncomingFilterFromInterFormMessage(incomingObjectMsg);
                dataSource.filtration.applyGlobalFilter(incomingParentFormFilter);

                //и что теперь с ним делать? 
                // ну, применять при каждом чтении из базы, видимо
                // это ReadItems, но ReadItems - это датасорс
                // надо чтобы он применялся при каждом чтен
            }
        }

        //public delegate void DFCEventProcessingDelegate();
        
        //public event actionDelegate FormRefilled;

        public event Action FormRefilled;

        public string tag="";

        public int treeLevelsToShow = -1;  //сколько уровней дерева показывать, если dfc древовидный

        public bool isTreeViewBased
        {
            get
            {
                return dataSource.isTreeViewBased;
            }
        }

        public IKeeper dataSource { get; set; }

        public Form form;
        public IDFCSearchControl searchControl
        {
            get;
            set;
        } = null;

        public Lib.LinePaintingRuleHolder paint = new Lib.LinePaintingRuleHolder();

        MappedExternalCrudMatrixCollection externalCrudMatrixCollection;
        public iFormCrudOperator crudOperator { get; private set; }
        public Lib.FrmCrudModeEnum frmCrudMode { get { return crudOperator.frmCrudMode; } }
        public bool fillViewControlFlag; //TODO это не универсальный код, контролов тех и тех может быть N, и это надо учитывать для каждого отдельно

        private bool _fillEditControlsFlag;
        public bool fillEditControlsFlag
        {
            //чтобы валидаторы не срабатывали, когда ты пытаешься перейти с записи на запись
            get { return _fillEditControlsFlag; }
            set {

            //    fn.dp("");
              //  fn.dp("fillEditControlsFlag changed to" + value.ToString());
              //  fn.dp("");

                _fillEditControlsFlag = value; }
        }


        public bool validatingEditControlFlag;

        //public MappedRecordBasedControls mappedRecordBasedControls;
        //public MappedViewControls MappedGridBasedControls;
            
        public Lib.ObjectOperationResult makeGroupQuery( Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField)
        {
            return dataSource.makeGroupQuery(groupQueryType, targetRelation, targetField);
        }

        public Lib.Filter incomingParentFormFilter=null;

        public CurrentRecord currentRecord;

        public void applyLocalFilter(Lib.Filter filter)
        {
            dataSource.filtration.applyLocalFilter(filter);
            fillTheForm();
        }
        public void applyGlobalFilter( Lib.Filter filter)
        {
            dataSource.filtration.applyGlobalFilter(filter);
            fillTheForm();
        }
        public void resetFilter()
        {
            dataSource.filtration.resetFilter();
            fillTheForm();
        }

        public void imSorted_EventHandler(Lib.Sorter sorter)
        {
            //это функция, которая отрабатывает когда внутри какого-то IGridBasedCtrl происходит сортировка
            //fn.dp(sorter.sortingRuleList.Count.ToString());
            dataSource.sort.applySorter(sorter);
            fillTheForm();
        }

        public void reReadItem (IKeepable x)
        {
            crudOperator.reReadItem(x);
        }

        public ObjectMessenger.ObjectMessengerClient messenger { get; set; }

        public List<string> selectedItemsIds { get { return crudOperator.selectedItemsIds; } }
        
        public void resetMe()
        {
            //для ситуации, когда dfc есть, контролы замаплены, но датасорс не может быть сформирован

            dataSource.clear();
            fillTheForm();

        }
        public void readItems()
        {
            dataSource.readItems();
        }

        public void selectAll()
        {
            crudOperator.selectAll();
        }

        public void selectNone()
        {
            crudOperator.selectNone();
        }

        public string getFieldValueByFieldClassName(string fieldClassName)
        {
            IKeepable x = currentRecord.getMember();
            object x1;

            if (x != null)
            {
                x1 = x.getMyParameter(fieldClassName);
                if (x1 != null)
                {
                    return Convert.ToString(x1);
                }
            }
            return "";

        }

        //private bool _multiSelectionMode;

        public bool multiSelectionMode
        {
            get
            {
                return crudOperator.multiSelectionMode;
            }
            set
            {
                crudOperator.multiSelectionMode=value;
            }
        }
        
        #region dependency_processing

        public DependencyManager.DependencyManagerClient dependencyManager;

        private void readMyMail(List<ObjectMessenger.ObjectMessage> messages)
        {
            //делегат - читалка почты

            foreach (ObjectMessenger.ObjectMessage m in messages)
            {
                if (m.msgType == ObjectMessenger.ObjectMessageTypeNeum.FormManagersCommunicationMessage)
                {
                    if (m.args == null) { continue; }

                    ObjectMessenger.FormmManagerCommunicationMessageArgs args = (ObjectMessenger.FormmManagerCommunicationMessageArgs)m.args;

                    if (args.msgType2 == ObjectMessenger.FormmManagerCommunicationMessageTypeEnum.changeFilter)
                    {
                        //если это команда на изменение фильтра
                        if (args.filter == null) { continue; }

                        //Lib.Filter f = args.filter;
                        //итак, вот фильтр. Теперь надо что-то с ним сделать. Применить как есть? а подойдет? Это ж надо его сформировать в обьекте-отправителе. 
                        //т.е. объект-отправитель должен знать, какое там поле, значение, и какой объект филдинфо.

                        applyGlobalFilter(args.filter);
                        
                    }
                }
            }

        }

        #endregion

        public string className { get { return this.GetType().Name; } } //TODO потворяющийся во многих классах код
        private void reloadMe()
        {
            dataSource.readItems();
            fillTheForm();
        }

        //выставляет видимость контролов в зависимости от того, какая сейчас запись
        private void setCtrlFirstLastVisibility()
        {
            crudOperator.setCtrlFirstLastVisibility();
        }

        public void addGridBasedControlMapping(IGridBasedControl tc) { crudOperator.addGridBasedControlMapping(tc); }
        public void addRecordBasedControlMapping(IRecordBasedControl tc, string _srcDataFieldClassName) { crudOperator.addRecordBasedControlMapping(tc, _srcDataFieldClassName); }
        public void addButtonMapping(RIFDC_Button _mappedPhysicalButton, string _paramStr="") { crudOperator.addButtonMapping(_mappedPhysicalButton, _paramStr); }

        public MappedExternalCrudMatrixCollection.ExternalCrudMatrixMapping addExternalCrudMatrixMaping (Type _targetForm, string _mappingCode) 
        {
           return  externalCrudMatrixCollection.addExternalCrudMatrixMaping(_targetForm, _mappingCode);
        }

        public class MappedExternalCrudMatrixCollection
        {
            //это класс хранит коллекцию форм, в которых можно открыть текущую запись
            //напр, список торговых точек -> открыть операции по этой торговой точке

            public List<ExternalCrudMatrixMapping> items = new List<ExternalCrudMatrixMapping>();

            public DataFormComponent parent;

            public MappedExternalCrudMatrixCollection(DataFormComponent _parent)
            {
                parent = _parent;
            }

            public class ExternalCrudMatrixMapping
            {
            //    public IRIFDCCrudForm  targetForm;
                public Type targetForm;
                public string mappingCode;
                

                public Lib.RelationsPackage realtionspackage = new Lib.RelationsPackage();

                MappedExternalCrudMatrixCollection parent;

                public ExternalCrudMatrixMapping(MappedExternalCrudMatrixCollection _parent)
                {
                    parent = _parent;
                }

                public void addRelation (Relations.Relation r)
                {
                    realtionspackage.addRelation(r);
                }

            }
            public ExternalCrudMatrixMapping addExternalCrudMatrixMaping(Type _targetForm, string _mappingCode)
            {
                ExternalCrudMatrixMapping c = new ExternalCrudMatrixMapping(this);
                c.targetForm = _targetForm;
                c.mappingCode = _mappingCode;
                items.Add(c);
                return c;
            }
            public ExternalCrudMatrixMapping getExternalCrudMatrixMaping(string _mappingCode)
            {
                foreach (ExternalCrudMatrixMapping m in items)
                {
                    if (m.mappingCode == _mappingCode) return m;
                }
                return null;
            }

            public void showForm(string _mappingCode)
            {
                if (_mappingCode == "") return;
                
                ExternalCrudMatrixMapping x = getExternalCrudMatrixMaping(_mappingCode);

                if (x==null) return;

                try
                {
                    IRIFDCCrudForm f = (IRIFDCCrudForm)System.Activator.CreateInstance(x.targetForm);
                    Lib.InterFormMessage m = new Lib.InterFormMessage();
                    m.targetObject = parent.currentRecord.getMember();
                    m.realtionspackage = x.realtionspackage;
                    f.startMsg = m;

                    if (RIFDC_App.mainWindowFrmInstance != null) f.MdiParent = RIFDC_App.mainWindowFrmInstance;

                    f.Show();
                }
                catch
                {

                }
            }



        }
        public class CurrentRecord
        {
            private DataFormComponent x;
            public CurrentRecord(DataFormComponent _x)
            {
                x = _x;
            }

            public bool isFirst()
            {
                return x.dataSource.currentRecord.isFirst();
            }
            public bool isLast()
            {
                return x.dataSource.currentRecord.isLast();
            }


            //вот в чем фигня - когда я делаю мувнекст, в датасорсе меняется ИД, но я этого тут не вижу. Надо, чтобы контролы это считали.
            // ""отобразить текущую запись в контролах"", надо что то типа того.
            public void moveNext()

            {
                if (!isLast()) { x.dataSource.currentRecord.moveNext(); x.reflectCurrentRecordInControls(); }
            }
            public void movePrevious()
            {
                if (!isFirst()) { x.dataSource.currentRecord.movePrevious(); x.reflectCurrentRecordInControls(); }
            }

            public string id
            {
                get { return x.dataSource.currentRecord.id; }
                set
                {
                    x.dataSource.currentRecord.id = value;
                    x.reflectCurrentRecordInControls();
                    x.crudOperator.raiseFormEvent(FormEventTypeEnum.currentRecordChanged);
                }
            }

            public int index
            {
                get { return x.dataSource.currentRecord.index; }
                set
                {
                    x.dataSource.currentRecord.index = value;
                    x.reflectCurrentRecordInControls();
                    x.crudOperator.raiseFormEvent(FormEventTypeEnum.currentRecordChanged);
                }
            }
            public IKeepable getMember() //возвращает текущую запись 
            {
                return x.dataSource.currentRecord.getMember();
            }
        }

        //ставит текущую запись формы в контролы
        public void reflectCurrentRecordInControls()
        {
            crudOperator.reflectCurrentRecordInControls();
        }

        //заполняет элементы formManager-a данными из датасорса
        public void fillTheForm()
        {
            crudOperator.fillTheForm();
            FormRefilled?.Invoke(); 
        }

        public void openMyHistoryFrm()
        {
            IKeepable item = dataSource.currentRecord.getMember();

            if (item == null) return;

            Lib.InterFormMessage msg = Lib.InterFormMessage.getInstance(item);

            dataSource.openMyHistoryFrm(msg);
        }
        
        public void openGroupOperationsFrm()
        {
            //открывает форму, где можно делать групповые операции с объектами
            Lib.InterFormMessage msg = new Lib.InterFormMessage
            {
                caller = this,
                targetKeeper = dataSource,
                selectedItemsIds = this.selectedItemsIds
            };

            if (selectedItemsIds.Count==0 && this.multiSelectionMode==true)
            {
                fn.mb_info("Элементы не выбраны");
                return;
            }

            GroupOperationsFrm frm = new GroupOperationsFrm();
            frm.startMsg = msg;
            frm.ShowDialog();
        }

        public string entityType
        {
            get
            {
                return dataSource.sampleObject.entityType;
            }
        }

        // это контролы, где отображаются списки объектов

        public void EventHandler_Common_ButtonCLick(object sender, EventArgs e)
        {
            //нажатие замапленной кнопки. таким образом, на кнопки не надо вешать вручную обработчики.
            crudOperator.raiseFormEvent(FormEventTypeEnum.btnClick, sender);
        }

        public interface iFormCrudOperator
        {
            //реализует один из 4х способов CRUD. Работает по паттерну Стратегия.
            Lib.FrmCrudModeEnum frmCrudMode { get; }
            EventHandlerCollection_RecordBasedControl eventHandlers_RecordBC { get; set; }
            EventHandlerCollection_GridBasedControl eventHandlers_GridBC { get; set; }

            void selectAll();

            void selectNone();

            List<string> selectedItemsIds { get; }
            void reReadItem(IKeepable x);
            void reflectCurrentRecordInControls();
            void fillTheForm(string idToSet = "");
            bool multiSelectionMode { get; set; }
            void addGridBasedControlMapping(IGridBasedControl tc);
            void addRecordBasedControlMapping(IRecordBasedControl tc, string _srcDataFieldClassName);
            void addButtonMapping(IRIFDCButton _mappedPhysicalButton, string _paramStr="");
            void processFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null);
            void raiseFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null);
            void setCtrlFirstLastVisibility();
            Lib.EventProcessigResult addNewRecord(bool addRootElement= false);
            Lib.EventProcessigResult saveRecord(Control c);
            void doCancelEdit();

            //   void addEventHandlers_GridBasedControls(DataFormComponent.MappedGridBasedControls.MappedGridBasedControl m);

            //     void addEventHandlers_RecordBasedControls(DataFormComponent.MappedRecordBasedControls.MappedRecordBasedControl m);
            void addCommonEventHandlers();

            void prepareStateActions(); //забить туда контролы, которые будут показываться / скрываться в зависимости от состояния формы

            void expandAll();
            void collapseAll();

        }

        public class FormCrudOperator_Fly : CrudOperatorPattern, iFormCrudOperator
        {
            public FormCrudOperator_Fly(DataFormComponent _parent) : base(_parent)
            {
                /* 
                 * отдано в абстрактных класс-предок
                 * parent = _parent;
                 * 
                eventHandlers_RecordBC = new EventHandlerCollection_RecordBasedControl();
                eventHandlers_GridBC = new EventHandlerCollection_GridBasedControl();

                а здесь мы только занимемся обвешиванием этих объектов методами
                */

                eventHandlers_RecordBC.textBox_TextChanged_Processor = EventHandler_Fly_TextBoxTextChanged; //посимвольный ввод, готово
                eventHandlers_RecordBC.textBox_Leave_Processor = EventHandler_Fly_TextBoxLeave; //
                eventHandlers_RecordBC.dTPicker_ValueChanged_Processor = EventHandler_Fly_dtPickerValueChanged;
                eventHandlers_RecordBC.checkBox_CheckedChanged_Processor = EventHandler_Fly_CheckBoxCheck;
                eventHandlers_RecordBC.ComboBox_SelectedIndexChanged_Processor = EventHandler_Fly_ComboBoxChange;

                eventHandlers_GridBC.dataGridView_SelectionChanged_Processor = EventHandler_dg_SelectionChanged;

                eventHandlers_GridBC.treeView_SelectionChanged_Processor = EventHandler_TreeView_SelectionChanged;

                addCommonEventHandlers();
                prepareStateActions();
            }

            //процессинг событий контролов

            public void EventHandler_Fly_TextBoxTextChanged(object sender, EventArgs e)
            {
                //процессинг посимвольного контроля ввода, т.е. какие-то символы можно ввести, какие-то - нет

                //fn.dp("Called EventHandler_Fly_TextBoxTextChanged");
                //parent.fillEditControlsFlag = false;

                if (parent.fillEditControlsFlag) { /* tb.Tag = tb.Text; */ return; }
                if (parent.validatingEditControlFlag) { return; }
                parent.validatingEditControlFlag = true;

                //raiseFormEvent(FormEventTypeEnum.textBoxChanged, sender);
                //валидация посимвольного ввода
                Fly_ProcessTextBoxTextChanged(sender);

                parent.validatingEditControlFlag = false;
            }

            private void EventHandler_dg_SelectionChanged(object sender, EventArgs e)
            {
                DataGridView _dg = (DataGridView)sender;

                if (!parent.fillViewControlFlag)
                {           //TODO да, это надо куда-то спрятать
                    if (_dg.Rows.Count > 0 && _dg.SelectedRows.Count > 0)
                    {
                        parent.crudOperator.raiseFormEvent(FormEventTypeEnum.currentRecordChangeAttempt, _dg);
                    }
                }

            }
            private void EventHandler_TreeView_SelectionChanged(object sender, EventArgs e)
            {
                TreeView _tvr = (TreeView)sender;

                if (!parent.fillViewControlFlag)
                {           //TODO да, это надо куда-то спрятать
                    if (_tvr.Nodes.Count > 0)
                    {
                        parent.crudOperator.raiseFormEvent(FormEventTypeEnum.currentRecordChangeAttempt, _tvr);
                    }
                }

            }

            public void EventHandler_Fly_CheckBoxCheck(object sender, EventArgs e)
            {
                    controlSimpleChange(sender, e);
                }
            public void EventHandler_Fly_dtPickerValueChanged(object sender, EventArgs e)
            {
                controlSimpleChange(sender, e);
            }

            public void EventHandler_Fly_ComboBoxChange(object sender, EventArgs e)
            {
                controlSimpleChange(sender, e);
            }

            public void reReadItem(IKeepable x)
            {
                mappedGridBasedControls.reReadItem(x);
            }

            public void controlSimpleChange(object sender, EventArgs e)
            {
                
                if (parent.fillEditControlsFlag) { return; }

                //raiseFormEvent(FormEventTypeEnum.comboBoxSelectionChanged, sender);
                //если юзер что-то выбрал в комбобоксе
                //TODO а как же leave-валидация в случае комбобоксов? ну по идее там нельзя выбрать что-то, что не в списке
                
                Control c = (Control)sender;

                Lib.EventProcessigResult r0 = saveRecord(c);

                if (!r0.success)
                {
                    fn.mb_info(r0.msg);

                    //теперь еще надо не дать уйти с поля
                    c.Focus();
                }
            }

            public void EventHandler_Fly_TextBoxLeave(object sender, EventArgs e)
            {
                //хендлер события TextBoxLeave в режиме Fly
                if (parent.validatingEditControlFlag) { return; }
                if (parent.fillEditControlsFlag) { return; }
                parent.validatingEditControlFlag = true;

                //уход с поля данных
                //проверить, изменилось или нет
                // а если он ушел, а там как было пустое поле так и оставлось -- это dirty -- 

                bool leaveChk = Fly_ProcessTextBoxLeave(sender); //проверка на leave

                if (!leaveChk) return;

                MappedRecordBasedControls.MappedRecordBasedControl tbb = mappedRecordBasedControls.getControlMapping((Control)sender);

                //if (!tbb.isDirty) return;  // правильнее проверять dirty в saveRecord

                //MappedRecordBasedControls.MappedRecordBasedControl m = mappedRecordBasedControls.getControlMapping((Control)sender);
                Lib.EventProcessigResult r = saveRecord((Control)sender); //сохранение текущей записи

                if (!r.success)
                {
                    fn.mb_info(r.msg);
                    tbb.targetControl.Focus(); //не дать уйти с поля
                    tbb.targetControl.selectionStart = fn.toStringNullConvertion(tbb.getTargetControlValue()).Length;
                }

                parent.validatingEditControlFlag = false;
            }

            public void addCommonEventHandlers()
            {
                this.parent.form.KeyDown += Form_KeyDown_event_handler;
            }

            private void Form_KeyDown_event_handler(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape) raiseFormEvent(FormEventTypeEnum.currentRecordEditCancelAttenpt);
            }


            //процессоры обработчиков событий TODO может, они и не будут нужны, если я перенесу это в отдельный класс?
            public void Fly_ProcessTextBoxTextChanged(object sender)
            {
                //посимвольный контроль ввода, т.е. какие-то символы можно ввести, какие-то - нет
                //в общем, раньше тут был анализатор текста на предмет что поменялось, теперь просто проверка сего текста при каждом изменении поля

                MappedRecordBasedControls.MappedRecordBasedControl tbb = mappedRecordBasedControls.getControlMapping((Control)sender);
                if (!tbb.validationIsOn_symbol) return;
                int cursor = tbb.targetControl.selectionStart;
                int rezPos;
                string newText;
                string newTextValidated;

                // string oldText;

                newText = fn.toStringNullConvertion(tbb.getTargetControlValue());

                // if (tbb.targetControl.targetControl.Tag == null) oldText = ""; else oldText = tbb.targetControl.targetControl.Tag.ToString();

                // Validation.ValidationResult vr = tbb.makeSymbolValidation();

                Validation.ValidationResult vr = parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.symbol, tbb.srcDataFieldClassName, tbb.targetControl.targetControl.Text);
                newTextValidated = vr.validatedValue.ToString();
                tbb.targetControl.targetControl.Text = newTextValidated;
                int delta = newText.Length - newTextValidated.Length;

                rezPos = cursor - delta;
                rezPos = (rezPos < 0) ? 0 : rezPos;

                tbb.targetControl.selectionStart = rezPos;

                #region oldSymbolValidationText

    
                #endregion

            }

            public bool Fly_ProcessTextBoxLeave(object sender)
            {
                //это leave-проверка в режиме флай
                bool rez = true;
                Control tb = (Control)sender;
                MappedRecordBasedControls.MappedRecordBasedControl m = mappedRecordBasedControls.getControlMapping(tb);
                
                object value = m.getTargetControlValue();

                bool itsNull = (value == null);

                //if (m.isDirty && m.validationIsOn_leave) //проверять в любом случае, т.к. может оно из базы приехало такое
                if (m.validationIsOn_leave) //можно и состояние проверять, но dirty надежнее
                {
                    Validation.ValidationResult vr = parent.dataSource.sampleObject.validateMyParameter(
                                Validation.ValidationTypeEnum.leave,
                                m.srcDataFieldClassName,
                                value); //и вот это я тащу аж из объекта
                    if (vr.validationSuccess)
                    {
                        //если leave валидация success, ставим возвращенное value и уходим с поля
                        m.setTargetControlValue(vr.validatedValue);
                        parent.currentState = (int)FormStateEnum.frmStateView;
                    }
                    else
                    {
                        //если leave валидация !success, возвращаем пользователя на поле, показываем сообщение
                        tb.Focus();
                        fn.mb_info(vr.validationMsg);
                    }
                    rez = vr.validationSuccess;
                }

                //это на случай, когда оно не dirty, но надо оставить в поле значение, которое соответствует null
               // if ((!m.isDirty) && itsNull) m.setTargetControlValue(null);

                parent.validatingEditControlFlag = false;
                return rez;
            }

            public List<string> selectedItemsIds
            {
                get
                {
                    
                    if (mappedGridBasedControls.items.Count>0)
                    {
                        if (multiSelectionMode)
                        {
                            MappedGridBasedControls.MappedGridBasedControl mgc = mappedGridBasedControls.getMultiSelectRepresentableControl();

                            if (mgc != null)
                            {
                                return mgc.targetControl.selectedItemsIds;
                            }
                            else
                            { 
                                return new List<string>(); 
                            }
                        }
                        else
                        {
                            List<string> rez = new List<string>();
                            IKeepable x = parent.currentRecord.getMember();
                            if (x!=null)
                            {
                                rez.Add(x.id);
                            }
                            return rez;
                        }
                    }
                    else
                    {
                        return new List<string>();
                    }
                    
                }
            }

            public void reflectCurrentRecordInControls()
            {
                //отобразить текущую запись в контролах
                parent.fillViewControlFlag = true;
                mappedGridBasedControls.setCurrentId(parent.currentRecord.id);
                parent.fillViewControlFlag = false;

                fillEditControls(parent.currentRecord.getMember());
                
                setCtrlFirstLastVisibility();
            }
            public void fillEditControls(IKeepable t)
            {
                //заполняет edit-контролы формы переменной типа T исходя из маппинга

                if (t == null) return;

                parent.fillEditControlsFlag = true;
                ObjectParameters.ObjectParameter p;
                object tmp1 = null;

                foreach (MappedRecordBasedControls.MappedRecordBasedControl mc in mappedRecordBasedControls.items)
                {
                    //Logger.log("VIEW", "getting parameter " + mc.srcDataFieldClassName);
                    tmp1 = t.getMyParameter(mc.srcDataFieldClassName);
                    //Logger.log("VIEW", tmp1);
                    mc.setTargetControlValue(tmp1);
                }
                parent.fillEditControlsFlag = false;

            }

            public void fillTheForm(string idToSet="")
            {

                //если в датасорсе вообще что-нибудь есть
                //  if (dataSource.items.count > 0)

                parent.fillViewControlFlag = true;

                //заполнить все вью-контролы
                mappedGridBasedControls.fillAllControls();

                //сделать первую запись в датасорсе текущей
                if (parent.dataSource.count > 0)
                {
                    if (idToSet == "")
                    {
                        parent.currentRecord.index = 0;
                    }
                    else
                    {
                        parent.currentRecord.id = idToSet;
                    }
                    parent.currentState = (int)FormStateEnum.frmStateView;
                }
                else
                {
                    parent.currentState = (int)FormStateEnum.frmStateLockedNoRecords;
                }

                parent.fillViewControlFlag = false;

            }
            public void setCtrlFirstLastVisibility()
            {
                bool first = parent.currentRecord.isFirst();
                bool last = parent.currentRecord.isLast();
                List<MappedButtons.MappedButton> ml_next = mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMoveNextRecord);
                List<MappedButtons.MappedButton> ml_prev = mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMovePrevRecord);

                if ((!first) && (!last))
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked= false; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = false; }
                }
                if (first)
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked = false; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = true; }
                }
                if (last)
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked = true; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = false; }
                }
            }

            #region stateActions
            public void prepareStateActions()
            {
               // StateMachine.actionDelegate setElementsLocked = _setElementsLocked;
                // StateMachine.actionDelegate setElementsUnLocked = _setElementsUnLocked;

                parent.addDelegateAction((int)FormStateEnum.frmStateLockedNoRecords, ActionOnEnterOrLeaveEnum.Enter, setElementsLockedNoData);
                parent.addDelegateAction((int)FormStateEnum.frmStateLockedNoRecords, ActionOnEnterOrLeaveEnum.Leave, setElementsUnLockedNoData);
            }

            //lock на нет данных
            private void setElementsLockedNoData(){toggleElementNoDataLock(true);}
            private void setElementsUnLockedNoData() { toggleElementNoDataLock(false); }
            private void toggleElementNoDataLock (bool value)
            {
                mappedRecordBasedControls.items.ForEach(x => x.targetControl.locked = value);
                mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMoveNextRecord).ForEach(x => x.buttonObject.locked = value);
                mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMovePrevRecord).ForEach(x => x.buttonObject.locked = value);
                mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMovePrevRecord).ForEach(x => x.buttonObject.locked = value);
            }


            #endregion

            public bool multiSelectionMode
            {
                get
                {
                    return mappedGridBasedControls.multiSelectionMode;
                }
                set
                {
                    mappedGridBasedControls.multiSelectionMode = value;
                }
            }

            public Lib.EventProcessigResult addNewRecord(bool addRootElement=false)
            {
                //добавление новой записи 

                // в режиме fly новая запись создается сразу, со значениями по умолчанию
                doCancelEdit();

                if (!addRootElement && (parent.dataSource.count == 0)) return Lib.EventProcessigResult.sayNo("Нет родительского элемента"); 

                IKeepable t = null;
                IKeepable parentObject = null;

                t = parent.dataSource.createNewObject_notInserted();

                if (parent.isTreeViewBased)
                {
                    if (!addRootElement) parentObject = parent.dataSource.currentRecord.getMember();
                    if (parentObject != null) t.setMyParameter("parentId", parentObject.id);
                }

                //Здесь ситуация ткова, что этот объект фильтруется многими входящими
                //их надо все перебарть и в зависимые поля этого объекта поставить текущие значения матер - полей parent-объектов, которые были переданы через фильтр

                    // 1 получить все входящие зависимости этого объекта
                    // getMyDFCDependencies 
                    // предполагаем, что если оно фильтруется чем-то по зависимому полю, то это поле надо из фильтра заполнять

                t.saveMyPhoto();

                //сохраняем в базу
                Lib.ObjectOperationResult rez = parent.dataSource.saveItem(t);
                if (rez.success == false)
                {
                    return Lib.EventProcessigResult.getInstance(false, rez.msg);
                }

                string id = rez.createdObjectId;

                //добавляем в датасорс
                parent.dataSource.addExistingObject(t);

                //обновляем вью-контролы, чтобы ее стало видно
                // эта запись должна добавиться в контрол в общем порядке сортировки

                parent.fillTheForm();

                //установить текущую запись формы
                parent.currentRecord.id = id;

                //тут должно быть изменение режима формы, т.е. каждый раз как мы перезаливаем контролы и ставим id, надо смотреть режим формы

                //parent.MappedGridBasedControls.setCurrentId(id); 

                return null;
            }
            public Lib.EventProcessigResult saveRecord(Control c)
            {
                //сохранить запись

                //проверить, изменилось ли поле
                
                MappedRecordBasedControls.MappedRecordBasedControl m = mappedRecordBasedControls.getControlMapping(c);
                
                if (m == null) 
                {
                    return Lib.EventProcessigResult.getInstance(false, "Не найден маппинг контрола " + c.Name);
                }

                if (!m.isDirty) 
                {
                    //если данные не изменились, то нечего апдейтить
                    return Lib.EventProcessigResult.sayOk();
                } 

                //валидация по бизнес-правилам
                if (m.validationIsOn_business)
                {
                    Validation.ValidationResult vr = parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.business, m.srcDataFieldClassName, m.getTargetControlValue());
                    if (vr.validationSuccess == false)
                    {
                        return Lib.EventProcessigResult.sayNo(vr.validationMsg);
                    }
                }

                object tmp;
                object tmp0;

                //найти ту запись, которую надо изменить
                foreach (IKeepable t in parent.dataSource.items)
                {
                    if (t.id == parent.dataSource.currentRecord.id) //запись найдена
                    {
                        //установить этот параметр
                        //nullability учитывается в объекте
                        //текущее значение параметра в объекте, на случай, если придется возвращать все обратно
                        tmp0 = t.getMyParameter(m.srcDataFieldClassName); 

                        //значение из контрола
                        tmp = m.getTargetControlValue();

                        
                        //сохранение в объекте
                        Lib.ObjectOperationResult setParameterResult = t.setMyParameter(m.srcDataFieldClassName, tmp);

                        if (!setParameterResult.success)
                        {
                            //вернуть в поле старое значение и выход
                            m.setTargetControlValue(tmp0);
                            
                            return Lib.EventProcessigResult.sayNo("Введенное значение не подходит для данного поля! Ошибка: "+setParameterResult.msg);
                        }

                        //сохранение в базе в режиме "сохранять только те поля, что были изменены"
                        Lib.ObjectOperationResult or= parent.dataSource.saveItem(t);

                        if (or.success)
                        {
                            t.saveMyPhoto();

                            //надо, чтобы вью-контролы перечитали в себя эту запись, т.к. там могут быть изменения
                            mappedGridBasedControls.reReadItem(t);

                            //выставить в контрол то, что реально было сохранено в объект
                            m.setTargetControlValue(tmp);


                            return Lib.EventProcessigResult.sayOk();
                        }
                        else
                        {
                            //возвращаем старое значение параметра
                            t.setMyParameter(m.srcDataFieldClassName, tmp0);
                            return Lib.EventProcessigResult.sayNo(or.msg);
                        }
                    }
                }
                return Lib.EventProcessigResult.sayNo("Unknown error occured");
            }
            public void doCancelEdit() //отмена редактирования
            {
                if (parent.dataSource.count == 0) return; //TODO обрабоать как то более формально
                foreach (MappedRecordBasedControls.MappedRecordBasedControl m in mappedRecordBasedControls.items)
                {
                    if (m.isDirty) m.restoreTargetControlValue();
                }
                parent.currentState = (int)FormStateEnum.frmStateView;
            }

            public void processFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null)
            {
                //процессит события, происходящие в форме: переход из поля в поле, попытка обновить запись, нажатие кнопок
                //например, отлавливает переход из поля в поле и не дает уйти, если включен соотв. вид валидации

                //DataSet d = new DataSet();

                Control c;

                switch (formEventType)
                {
                    //keyPress
                    case FormEventTypeEnum.keyPress:
                        //нажата клавиша в форме
                        if (ke.KeyCode == Keys.Escape)
                        {
                            raiseFormEvent(FormEventTypeEnum.currentRecordEditCancelAttenpt);
                        }
                        break;

                    //textBoxChanged
                    case FormEventTypeEnum.textBoxChanged:

                        break;

                    //checkBoxCheckedChanged

                    //fieldLeft
                    case FormEventTypeEnum.fieldLeft:

                        break;

                    //btnClick 
                    case FormEventTypeEnum.btnClick:
                        //нажата кнопка
                        //надо найти, что замаплено на эту кнопку и вызвать это

                        MappedButtons.MappedButton mb = mappedButtons.getButtonMapping(sender);

                        if (mb == null) { return; }

                        switch (mb.buttonObject.btnType)
                        {
                            case FormBtnTypeEnum.btnAddNew:
                                addNewRecord();
                                break;

                            case FormBtnTypeEnum.btnTreeViewAddRootElement:
                                addNewRecord(true);
                                break;

                            case FormBtnTypeEnum.btnEdit: //это будет только в 2, где окно открывается; пока не делаем

                                break;

                            case FormBtnTypeEnum.btnCancelEdit:
                                doCancelEdit();
                                break;

                            case FormBtnTypeEnum.btnDelete:

                                //удалить запись
                                bool b;

                                List<string> processionIdList = parent.selectedItemsIds;

                                b = fn.mb_confirmAction($"Будет удалено записей: {processionIdList.Count}. Продолжить?") ;

                                if (!b) return;

                                List<IKeepable> processionObjectList = new List<IKeepable>();

                                processionIdList.ForEach(x => {

                                        processionObjectList.Add(parent.dataSource.getItemById(x));

                                });

                                List<Lib.ObjectOperationResult> rez = parent.dataSource.deleteMultipleItems(processionObjectList);

                                Lib.MultipleOperationHandler mult = new Lib.MultipleOperationHandler(rez);

                                fn.mb_info(mult.rezultMsgText1);



                                /*
                                if (parent.multiSelectionMode)
                                {

                                    //теперь надо взять и перебрать все выделенные 
                                    //наверное DFC.selectedItemIds - делать ID
                                    parent.selectedItemsIds.ForEach(x=> {

                                        

                                    });
                                }
                                else
                                {
                                    b = fn.mb_confirmAction("Удалить?");

                                    if (!b) return;

                                    int neededPosition = parent.currentRecord.index + 1;

                                    Lib.ObjectOperationResult or = parent.dataSource.deleteItem(parent.currentRecord.getMember());

                                    if (or.success)
                                    {
                                        parent.fillTheForm();
                                        //сохранить курсор в той же позиции в гриде после удаления
                                        int count = parent.dataSource.count;
                                        if (count < neededPosition) { neededPosition = count; }
                                        parent.currentRecord.index = neededPosition - 1;

                                    }
                                    else
                                    {
                                        fn.mb_info($"Невозможно удалить элемент: {or.msg}");
                                    }
                                }

                                */

                                break;


                            case FormBtnTypeEnum.btnRecordMoveNextRecord:
                                parent.currentRecord.moveNext();
                                break;

                            case FormBtnTypeEnum.btnRecordMovePrevRecord:
                                parent.currentRecord.movePrevious();
                                break;

                            case FormBtnTypeEnum.btnReloadDataFormComponent:
                                parent.reloadMe();
                                break;

                            case FormBtnTypeEnum.btnOpenHistoryForm:
                                parent.openMyHistoryFrm();
                                break;

                            case FormBtnTypeEnum.btnOpenGroupOperationsForm:
                                parent.openGroupOperationsFrm();
                                break;

                            case FormBtnTypeEnum.btnOpenExternalCrudForm:
                                fn.ParamStringManager pm = new fn.ParamStringManager(mb.paramStr);
                                string code = pm.getParamValue("mappingCode");
                                parent.externalCrudMatrixCollection.showForm(code);
                                break;

                            case FormBtnTypeEnum.btnSaveRecord:
                                // тут можно ничего не делать, кнопка это просто место, куда перемещается курсор
                                // вызывая событие leave контрола
                                break;

                            case FormBtnTypeEnum.btnSortBySelectedFieldAsc:
                                //тут надо пересортировать коллекцию по некому полю

                                // теперь надо как-то взять sorting rule

                                //для этого нужен fieldInfoObject того контрола, на котором сейчас фокус
                                // да, но ведь фокус может стоять где угодно, когда нажата кнопка сортировать.
                                // значит, надо взять form.focused 
                                // и сфокусированы контро - это поведени dfc.
                                //нАВЕРНОе, стоит ввести параметр fieldInfo.sortable, если оно true, то по этому полю сортировать можно

                                MappedRecordBasedControls.MappedRecordBasedControl xAsc= mappedRecordBasedControls.currentlyFocused;
                                if (xAsc == null ) return;
                                Lib.Sorter sr_asc = new Lib.Sorter();
                                sr_asc.addNewSortingRule(xAsc.fieldInfoObject, Lib.AscDescSortEnum.Asc);
                                parent.dataSource.sort.applySorter(sr_asc);
                                parent.fillTheForm(); // Todo надо чтобы он ставил ту же текущую запись
                                xAsc.targetControl.focus();
                                break;
                            
                            case FormBtnTypeEnum.btnSortBySelectedFieldDesc:
                                MappedRecordBasedControls.MappedRecordBasedControl xDesc = mappedRecordBasedControls.currentlyFocused;
                                if (xDesc == null) return;

                                Lib.Sorter sr_desc = new Lib.Sorter();
                                sr_desc.addNewSortingRule(xDesc.fieldInfoObject, Lib.AscDescSortEnum.Desc);
                                parent.dataSource.sort.applySorter(sr_desc);
                                parent.fillTheForm(); // Todo надо чтобы он ставил ту же текущую запись
                                xDesc.targetControl.focus();
                                break;



                            case FormBtnTypeEnum.btnToggleMultiSelectionMode:
                                parent.multiSelectionMode = !parent.multiSelectionMode;
                                break;

                            case FormBtnTypeEnum.btnSelectAll:
                                parent.selectAll();
                                break;

                            case FormBtnTypeEnum.btnSelectNone:
                                parent.selectNone();
                                break;

                            case FormBtnTypeEnum.btnTreeViewExpandAll:
                                parent.crudOperator.expandAll();
                                break;
                            case FormBtnTypeEnum.btnTreeViewCollapseAll:
                                parent.crudOperator.collapseAll();
                                break;



                        }
                        break;

                    case FormEventTypeEnum.currentRecordChangeAttempt:
                        //TODO кнопки туда и сюда тоже вызывают это событие
                        //взять вью контрол, с которого это приехало
                        c = (Control)sender;
                        MappedGridBasedControls.MappedGridBasedControl v = mappedGridBasedControls.getControlMapping(c);
                        if (v != null)
                        {
                            string id = Convert.ToString(v.targetControl.getValue());
                            parent.currentRecord.id = id;
                        }

                        break;


                    case FormEventTypeEnum.currentRecordChanged:

                        raiseFormEvent(FormEventTypeEnum.dependencyProcessionRequest);

                        break;

                    case FormEventTypeEnum.currentRecordEditCancelAttenpt:
                        //попытка отменить сделанные изменения при редактировании записи - нажатие escape
                        // Если это сигн - восстановить форму из объекта
                        // если это флай - восстановить текущее поле из объекта

                        doCancelEdit();
                        break;


                    //контролы, где нет валидации
                    case FormEventTypeEnum.checkBoxCheckedChanged:

                    case FormEventTypeEnum.dtPickerSelectionChanged:

                    case FormEventTypeEnum.comboBoxSelectionChanged:

                        break;

                    case FormEventTypeEnum.dependencyProcessionRequest:

                        DependencyManager.DependencyArgs d_args = new DependencyManager.DependencyArgs();

                        d_args.targetFieldId = parent.currentRecord.id;
                        parent.dependencyManager.processMyDependencies(d_args);
                        break;

                }
            }
            public Lib.FrmCrudModeEnum frmCrudMode { get { return Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly; } }
            //инициация событий формы
            public void raiseFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null)
            {
                processFormEvent(formEventType, sender, e, ke);
            }

            public enum FormStateEnum
            {
                frmStateView = 1, //обычный просмотр
                frmStateEdit = 2, // user редактирует поле
                frmStateLockedNoRecords = 3 // нет записей, доступно только добавление
            }

            public void expandAll()
            {
                mappedGridBasedControls.expandAll();
            }
            public void collapseAll()
            {
                mappedGridBasedControls.collapseAll();
            }

        }


        public class FormCrudOperator_GridOnly : CrudOperatorPattern // iFormCrudOperator
        {
            public FormCrudOperator_GridOnly(DataFormComponent _parent) : base(_parent)
            {
                eventHandlers_GridBC.dataGridView_SelectionChanged_Processor = _dg_SelectionChanged;
            }
            public bool multiSelectionMode { get; set; }
            //процессинг событий контролов
            private void _dg_SelectionChanged(object sender, EventArgs e)
            {
                DataGridView _dg = (DataGridView)sender;

                if (!parent.fillViewControlFlag)
                {           //TODO да, это надо куда-то спрятать
                    if (_dg.Rows.Count > 0 && _dg.SelectedRows.Count > 0)
                    {
                        parent.crudOperator.raiseFormEvent(FormEventTypeEnum.currentRecordChangeAttempt, _dg);
                    }
                }
            }

            
            public void addCommonEventHandlers()
            {
                this.parent.form.KeyDown += Form_KeyDown_event_handler;
            }

            private void Form_KeyDown_event_handler(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape) raiseFormEvent(FormEventTypeEnum.currentRecordEditCancelAttenpt);
            }


            public void reflectCurrentRecordInControls()
            {
                //отобразить текущую запись в контролах
                parent.fillViewControlFlag = true;
                mappedGridBasedControls.setCurrentId(parent.currentRecord.id);
                parent.fillViewControlFlag = false;

               // fillEditControls(parent.currentRecord.getMember());
               //setCtrlFirstLastVisibility();
            }
            public void fillTheForm(string idToSet = "")
            {

                //если в датасорсе вообще что-нибудь есть
                //  if (dataSource.items.count > 0)

                parent.fillViewControlFlag = true;

                //заполнить все вью-контролы
                mappedGridBasedControls.fillAllControls();

                //сделать первую запись в датасорсе текущей
                if (parent.dataSource.count > 0) parent.currentRecord.index = 0;

                parent.fillViewControlFlag = false;

            }
            public void setCtrlFirstLastVisibility()
            {
                bool first = parent.currentRecord.isFirst();
                bool last = parent.currentRecord.isLast();
                List<MappedButtons.MappedButton> ml_next = mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMoveNextRecord);
                List<MappedButtons.MappedButton> ml_prev = mappedButtons.getButtonsOfType(FormBtnTypeEnum.btnRecordMovePrevRecord);

                if ((!first) && (!last))
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked = false; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = false; }
                }
                if (first)
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked = false; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = true; }
                }
                if (last)
                {
                    foreach (MappedButtons.MappedButton m in ml_next) { m.buttonObject.locked = true; }
                    foreach (MappedButtons.MappedButton m in ml_prev) { m.buttonObject.locked = false; }
                }

            }

            public void prepareStateActions()
            {

            }
            public Lib.EventProcessigResult addNewRecord()
            {
               /* doCancelEdit();

                //получаем новую запись со значениями по умолчани
                IKeepable t = parent.dataSource.createNewObject_notInserted();
                t.hash = fn.randomString(10);

                //сохраняем в базу
                string id = parent.dataSource.saveItem(t).createdObjectId;
                // t.saveMyPhoto();

                //добавляем в датасорс
                parent.dataSource.addExistingObject(t);

                //обновляем вью-контролы, чтобы ее стало видно
                // эта запись должна добавиться в контрол в общем порядке сортировки

                mappedGridBasedControls.fillAllControls();

                t.hash = ""; //он ее сохранит, и таким образом хеш сотрется

                parent.currentRecord.id = id;

                //parent.MappedGridBasedControls.setCurrentId(id); 
                */

                return null;
            }
            public Lib.EventProcessigResult saveRecord(Control c)
            {
                //сохранить запись, когда мы сохраняем каждое поле в записи по отдельности, т.е. по мере апдейта

                //проверить, изменилось ли поле
                Lib.EventProcessigResult r = new Lib.EventProcessigResult();
                MappedRecordBasedControls.MappedRecordBasedControl m = mappedRecordBasedControls.getControlMapping(c);
                r.success = false;
                if (m == null) { r.success = false; r.msg = "Не найден маппинг контрола " + c.Name; return r; }
                if (!m.isDirty) { r.success = true; return r; } //если данные не изменились, то нечего апдейтить

                //валидация по бизнес-правилам

                Validation.ValidationResult vr = parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.business, m.srcDataFieldClassName, m.getTargetControlValue());
                if (vr.validationSuccess == false)
                {
                    r.success = false;
                    r.msg = vr.validationMsg;
                    return r;
                }

                object tmp;
                //найти ту запись, которую надо изменить
                foreach (IKeepable t in parent.dataSource.items)
                {
                    if (t.id == parent.dataSource.currentRecord.id) //запись найдена
                    {
                        //установить этот параметр
                        //nullability учитывается в объекте

                        tmp = m.getTargetControlValue();
                        ObjectParameters.setObjectParameter(t, m.srcDataFieldClassName, tmp);

                        parent.dataSource.saveItem(t);
                        //t.saveMyPhoto();

                        //надо, чтобы вью-контролы перечитали в себя эту запись, т.к. там могут быть изменения
                        mappedGridBasedControls.reReadItem(t);

                        r.success = true;
                        return r;
                    }
                }
                r.success = false;
                r.msg = "запись не найдена в датасорсе";
                return r;
            }

            public void doCancelEdit() //отмена редактирования
            {
              /*  foreach (MappedRecordBasedControls.MappedRecordBasedControl m in mappedRecordBasedControls.items)
                {
                    if (m.isDirty) m.restoreTargetControlValue();
                } */
               // parent.currentState = FormStateEnum;
            }

            public void processFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null)
            {
                //процессит события, происходящие в форме: переход из поля в поле, попытка обновить запись, нажатие кнопок
                //например, отлавливает переход из поля в поле и не дает уйти, если включен соотв. вид валидации

                Control c = (Control)sender;
                //Logger.log("EVENT", "");
                

                switch (formEventType)
                {
                    //keyPress
                    case FormEventTypeEnum.keyPress:
                        //нажата клавиша в форме
                        if (ke.KeyCode == Keys.Escape)
                        {
                            raiseFormEvent(FormEventTypeEnum.currentRecordEditCancelAttenpt);
                        }
                        break;

                    //textBoxChanged
                    case FormEventTypeEnum.textBoxChanged:
                        break;

                    //checkBoxCheckedChanged

                    //fieldLeft
                    case FormEventTypeEnum.fieldLeft:

                        break;

                    //btnClick 
                    case FormEventTypeEnum.btnClick:
                        //нажата кнопка
                        //надо найти, что замаплено на эту кнопку и вызвать это
                        MappedButtons.MappedButton mb = mappedButtons.getButtonMapping(sender);

                        if (mb == null) { return; }

                        switch (mb.buttonObject.btnType)
                        {
                            case FormBtnTypeEnum.btnAddNew:
                                addNewRecord();
                                break;

                            case FormBtnTypeEnum.btnEdit: //это будет только в 2, где окно открывается; пока не делаем

                                break;

                            case FormBtnTypeEnum.btnCancelEdit:
                                doCancelEdit();
                                break;

                            case FormBtnTypeEnum.btnDelete:
                                //удалить запись

                                List<IKeepable> targetItems = new List<IKeepable>();



                                if (parent.isTreeViewBased)
                                {
                                    //если это дерево, то нод может содаржать потомков
                                    //если потомки есть удалять нельзя 
                                    //взять маппед покнтрол, а из него 

                                }
                                else

                                {
                                    
                                    

                                    bool b = fn.mb_confirmAction("Удалить элемент?");

                                    if (!b) return;



                                    int neededPosition = parent.currentRecord.index + 1;

                                    Lib.ObjectOperationResult or = parent.dataSource.deleteItem(parent.currentRecord.getMember());

                                    if (or.success)
                                    {
                                        parent.fillTheForm();
                                    }
                                    else
                                    {
                                        fn.mb_info($"Невозможно удалить элемент");
                                    }
                                    //сохранить курсор в той же позиции в гриде после удаления
                                    int count = parent.dataSource.count;
                                    if (count < neededPosition) { neededPosition = count; }
                                    parent.currentRecord.index = neededPosition - 1;


                                }



                                break;

                            case FormBtnTypeEnum.btnTestA:
                                break;
                            case FormBtnTypeEnum.btnTestB:
                                break;

                            case FormBtnTypeEnum.btnRecordMoveNextRecord:
                                parent.currentRecord.moveNext();
                                break;

                            case FormBtnTypeEnum.btnRecordMovePrevRecord:
                                parent.currentRecord.movePrevious();
                                break;

                            case FormBtnTypeEnum.btnReloadDataFormComponent:
                                parent.reloadMe();
                                break;
                            case FormBtnTypeEnum.btnOpenHistoryForm:
                                parent.openMyHistoryFrm();
                                break;
                        }
                        break;

                    case FormEventTypeEnum.currentRecordChangeAttempt:
                        //TODO кнопки туда и сюда тоже вызывают это событие
                        //взять вью контрол, с которого это приехало
                        MappedGridBasedControls.MappedGridBasedControl v = mappedGridBasedControls.getControlMapping(c);
                        if (v != null)
                        {
                            string id = Convert.ToString(v.targetControl.getValue());
                            parent.currentRecord.id = id;
                        }
                        break;


                    case FormEventTypeEnum.currentRecordChanged:
                        raiseFormEvent(FormEventTypeEnum.dependencyProcessionRequest);
                        break;

                    case FormEventTypeEnum.currentRecordEditCancelAttenpt:
                        doCancelEdit();
                        break;


                    //контролы, где нет валидации
                    case FormEventTypeEnum.checkBoxCheckedChanged:

                    case FormEventTypeEnum.dtPickerSelectionChanged:

                    case FormEventTypeEnum.comboBoxSelectionChanged:

                        break;


                    case FormEventTypeEnum.dependencyProcessionRequest:
                        DependencyManager.DependencyArgs d_args = new DependencyManager.DependencyArgs();
                        d_args.targetFieldId = parent.currentRecord.id;
                        parent.dependencyManager.processMyDependencies(d_args);
                        break;

                }
            }
            public Lib.FrmCrudModeEnum frmCrudMode { get { return Lib.FrmCrudModeEnum.GridOnly; } }
            //инициация событий формы
            public void raiseFormEvent(FormEventTypeEnum formEventType, object sender = null, EventArgs e = null, KeyEventArgs ke = null)
            {
                processFormEvent(formEventType, sender, e, ke);
            }
        }

        public abstract class CrudOperatorPattern
        {

            //это событийный интерфейс, который круд-оператор дает RIFDC-контролу, а контрол сам выбирает из него то, что ему (контролу) надо

            public EventHandlerCollection_RecordBasedControl eventHandlers_RecordBC { get; set; }
            public EventHandlerCollection_GridBasedControl eventHandlers_GridBC { get; set; }

            public MappedGridBasedControls mappedGridBasedControls;

            public MappedRecordBasedControls mappedRecordBasedControls;
            
            public MappedButtons mappedButtons;

            public DataFormComponent parent;

            public void selectAll()
            {
                mappedGridBasedControls.items.ForEach(x=> { x.selectAll(); });
            }

            public void selectNone()
            {
                mappedGridBasedControls.items.ForEach(x => { x.selectNone(); });
            }

            public CrudOperatorPattern(DataFormComponent _parent)
            {
                parent = _parent;

                mappedRecordBasedControls = new MappedRecordBasedControls(parent);
                mappedGridBasedControls = new MappedGridBasedControls(parent);

                mappedButtons = new MappedButtons(parent);

                eventHandlers_RecordBC = new EventHandlerCollection_RecordBasedControl();
                eventHandlers_GridBC = new EventHandlerCollection_GridBasedControl();
            }

            public void addGridBasedControlMapping(IGridBasedControl tc) { tc.paint = parent.paint; mappedGridBasedControls.addControlMapping(tc); }
            public void addRecordBasedControlMapping(IRecordBasedControl tc, string _srcDataFieldClassName) { mappedRecordBasedControls.addControlMapping(tc, _srcDataFieldClassName); }
            public void addButtonMapping(IRIFDCButton _mappedPhysicalButton, string _paramStr="") { mappedButtons.addButtonMapping(_mappedPhysicalButton, _paramStr); }



            //общий предок всех CRUD-операторов, содержит определния grid и record, чтобы каждый раз это не писать в разных круд-оператороах
            public class MappedGridBasedControls
            {
                //список контролов, где отображаются множества записей, напр, grid или treeview

                public List<MappedGridBasedControl> items = new List<MappedGridBasedControl>();

                public DataFormComponent parent;

                public MappedGridBasedControl getMultiSelectRepresentableControl()
                {
                    if (items.Count == 1) return items[0];

                    var _items = items.Where(x => x.targetControl.isMultiSelectRepresentable).ToList();

                    if (_items?.Count > 0)
                    { 
                        return _items[0]; 
                    }
                    else
                    { 
                        return null; 
                    }
                       
                }

                private bool _multiSelectionMode;

                public bool multiSelectionMode
                {
                    get
                    {
                        return _multiSelectionMode;
                    }
                    set
                    {
                        _multiSelectionMode = value;
                        foreach (MappedGridBasedControl m in items) { m.multiSelectionMode = _multiSelectionMode; }
                    }
                }

                public MappedGridBasedControls(DataFormComponent _parent)
                {
                    parent = _parent;
                }
                public void setCurrentId(string id)
                {
                    foreach (MappedGridBasedControl m in items) { m.setCurrentId(id); }
                }

                public class MappedGridBasedControl
                {
                    public IGridBasedControl targetControl;

                    MappedGridBasedControls parent;

                    public MappedGridBasedControl(MappedGridBasedControls _parent)
                    {
                        parent = _parent;
                    }

                    public void selectAll()
                    {
                        targetControl.selectAll();
                    }

                    public void selectNone()
                    {
                        targetControl.selectNone();
                    }


                    private bool _multiSelectionMode;

                    public bool multiSelectionMode
                    {
                        get
                        {
                            return _multiSelectionMode;
                        }
                        set
                        {
                            _multiSelectionMode = value;
                            targetControl.multiSelectMode = _multiSelectionMode;
                        }
                    }

                    public void reReadItem(IKeepable t)
                    {
                        targetControl.reReadItem(t);
                    }
                    public void setCurrentId(string id)
                    {
                        targetControl.setCurrentId(id);
                    }

                    public void addEventHandlers()
                    {
                        targetControl.addEventHandlers();
                    }
                }
                public MappedGridBasedControl addControlMapping(IGridBasedControl tc)
                {
                    MappedGridBasedControl c = new MappedGridBasedControl(this);
                    tc.eventHandlers_GridBC = parent.crudOperator.eventHandlers_GridBC;
                    tc.addEventHandlers();
                    c.targetControl = tc;
                    items.Add(c);

                    tc.ImSorted += parent.imSorted_EventHandler;

                    return c;
                }
                public MappedGridBasedControl getControlMapping(Control c)
                {
                    foreach (MappedGridBasedControl m in items)
                    {
                        if (m.targetControl.targetControl == c) return m;
                    }
                    return null;
                }

                public void reReadItem(IKeepable t)
                {
                    //по новой читает объект t в грид. Нужно когда запись обновилась и надо обновить данные в гриде.
                    foreach (MappedGridBasedControl c in items)
                    {
                        c.reReadItem(t);
                    }
                }

                public void fillAllControls()
                {
                    foreach (MappedGridBasedControl m in items) 
                    { 
                        m.targetControl.fillMe(); 
                    }
                }

                public void expandAll()
                {
                    items.ForEach(x => x.targetControl.expandAll());
                }
                public void collapseAll()
                {
                    items.ForEach(x => x.targetControl.collapseAll());
                }

            }

            public class MappedRecordBasedControls
            {
                //список контролов, где отображаются поля 1 конкретной записи - обычно там редактируются поля текущей записи
                public List<MappedRecordBasedControl> items = new List<MappedRecordBasedControl>();

                public DataFormComponent parent;

               private MappedRecordBasedControl _currentlyFocused;

                public MappedRecordBasedControl currentlyFocused
                {
                    // Record Based контрол, на котором сейчас фокус
                    get
                    {
                        return _currentlyFocused;
                    }
                }

                public MappedRecordBasedControls(DataFormComponent _parent)
                {
                    parent = _parent;
                }

                public class MappedRecordBasedControl
                {
                    public IRecordBasedControl targetControl;

                    public string srcDataFieldClassName;

                    public MappedRecordBasedControls parent;

                    public void igotfocus()
                    {
                        parent._currentlyFocused = this;
                    }

                    public MappedRecordBasedControl(MappedRecordBasedControls _parent, IRecordBasedControl _targetControl, string _srcDataFieldClassName)
                    {
                        parent = _parent;
                        srcDataFieldClassName = _srcDataFieldClassName;
                        targetControl = _targetControl;
                        fieldInfoObject = parent.parent.dataSource.sampleObject.getFieldInfoByFieldClassName(srcDataFieldClassName);
                        targetControl.gotFocus += ()=> igotfocus();
                    }

                    public bool validationIsOn_business
                    {
                        get { return fieldInfoObject.validationInfo.hasBusinessValidation; }
                    }
                    public bool validationIsOn_leave
                    {
                        get { return fieldInfoObject.validationInfo.hasLeaveValidation; }
                    }
                    public bool validationIsOn_symbol
                    {
                        get { return fieldInfoObject.validationInfo.hasSymbolValidation; }
                    }

                    public Lib.FieldInfo fieldInfoObject = null;

                    public object actualValue
                    {
                        //текущее значение, используется для понимания того, изменились данные или нет
                        get
                        {
                            IKeepable _t;

                            object tmp;

                            _t = parent.parent.currentRecord.getMember();

                            tmp = _t.getMyParameter(srcDataFieldClassName);

                            return tmp;
                        }
                    }

                    public string nullText = ""; //содержание поля, при котором оно считается пустым

                    public bool isDirty
                    {
                        get
                        {
                            //если то, что в поле, не рвно тому, что в объекте-- с учетом типа
                            object val1 = actualValue;
                            object val2 = getTargetControlValue();
                            bool equal = Lib.RIFDCObjectsEqual(val1, val2, fieldInfoObject.fieldType);

                            //Logger.log("VIEW", string.Format("CHECKING EQUAL: VAL1={0}, VAL2={1}, REZ= {2}", val1, val2, equal ? "CLEAR" : "DIRTY"));

                            return !equal;
                        }
                    }

                    public object getTargetControlValue()
                    {
                        //здесь надо сравнить вэлью контрола с его emptyValue, и если оно = ему то вернуть null
                        //значит, надо дать возможность присваивать emptyValue RIFDC контролу
                        return targetControl.getValue();
                    }
                    public void setTargetControlValue(object value)
                    {

                        //учитываем nullability объектов, т.к. это относится именно к слою форм
                        // if (value == null && fieldInfoObject.nullabilityInfo.allowNull) value = Lib.fieldTypeNullValue(fieldInfoObject.fieldType);

                        bool nullable = fieldInfoObject.nullabilityInfo.allowNull;
                        bool valueIsNull = (value == null);
                        if (valueIsNull && nullable) value = null;
                        if (!nullable && valueIsNull) value = fieldInfoObject.nullabilityInfo.defaultValue;
                        targetControl.setValue(value);
                    }

                    public void restoreTargetControlValue()
                    {
                        //вернуть в контрол сохраненное значение (чтобы отменить редактирование)
                        setTargetControlValue(actualValue);
                    }

                    public Validation.ValidationResult makeBusinessValidation()
                    {
                        return parent.parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.business, srcDataFieldClassName, getTargetControlValue());
                    }
                    public Validation.ValidationResult makeLeaveValidation()
                    {
                        //тут надо передать кол-во знаков после запятой
                        return parent.parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.leave, srcDataFieldClassName, getTargetControlValue());
                    }

                    public Validation.ValidationResult makeSymbolValidation()
                    {
                        //тут надо передать кол-во знаков после запятой
                        return parent.parent.dataSource.sampleObject.validateMyParameter(Validation.ValidationTypeEnum.symbol, srcDataFieldClassName, getTargetControlValue());
                    }

                    public void addEventHandlers()
                    {
                        //тут логика такая, что каждый контрол сам знает, как навешивать на себя ивентхендлеры. 
                        //поэтому мы не отдаетм это в круд манагер. делаем по другому: круд манагер предоставляет стд ивенит хендлер коллекшен,
                        // а контрол ее берет и выбирает то, что ему надо.

                        //parent.parent.crudOperator.addEventHandlers_RecordBasedControls (this);
                        targetControl.addEventHandlers();
                    }
                }

                public MappedRecordBasedControl addControlMapping(IRecordBasedControl tc, string _srcDataFieldClassName)
                {
                    tc.eventHandlers_RecordBC = parent.crudOperator.eventHandlers_RecordBC;
                    tc.addEventHandlers();
                    MappedRecordBasedControl c = new MappedRecordBasedControl(this, tc, _srcDataFieldClassName);
                    items.Add(c);
                    return c;
                }
                public MappedRecordBasedControl getControlMapping(Control c)
                {
                    foreach (MappedRecordBasedControl m in items)
                    {
                        if (m.targetControl.targetControl == c) return m;
                    }
                    return null;
                }

            }

            //это замапленные кнопки
            public class MappedButtons
            {
                DataFormComponent formManager;

                public List<MappedButton> items = new List<MappedButton>();

                public MappedButtons(DataFormComponent _formManager)
                {
                    formManager = _formManager;
                }

                public void addButtonMapping(IRIFDCButton _mappedPhysicalButton, string _paramStr="")
                {

                    // _paramStr = строка вида param=value
                    MappedButton m = new MappedButton(this, _paramStr);
                    m.parent = this;
                    m.buttonObject = _mappedPhysicalButton;
                    m.addEventHandlers();
                    items.Add(m);
                }
                public MappedButton getButtonMapping(Object c)
                {
                    ObjectParameters.ObjectParameter p = ObjectParameters.getObjectParameterByName(c, "Name");
                    
                    if (p == null) return null;

                    string objectName = fn.toStringNullConvertion(p.value);

                    if (objectName == "") return null;

                    foreach (MappedButton m in items)
                    {
                        if (m.buttonObject.name == objectName) { return m; }
                    }
                    return null;
                }

                public List<MappedButton> getButtonsOfType(FormBtnTypeEnum _btnType)
                {
                    List<MappedButton> ml = new List<MappedButton>();
                    foreach (MappedButton m in items)
                    {
                        if (m.buttonObject.btnType == _btnType) { ml.Add(m); }
                    }
                    return ml;
                }

                public class MappedButton
                {
                    public FormBtnTypeEnum btnType { get { return buttonObject.btnType; } }

                    public MappedButtons parent;
                    public string paramStr;

                    public IRIFDCButton buttonObject;
                    public MappedButton(MappedButtons _parent, string _paramStr="")
                    {
                        parent = _parent;
                        paramStr = _paramStr;
                    }
                    public void addEventHandlers()
                    {
                        buttonObject.addEventHandler(parent.formManager.EventHandler_Common_ButtonCLick);
                    }
                }


            }

        }

    }


}

