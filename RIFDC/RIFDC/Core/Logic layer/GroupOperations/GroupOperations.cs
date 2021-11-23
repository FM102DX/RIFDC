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
        Lib.FieldInfo currentlySelectedFieldInfo = null;
        Control actualInputControl = null;
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
             
            if (actualInputControl != null)
            {
                grpOperationSetValue.Controls.Remove(actualInputControl);
                actualInputControl.Dispose();
            }

            currentlySelectedFieldInfo = startMsg.targetKeeper.sampleObject.fieldsInfo.getFieldInfoObjectByFieldClassName(cbxSetValueParameter.SelectedValue.ToString());

            //TODO тут надо создавать кипер без датарума
            keeper = RIFDC_App.iKeeperSampleHolder.getIKeeperByEntityType(startMsg.targetKeeper.sampleObject.entityName);
            keeper.dataRoom = null;
            dfc = new DataFormComponent(keeper, this, Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly);
            dfc.tag = "сардина-1";

            if (currentlySelectedFieldInfo.isStringValue)
            {
                //теперь создаем контрол под выбранное филдинфо, и мапим его на DFC
                TextBox newTb = new TextBox();
                newTb.Location = new System.Drawing.Point(19, 83);
                newTb.Size = new System.Drawing.Size(341, 120);
                newTb.Multiline = true;
                newTb.BackColor = Color.Green;
                actualInputControl = newTb;
                grpOperationSetValue.Controls.Add(newTb);
                dfc.addRecordBasedControlMapping(new RIFDC_TextBox(newTb), fn.toStringNullConvertion(cbxSetValueParameter.SelectedValue));
            }
            
            if (currentlySelectedFieldInfo.fieldType== Lib.FieldTypeEnum.Double || currentlySelectedFieldInfo.fieldType == Lib.FieldTypeEnum.Int)
            {
                //теперь создаем контрол под выбранное филдинфо, и мапим его на DFC
                TextBox newTb = new TextBox();
                newTb.Location = new System.Drawing.Point(19, 83);
                newTb.Size = new System.Drawing.Size(341, 120);
                newTb.Multiline = false;
                newTb.BackColor = Color.Yellow;
                actualInputControl = newTb;
                grpOperationSetValue.Controls.Add(newTb);
                dfc.addRecordBasedControlMapping(new RIFDC_TextBox(newTb), fn.toStringNullConvertion(cbxSetValueParameter.SelectedValue));
            }

            if (currentlySelectedFieldInfo.fieldType == Lib.FieldTypeEnum.Bool)
            {
                //теперь создаем контрол под выбранное филдинфо, и мапим его на DFC
                CheckBox newCb = new CheckBox();
                newCb.Location = new System.Drawing.Point(19, 83);
                actualInputControl = newCb;
                grpOperationSetValue.Controls.Add(newCb);
                dfc.addRecordBasedControlMapping(new RIFDC_CheckBox(newCb), fn.toStringNullConvertion(cbxSetValueParameter.SelectedValue));
            }


            keeper.clear();
            keeper.createNewObject_inserted();
            dfc.fillTheForm();
        }


        private Lib.FieldInfo selectedFieldInfo
        {
            get
            {
                string s = fn.toStringNullConvertion(cbxSetValueParameter.SelectedValue.ToString());

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
            fn.mb_info(string.Join(" ", items.Select(x => x.id).ToList()));
        }

        private void cbxSetValueParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (fillEditControlsFlag) return;
            fillEditControlsFlag = true;
            
            doSelectWorkingField();
            
            fn.dp("selected field is "+ cbxSetValueParameter.SelectedValue.ToString());
            fillEditControlsFlag = false;
        }

        private void btnDoAction_Click(object sender, EventArgs e)
        {
            List<string> success = new List<string>();
            List<string> errors = new List<string>();

            Lib.ObjectOperationResult or;
            Lib.ObjectOperationResult or1;
            object newVal = dfc.dataSource.currentRecord.getMember().getMyParameter(selectedFieldInfo.fieldClassName);
            foreach (IKeepable x in items)
            {
                try
                {
                    or = x.setMyParameter(selectedFieldInfo.fieldClassName, newVal);
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
            /*
            string rez = string.Format("Результат групповой операции:{0}{1}{2}{3}",
                            fn.chr13,
                            string.Join(fn.chr13, success), 
                            fn.chr13, 
                            string.Join(fn.chr13, errors));
*/

            string rez = $"Результат групповой операции:{fn.chr13}Общее количество объектов: {items.Count} {fn.chr13}Успешно: {success.Count}{fn.chr13} Ошибок: {errors.Count}";

            fn.mb_info(rez);
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
                tbValue.Text = fn.toStringNullConvertion(vr.validatedValue);
            }
            else
            {
                //если leave валидация !success, возвращаем пользователя на поле, показываем сообщение
                tbValue.Focus();
                fn.mb_info(vr.validationMsg);
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
                tbValue.Text = fn.toStringNullConvertion(vr.validatedValue);
            }
            else
            {
                //если leave валидация !success, возвращаем пользователя на поле, показываем сообщение
                tbValue.Focus();
                fn.mb_info(vr.validationMsg);
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
