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
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;



namespace RIFDC
{


    public class HistorySaver
    {
        //класс, который сохраняет историю изменений объекта и дает возможность откатить последнюю операцию

        //TODO сейчас это сохранятется по принципу сохранилось - хорошо, в будущем надо сделать контроль, что это сохранилось

        ItemKeeper<HistorySaverUnit> items;
        
        IDataRoom dataRoom;
        
        public Lib.ObjectOperationResult doRollbackOperation (HistorySaverUnit hsUnit)
        {
            // это откат последнего изменения

            Fn.Dp("Rolling back history operation ");

            //тут надо 1) вернуть старое значение в тот объект

            object valueToReturn = hsUnit.oldValue;

            

            return null;
        }
        public Lib.ObjectOperationResult doSaveHistory(IKeeperAlterationPackage alt)
        {
            //список полей, кот. надо сохранить
            
            clear();

            HistorySaverUnit h;

            foreach (Lib.FieldInfo f in alt.items)
            {
                h = new HistorySaverUnit();
                //H.setMyParameter("entityName", H.entityName);
                h.setMyParameter("objectId", alt.id);
                h.setMyParameter("targetEntityName", alt.entityName);
                h.setMyParameter("oldValue", f.actualValue);
                h.setMyParameter("newValue", f.newValue);
                h.setMyParameter("fieldClassName", f.fieldClassName);
                h.setMyParameter("dateTimeOfChange", alt.alterationDateTimePoint);
                h.setMyParameter("userId", RIFDC_App.currentUserId);
                items.addExistingObject(h);
            }
             
            //теперь надо сохранить это в базе

            foreach (HistorySaverUnit  h0 in items.actualItemList)
            {
               Lib.ObjectOperationResult dbr = items.saveItem(h0);
            }
            return Lib.ObjectOperationResult.sayOk();
        }

        public Lib.ObjectOperationResult doDeleteHistory(IKeepable t)
        {
            // полностью удалить историю по объекту
            if (t==null)
            {
                return Lib.ObjectOperationResult.sayNo("Object is null");
            }
            if (t.id=="")
            {
                return Lib.ObjectOperationResult.sayNo("Object has no id");
            }

            IKeeper HistoryManagerDataSource = ItemKeeper<HistorySaver.HistorySaverUnit>.getInstance(RIFDC_App.mainDataRoom);

            Lib.Filter myFilter = new Lib.Filter();

            myFilter.addNewFilteringRule(
                HistoryManagerDataSource.sampleObject.getFieldInfoByFieldClassName("objectId"),
                 Lib.RIFDC_DataCompareOperatorEnum.equal,                 
                 t.id, Lib.Filter.FilteringRuleTypeEnum.NotSpecified);

            return  HistoryManagerDataSource.deleteFiteredPackege(myFilter);

        }

        public IKeeper getMyHistoryIKeeperObject (IKeepable t)
        {
            IKeeper myHistoryIKeeperObject = ItemKeeper<HistorySaver.HistorySaverUnit>.getInstance(dataRoom);

            Lib.Filter myFilter = new Lib.Filter();

            myFilter.addNewFilteringRule(
                myHistoryIKeeperObject.sampleObject.getFieldInfoByFieldClassName("objectId"),
                 Lib.RIFDC_DataCompareOperatorEnum.equal, 
                 t.id, Lib.Filter.FilteringRuleTypeEnum.ParentFormFilteringRule);

            myHistoryIKeeperObject.filtration.applyGlobalFilter(myFilter);

            myHistoryIKeeperObject.readItems();

            return myHistoryIKeeperObject;
        }


        public static HistorySaver getInstance(IDataRoom _dataRoom, IKeepable _sampleObject=null)
        {
            return new HistorySaver(_dataRoom, _sampleObject);
        }

        private HistorySaver (IDataRoom _dataRoom, IKeepable _sampleObject=null)
        {
            dataRoom = _dataRoom;
            items = ItemKeeper<HistorySaverUnit>.getInstance(dataRoom);
        }

        //public List<Lib.FieldInfo> myDirtyFieldInfo = new List<Lib.FieldInfo>(); //список fieldInfo, которые dirty. используется для сохранения истории изменений
        private void clear()
        {
            items.clear();
        }



        public class HistorySaverUnit: KeepableClass
        {
            //это строка таблицы с историей, т.е. объект, содержащий изменение параметра какого-либо  бизнес-объекта
            public override string tableName { get { return "ObjectHistory"; } }
            public override string entityName { get { return "HistorySaverUnit"; } }

            public string objectId { get; set; } = "";

            public string newValue { get; set; } = "";
            public string oldValue { get; set; } = "";

            public string targetEntityName { get; set; } = "";

            public string fieldClassName { get; set; } = "";

            public DateTime dateTimeOfChange;
            public string userId { get; set; } = "";
            //TODO для этой сущности бы и не надо дату создания / последнего обновления, но пока не будем это убирать

            public override Lib.FieldsInfo _get_fieldsInfo()
            {
                Lib.FieldsInfo f = getInitialFieldsInfoObject(true, false, false);
                
                Lib.FieldInfo x;

                //entityName
                x = f.addFieldInfoObject("entityName", "entityName", Lib.FieldTypeEnum.String);


                //objectId
                x = f.addFieldInfoObject("objectId", "objectId", Lib.FieldTypeEnum.String);
                x.caption = "Id";

                //oldValue
                x = f.addFieldInfoObject("oldValue", "oldValue", Lib.FieldTypeEnum.String);
                x.caption = "Старое значение";

                //targetEntityName
                x = f.addFieldInfoObject("targetEntityName", "targetEntityName", Lib.FieldTypeEnum.String);
                x.caption = "Тип объекта";

                //newValue
                x = f.addFieldInfoObject("newValue", "newValue", Lib.FieldTypeEnum.String);
                x.caption = "Новое значение";

                //fieldClassName
                x = f.addFieldInfoObject("fieldClassName", "fieldClassName", Lib.FieldTypeEnum.String);
                x.caption = "Параметр";

                //dateTimeOfChange
                x = f.addFieldInfoObject("dateTimeOfChange", "dateTimeOfChange", Lib.FieldTypeEnum.DateTime);
                x.caption = "Дата/время изменения";

                //TODO проверка дублиукаты поей в fieldInfo
                //userId
                x = f.addFieldInfoObject("userId", "userId", Lib.FieldTypeEnum.String);
                x.caption = "Id пользователя";

                return f;
            }

            public class MyControlFormats
            {
                public class HistorySaverUnitControlFormat : Lib.GridBasedControlFormat, Lib.IControlFormat
                {
                    HistorySaverUnitControlFormat(IKeepable _sampleObject) : base(_sampleObject)
                    {

                    }
                    public static HistorySaverUnitControlFormat getMyInstance(IKeepable _sampleObject)
                    {
                        // if (parent == null) return null;
                        string tmp;
                        HistorySaverUnitControlFormat g = new HistorySaverUnitControlFormat(_sampleObject);
                        Lib.FieldsInfo f = _sampleObject.fieldsInfo;

                        tmp = "targetEntityName"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                        tmp = "fieldClassName"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                        tmp = "newValue"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                        tmp = "oldValue"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                        tmp = "dateTimeOfChange"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                        tmp = "userId"; g.addFormatLine(tmp, 170, f.getFieldInfoObjectByFieldClassName(tmp).caption);

                        return g;
                    }

                }
            }
        }
        
    }

 
}