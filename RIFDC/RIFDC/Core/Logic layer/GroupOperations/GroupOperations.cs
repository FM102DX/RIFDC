using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonFunctions;

namespace RIFDC
{
    public partial class GroupOperationsFrm : Form, IRIFDCCrudForm
    {
        public GroupOperationsFrm()
        {
            InitializeComponent();
        }

        DataFormComponent dfc;
        IKeeper keeper;
        bool fillEditControlsFlag;
        private void frmMakeGroupOperation_Load(object sender, EventArgs e)
        {

            if (startMsg.targetKeeper == null)
            {
                this.Close();
            }

            // понят какой набор записей модифицировать
            //и как вообще его передавать
            //в принципе, это actualItemsList
            //а) все, но вдель там есть какие то входящие фильтры, да?
            //б) выделенный набор

            //берем selectedItemsIds , если оно пустое, берем 
            //определить список элементов, которым присваивается значение

            List<string> itemsIds = new List<string>();

            bool hasIdList = true;

            itemsIds = startMsg.caller.selectedItemsIds;

            if (itemsIds == null)
            {
                hasIdList = false;
            }
            else
            {
                if (itemsIds.Count == 0) hasIdList = false;
            }

            if (hasIdList)
            {
                itemsIds.ForEach(x =>
                items.Add(startMsg.targetKeeper.getItemById(x))
                );
            }
            else
            {
                items = startMsg.targetKeeper.actualItemList;
            }

            // итак, у нас есть список элементов, которому будет присвоено значение

            ibTargetObjects.Text = string.Format("Тип объектов= {0}, количество объектов={1}", startMsg.targetKeeper.sampleObject.entityName, items.Count);

            // дальше, взять значение из поля и присвоить

            // это ж значение из поля должно быть как-то валидировано, да?
            // и хорошо бы, чтобы оно было валидировано объектом
            // а если это дата? или подстановка? или 
            fillEditControlsFlag = true;
            cbxSetValueParameter.ValueMember = "fieldClassName";
            cbxSetValueParameter.DisplayMember = "fieldClassName";
            cbxSetValueParameter.DataSource = startMsg
                                              .targetKeeper
                                              .sampleObject
                                              .fieldsInfo
                                              .fields
                                              .Where(
                                                x => x.isAvialbeForGroupOperations &&
                                                x.fieldType!= Lib.FieldTypeEnum.DateTime &&
                                                x.fieldType != Lib.FieldTypeEnum.Date &&
                                                x.fieldType!= Lib.FieldTypeEnum.Time &&
                                                x.fieldClassName.ToLower() !="id")
                                              .ToList();
            
            doSelectWorkingField();
            fillEditControlsFlag = false;
        }

        private void doSelectWorkingField()
        {
            //tbValue.Text = "";

            //Delegate[] delegAry = tbValue.TextChanged.GetInvocationList();

            keeper = RIFDC_App.iKeeperSampleHolder.getIKeeperByEntityType(startMsg.targetKeeper.sampleObject.entityName);
            dfc = new DataFormComponent(keeper, this, Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly);
            dfc.tag = "сардина-1";
            RemoveEventHandlerOfType("TextChanged", tbValue);
            RemoveEventHandlerOfType("Leave", tbValue);

            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbValue), fn.ConvertObjectToString(cbxSetValueParameter.SelectedValue));

