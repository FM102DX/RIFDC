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

    //RIFDC - Research inc ERM-движок 
    //предназначен для того, чтобы при создании приложений не писать для каждого типа объектов операции - чтение/запись/удаление в БД
    //когда программируем, мы делаем только объект с бизнес-полями, наследуюем его от keepable class
    //для упраления коллекциями объектов создается класс ItemKeeper<KeepableClass>

    //TODO - 6.12.2020 - читать из базы не все объекты, а часть. напр что-то типа механизма страниц, т.к. объектов там может быть оч. много, чтобы память не забивать. Ну пока он читает все.

    //TODO - не только сериализация объектов, но и автогенерация форм


    //TODO - объект готовит под себя базу, под свои поля == значит, будет список полей, то есть fieldlist
    //ну то есть я ввел название таблиц и забыл про это дело
    //это можно и позже сделать

    //TODO - кейс, когда у тебя объекты одного типа но с разной аналитикой хранятся в одной и той же таблице
    //решено-- вводим объект parentObject (хотя они и так не будут пересекаться, т.к. там есть отличающие их поля)


    public class IKeeperAlterationPackage
    {
        //это класс, который хранит пакет изменений для объекта IKeeper в виде List<fieldInfo>

        public string id = "";
        public DateTime alterationDateTimePoint;
        public string entityName = "";
        public List<Lib.FieldInfo> items = new List<Lib.FieldInfo>();
        public void add(Lib.FieldInfo f)
        {
            items.Add(f);
        }
        private IKeeperAlterationPackage() { }
        public static IKeeperAlterationPackage getInstance()
        {
            IKeeperAlterationPackage x = new IKeeperAlterationPackage();
            return x;
        }
    }

    public class KeepableClass : Relations.IRelatible, IKeepable
    {

        //private static bool fieldInfoObjectValidated=false; //результат проверки 

        public KeepableClass(IDataRoom _dataRoom)
        {
            //TODO дублирование методов в контрукторах убрать
            dataRoom = _dataRoom;
            fieldsInfo = _get_fieldsInfo();
            setNullability();
            setMyParameter("createdByUserId", RIFDC_App.currentUserId);
            setDefaultValues();
        }
        public KeepableClass()
        {
            fieldsInfo = _get_fieldsInfo();
            setNullability();
            setMyParameter("createdByUserId", RIFDC_App.currentUserId);
        }

        public virtual IKeeper getMyIkeeper()
        {
            return null;
        }

        public virtual bool isTreeViewBased
        {
            //является ли объект деревом. Если да, то в наследнике необходимо перегрузить со значением true
            get
            {
                return false;
            }
        }

        public void setDefaultValues(bool forNonNullableOnly = true)
        {
            //выставляет для параметров объекта значения по умолчанию
            if (forNonNullableOnly)
            {
                fieldsInfo.fields.Where(x => x.nullabilityInfo.allowNull == false).ToList().ForEach(y=> {
                    setMyParameter(y.fieldClassName, y.nullabilityInfo.defaultValue);
                });
            }

        }

        public string id { get; set; }

        public virtual string displayNameLong { get { return ""; } }

        public string parentId { get; set; } //id parent-объекта, т.е. объекта того же типа, но того, который выше по дереву

        public int level { get; set; } = 0; //уровень в иерархии

        public DateTime createdDateTime;

        public DateTime lastModifiedDateTime;

        public string createdByUserId = "";

        public IDataRoom dataRoom { get; set; }

        public virtual Lib.KeepableClassStructureTypeEnum structureType { get; }
        
        public object holderObject;

        //порядок среди аналогичных элементов. Если холдер линейный, то внутри всего множества, если древовидный - внутри конкретного родителя
        public int order { get; set; }

        public void saveMyPhoto()
        {
            //сохраняет значения параметров объекта в переменной
            //Нужно для того, чтобы понимать, какие записи были изменены с момента прочтения (dirty)

            foreach (Lib.FieldInfo x in fieldsInfo.fields)
            {
                x.actualValue = getMyParameter(x.fieldClassName);
            }
        }

        public Validation.ValidationResult validateMyParameter(Validation.ValidationTypeEnum validationType, string parameterClassName, object value)
        {

            //определяет, может ли value быть значением / частью значения параметра parameterName исходя из набора правил валидации
            //обычно: 
            // посимвольная валидация возвращает true/false, т.е. проходит символ или нет
            // leave -валидация возвращает правильное значение поля (убирает из него ненужные символы и выражения)
            // бизнес-валидация возвращает ошибки, связанные с бизнес-правилами. Напр, цена кофе не может быть меньше 80 руб.

            Validation.ValidationResult vr = new Validation.ValidationResult();
            vr.validationSuccess = false;

            Lib.FieldInfo f = fieldsInfo.getFieldInfoObjectByFieldClassName(parameterClassName);
            if (f == null)
            {
                vr.validationMsg = "Ошибка: не найден объект fieldInfo";
                return vr;
            }


            //это очень важная строчка, которая гарантирует, что в функцию валидации будет передан object именно того типа, что хранится в объекте
            // 11 фев 2021 это передано в функции, т.к. если это делать здесь, теряется содержимое value, и тогда валидационные функции не могут работать как надо
            //value = Lib.convertedObjectRIFDCTypes(f.fieldType, value);

            vr = Validation.validate(f, validationType, value);
            return vr;
        }



        public Lib.ObjectOperationResult setMyParameter(string parameterClassName, object value)
        {
            //Lib.CommonOperationResult or = new Lib.CommonOperationResult();
            // 01.06.2021 TODO много дублирования, запутывает
            Lib.FieldInfo f = this.fieldsInfo.getFieldInfoObjectByFieldClassName(parameterClassName);

            if (f == null)
            {
                return Lib.ObjectOperationResult.sayNo("Не найден объект fieldInfo для параметра ");
            }

            //nullable
            // пришел null => присвоить null
            // пришел !null => присвоить value через try-catch, если нет то null

            //!nullable
            // пришел null => присвоить defaultValue
            // пришел !null => присвоить value через try-catch, если нет то defaultValue
            ObjectParameters.ObjectParameterOperationResult setValueOperationResult;

            if (f.nullabilityInfo.allowNull)
            {
                if (value == null)
                {
                    f.nullabilityInfo.considerNull = true;
                    return Lib.ObjectOperationResult.sayOk();
                }
                else
                {
                    try
                    {
                        setValueOperationResult = ObjectParameters.setObjectParameter(this, parameterClassName, value);
                        if (setValueOperationResult.success)
                        {
                            f.nullabilityInfo.considerNull = false; //скинуть эту галочку
                            return Lib.ObjectOperationResult.sayOk();
                        }
                        else
                        {
                            return Lib.ObjectOperationResult.sayNo(setValueOperationResult.msg);
                        }
                        
                    }
                    catch
                    {
                        //TODO интересно, что тут сделать - 
                        // а) выдать ошибку 
                        // б) присвоить null 
                        // в) ничего не делать?

                        //выбираю ошибку
                        return Lib.ObjectOperationResult.sayNo("Catch error while trying to assign value");
                    }
                }
            }
            else
            {
                if (value == null)
                {
                    setValueOperationResult = ObjectParameters.setObjectParameter(this, parameterClassName, f.nullabilityInfo.defaultValue);
                    if (setValueOperationResult.success)
                    {
                        return Lib.ObjectOperationResult.sayOk();
                    }
                    else
                    {
                        return Lib.ObjectOperationResult.sayNo(setValueOperationResult.msg);
                    }
                }
                else
                {
                    try
                    {
                        setValueOperationResult =ObjectParameters.setObjectParameter(this, parameterClassName, value);
                        if (setValueOperationResult.success)
                        {
                            return Lib.ObjectOperationResult.sayOk();
                        }
                        else
                        {
                            return Lib.ObjectOperationResult.sayNo(setValueOperationResult.msg);
                        }
                        //а здесь галочку не скидываем, т.к. если !nullable, то 

                    }
                    catch
                    {
                        setValueOperationResult = ObjectParameters.setObjectParameter(this, parameterClassName, f.nullabilityInfo.defaultValue);
                        if (setValueOperationResult.success)
                        {
                            return Lib.ObjectOperationResult.sayOk();
                        }
                        else
                        {
                            return Lib.ObjectOperationResult.sayNo(setValueOperationResult.msg);
                        }
                    }
                }
            }
        }

        public void setMyMultipleParameters(IKeeperAlterationPackage source)
        {
            foreach (Lib.FieldInfo f in source.items)
            {
                setMyParameter(f.fieldClassName, f.actualValue);
            }
        }

        public object getMyParameter(string fieldClassName)
        {
            //сюда закладываем логику nullabiity
            // если nullable & isnull, return null
            // если nullable & !isnull, return value

            // если !nullable, return value

            //таким образом, у каждого поля надо указывать то значение, которое мы считаем null (?) - да, но это так не работает
            // например, у дат есть значение, которое мы считаем null - это 01 01 0001 00 00 00, т.е. дата сама по себе null null не принимает
            // 03/01/2021 -- использую галку, nummConsideredValue НЕ использую

            //вот оно. если там геттер, то значение не присваивается, т.к. это не произжает через 

            Lib.FieldInfo f = fieldsInfo.getFieldInfoObjectByFieldClassName(fieldClassName);

            if (f == null) return null;

            bool nullable = f.nullabilityInfo.allowNull;

            bool isNull = f.nullabilityInfo.considerNull;

            // надо как-то узнать, является ли поле геттером, т.к. если это геттер, то знак nullabilityInfo.isNull не был сброшен
            bool isGetter = ObjectParameters.isItOnlyGetter(this, fieldClassName);

            if (isNull && isGetter) isNull = false; // геттер не может быть null

            object defaultValue = f.nullabilityInfo.defaultValue;

            if (nullable & isNull) return null;

            object value = ObjectParameters.getObjectParameterByName(this, fieldClassName)?.value;

            if (nullable & (!isNull)) return value;

            if ((!nullable) & (value == null)) return defaultValue;

            return value;
        }

        public List<IKeepable> getMyDependentObjectsFromRelation (Relations.Relation relation)
        {
            //это притаскивает зависимые объекты из этого relation c фильтром по связанному полю данного объекта
            // итак
            Relations.Relation.RelationSide otherRelSide = relation.getAnotherRelationSide(this);
            Relations.Relation.RelationSide mylSide = relation.getMyRelationSide(this);
            IKeeper targetKeeper = otherRelSide.myIKeeper;
            if (targetKeeper == null) return null;

            //ок, тогда вот его кипер, теперь надо сюда применить фильтр и прочитать
            
            List<IKeepable> rez = new List<IKeepable>();

            Lib.Filter filter = new Lib.Filter();

            string _val = fn.toStringNullConvertion(getMyParameter(mylSide.fieldName));

            if (_val == "") return null;

            filter.addNewFilteringRule(otherRelSide.fieldInfo, Lib.RIFDC_DataCompareOperatorEnum.equal, _val, Lib.Filter.FilteringRuleTypeEnum.NotSpecified, mylSide.targetClass);
            targetKeeper.filtration.applyGlobalFilter(filter);

            targetKeeper.readItems();

            foreach (IKeepable x in targetKeeper.items)
            {
                rez.Add(x);
            }

            return rez;
        }

        public IKeeperAlterationPackage getMyDirtyFields4History()
        {
            if (!fieldsInfo.historySavingAlloyed) return null;

            IKeeperAlterationPackage x = IKeeperAlterationPackage.getInstance();
            
            Lib.FieldInfo f0;
            string newVal;
            string oldVal;
            x.entityName = entityName;
            foreach (Lib.FieldInfo f in fieldsInfo.fields)
            {
                if (f.saveHistory)
                {
                    if (isFieldDirty(f.fieldClassName))
                    {
                        oldVal = fn.toStringNullConvertion(f.actualValue);
                        newVal = fn.toStringNullConvertion(getMyParameter(f.fieldClassName));
                        f0 = new Lib.FieldInfo();
                        f0.fieldClassName = f.fieldClassName;
                        f0.actualValue = oldVal;
                        f0.newValue = newVal;
                        x.add(f0);

                       // Logger.log("getMyDirtyFields4History_test", string.Format("oldValue={0} newValue={1} ", oldVal, newVal));
                    }
                }
            }
            return x;
        }

        //возвращает - был ли изменен объект с момента прочтения из базы (или  с точки сохранения)
        public bool isFieldDirty(string fieldClassName)
        {
            bool b = false;
            object val1 = null;
            object val2 = null;
            //взять значени из текщего поля объекта
            //взять значение этого же поля из actualValues
            //сравнить

            Lib.FieldInfo f = fieldsInfo.getFieldInfoObjectByFieldClassName(fieldClassName);

            //val1 =  ObjectParameters.getObjectParameterByName(this, fieldClassName).value;
            val1 = getMyParameter(fieldClassName);
            val2 = f.actualValue;

            //fn.dp(fieldClassName + "current=" + val1.ToString() + " saved="+val2.ToString() + ", type is " + f.fieldType.ToString());

            //TODO слишком простой подход к сравнению объектов, хотя и рабочий
            if (!Lib.RIFDCObjectsEqual(val1, val2, f.fieldType))
            {
                b = true;
            }
            //fn.dp((!b) ? "Равны" : "Не равны");

            // Logger.log("COMPARE", string.Format("Comparing val1={0} and val2={1} at field {2} rezult is {3}", val1, val2, fieldClassName, (b ? "dirty" : "not dirty")));
            return b;
        }

        public Lib.FieldsInfo fieldsInfo { get; } //список полей

        public virtual Lib.FieldsInfo _get_fieldsInfo() { return null; }

        //public Lib.FieldsInfo actualValues; //список полей со значениями 
        public virtual string tableName { get; }
        public virtual string entityName { get; } //TODO проверка классов на уникальность этого поля

        public virtual bool storeSerialized { get; }

        //объект, из которго создан данный. напр, для связки Project=> ProjectPages=> Page, для объекта Page  parentObject-ом будет Project

        //public KeepableClass parentObject { get; set; }

        //это не надо, тк. достается по groupObject

        private void setNullability()
        {
            //отработать nullability и присвоить значения по умолчанию где нужно

            foreach (Lib.FieldInfo f in this.fieldsInfo.fields)
            {
               // fn.dp(f.fieldClassName);

                if (f.nullabilityInfo.allowNull)
                {
                    //если указано значение по умолчанию, то присвоить его, если нет - просто занулить и все
                    if (f.nullabilityInfo.defaultValue == null)
                    {
                        f.nullabilityInfo.considerNull = true;
                    }
                    else
                    {
                        setMyParameter(f.fieldClassName, f.nullabilityInfo.defaultValue);
                    }
                }
                else
                {
                    //там была проверка на это, так что если f.nullabilityInfo.allowNull = 0 то defaultValue есть всегда
                    setMyParameter(f.fieldClassName, f.nullabilityInfo.defaultValue);
                }
            }
        }

        public string generateMyId()
        {
            //генерит id этого объекта
            return entityName + "-" + fn.generate4blockGUID();
        }

        public virtual string displayId
        {
            //для отображения в контролах, которые требуют публичных геттеров, напр Combobox
            get { return fn.toStringNullConvertion(id); }
        }
        public virtual string displayName
        {
            //для отображения в контролах, которые требуют публичных геттеров, напр Combobox
            get { return ""; }
        }

        public Lib.FieldInfo getFieldInfoByFieldClassName(string fieldClassName)
        {
            return fieldsInfo.getFieldInfoObjectByFieldClassName(fieldClassName);
        }

        public string entityType
        {
            get
            {
                return fn.getEntityTypeFromFullTypeNameString(GetType().ToString());
            }
        }
        public string objectDump()
        {
            //значения всех полей объекта в строку
            return Lib.getObjectStr(this);
        }

        private bool valueEqualsDefaultValue (Lib.FieldInfo f)
        {
            object val1 = getMyParameter(f.fieldClassName); ;
            object val2 = f.nullabilityInfo.defaultValue;
            bool equal = Lib.RIFDCObjectsEqual(val1, val2, f.fieldType);
            return equal;
        }

        public bool isDataPresenceValid
        {
            get
            {
                //присутствуют ли значения во всех полях, которые определяют не-пустые строки
                //используется при чтении из экселя, чтобы не читать пустые строки
                bool valid = true;
                foreach (Lib.FieldInfo f in fieldsInfo.fields)
                {
                    if (f.isDataPresenceMarker)
                    {
                        //если оно nullable
                        try
                        {
                            if (f.nullabilityInfo.allowNull && f.nullabilityInfo.considerNull) { valid = false; }
                          //  if ((!f.nullabilityInfo.allowNull) && valueEqualsDefaultValue(f)) { valid = false; }
                            //    fn.dp(String.Format("f.name={0}, allowNull={1}, isNull={2}", f.fieldClassName, f.nullabilityInfo.allowNull, f.nullabilityInfo.isNull));

                        }
                        catch (Exception e)
                        {
                            //fn.dp("Point 0 " + e.Message);
                        }
                    }
                }
                return valid;
            }
        }

        public Lib.FieldsInfo getInitialFieldsInfoObject(
                                bool _allowId = true,
                                bool _allowDateTimeOfCreation = true,
                                bool _allowDateTimeOfLastChange = true,
                                bool _allowCreatedByUserId = true)
        {
            //возвращает объект, где уже добавлены все обязательные поля - id и прочее
            Lib.FieldsInfo f = new Lib.FieldsInfo();
            f.myType = this.GetType();
            f.allowId = _allowId;
            f.allowDateTimeOfCreation = _allowDateTimeOfCreation;
            f.allowDateTimeOfLastChange = _allowDateTimeOfLastChange;
            f.allowCreatedByUserId = _allowCreatedByUserId;
            f.tableName = tableName;
            f.entityName = entityName;
            Lib.FieldInfo x;


            if (f.allowId)
            {
                x = f.addFieldInfoObject("id", "id", Lib.FieldTypeEnum.String); x.isPrimaryKey = true; // 27.03.2021  все, Id теперь строковый
            }

            //эти 2 ставятся базой в моменты создания и изменения
            if (f.allowDateTimeOfCreation)
            {
                f.addFieldInfoObject("createdDateTime", "createdDateTime", Lib.FieldTypeEnum.DateTime);
            }

            if (f.allowDateTimeOfLastChange)
            {
                f.addFieldInfoObject("lastModifiedDateTime", "lastModifiedDateTime", Lib.FieldTypeEnum.DateTime);
            }
            if (f.allowCreatedByUserId)
            {
                f.addFieldInfoObject("createdByUserId", "createdByUserId", Lib.FieldTypeEnum.String);
            }
            if (isTreeViewBased)
            {
                f.addFieldInfoObject("parentId", "parentId", Lib.FieldTypeEnum.String);
                f.addFieldInfoObject("level", "level", Lib.FieldTypeEnum.Int);
            }
            x = f.addFieldInfoObject("searchable", "", Lib.FieldTypeEnum.String);
            x.parameterSignificanceInfo.significanceType = Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.LocallyCountableGetter;



            // 27.03.2021 переход на новую систему ID
            //x = f.addFieldInfoObject("dbprefix", "dbprefix", Lib.FieldTypeEnum.String); x.nullabilityInfo.allowNull = false;
            //  x = f.addFieldInfoObject("_id", "_id", Lib.FieldTypeEnum.Int); x.isCounter = true;

            return f;
        }


        public Lib.Filter getIncomingFilterFromInterFormMessage(Lib.InterFormMessage msg)
        {
            //создать фильтр, используя InterFormMessage

            if (msg.realtionspackage == null) return null;
            if (msg.realtionspackage.count == 0) return null;


            Lib.Filter f = new Lib.Filter();

            string s = "";

            Relations.Relation.RelationSide rMe = null;

            Relations.Relation.RelationSide rMaster = null;

            //теперь надо взять этот realtionspackage и найти там тот объект relation, где одной из сторон является объект данного типа
            List<Relations.Relation> myRels = msg.realtionspackage.getMyRelations(entityType);
            if (myRels.Count == 0) return null;

            //если нам нужно извлечь фильтр из сообщения, то он будет в первом же "нашем" объекте relation

            try
            {
                rMe = myRels[0].getRealtionSideByClassName(entityType);
                if (rMe.relationSideQualifier == "A")
                {
                    rMaster = myRels[0].B;
                }
                else
                {
                    rMaster = myRels[0].A;
                }
            }
            catch
            {
                return null;
            }

            if (rMe == null || rMaster == null) return null;

            f.addNewFilteringRule(
                rMe.fieldInfo,
                Lib.RIFDC_DataCompareOperatorEnum.equal,
                fn.toStringNullConvertion(msg.targetObject.getMyParameter(rMaster.fieldName)),
                Lib.Filter.FilteringRuleTypeEnum.ParentFormFilteringRule,
                rMaster.targetClass);

            return f;

        }

        public string searchable { get { return string.Join(" ", fieldsInfo.getMySearchableFields().Select(x => getMyParameter(x.fieldClassName)).ToList()); }  }
        //{ get { return ""; } }

        public bool needToSaveMyHistory 
        { 
            get 
            { 
                return fieldsInfo.historySavingAlloyed;
            }
        }

        public virtual string getMyStringRepresentation()
        {
            return "";
        }

    }

    public partial class ItemKeeper<T> : IKeeper where T : IKeepable, new()
    {
        //это класс, хранящий список объектов Т и поддерживающий их CRUD
        ItemKeeper(IDataRoom _dataRoom = null, bool drop = false)
        {
            //  if (_dataRoom == null) { fn.dp("ERROR: ItemKeeper creation requires dataRoom object"); return; }
            //Можно ли создавать IK без DR ?
            items0 = new FilteredSortedItemList(this);
            filtration = items0;
            sort = items0;
            currentRecord = new CurrentRecord(this);
            dataRoom = _dataRoom;

            if (!memoryOnlyMode)
            {
                // 27.03.2021 новая система id, теперь каждый старт ItemKeeper сопровождается проверкой таблицы
                Lib.DbOperationResult rte = checkMyTable(drop);
                if (!rte.success)
                {
                    myStatusIsOk = false;
                }
            }

        }

        public bool myStatusIsOk = true;

        public static ItemKeeper<T> getInstance(IDataRoom _dataRoom = null, bool drop=false)
        {
            ItemKeeper<T> x = new ItemKeeper<T>(_dataRoom, drop);
            return x;
        }

        Random rnd = new Random();

        public IKeepable GetRandomObject()
        {
            int cnt = actualItemList.Count;

            if (cnt == 0) return null;

            

            int _rnd = rnd.Next(0, cnt);

            return actualItemList[_rnd];

        }
        public bool isTreeViewBased { get { return sampleObject.isTreeViewBased; } }

        public bool readingItemsFlag = false;

        public IEnumerable items { get { return items0; } }
        FilteredSortedItemList items0 { get; } //список элементов
        public List<IKeepable> actualItemList { get { return items0.actualItemList; } }
        public IKeepable sampleObject { get { T t = new T(); return t; } } // экземпляр хранимого объекта, чтобы пользоваться его внутренней логикой
        public IFilteratonMethodGroup filtration { get; }
        public ISortMethodGroup sort { get; }
        public IDataRoom dataRoom { get; set; }

        public IKeeperWrap parentKeeper = null;

        public bool memoryOnlyMode
        {
            get
            {
                if (dataRoom == null) return true; else return false;
            }
        }

        public ICurrentRecord currentRecord { get; } //сведения о текущей записи

        private string _currentId; //id текущего элемента //TODO может, их в CurrentRecord и хранить? чтобы тут не светились

        private int _currentIndex; //индекс текущего элемента в коллекции

        public Lib.ObjectOperationResult makeGroupQuery(Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField)
        {
            // выполняет групповые запросы для каждого элемента данной сущности по зависимым сущностям
            // генерит запрос типа, например, Select COUNT…  UNION select Count.... 
            //это пересчет статистики, но, возможно, TODO стоит это сделать в рамках технологии relationChains как тип поля

            List<IUniversalRowDataContainer> rez= dataRoom.makeGroupQuery(this, groupQueryType, targetRelation, targetField);
            
            object val;

            foreach (IKeepable x in items)
            {
                val = Lib.getFieldValueFromUDC(rez, "extid", x.id, "reznum");
                x.setMyParameter(targetField, val);
                saveItem(x);
            }

            return null;
        }
        public Lib.DbOperationResult checkMyTable(bool drop=false)
        {
            //создает таблицу переданного класса T
            //проверяем, есть ли там tableName  в базе
            if (memoryOnlyMode)
            {
                return null;
            }
            else
            {
                return dataRoom.checkObjectTable(sampleObject, drop);
            }
        }

        public Lib.ObjectOperationResult saveItem(IKeepable t)
        {
            if (t == null) return Lib.ObjectOperationResult.sayNo("Cant save object because it's null");

            bool isItItemUpdate = (fn.toStringNullConvertion(t.id) != "");

            if (memoryOnlyMode)
            {
                return Lib.ObjectOperationResult.sayOk();
            }
            else
            {
                IKeeperAlterationPackage alt = t.getMyDirtyFields4History();
                Lib.DbOperationResult dbr = dataRoom.saveObject(t);

                //здесь их уже нет, значит, надо сохранить
                if (dbr.success & isItItemUpdate & t.needToSaveMyHistory)
                {
                    if (alt != null)
                    {
                        //сохранить историю
                        alt.alterationDateTimePoint = dbr.updatedDateTime;
                        alt.id = fn.toStringNullConvertion(t.getMyParameter("id"));
                        Lib.ObjectOperationResult opr = HistorySaver.getInstance(dataRoom).doSaveHistory(alt);
                        if (!opr.success)
                        {
                            //вернуть все обратно
                            t.setMyMultipleParameters(alt);
                        }
                    }
                }
                else
                {
                    Lib.ObjectOperationResult.sayNo("Error saving object");
                }

                t.saveMyPhoto();

                return Lib.ObjectOperationResult.getInstance(dbr.success, dbr.msg, dbr.createdObjectId);
            }
        }

        public Lib.ObjectOperationResult deleteItem(IKeepable t, bool silent=false)
        {

            // TODO onlySetDeletionMark - это пометка на удаление, функциональность пока не реализована
            // TODO дублирование кода

            if (memoryOnlyMode)
            {
                removeItem_from_collection(t.id);
                return Lib.ObjectOperationResult.sayOk();
            }
            else
            {
                Lib.ObjectOperationResult _rez = canDeleteItem(t);
                if (!_rez.success)
                {
                    //if(!silent) fn.mb_info(_rez.msg);
                    return Lib.ObjectOperationResult.sayNo(_rez.msg);
                }

                Lib.DbOperationResult dbr = dataRoom.deleteItem(t);

                if (dbr.success)
                {
                    removeItem_from_collection(t.id);
                    return Lib.ObjectOperationResult.sayOk();
                }
                else
                {
                    return Lib.ObjectOperationResult.getInstance(false, dbr.msg);
                }
            }
        }

        public List<Lib.ObjectOperationResult> deleteMultipleItems(List<IKeepable> tl, bool silent = false)
        {
            // удалить несколько элементов

            List<Lib.ObjectOperationResult> rez = new List<Lib.ObjectOperationResult>();

            foreach (IKeepable x in tl)
            {
                rez.Add(deleteItem(x, silent));
            }

            return rez;

        }

        public Lib.ObjectOperationResult canDeleteItem(IKeepable t)
        {
            //здесь определяем, можно ли удалить данный объект

            //если это дерево, то там не дожлжно быть потомков
            if(isTreeViewBased)
            {
                IKeeper tmpKeeper = sampleObject.getMyIkeeper();
                tmpKeeper.dataRoom = RIFDC_App.mainDataRoom;
                Lib.Filter tmpFilter = new Lib.Filter();
                tmpFilter.addNewFilteringRule(t.fieldsInfo.getFieldInfoObjectByFieldClassName("parentId"), Lib.RIFDC_DataCompareOperatorEnum.equal, t.id, Lib.Filter.FilteringRuleTypeEnum.NotSpecified);
                tmpKeeper.filtration.applyGlobalFilter(tmpFilter);
                tmpKeeper.readItems();
                if (tmpKeeper.count > 0)
                {
                    tmpKeeper = null;
                    GC.Collect();
                    return Lib.ObjectOperationResult.sayNo("Невозможно удалить элемент, т.к. у него есть дочерние элементы");
                }
                tmpKeeper = null;
                tmpFilter = null;
                GC.Collect();
            }

            // перебрать все связи объекта, где его присутствие обязательно, т.е. свзяи obligatory
            // посмотреть, еслть ли по этим связям зависимые сцщности

            List<Relations.Relation> rels = RIFDC_App.relationsHolder.getMyRelations_where_Im_obligatory(t);

            foreach (Relations.Relation rel in rels)
            {
                Relations.Relation.RelationSide rls_my_side = rel.getMyRelationSide(t);
                Relations.Relation.RelationSide rls_another_side = rel.getAnotherRelationSide(t);

                //а теперь надо посомтреть, есть ли по этому relation что-нибудь
                // для этого надо создать ItemKeeper по этому типу объекта , сделать фильтр и прочитать элементы
                // если их >0, удалять нельзя
                //ItemKeeper<> 
                //ItemKeeper<rls.targetClass> x = new ItemKeeper<rls.targetClass>();

                IKeeper x = RIFDC_App.iKeeperSampleHolder.getIKeeperByEntityType(rls_another_side.targetClassName);
                x.dataRoom = RIFDC_App.mainDataRoom;
                if (x == null) return Lib.ObjectOperationResult.sayNo();

                Lib.Filter f = new Lib.Filter();
                f.addNewFilteringRule(
                    rls_another_side.fieldInfo,
                    Lib.RIFDC_DataCompareOperatorEnum.equal,
                    fn.toStringNullConvertion(t.getMyParameter(rls_my_side.fieldInfo.fieldClassName)),
                    Lib.Filter.FilteringRuleTypeEnum.NotSpecified);
                x.filtration.applyGlobalFilter(f);
                x.readItems();
                //теперь вопрос - я должен проверять только зависимость по этому объекту, или еще там фильтры по текущему пользователю и т.п.?
                //будем исходить из того, что бизнес-объект уникален везде

                if (x.count > 0) 
                { 
                    return Lib.ObjectOperationResult.sayNo("The object you're trying to delete has instances of dependent entity: " + rls_another_side.targetClassName); 
                }
            }

            return Lib.ObjectOperationResult.sayOk();

        }


        public Lib.ObjectOperationResult deleteFiteredPackege(Lib.Filter filter)
        {
            if (filter == null)
            {
                return Lib.ObjectOperationResult.sayNo("One or more arguments are null");
            }

            if (memoryOnlyMode)
            {
                return Lib.ObjectOperationResult.sayNo("It's memoryOnlyMode");
            }
            //todo тут тоже надо вписать правила удаления
            Lib.DbOperationResult dbr = dataRoom.deleteFiteredPackege(sampleObject, filter);

            if (dbr.success)
            {
                return Lib.ObjectOperationResult.sayOk();
            }
            else
            {
                return Lib.ObjectOperationResult.sayNo(dbr.msg);
            }
        }

        public IKeepable getItemById(string id)
        {
            foreach (T item in items0)
            {
                if (item.id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public void addExistingObject(IKeepable t)
        {
            items0.addExistingObject((T)t);
            //ну да, получается каждый раз по добавлении или удалении юнита он делает сортировку и фильтрацию, а как по другому-то?
            //filtration.reApplyFilter();
            //sort.reApplySorter();
        }

        public IKeepable createNewObject_notInserted()
        {
            //просто чистый объект без ничего
            IKeepable t0 = new T();

            // если есть предустановка, то сохранить ее значение в соотв. поле
            if (filtration.myActualGlobalFilter != null)
            {
                foreach (Lib.Filter.FilteringRule fr in filtration.myActualGlobalFilter.filteringRuleList)
                {
                    if (fr.filteringRuleType == Lib.Filter.FilteringRuleTypeEnum.ParentFormFilteringRule)
                    {
                        t0.setMyParameter(fr.fieldClassName, fr.filtrationValue);
                    }

                    if (fr.filteringRuleType == Lib.Filter.FilteringRuleTypeEnum.ParentDFCFilteringRule)
                    {
                        t0.setMyParameter(fr.fieldClassName, fr.filtrationValue);
                    }
                }
            }

            t0.fieldsInfo.fields
                                .Where(x=>x.nullabilityInfo.allowNull==false)
                                .ToList()
                                .ForEach(y=> {

                                    fn.dp(string.Format("fieldClassName={0} defaultValue={1} allownull={2}", y.fieldClassName, y.nullabilityInfo.defaultValue, y.nullabilityInfo.allowNull));
                                    t0.setMyParameter(y.fieldClassName, y.nullabilityInfo.defaultValue); });

            return t0;
        }
        public IKeepable createNewObject_inserted()
        {
            IKeepable t0 = createNewObject_notInserted();
            addExistingObject(t0);
            return t0;
        }
        public void clear()
        {
            items0.clear();
        }
        public int count
        {
            get { return items0.actualItemList.Count(); }
        }
        private void removeItem_from_collection(string id)
        {
            items0.removeItem(id);
        }


        //public void readItems(Lib.Filter filter = null, bool append = false) //20.04. теперь readItems читает все только с myActualFilter, он неразрывно с ним связан
        public void readItems(bool append = false)
        {
            // читает множество объектов Т из базы
            // все, что касается конструирования запросов к конкретному виду БД и их выполнения, отдано классу IDataRoom
            // здесь только то, что касается хранения объектов в колекции

            Lib.Filter filter = filtration.myActualGlobalFilter;

            if (memoryOnlyMode) return;

            readingItemsFlag = true;

            if (!append) items0.clear();

            List<IUniversalRowDataContainer> udc = dataRoom.readItems(sampleObject, filter);

            if (udc==null)
            {
                return;
            }

            object _tmp;

            foreach (Lib.UniversalDataKeeper d in udc)
            {
                //перебираем по порядку поля, кот. надо присвоить
                T newObject = new T();
                foreach (Lib.FieldInfo f in newObject.fieldsInfo.fields) // это метаполя, котоыре надо присвоить
                {
                    _tmp = d.getValueByName(f.fieldClassName);
                    newObject.setMyParameter(f.fieldClassName, _tmp);
                }

                //теперь присвоить холдер, т.е. присваиваем создаваемому объекту ссылку на этот класс
                ObjectParameters.setObjectParameter(newObject, "holderObject", this);

                // если там нет id, сделать его
                if (fn.toStringNullConvertion(newObject.id) == "") newObject.setMyParameter("id", newObject.generateMyId() + "-tmp");

                //сохранить свои параметры
                if (newObject.isDataPresenceValid)
                {
                    try
                    {
                        newObject.saveMyPhoto();
                        addExistingObject(newObject);
                    }
                    catch (Exception e)
                    {
                       // fn.dp("Point 1 " + e.Message);
                    }
                }
            }
            readingItemsFlag = false;
        }

        public void arrangeTreeLevels()
        {
            //пересчитать levels для всего дерева 
            //просто берем тех, кто parent="" и перебираем их
            //затратная процедура
            if (!isTreeViewBased) return;
            actualItemList.Where(x => fn.toStringNullConvertion(x.parentId) == "")
                          .ToList()
                          .ForEach(y=> {
                              calculateTreelevels4Element(y);
                          });
        }

        private void calculateTreelevels4Element (IKeepable element)
        {
            //вычисляем level передаваемого элемента и всех потомков
            //считаем, что level родителя достоверен
            if (!isTreeViewBased) return;

            //берем родителя

            IKeepable myParent = actualItemList.Where(x => x.id == element.parentId).FirstOrDefault();

            if (myParent == null)
            { 
                element.setMyParameter("level", 0);
            }
            else
            {
                element.setMyParameter("level", myParent.level + 1);
            }

            saveItem(element);

            actualItemList.Where(x => x.parentId == element.id).ToList().ForEach(y => {

                calculateTreelevels4Element(y);

            });
        }

        public string entityType
        {
            get
            {
                return sampleObject.entityType;
            }
        }
        public void simpleObjectDump()
        {
            //простой листинг items в консоль
            //fn.dp("This is dump of IKeeper: "+ entityType);
            foreach (IKeepable t in actualItemList)
            {
                Logger.log(t.getMyStringRepresentation());
            }
        }

        public class CurrentRecord : ICurrentRecord
        {
            //хранит данные о текущей записи
            private ItemKeeper<T> x;
            T member;
            public CurrentRecord(ItemKeeper<T> _x)
            {
                x = _x;
            }

            public bool isFirst()
            {
                return x._currentIndex == 0;
            }
            public bool isLast()
            {
                return x._currentIndex == x.items0.count - 1;
            }

            public void moveNext()
            {
                if (!isLast()) { index = x._currentIndex + 1; }
            }
            public void movePrevious()
            {
                if (!isFirst()) { index = x._currentIndex - 1; }
            }

            public string id
            {
                get { return x._currentId; }
                set
                {
                    x._currentId = value;


                    //теперь надо найти в коллекции items элемент с таким id, и взять его индекс, чтобы присвоить св-ву _currentIndex

                    MemberSearchResult mr = x.items0.getMemberById(value);
                    if (mr != null)
                    {
                        //если элеменет с таким ID существует
                        member = (T)mr.member; //TODO очень спорный ход

                        x._currentIndex = mr.index;
                    }
                }
            }

            public int index
            {
                get { return x._currentIndex; }
                set
                {
                    if (x == null) return;
                    if (x.count == 0) return;
                    MemberSearchResult mr = x.items0.getMemberByIndex(value);
                    x._currentIndex = value;
                    x._currentId = mr.member.id;
                    member = (T)mr.member;
                    //TODO потенциальный источник ошибок, т.к. может быть передана величина , большая кол-ва записей
                }
            }
            public IKeepable getMember() //возвращает текущую запись 
            {
                return member;
            }

        }

        public class FilteredSortedItemList : IEnumerable, IFilteratonMethodGroup, ISortMethodGroup
        {
            // класс хранит коллекцию объектов <T> и выполняет ее сортировку и фльтрацию (с возможностью применить/сбросить фильтр)
            // этот класс не самостоятелен, он работает внутри ItemKeeper и не может существовать без его методов. 
            // TODO хотя можно через интерфейс попробовать соединить с чем-то внешним

            //делается для следующих вещей:
            // 1) чтобы нельзя было добавить элемент непосредственно в коллекцию items
            // 2) чтобы выполнять фильтрацию внутри одного объекта  (т.е. для внешних объектов это одна и та же коллекция Items) без дополнительной необходимости каждый раз читать все из базы, что облегчит быстрый поиск

            ItemKeeper<T> parent;

            //public Lib.FiltrationTypeEnum filtrationType { get; set; }

            //public bool isNowFiltered { get { return actualFilter != null; } }
            //public bool itsGlobalFiltration { get { return filtrationType == Lib.FiltrationTypeEnum.global; } }

            public MemberSearchResult getMemberById(string id)
            {
                bool condition = (localFiltrationIsOn);
                List<IKeepable> tmp = (condition ? ItemsFiltered : ItemsBase);

                MemberSearchResult mr = new MemberSearchResult();

                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i].id == id)
                    {
                        //member = x.items[i];
                        mr.index = i;
                        mr.member = tmp[i];
                        return mr;
                    }
                }
                return null;
            }
            public MemberSearchResult getMemberByIndex(int index)
            {
                bool condition = localFiltrationIsOn;
                List<IKeepable> tmp = (condition ? ItemsFiltered : ItemsBase);

                MemberSearchResult mr = new MemberSearchResult();
                try
                {
                    mr.index = index;
                    mr.member = tmp[index];
                    return mr;
                }
                catch
                {
                    return null;
                }


            }
            public List<IKeepable> actualItemList
            {
                get
                {
                    bool condition = (localFiltrationIsOn);
                    if (condition) return ItemsFiltered; else return ItemsBase;
                }
            }

            public Lib.Filter myActualGlobalFilter { get { if (actualGloblalFilter == null) { actualGloblalFilter = new Lib.Filter(); } return actualGloblalFilter; } }
            public Lib.Filter myActualLocalFilter { get { if (actualLocalFilter == null) { actualLocalFilter = new Lib.Filter(); } return actualLocalFilter; } }

            public Lib.Sorter myActualSorter { get { if (actualSorter == null) { actualSorter = new Lib.Sorter(); } return actualSorter; } }

            private Lib.Filter actualGloblalFilter;
            
            private Lib.Filter actualLocalFilter;

            private Lib.Sorter actualSorter;

            private List<IKeepable> ItemsBase = new List<IKeepable>();

            private List<IKeepable> ItemsFiltered = new List<IKeepable>();

            public int count
            {
                
                
                get
                {
                    return actualItemList.Count();
                    //if (localFiltrationIsOn) { return ItemsFiltered.Count; } else { return ItemsBase.Count; }
                }
            }
            public void clear()
            {
                //полностью очистить коллекцию
                //resetFilter();
                ItemsBase.Clear();
            }

            public void addExistingObject(T t)
            {
                //добавляет уже существующий объект
                // но это добавление только в коллекцию, а за добавление в базу этот объект не отвечает

                ItemsBase.Add(t);

                if (!parent.readingItemsFlag)
                {
                    if (localFiltrationIsOn) { applyActualLocalFilter(); }
                }

                // TODO + apply sort
            }

            public void removeItem(string id)
            {
                MemberSearchResult m = getMemberById(id);
                ItemsBase.Remove(m.member);
                //if (filtrationType == Lib.FiltrationTypeEnum.inHouse && isNowFiltered) { reApplyFilter(); }
            }

            public FilteredSortedItemList(ItemKeeper<T> _parent)
            {
                parent = _parent;
            }

            public IEnumerator GetEnumerator()
            {
                //если включена локальная фильтрация, то вернуть ItemsFiltered, иначе ItemsBase
                if (localFiltrationIsOn)
                {
                    return ItemsFiltered.GetEnumerator();
                }
                else
                {
                    return ItemsBase.GetEnumerator();
                }
            }

            #region filtration

            private bool localFiltrationIsOn
            {
                get
                {
                    if (actualLocalFilter == null) return false;
                    if (actualLocalFilter.filteringRuleList.Count ==0) return false;
                    return true;
                }
            }
            private bool globalFiltrationIsOn
            {
                get
                {
                    return (actualGloblalFilter != null);
                }
            }
            public void applyLocalFilter(Lib.Filter filter)
            {
                //сначала дополнить существующий фильтр, потом применить его к объектам
                actualLocalFilter = alterFilter(actualLocalFilter, filter);
                applyActualLocalFilter();
            }
            public void applyGlobalFilter(Lib.Filter filter)
            {
                //сначала дополнить существующий фильтр, потом применить его к объектам
                actualGloblalFilter = alterFilter(actualGloblalFilter, filter);
                applyActualGlobalFilter();
            }
            private void applyActualLocalFilter()
            {
                //существование этой функции отдельно обусловлено тем, что из разных мест вызывается повторное применение фильтров
                implementLocalFilter();
            }
            private void applyActualGlobalFilter()
            {
                //существование этой функции отдельно обусловлено тем, что из разных мест вызывается повторное применение фильтров
                implementGlobalFilter();
            }
            private void implementLocalFilter()
            {
                //непосредственное применение локального фильтра к объектам

                if (!localFiltrationIsOn) return;

                ItemsFiltered.Clear();
                IFiltrationMachine filtrationMachine = new StdFiltrationMachine(actualLocalFilter);
                ItemsFiltered = filtrationMachine.getResult(ItemsBase);
            }

            private void implementGlobalFilter()
            {
                //непосредственное применение фильтра к объектам
                parent.readItems();//пименить глобальный фильтр означает просто перечитать из источника данных

            }



            private Lib.Filter alterFilter(Lib.Filter filterBig,  Lib.Filter filterSmall)
            {
                //модифицирует первый фильтр вторым
                if (filterSmall == null) return filterBig;

                if (filterBig != null)
                {
                    if(filterSmall.filteringRuleList.Count>0)
                    {
                        foreach (Lib.Filter.FilteringRule r in filterSmall.filteringRuleList)
                        {
                            filterBig.addExistingFilteringRule(r);
                        }
                    }
                }
                else
                {
                    filterBig = filterSmall;
                }
                return filterBig;

            }
            public void resetSearshFilter(Lib.FiltrationTypeEnum filtrationType)
            {
                resetFilterParametric(filtrationType, Lib.Filter.FilteringRuleTypeEnum.SearchFilteringRule);
            }
            public void resetFilter(Lib.FiltrationTypeEnum filtrationType = Lib.FiltrationTypeEnum.Global)
            {
                resetFilterParametric(filtrationType);
            }
                public void resetFilterParametric(Lib.FiltrationTypeEnum filtrationType, Lib.Filter.FilteringRuleTypeEnum filteringRuleType = Lib.Filter.FilteringRuleTypeEnum.NotSpecified, Type setByObjectOfType = null)
            {
                //снимает действие фильтра по заданному типу и объекту, ли по  всем, если не задано
                
                Lib.Filter actualFilter;

                bool frTypeSpecified = filteringRuleType != Lib.Filter.FilteringRuleTypeEnum.NotSpecified;

                bool setByObjectSpecified = setByObjectOfType != null;


                if (filtrationType == Lib.FiltrationTypeEnum.Global)
                {
                    if (actualGloblalFilter==null) return; // потому что его и так нет, нечго там снимать
                    
                    if (frTypeSpecified || setByObjectSpecified)
                    {
                        actualGloblalFilter.resetMeByFilteringType_and_setByWhatObject(filteringRuleType, setByObjectOfType);
                    }
                    else
                    {
                        actualGloblalFilter = null;
                    }
                    applyActualGlobalFilter();

                }
                else 
                {
                    if (actualLocalFilter == null) return; // потому что его и так нет, нечго там снимать

                    if (frTypeSpecified || setByObjectSpecified)
                    {
                        actualLocalFilter.resetMeByFilteringType_and_setByWhatObject(filteringRuleType, setByObjectOfType);
                    }
                    else
                    {
                        actualLocalFilter = null;
                    }
                    applyActualLocalFilter();
                }
            }

            #endregion

            #region sort

            public void applyActualSorter()
            {
                applySorter(null);
            }

            public void alterActualSorter(Lib.Sorter sorter)
            {
                //да, тут именно sorter, а не sorting rule. т.к. может быть несколько правил внутри переданного sorter
                //это моифицирует текущий фильтр без перечитывания элементов
                if (actualSorter != null)
                {
                    //перебрать входящие правила сортировки

                    foreach (Lib.Sorter.SortingRule sr in sorter.sortingRuleList)
                    {
                        actualSorter.deleteRelativeSortingRules(sr);
                        actualSorter.addExistingSortingRule(sr);
                    }
                }
                else
                {
                    actualSorter = sorter;
                }
            }

            public void applySorter(Lib.Sorter sorter=null)
            {

                if (sorter != null) alterActualSorter(sorter);
                //тут надо взять актуальную коллекцию и пересортировать ее по полю
                //в данный момент вообще вся сортировка делается на клиенте

                if (localFiltrationIsOn)
                    ItemsFiltered = _applySorterToCollection(actualSorter, ItemsFiltered);
                else
                    ItemsBase= _applySorterToCollection(actualSorter, ItemsBase);
            }
            public List<IKeepable> _applySorterToCollection(Lib.Sorter sorter,  List<IKeepable> collection)
            {
                //тут надо взять актуальную коллекцию и пересортировать ее по полю

                //Итак, а вот тут надо применить к коллекции много правил сортировки, последовательно
                //linq в помощь, видимо
                if (sorter == null) return collection;
                if (collection == null) return null;

                List<IKeepable> _collection = collection;
                //перебрать парвила, начиная с самого последнего, и каждое правило применить к коллекуии
                bool amIProperty;
                sorter.sortingRuleList
                    .OrderByDescending(s => s.ruleOrder)
                    .ToList()                    
                    .ForEach(s=>
                    {
                        amIProperty = s.fieldInfoObject.amIProperty();
                        if (s.sortingDirection == Lib.AscDescSortEnum.Asc)
                        {
                            if (amIProperty)
                            {
                                _collection = _collection.OrderBy(m => m.GetType().GetProperty(s.fieldInfoObject.fieldClassName).GetValue(m)).ToList();
                            }
                            else
                            {
                                //Lib.IKeepableListSimpleDump(_collection, fn.chr10 + "***Before sort-ASC");
                                _collection = _collection.OrderBy(m => m.GetType().GetField(s.fieldInfoObject.fieldClassName).GetValue(m)).ToList();
                                //Lib.IKeepableListSimpleDump(_collection, fn.chr10 + "***After sort");
                            }
                        }
                        else
                        {
                            if (amIProperty)
                            {
                                _collection = _collection.OrderByDescending(m => m.GetType().GetProperty(s.fieldInfoObject.fieldClassName).GetValue(m)).ToList();
                            }
                            else
                            {
                                //Lib.IKeepableListSimpleDump(_collection, fn.chr10 + "***Before sort-DESC");
                                _collection = _collection.OrderByDescending(m => m.GetType().GetField(s.fieldInfoObject.fieldClassName).GetValue(m)).ToList();
                                //Lib.IKeepableListSimpleDump(_collection, fn.chr10 + "***After sort");
                            }
                        }
                    });
                return _collection;
            }

            public void resetSorter()
            {
                actualSorter = null;
                //TODO тут должно быть что то еще - пересортировать элементы напр
            }

            #endregion
        }

        public class MemberSearchResult
        {
            public IKeepable member;
            public int index;
        }

        public void openMyHistoryFrm(Lib.InterFormMessage ifMsg)
        {
            //08.04.2021
            //открывает форму редактирования истории объекта
            //Todo не открывать форму если по объекту не хранится история или она пустая

            HistoryManagerFrm frm = new HistoryManagerFrm();

            frm.startMsg = ifMsg;

            frm.ShowDialog();
        }

    }


}