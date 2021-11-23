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
using System.Text.RegularExpressions;
using System.Drawing;


//сервисные классы 123 456 

namespace RIFDC
{
    public static class Lib
    {
        //пакет ощих функций и классов, испольуемых фреймворком
        public class LinePaintingRuleHolder
        {
            List<LinePaintingRule> items = new List<LinePaintingRule>();
            
            public void addPaintingRuleMapping(string fieldClassName, string fieldValue, Color colorCode)
            {
                LinePaintingRule l = new LinePaintingRule();
                l.fieldClassName = fieldClassName;
                l.fieldValue = fieldValue;
                l.colorCode = colorCode;
                items.Add(l);
            }

            public Color getMyColor(IKeepable x)
            {
                string tmp;
                foreach (LinePaintingRule r in items)
                {
                    tmp = fn.toStringNullConvertion(x.getMyParameter(r.fieldClassName));
                    //fn.dp(string.Format("GETTING COLOR: parameter={0}, value={1}, returned color={2}", r.fieldClassName, tmp, ""));
                    if (tmp == r.fieldValue)
                    {
                        //fn.dp("Returning color = " + r.colorCode.ToString());
                        return r.colorCode;
                    }
                }
                return Color.White;
            }

            public class LinePaintingRule
            {
                public string fieldClassName;
                public string fieldValue;
                public Color colorCode;
            }
        }

        public static void IKeepableListSimpleDump(List<IKeepable> lst, string msg ="")
        {
            if (msg != "") fn.dp(msg);
            lst.ForEach(x=> { fn.dp(string.Format("id={0} name={1}", x.id, x.displayName)); });
        }

        public interface IControlFormat
        {
            List<IControlFormatLine> lines { get; set; }
        }

        public class MultipleOperationHandler
        {

            List<Lib.ObjectOperationResult> _rezList;

            public MultipleOperationHandler(List<Lib.ObjectOperationResult> rezList)
            {
                _rezList = rezList;
            }

            public int totalCount { get { return _rezList.Count(); } }

            public int successCount { get { return _rezList.Where(x => x.success).Count(); } }
            
            public int failCount { get { return _rezList.Where(x => !x.success).Count(); } }

            public string rezultMsgText1 { get { return $"Обработано объектов: {totalCount} {fn.chr13} успешно: {successCount} {fn.chr13} ошибка: {failCount} ";} }
            


        }
        public interface IControlFormatLine
        {
            string fieldClassName { get; set; }
            int colWidth { get; set; }
            string caption { get; set; }
        }

        public class GridBasedControlFormat : IControlFormat
        {
            //это формат, который можно передать в GridBasedControl, и контрол сам выставить колонки, подписи и пр.
            public List<IControlFormatLine> lines { get; set; } = new List<IControlFormatLine>();
            public IKeepable sampleObject;
            public GridBasedControlFormat(IKeepable _sampleObject)
            {
                sampleObject = _sampleObject;
            }


            public void addFormatLine(string _fieldClassName, int _colWidth, string _caption)
            {
                GridBasedControlFormatLine vl = new GridBasedControlFormatLine();
                vl.fieldClassName = _fieldClassName;
                vl.colWidth = _colWidth;
                vl.caption = _caption;
                lines.Add(vl);
            }

            public class GridBasedControlFormatLine : IControlFormatLine
            {
                public string fieldClassName { get; set; }
                public int colWidth { get; set; }
                public string caption { get; set; }
            }
        }

        public static object getFieldValueFromUDC(List<IUniversalRowDataContainer> rowList, string searchFieldName, string searchValue, string valueFieldName)
        {
            //сравниваем строки, т.к. у нас ID строковые

            if (rowList ==null) return null;
            if (rowList.Count==0) return null;
            
            string val0=null;
            object val=null;

            foreach (IUniversalRowDataContainer x in rowList)
            {
                val0 = fn.toStringNullConvertion(x.getValueByName(searchFieldName));

                if (val0== searchValue)
                {
                    val = fn.toStringNullConvertion(x.getValueByName(valueFieldName));
                    return val;
                }
            }
            return null;
        }

        public class UniversalDataKeeper : IUniversalRowDataContainer
        {
            public List<IUniversalDataElement> items { get; set; } = new List<IUniversalDataElement>();

            public object getValueByName(string name)
            {
                foreach (DataElement x in items)
                {
                    if (x.name.ToLower() == name.ToLower()) 
                    {
                        return x.value; 
                    }
                }
                return null;
            }

            public void setValueByName(string name, object value)
            {
                foreach (DataElement x in items)
                {
                    if (x.name.ToLower() == name.ToLower())
                    {
                        x.value = value;
                        return;
                    }
                }
            }

            public void addNewElement(string _name, object _value)
            {
                DataElement d = new DataElement() { name = _name, value = _value };
                items.Add(d);
            }
            public class DataElement : IUniversalDataElement
            {
                public string name { get; set; }
                public object value { get; set; }
            }

            public string getMyDump()
            {
                return string.Join(";", items.Select(x=> $"{x.name}={x.value}"));
            }
        }