            keeper.clear();
            keeper.createNewObject_inserted();
            dfc.fillTheForm();
            //keeper.currentRecord.index = 0;
        }


        private void RemoveEventHandlerOfType(string eventName, Control c)
        {
            var currentEvent = c.GetType().GetEvents().FirstOrDefault(ev => ev.Name == eventName);
            var type = c.GetType();
            if (currentEvent != null)
            {
                //это объект "event"

                //FieldInfo[] eventFieldInfoArray =
                //    tbValue.GetFields();

                EventInfo[] x0 = tbValue.GetType().GetEvents();

                FieldInfo eventFieldInfo = type.GetField(eventName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                
                if (eventFieldInfo==null)
                {
                    fn.Dp("Event not found:  "+ eventName);
                    return;
                }
                //это проде как его значение, т.е. делегат
                eventFieldInfo.SetValue(c, null);
            }
        }


        private Lib.FieldInfo selectedFieldInfo
        {
            get
            {
                string s = fn.ConvertObjectToString(cbxSetValueParameter.SelectedValue.ToString());

                if (s == "") return null;

                Lib.FieldInfo f = startMsg.targetKeeper.sampleObject.fieldsInfo.getFieldInfoObjectByFieldClassName(s);

                return f;
            }
        }

        public Lib.InterFormMessage startMsg { get; set; } = null;

        List<IKeepable> items = new List<IKeepable>();

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServiceFucntions.mb_info(string.Join(" ", items.Select(x => x.id).ToList()));
        }

        private void cbxSetValueParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fillEditControlsFlag) return;
            fillEditControlsFlag = true;
            doSelectWorkingField();
            fn.Dp("selected field is "+ cbxSetValueParameter.SelectedValue.ToString());
            fillEditControlsFlag = false;
        }

        private void btnDoAction_Click(object sender, EventArgs e)
        {
            //сначала валидация
            //надо валидировать, но оно само, там обработчики событий

            List<string> success = new List<string>();
            List<string> errors = new List<string>();

            Lib.ObjectOperationResult or;
            Lib.ObjectOperationResult or1;

            foreach (IKeepable x in items)
            {
                try
                {
                    or = x.setMyParameter(selectedFieldInfo.fieldClassName, tbValue.Text);
                    if (or.success)
                    {
                        or1= startMsg.targetKeeper.saveItem(x);
                        if (or1.success)
                        {
                            success.Add(string.Format("id={0}: {1}", x.id, "success"));

                        }
                        else
                        {
                            errors.Add(string.Format("id={0}: error saving into db msg={1} ", x.id, or.msg));
                        }
                    }
                    else
                    {
                        errors.Add(string.Format("id={0}: error saving object msg={1} ", x.id, or.msg));
                    }

                }
                catch
                {
                    errors.Add(string.Format("id={0}: fail ", x.id));
                }
            }

            string rez = string.Format("Результат групповой операции:{0}{1}{2}", 
                            string.Join(fn.Chr13, success), 
                            fn.Chr13, 
                            string.Join(fn.Chr13, errors));

            ServiceFucntions.mb_info(rez);
        }



        private void tbValue_TextChanged(object sender, EventArgs e)
        {
            //ввод символа, надо посимвольную валидацию учитывать
          /*  
           *  if (!selectedFieldInfo.validationInfo.hasSymbolValidation) return;
            Validation.ValidationResult vr = startMsg
                                            .targetKeeper
                                            .sampleObject
                                            .validateMyParameter(Validation.ValidationTypeEnum.symbol, selectedFieldInfo.fieldClassName, tbValue.Text);
            tbValue.Text = vr.validatedValue.ToString();
            */
        }

        private void tbValue_Leave(object sender, EventArgs e)
        {
           /*
            *
            * if (!selectedFieldInfo.validationInfo.hasLeaveValidation) return;

            Validation.ValidationResult vr = startMsg.targetKeeper.sampleObject.validateMyParameter(
            Validation.ValidationTypeEnum.leave,
            selectedFieldInfo.fieldClassName,
            tbValue.Text);

            //и вот это я тащу аж из объекта
            if (vr.validationSuccess)
            {
                //если leave валидация success, ставим возвращенное value и уходим с поля
                tbValue.Text = fn.ConvertObjectToString(vr.validatedValue);
            }
            else
            {
                //если leave валидация !success, возвращаем пользователя на поле, показываем сообщение
                tbValue.Focus();
                fn.MessageBoxInfo(vr.validationMsg);
                return;
            }

            if (!selectedFieldInfo.validationInfo.hasBusinessValidation) return;

            vr = startMsg.targetKeeper.sampleObject.validateMyParameter(
            Validation.ValidationTypeEnum.business,
            selectedFieldInfo.fieldClassName,
            tbValue.Text);

            //и вот это я тащу аж из объекта
            if (vr.validationSuccess)
            {
                //если leave валидация success, ставим возвращенное value и уходим с поля
                tbValue.Text = fn.ConvertObjectToString(vr.validatedValue);
            }
            else
            {
                //если leave валидация !success, возвращаем пользователя на поле, показываем сообщение
                tbValue.Focus();
                fn.MessageBoxInfo(vr.validationMsg);
                return;
            }
            */

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