        public static bool RIFDCObjectsEqual(Object val1, Object val2, FieldTypeEnum RIFDCType)
        {
            if ((val1 == null) && (val2 == null)) return true;

            bool rez = false;//сравнить 2 величины исходя из типа
            try
            {
                switch (RIFDCType)
                {
                    case FieldTypeEnum.Memo:
                    case FieldTypeEnum.String:
                        if (Convert.ToString(val1) == Convert.ToString(val2)) { return true; } else { return false; }

                    case FieldTypeEnum.Int:
                        if (Convert.ToInt32(val1) == Convert.ToInt32(val2)) { return true; } else { return false; }

                    case FieldTypeEnum.Double:
                        if (Convert.ToDouble(val1) == Convert.ToDouble(val2)) { return true; } else { return false; }

                    case FieldTypeEnum.Bool:
                        if (Convert.ToBoolean(val1) == Convert.ToBoolean(val2)) { return true; } else { return false; }

                    case FieldTypeEnum.Date:
                    case FieldTypeEnum.Time:
                    case FieldTypeEnum.DateTime:
                        if (Convert.ToDateTime(val1) == Convert.ToDateTime(val2)) { return true; } else { return false; }
                    //TODO тут еще типы

                    default:
                        return false;
                }
            }
            catch
            {
                rez = false;
            }
            return rez;

        }
        public static CommonOperationResult convertedObjectRIFDCTypes(FieldTypeEnum typeVar, object value)
        {
            //возвращает object - обертку исходя из того, какой тип передан в typeStr
            //это обертка над fn.convertedObject

            string s;
            switch (typeVar)
            {
                case FieldTypeEnum.Bool:
                    s = "System.String";
                    break;

                case FieldTypeEnum.Time:
                case FieldTypeEnum.Date:
                    s = "System.DateTime";
                    break;

                case FieldTypeEnum.Double:
                    s = "System.Double"; //todo - проверить, так ли это реально называется
                    break;

                case FieldTypeEnum.Int:
                    s = "System.Int32";
                    break;

                case FieldTypeEnum.Memo:
                case FieldTypeEnum.String:

                    s = "System.String";
                    break;
                default:
                    s = "";
                    break;
            }

            fn.CommonOperationResult rez= fn.convertedObject(s, value);
            if (rez.success)
            {
                return CommonOperationResult.returnValue(rez.returningValue);
            }
            else
            {
                return CommonOperationResult.sayNo();
            }
            
        }
        public class FieldsInfo
        {
            //объект, который содержит сведения о полях класса (т.е. объектах типа FieldInfo) для передачи их в различные конструкторы
            //public int id;
            public string tableName;
            public string entityName;
            public List<Lib.FieldInfo> fields;
            public bool allowId = true;
            public bool allowDateTimeOfCreation = true;
            public bool allowDateTimeOfLastChange = true;
            public bool allowCreatedByUserId = true;
            public Type myType;


            public List<Lib.FieldInfo> getMySearchableFields()
            {
                return fields.Where(x => x.isSearchable).ToList();
            }
            public List<Relations.RelationsChain.RelationsChainElement> relationsChainUniqueElements
            {
                get
                {
                    //вернуть список всех RelationsChainElement во всех fieldinfo, где significanceType =OuterDependable
                    //используется для формирования запросов left join

                    List<Relations.RelationsChain.RelationsChainElement> rez0=new List<Relations.RelationsChain.RelationsChainElement>();
                    List<Relations.RelationsChain.RelationsChainElement> rez = new List<Relations.RelationsChain.RelationsChainElement>();
                    fields.Where(f => f.parameterSignificanceInfo.significanceType == ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable).
                        ToList().
                        ForEach(x => x.parameterSignificanceInfo.relationsChain.items.ForEach(z => { rez0.Add(z); } ));

                    //теперь надо пройти на уникальность
                    bool ex;
                    foreach (Relations.RelationsChain.RelationsChainElement x0 in rez0)
                    {
                       // fn.dp(x0.ToString());
                        ex = rez.Exists(x => x.Equals(x,x0));
                      //  fn.dp("EX="+ex.ToString());
                        if (!ex) rez.Add(x0);
                    }
                    return rez;
                }
            }

            public FieldsInfo()
            {
                //tableName = _tableName;
                fields = new List<Lib.FieldInfo>();

            }

            public bool historySavingAlloyed
            {
                get
                {
                    int y = fields.Where(x => x.saveHistory).Count();
                    return (y > 0);
                }
            }

            private int howMenyFieldsOfType(ParameterSignificanceInfo.ParameterSignificanceTypeEnum x)
            {
                int counter = 0;
                foreach (Lib.FieldInfo f in fields)
                {
                    if (f.parameterSignificanceInfo.significanceType == x)
                    {
                        counter++;
                    }
                }
                return counter;
            }

            public int howMenySolidFields()
            {
                return howMenyFieldsOfType(ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid);
            }

            public int howMenyOuterDependableFields()
            {
                return howMenyFieldsOfType(ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable);
            }

            public bool hasOuterJoinFields
            {
                get
                {
                    foreach (Lib.FieldInfo f in fields)
                    {
                        if (f.parameterSignificanceInfo.significanceType == ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable) return true;
                    }
                    return false;
                }
            }
            public string getMyDump()
            {
                string s = "FIELDINFO DUMP: ";
                foreach (Lib.FieldInfo f in fields)
                {
                    s += f.fieldDbName + " = " + f.actualValue + " | ";
                }
                return s;
            }

            public Lib.FieldInfo getFieldInfoObjectByFieldClassName(string fieldClassName)
            {
                Lib.FieldInfo f = null;

                foreach (Lib.FieldInfo f0 in fields)
                {
                    if (f0.fieldClassName.ToLower() == fieldClassName.ToLower())
                    {
                        f = f0;
                        break;
                    }
                }
                return f;
            }

            public Lib.FieldInfo addFieldInfoObject(string fieldClassName, string fieldDbName, FieldTypeEnum fieldType)
            {
                Lib.FieldInfo f = new Lib.FieldInfo();
                f.parent = this;
                f.fieldClassName = fieldClassName;
                f.fieldDbName = fieldDbName;
                f.fieldType = fieldType;
                fields.Add(f);
                return f;
            }

            public bool validateMySelf()
            {
                //проверка правильности составления полей fieldInfo

                List<string> s = new List<string>();

                foreach (Lib.FieldInfo f in fields)
                {

                    s.Add(f.fieldClassName);

                    // 1) если поле nullable, должно быть указано defaultValue
                    // не нужно, defaultValue будем брать  из типа
                    /*
                                        if (f.allowNull && f.defaultValue == null)
                                        {
                                                fn.dp("Class with tableName= "+tableName+" validation error at " + f.fieldClassName + ": defaultValue is required for nullable fields");
                                                return false;
                                        }

                                        */

                    // 2) если это bool, он не может быть null
                    if (f.fieldType == FieldTypeEnum.Bool && f.nullabilityInfo.allowNull == true)
                    {
                        fn.dp("Class with tableName= " + tableName + " validation error at " + f.fieldClassName + ": bool fields can't allow null");
                        return false;
                    }
                    // 3) что такое поле реально есть в объекте

                }

                // 4) если поле с таким className уже есть в коллекции, т.е. чтобы не было дубликатов
                string duplicates = fn.stringListDuplicates(s);
                if (duplicates != "")
                {
                    fn.dp("Class with tableName= " + tableName + " validation error: Duplicates in parameter declaration: " + duplicates);
                    return false;
                }
                return true;
            }

            //эта структура хранит форматы контролов
            public Dictionary<string, Lib.IControlFormat> controlFormats = new Dictionary<string, Lib.IControlFormat>();

            //она заполняется там же, где fieldinfo, например:
            /*
             * 
             *  Lib.GridBasedControlFormat g = new Lib.GridBasedControlFormat();
                tmp = "npp"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName (tmp).caption);
                tmp = "titul"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                tmp = "mark"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                tmp = "ERCode"; g.addFormatLine(tmp, 100, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                tmp = "name"; g.addFormatLine(tmp, 200, f.getFieldInfoObjectByFieldClassName(tmp).caption);

                */
        }

        public class FieldInfo
        {
            public FieldsInfo parent;
            public string fieldClassName { get; set; }
            public string fieldDbName;
            public string caption { get; set; } = ""; //для подписей в контролах
            public FieldTypeEnum fieldType;
            public object actualValue;
            public object newValue; //в ситуации когда поле dirty, сохраняет значение, которое есть в объекте
            public bool saveHistory = false; // сохранять ли историю конкретно по этому полю
            public ParameterSignificanceInfo parameterSignificanceInfo = new ParameterSignificanceInfo();
            //public bool isDbStorable = true;
            public bool isSearchable = false;
            public bool isAvialbeForGroupOperations;
            //public bool sortable = false; //можно ли по этому полю сортировать в DFC
            public bool amIProperty()
            {
                //является ли параметр объекта field или property
                PropertyInfo x = null;
                try
                {
                    x = parent.myType.GetProperty(fieldClassName);
                    if (x != null) return true;
                }
                catch
                {

                }
                return false;
            }
            
            public bool isNull { get { return nullabilityInfo.allowNull && nullabilityInfo.considerNull; } }

            public bool isPrimaryKey = false;

            public NullabilityInfo nullabilityInfo = new NullabilityInfo();

            public long excelFileBoundColumnNumber = 0; // 1-based  номер колонки эксель файла

            public bool isUnique = false; //если isIdField=true, то игнорируется
            public bool isDataPresenceMarker = false; //наличие/отсутствие данных в этом поле говорит о том, что строки в источнике  закончились (даже если ID есть)
            public bool isCounter = false; //является ли счетиком 
            //TODO check что каунтер единственный
            public bool isStringValue
            {
                get
                {
                    if (fieldType == FieldTypeEnum.String || fieldType == FieldTypeEnum.Memo) { return true; }
                    return false;
                }
            }
            public bool isDateTileLikeValue
            {
                get
                {
                    if (fieldType == FieldTypeEnum.Date || fieldType == FieldTypeEnum.Time || fieldType == FieldTypeEnum.DateTime) { return true; }
                    return false;
                }
            }



            public ValidationInfo validationInfo = new ValidationInfo();

            public bool isDateTypeParameter
            {
                get
                {
                    return fieldType == FieldTypeEnum.Date || fieldType == FieldTypeEnum.DateTime || fieldType == FieldTypeEnum.Time;
                }
            }
        }

        public class NullabilityInfo
        {
            public bool allowNull = true;
            public object defaultValue = null;
            public bool considerNull = false;  //это способ присвоить null объекту  // Это опасная галка, лучше бы определять NUll как нибудь это вычисляя
        }

        public class ParameterSignificanceInfo
        {
            //но ведь это существует по какому-то realtion, верно?
            public ParameterSignificanceTypeEnum significanceType = ParameterSignificanceTypeEnum.Solid;
            //public Relations.Relation targetRelation=null;
            //public string relativeFieldClassName=""; //это филдкласснейм, который надо вывести. если оставить пустым, то будет выведено id
            public Relations.RelationsChain relationsChain;



            //здесь TargetFunction - first, last, min, max и др, но пока оставим first

            public enum ParameterSignificanceTypeEnum
            {
                Solid=1,  //это если присутствует в основной таблице
                OuterDependable = 2, //вьюшное поле, которое мы тащим через через left join по цепчке relations
                LocallyCountableGetter=3
            }

        }


        public class ValidationInfo
        {
            public ValidationInfoCluster businessValidation = new ValidationInfoCluster();
            public ValidationInfoCluster leaveValidation = new ValidationInfoCluster();
            public ValidationInfoCluster symbolValidation = new ValidationInfoCluster();
            public ValidationFormat myValidationFormat = null; //формат валидации - специальный класс, где прописаны правила
            public bool hasFormat
            {
                get { return !(myValidationFormat == null); }
            }
            public bool hasBusinessValidation
            {
                get
                {
                    if (!hasFormat)
                    {
                        return businessValidation.hasValidation;
                    }
                    else
                    {
                        return businessValidation.hasValidation || myValidationFormat.hasBusinessValidation;
                    }
                }
            }
            public bool hasLeaveValidation
            {
                get
                {
                    if (!hasFormat)
                    {
                        return leaveValidation.hasValidation;
                    }
                    else
                    {
                        return leaveValidation.hasValidation || myValidationFormat.hasLeaveValidation;
                    }
                }
            }

            public bool hasSymbolValidation
            {
                get
                {
                    if (!hasFormat)
                    {
                        return symbolValidation.hasValidation;
                    }
                    else
                    {
                        return symbolValidation.hasValidation || myValidationFormat.hasSymbolValidation;
                    }
                }
            }

            public List<Validation.IValidationFunction> businessValidationRuleFullList
            {
                //полный список бизнес VRL включая те, что в формате (их в первую очередь)
                get
                {
                    return Validation.mergeVrls(businessValidation.rules, (myValidationFormat == null) ? null : myValidationFormat.businessValidation.rules);
                }

            }

            public List<Validation.IValidationFunction> leaveValidationRuleFullList
            {
                //полный список бизнес VRL включая те, что в формате (их в первую очередь)
                get
                {
                    return Validation.mergeVrls(leaveValidation.rules, (myValidationFormat == null) ? null : myValidationFormat.leaveValidation.rules);
                }

            }
            public List<Validation.IValidationFunction> symbolValidationRuleFullList
            {
                //полный список бизнес VRL включая те, что в формате (их в первую очередь)
                get
                {
                    return Validation.mergeVrls(symbolValidation.rules, (myValidationFormat == null) ? null : myValidationFormat.symbolValidation.rules);
                }

            }

        }
        public class ValidationInfoCluster
        {
            public List<Validation.IValidationFunction> rules = new List<Validation.IValidationFunction>();
            public bool hasValidation
            {
                get
                {
                    if (!fn.listIsNullOrEmpty(rules))
                    {
                        if (rules.Count > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }

            }
        }


        public class ValidationFormat : ValidationInfo
        {
            //ValidationFormat - это тот же ValidationInfo, только без myValidationFormat
            public new Validation.IValidationFormat myValidationFormat = null; //формат валидации - специальный класс, где прописаны правила
            public new bool hasFormat { get { return false; } }
        }

        public static string getObjectStr(KeepableClass x)
        {
            string s = "";
            //Type xt = x.GetType(); //это типа переданного объекта

            // это поля, которые содержаться в типе переданного объекта
            System.Reflection.FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (Lib.FieldInfo f in x.fieldsInfo.fields) // метаполя, которые должны быть
            {
                foreach (System.Reflection.FieldInfo f0 in newObjectFields)
                {
                    if (f0.Name.ToLower() == f.fieldDbName.ToLower())
                    {
                        s += f0.GetValue(x).ToString() + " | ";
                    }
                }

                foreach (PropertyInfo f0 in newObjectProperties)
                {
                    if (f0.Name.ToLower() == f.fieldDbName.ToLower())
                    {
                        s += f0.GetValue(x).ToString() + " | ";
                    }
                }
            }
            return s;
        }

        //MSG-классы
        public class EventProcessigResult
        {
            public bool success;
            public string msg;
            public static EventProcessigResult getInstance(bool _success, string _msg)
            {
                EventProcessigResult c = new EventProcessigResult();
                c.success = _success;
                c.msg = _msg;
                return c;
            }
            public static EventProcessigResult sayOk(string _msg = "")
            {
                return getInstance(true, _msg);
            }
            public static EventProcessigResult sayNo(string _msg = "")
            {
                return getInstance(false, _msg);
            }
        }

        public class CommonOperationResult
        {
            public bool success;
            public string msg;
            public object returnedValue=null;

            public string returningColumnName;
            public static CommonOperationResult getInstance(bool _success, string _msg, string _returningColumnName = "", object _returnedValue=null)
            {
                CommonOperationResult c = new CommonOperationResult();
                c.returningColumnName = _returningColumnName;
                c.success = _success;
                c.msg = _msg;
                c.returnedValue = _returnedValue;
                return c;
            }
            public static CommonOperationResult getInstance(bool _success, object _returnedValue = null, string _msg="")
            {
                CommonOperationResult c = new CommonOperationResult();
                c.success = _success;
                c.msg = _msg;
                c.returnedValue = _returnedValue;
                return c;
            }
            public static CommonOperationResult sayOk(string _msg = "") { return getInstance(true, _msg); }
            
            public static CommonOperationResult returnValue(object _returnedValue, string _msg = "") { return getInstance(true, _returnedValue, _msg); }
            public static CommonOperationResult sayNo(string _msg = "") { return getInstance(false, _msg); }

        }

        public class ObjectOperationResult
        {
            //результат, возвращаемый после операций в объектном слое
            public bool success;
            public string msg;
            public string createdObjectId;

            public static ObjectOperationResult getInstance(bool _success, string _msg, string _createdObjectId = "")
            {
                ObjectOperationResult c = new ObjectOperationResult();
                c.createdObjectId = _createdObjectId;
                c.success = _success;
                c.msg = _msg;
                return c;
            }

            public static ObjectOperationResult sayOk(string _msg = "") { return getInstance(true, _msg); }
            public static ObjectOperationResult sayNo(string _msg = "") { return getInstance(false, _msg); }
        }

        public class DbOperationResult
        {
            public bool success;
            public int result;
            public string msg;
            public string createdObjectId;
            public int rowsAffected;
            public string returningColumnName = "";
            public DateTime insertedDateTime;
            public DateTime updatedDateTime;

            public static DbOperationResult getInstance(bool _success, string _msg, int _result = 0, string _createdObjectId = "", string _returningColumnName = "")
            {
                DbOperationResult d = new DbOperationResult
                {
                    success = _success,
                    msg = _msg,
                    result = _result,
                    createdObjectId = _createdObjectId,
                    returningColumnName = _returningColumnName,

                };
                return d;
            }
            public static DbOperationResult sayOk(string _msg = "") { return getInstance(true, _msg); }
            public static DbOperationResult sayNo(string _msg = "") { return getInstance(false, _msg); }
        }

        public class RelationsPackage
        {
            //коллекция relations, которая передается между формами, когда из одной формы открываешь другую с предзаданным объектом
            List<Relations.Relation> relations = new List<Relations.Relation>();
            public void addRelation(Relations.Relation r)
            {
                relations.Add(r);
            }

            public int count { get { return relations.Count; } }

            public List<Relations.Relation> getMyRelations(string className)
            {
                List<Relations.Relation> x = new List<Relations.Relation>();
                Relations.Relation.RelationSide rls = null;

                foreach (Relations.Relation rel in relations)
                {
                    rls = rel.getRealtionSideByClassName(className);
                    if (rls != null)
                    {
                        x.Add(rel);
                    }
                }
                return x;
            }


        }

        public delegate IDataCluster getExcelSTDCInstance(StaticExcelFile targetFile);

        public class InterFormMessage
        {
            //соообщение, которое одна форма передает другой когда, например, вы открываете одну форму из другой
            public IDataFormComponent caller;
            public IKeepable targetObject;
            public IKeepable sampleObject;
            public string msg;
            public RelationsPackage realtionspackage;
            public IKeeper targetKeeper;
            public List<string> selectedItemsIds;
            public getExcelSTDCInstance excelStaticDelegate; // делегат, который возвращает искомый датакластер
            public string targetEntityName;
            public IControlFormat controlFormat;
            public IDataRoom dataRoom;

            public InterFormMessage()
            {

            }
            public static InterFormMessage getInstance(IKeepable _targetObject, RelationsPackage _realtionspackage = null, string _msg = "")
            {
                InterFormMessage d = new InterFormMessage
                {
                    targetObject = _targetObject,
                    realtionspackage = _realtionspackage,
                    msg = _msg
                };
                return d;
            }
        }

        public static bool valueIsValidForType(string value, FieldTypeEnum fieldType)
        {
            //конвертируется ли строка в данные указанного типа
            switch (fieldType)
            {
                case FieldTypeEnum.Memo:
                case FieldTypeEnum.String:
                    //строки проходят без вопросов, т.к. любое значение можно сконвертировать в строку
                    return true;


                case FieldTypeEnum.Double:
                    try { double d = Convert.ToDouble(value); } catch { return false; }
                    break;

                case FieldTypeEnum.Int:
                    try { int i = Convert.ToInt32(value); } catch { return false; }
                    break;

                case FieldTypeEnum.Time:
                case FieldTypeEnum.Date:
                    try { DateTime d = Convert.ToDateTime(value); } catch { return false; }
                    break;

                case FieldTypeEnum.Bool:
                    try { DateTime d = Convert.ToDateTime(value); } catch { return false; }
                    break;

                default:
                    return false;
            }
            return true;
        }

        public static bool compareOperatorIsValidForType(RIFDC_DataCompareOperatorEnum dcOperator, FieldTypeEnum fieldType)
        {
            //применим ли данный оператор сравнения к данному типу данных
            //используется в фильтрации
            switch (fieldType)
            {
                case FieldTypeEnum.Memo:
                case FieldTypeEnum.String:
                    if (dcOperator == RIFDC_DataCompareOperatorEnum.contains ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.equal ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.notEuqal)

                    { return true; }
                    break;

                case FieldTypeEnum.Double:
                case FieldTypeEnum.Int:
                case FieldTypeEnum.Date:
                case FieldTypeEnum.Time:
                    if (dcOperator == RIFDC_DataCompareOperatorEnum.greaterEqual ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.lowerEqual ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.greater ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.lower ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.equal ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.notEuqal)
                    { return true; }
                    break;

                case FieldTypeEnum.Bool:
                    if (dcOperator == RIFDC_DataCompareOperatorEnum.equal ||
                        dcOperator == RIFDC_DataCompareOperatorEnum.notEuqal)

                    { return true; }
                    break;
            }
            return false;
        }

        public static string getLogicalOperatorName(RIFDC_LogicalOperators op)
        {
            switch (op)
            {
                case RIFDC_LogicalOperators.AND:
                    return "AND";

                case RIFDC_LogicalOperators.OR:
                    return "OR";

                case RIFDC_LogicalOperators.NOT:
                    return "NOT";

                case RIFDC_LogicalOperators.NotSpecified:
                    return "NotSpecified";

                default: return "";
            }
        }
        public enum FiltrationTypeEnum
        {
            Local = 1,
            Global = 2
        }
        public class Filter
        {
            //это фильтр, который передается на в метод *applyFilter Ikeeper для фильтрации
            // фильт - это множество объектов типа filteringRule, которые применяются в соответствии с _filteringExpression
            // filteringExpression - это выражение вида (((1 or 2 or 3) AND (4 or 5) AND (7)... ) OR 8), где цифра - это выражение "поле=значение"

            public List<FilteringRule> filteringRuleList = new List<FilteringRule>();

            string _customFilteringExpression = "";

            Lib.RIFDC_LogicalOperators logicalOperator = Lib.RIFDC_LogicalOperators.NotSpecified;

            public Filter(Lib.RIFDC_LogicalOperators _logicalOperator = Lib.RIFDC_LogicalOperators.OR)
            {
                if (_logicalOperator == Lib.RIFDC_LogicalOperators.OR || _logicalOperator == Lib.RIFDC_LogicalOperators.AND)
                {
                    logicalOperator = _logicalOperator;
                }
                else
                {
                    logicalOperator = Lib.RIFDC_LogicalOperators.OR;
                }
            }

            private void _addFilteringRule(FilteringRule _fr)
            {
                //надо понять, если ли такое правило
                // если это правило от parent сущности, то заменить

                bool sameType = false;
                bool setBySameEntity = false;
                bool haveSamefield= false;
                bool haveSameValue = false;

                foreach (FilteringRule fr in filteringRuleList)
                {
                    sameType = (fr.filteringRuleType == _fr.filteringRuleType || (fr.filteringRuleType!= FilteringRuleTypeEnum.NotSpecified));
                    //setBySameEntity = (_fr.setByObjectOfType == null) || (_fr.setByObjectOfType != null && fr.setByObjectOfType == _fr.setByObjectOfType);
                    setBySameEntity = (_fr.setByObjectOfType == null) || (_fr.setByObjectOfType != null && fr.setByObjectOfType == _fr.setByObjectOfType);
                    haveSamefield = fr.fieldInfoObject.fieldClassName == _fr.fieldInfoObject.fieldClassName;
                    haveSameValue = fr.filtrationValue == _fr.filtrationValue;
                    if (sameType && setBySameEntity)
                    {
                        //значит, мы нашли то самое правило, теперь смотрим, что в зависимости от типа с ним сделать
                        switch (fr.filteringRuleType)
                        {
                            case FilteringRuleTypeEnum.ParentFormFilteringRule:
                                //если это правило, установленное родительской формой, то просто заменить
                                _fr.ruleOrder = fr.ruleOrder;
                                filteringRuleList[filteringRuleList.IndexOf(fr)] = _fr;
                                return;

                            case FilteringRuleTypeEnum.ParentDFCFilteringRule:
                                //если это правило, установленное другим dfc внутри одной формы, то тоже просто заменить+
                                //TODO тут дублирование, но пока пусть
                                _fr.ruleOrder = fr.ruleOrder;
                                filteringRuleList[filteringRuleList.IndexOf(fr)] = _fr;

                                return;

                            case FilteringRuleTypeEnum.SearchFilteringRule:
                                if (haveSamefield && haveSameValue)
                                {
                                    //это то же самое правило, не трогать
                                    // _fr.ruleOrder = fr.ruleOrder;
                                    // filteringRuleList[filteringRuleList.IndexOf(fr)] = _fr;
                                    return;
                                }
                                break;
                                
                        }
                    }
                }

                //а еслион дошел до сюда, значнт, не нашел такого fr, и его надо добавить

                Lib.Filter.FilteringRuleValidationResult fvr = _fr.isValid();

                if (fvr.validationSuccess)
                {
                    _fr.ruleOrder = filteringRuleList.Count + 1; //автоматически присваиваем номер правилу
                    filteringRuleList.Add(_fr);
                }
                else
                {
                    //TODO более сложная обработка исключений
                    fn.mb_info("Созданное правило фильтрации не было добавлено: " + fvr.validationMsg);
                }
            }

            public void addNewFilteringRule(Lib.FieldInfo _fieldInfoObject, Lib.RIFDC_DataCompareOperatorEnum _filtrationOperator, string _filtrationValue, FilteringRuleTypeEnum _filteringRuleTypeEnum, Type _setByObjectOfType = null)
            {
                //создает правило по указанным параметрам

                if (_fieldInfoObject == null) return;

                FilteringRule fr = new FilteringRule(_fieldInfoObject, _filtrationOperator, _filtrationValue, _filteringRuleTypeEnum, _setByObjectOfType);

                _addFilteringRule(fr);
            }

            public void addExistingFilteringRule(FilteringRule _fr)
            {
                //если я пытаюсь добавить правило, уже созданное где-то -- напр, если я его откуда нибудь копирую

                _addFilteringRule(_fr);
            }

            public string filteringExpression
            {
                //если filteringExpression было хотя бы раз присвоено то считаем, что оно задано извне, и возвращаем его
                //но если оно присвоено не было, то возвращаем автосгенерированное
                get
                {
                    if (fn.toStringNullConvertion(_customFilteringExpression) == "")
                    {
                           return  generateDefaultFilteringExpression();
                    }
                    else
                    {
                        return _customFilteringExpression;
                    }
                       
                }
                set
                {
                    if (!filteringExpressionIsValid(value))
                    {
                        fn.mb_info("В объект Filter передано некорректное filtering expression: " + value);
                        //_filteringExpression = "";
                    }
                    else
                    {
                        _customFilteringExpression = value;
                    }

                }

            }

            public void resetMeByFilteringType_and_setByWhatObject(Lib.Filter.FilteringRuleTypeEnum filteringRuleType = Lib.Filter.FilteringRuleTypeEnum.NotSpecified, Type setByObjectOfType = null)
            {
                bool frTypeSpecified = filteringRuleType != FilteringRuleTypeEnum.NotSpecified;
                bool setByObjectSpecified = setByObjectOfType != null;

                if (!frTypeSpecified && !setByObjectSpecified) return;

                bool a = frTypeSpecified && setByObjectSpecified;
                bool b = frTypeSpecified && (!setByObjectSpecified);
                bool c = (!frTypeSpecified) && (setByObjectSpecified);

                bool condition = false;

                foreach (FilteringRule fr in filteringRuleList)
                {
                    condition =
                        (a && (fr.filteringRuleType == filteringRuleType && fr.setByObjectOfType == setByObjectOfType))
                            ||
                        (b && (fr.filteringRuleType == filteringRuleType))
                            ||
                        (c && (fr.setByObjectOfType == setByObjectOfType));

                    fr.deletionMark = condition;
                }

                filteringRuleList.RemoveAll(x => x.deletionMark);
            }


            private bool filteringExpressionIsValid(string filteringExpression)
            {
                //проверка валидности filteringExpression - это должна быть корректная фраза вроде (1 and 2) or not 3 с любым количеством скобок
                //как это делать
                // последовательной заменой атомарных выражений применением набора регексов
                //tbText.Text = "(((1and2and3and4and5)and(1and2and3and4and5)) and 8 ) and 99";

                string source = filteringExpression;

                string rez = "";

                bool hasMatches = false;

                bool atomarCorrect;
                Regex regex;
                string rgx = @" *\({1}[^\(\)]*\){1} *";

                MatchCollection mc;

                int iterationNo = 1;

                do
                {
                    //writeToRez((char)13 + "Итерация " + iterationNo.ToString());
                    //regex = new Regex("[^a-zA-Zа-яА-Я()+-_:;!?@#.*]", RegexOptions.None);

                    if (!source.Contains("("))
                    {
                        //признак того, что он позаменял все скобки и там осталось простое выражение
                        atomarCorrect = isAtomarExpression_level1_Valid(source);
                        if (atomarCorrect)
                        {
                            source = " expr ";
                        }
                        break;
                    }
                    else
                    {
                        regex = new Regex(rgx, RegexOptions.IgnoreCase);
                        mc = regex.Matches(source);
                        foreach (Match m in mc)
                        {
                            atomarCorrect = isAtomarExpression_level1_Valid(m.Value);
                            if (!atomarCorrect)
                            {
                                //writeToRez("ATOMAR INCORRECT ");
                                return false;
                            }
                        }

                        regex = new Regex(rgx, RegexOptions.IgnoreCase);
                        rez = regex.Replace(source, " expr ");
                        hasMatches = (rez != source);
                        //writeToRez(hasMatches ? "Есть замены " : "");
                        source = rez;
                        // writeToRez(source);
                        iterationNo += 1;
                    }
                }
                while (hasMatches);

                source = source.TrimEnd(' ');
                source = source.TrimStart(' ');
                bool rez1 = (source == "( expr )" || source == "expr");

                return rez1;

                //writeToRez(rez0 ? "Success" : "Выражение неверно!");
            }

            private bool isAtomarExpression_level1_Valid(string source)
            {
                //проверка правильности атомарного выражения (которое в скобках)
                //writeToRez("atomar_Приехало " + source);
                /*source = source.Replace("(", "");
                source = source.Replace(")", ""); */
                string rez = "";
                bool hasMatches = false;
                Regex regex;

                MatchCollection mc;

                //regex = new Regex(@"^ *not{0,1}[0-9]{1,2} *$", RegexOptions.IgnoreCase);
                regex = new Regex(@"^ *(not)? *[0-9]{1,2} *$", RegexOptions.IgnoreCase);
                //regex = new Regex(@""+tbMask.Text, RegexOptions.IgnoreCase);
                mc = regex.Matches(source);

                //writeToRez("atomar_Matches=" + mc.Count.ToString());

                if (mc.Count > 0)
                {
                    // writeToRez((char)13 + "atomar_Простое выражение");
                    foreach (Match m in mc)
                    {
                        //tbRez.Text += m.Value + (char)13;
                    }
                    return true;
                }
                else
                {
                    bool isItOrExpr = source.Contains("or");

                    //string digits = "(not{0,1} *[0-9]{1,2}| *expr{1})"; //not{0,1}
                    string digits = "( *(not)? *[0-9]{1,2} *| *expr{1} *)";

                    string op = isItOrExpr ? "(or){1}" : "(and){1}";

                    //string rgx = @"( *not? *" + digits + " *and{1} *not? *"+ digits + " *)|( *expr{1} *)";
                    string rgx = @"" + digits + op + digits;

                    do
                    {
                        //writeToRez("atomar_До замены" + source);
                        regex = new Regex(rgx, RegexOptions.IgnoreCase);
                        rez = regex.Replace(source, " expr ");
                        hasMatches = (rez != source);
                        source = rez;
                        //writeToRez("atomar_После замены" + source);
                    }
                    while (hasMatches);

                    source = source.TrimEnd(' ');
                    source = source.TrimStart(' ');

                    bool rez1 = (source == "( expr )" || source == "expr");

                    //writeToRez("atomar_" + rez1.ToString());
                    return rez1;
                }
            }

            private string generateDefaultFilteringExpression()
            {
                //генерирует выражение исходя из переданных объектов filteringRule

                //fr бывают разные, и в зависимости от того, что это за правила, надо их соединить по OR или AND

                // в данный момент есть всего 3 группы объектов filteringRule
                // 1) from parent form
                // 2) from another DFC
                // 3) from search / local filter

                // 1)) эти 3 группы соединяются по AND

                if (filteringRuleList.Count == 0) return "";

                string fromParentFormExpr = "";
                string fromDomesticDFCExpr = "";
                string fromNotSpecifiedExpr = "";
                string fromORTypeMultiselect = "";
                string fromORTypeSearch = "";

                //соединить по AND ParentFormFilteringRule
                fromParentFormExpr = concatFilteringExpr(FilteringRuleTypeEnum.ParentFormFilteringRule, "AND");

                //соединить по AND ParentDFCFilteringRule
                fromDomesticDFCExpr = concatFilteringExpr(FilteringRuleTypeEnum.ParentDFCFilteringRule, "AND");

                //соединить по AND где FilteringRuleTypeEnum.NotSpecified
                fromNotSpecifiedExpr = concatFilteringExpr( FilteringRuleTypeEnum.NotSpecified, "OR");

                //соединить по OR где FilteringRuleTypeEnum.SearchFilteringRule
                fromORTypeSearch = concatFilteringExpr(FilteringRuleTypeEnum.SearchFilteringRule, logicalOperator.ToString());

                //соединить по OR где FilteringRuleTypeEnum.ORTypeMultiselectFilteringRule
                fromORTypeMultiselect = concatFilteringExpr(FilteringRuleTypeEnum.ORTypeMultiselectFilteringRule, "OR");

                fn.stringByExpressionMerger mg = new fn.stringByExpressionMerger(" AND ");

                mg.addElement(fromParentFormExpr);
                mg.addElement(fromDomesticDFCExpr);
                mg.addElement(fromNotSpecifiedExpr);
                mg.addElement(fromORTypeMultiselect);
                mg.addElement(fromORTypeSearch);

                return mg.result;

                //return string.Join(" AND ", new List<string>() { fromParentFormExpr, fromDomesticDFCExpr, fromNotSpecifiedExpr, fromORTypeMultiselect, fromORTypeSearch })
            }


            private string concatFilteringExpr(FilteringRuleTypeEnum ft, string op)
            {
                string s = string.Join(op,
                                filteringRuleList
                                .Where(x => x.filteringRuleType == ft)
                                .Select(x => "B" + x.ruleOrder + "E")
                                .ToList());
                s = (s == "") ? "" : "(" + s + ")";
                return s;
            }
            public class FilteringRule
            {
                //это конкретное правило фильтрации
                public int ruleOrder;
                public FilteringRuleTypeEnum filteringRuleType;
                public Type setByObjectOfType;
                public string fieldClassName
                {
                    get { return fieldInfoObject.fieldClassName; }
                }
                public Lib.RIFDC_DataCompareOperatorEnum filtrationOperator;
                public string filtrationValue;
                public Lib.FieldInfo fieldInfoObject;

                public FilteringRule(Lib.FieldInfo _fieldInfoObject, Lib.RIFDC_DataCompareOperatorEnum _filtrationOperator, string _filtrationValue, FilteringRuleTypeEnum _filteringRuleType, Type _setByObjectOfType = null)
                {
                    filtrationOperator = _filtrationOperator;
                    filtrationValue = _filtrationValue;
                    fieldInfoObject = _fieldInfoObject;
                    filteringRuleType = _filteringRuleType;
                    setByObjectOfType = _setByObjectOfType;
                }

                public FilteringRuleValidationResult isValid()


                {
                    //проверяет, валидно ли это правило для данного объекта
                    // 1) указан правильный fieldClassName
                    // 2) указаны операторы, применимые к типу данных поля fieldClassName
                    // 3) filtrationValue годится для сравнения этим оператором (напр, строка "ываыв" - это не дата)

                    FilteringRuleValidationResult r = new FilteringRuleValidationResult();

                    // 1)  fieldClassName

                    Lib.FieldInfo f = fieldInfoObject;
                    if (f == null)
                    {
                        r.validationMsg = "Указано несуществующее fieldClassName в условиях фильтрации";
                        return r;
                    }

                    //2) теперь вопрос сопоставимости типов данных и операторов сравнения
                    Lib.FieldTypeEnum ft = f.fieldType;
                    bool operatorValid = Lib.compareOperatorIsValidForType(filtrationOperator, ft);
                    if (!operatorValid)
                    {
                        r.validationMsg = string.Format("Неверно указан оператор сравнения {0} для типа данных {1}  поля {2} в условиях фильтрации", filtrationOperator, ft.ToString(), fieldClassName);
                        return r;
                    }

                    // 3) корректное Value
                    bool valueIsValid = Lib.valueIsValidForType(filtrationValue, ft);
                    if (!valueIsValid)
                    {
                        r.validationMsg = string.Format("Неверно указано условие сравнения {0} для типа данных {1}  поля {2} в условиях фильтрации", filtrationValue, ft.ToString(), fieldClassName);
                        return r;
                    }

                    r.validationSuccess = true;
                    return r;
                }


                public string whereExpression
                {
                    get
                    {
                        //todo здесь дублирование кода с драйвером базы
                        Lib.FieldInfo f = fieldInfoObject;
                        string b = (f.fieldType == Lib.FieldTypeEnum.String | f.fieldType == Lib.FieldTypeEnum.Memo | f.fieldType == Lib.FieldTypeEnum.Date | f.fieldType == Lib.FieldTypeEnum.Time | f.fieldType == Lib.FieldTypeEnum.DateTime) ? "'" : "";
                        return string.Format("{0}={1}{2}{3}", f.fieldDbName, b, filtrationValue, b);
                    }
                }
                public bool deletionMark;
            }
            public class FilteringRuleValidationResult
            {
                public bool validationSuccess = false;
                public string validationMsg = "";
            }

            public enum FilteringRuleTypeEnum
            {
                ParentFormFilteringRule = 1,
                ParentDFCFilteringRule = 2,
                SearchFilteringRule = 3,
                ORTypeMultiselectFilteringRule =4, // это когда в родительской форме мультиселект надо 
                NotSpecified = 99
            }
        }

        public class Sorter
        {

            public delegate void ImSorted_EventHandler (Lib.Sorter sorter);
            
            public List<SortingRule> sortingRuleList = new List<SortingRule>();

            public bool doIHaveThisRule(SortingRule _sr)
            {
                if (_sr == null) return false;
                bool sameFieldInfo;
                bool sameSortDirection;
                foreach (SortingRule sr in sortingRuleList)
                {
                    sameFieldInfo = (sr.fieldInfoObject.fieldClassName == _sr.fieldInfoObject.fieldClassName);
                    sameSortDirection = true; //(sr.sortingDirection == _sr.sortingDirection);

                    if (sameFieldInfo && sameSortDirection) return true;
                }
                return false;
            }

            private void deleteSortingRule(SortingRule _sr)
            {
                sortingRuleList.Remove(_sr);
            }

            public void deleteRelativeSortingRules(SortingRule _sr)
            {
                //удалить все sr, где сортировка идет по тому же полю
                bool found;
                
                do
                {
                    found = false;
                    foreach (Lib.Sorter.SortingRule sr in sortingRuleList)
                    {
                        if (sr.fieldInfoObject.fieldClassName == _sr.fieldInfoObject.fieldClassName)
                        {
                            found = true;
                            sortingRuleList.Remove(sr);
                            break;
                        }
                    }
                }
                while (found);
            }

            private void _addSortingRule(SortingRule _sr)
            {
                if (doIHaveThisRule(_sr)) return;
                sortingRuleList.Add(_sr);
            }

            public void addNewSortingRule(Lib.FieldInfo _fieldInfoObject, Lib.AscDescSortEnum _sortingDirection)
            {
                SortingRule sr = new SortingRule(_fieldInfoObject, _sortingDirection);
                _addSortingRule(sr);
            }
            public void addExistingSortingRule(SortingRule _sr)
            {
                _addSortingRule(_sr);
            }

            public SortingRule ruleOnTheField(Lib.FieldInfo f)
            {
                if (f == null) return null;
                foreach (SortingRule sr in sortingRuleList)
                {
                    if (sr.fieldInfoObject.fieldClassName == f.fieldClassName)
                    {
                        return sr;
                    }
                }
                return null;
            }

            public class SortingRule
            {
                //это конкретное правило фильтрации
                public int ruleOrder;
                //public FilteringRuleTypeEnum filteringRuleType;
                //public Type setByObjectOfType;
                public string fieldClassName
                {
                    get { return fieldInfoObject.fieldClassName; }
                }

                public Lib.FieldInfo fieldInfoObject;
                public Lib.AscDescSortEnum sortingDirection;

                public SortingRule(Lib.FieldInfo _fieldInfoObject, Lib.AscDescSortEnum _sortingDirection)
                {
                    //это правило сортировки. оно ончеь простое - поле и порядок сортировки
                    //мы будем поддерживать только линейные правила сортировки, т.е. такого, как sorting expression, там не будет
                    fieldInfoObject = _fieldInfoObject;
                    sortingDirection = _sortingDirection;
                }

            }

            public enum SortingRuleSourceEnum
            {
                UserFieldSelection = 1, // пользователь поставил курсор на поле и нажал Сортировать Asc/Desc
                NotSpecified = 2
            }

        }

        public delegate void methodContainer();

        // ENUMS


        public enum AscDescSortEnum
        {
            Asc = 1,
            Desc = 2,
            NotSpecified = 3
        }

        public enum RIFDC_LogicalOperators
        {
            AND = 1,
            OR = 2,
            NOT = 3,
            NotSpecified = 99
        }

        public enum RIFDC_DataCompareOperatorEnum
        {
            equal = 1, // =
            greaterEqual = 2, //  >=
            lowerEqual = 3, // <= 
            greater = 4, // < 
            lower = 5, // >
            contains = 6,
            notEuqal = 7
        }

        public enum StorageMode
        {
            //режим хранения
            storeInDb = 1,
            storeAsFiles = 2,
            doNotStore = 3
        }

        public enum GroupQueryTypeEnum
        {
            Sum = 1,
            Count = 2
        }

        public enum KeepableClassStructureTypeEnum
        {
            Linear = 0,
            Tree = 1
        }
        public enum FieldTypeEnum
        {
            String = 0,
            Int = 1,
            Double = 2,
            Memo = 3,
            Bool = 4,
            Date = 5,
            Time = 6,
            DateTime = 7
        }

        public enum FrmCrudModeEnum
        {
            //режим, в котором осуществляется CRUD в данной форме

            GridOnly = 0, //все редактируем в гриде

            StandAloneFrmEdit = 1, //просомтр в гриде, но для редактирования открывается отдельная форма (страница) с полями

            GridAndFieldsOnTheFly = 2, //просмотр в гриде, редактирование здесь же в полях, запись апдейтится по факту выхода из поля (напр, удобно для заметок)
                                       //новая запись создается, а потом дается на редактирование. Напр, создать заметку, назвать Новая заметка.
                                       //TODO здесьапдейтить ли запись в БД сразу после изменения любого поля (тогда сильно много запросов к базе), или же после ухода человека с записи?

            GridAndFieldsSignificant = 3 //просмотр в гриде, просмотр здесь же в полях, запись выводится на редактирование кнопкой Edit,  апдейтится по кнопке Сохранить 
                                         //запись добавляется по кнопке Add, выводятся пустые поля, запись проверяется перед сохранением, т.е. не предсоздается
        }

        public enum GroupOperationTypeEnum
        {
            SetValue=1
        }
    }
}
