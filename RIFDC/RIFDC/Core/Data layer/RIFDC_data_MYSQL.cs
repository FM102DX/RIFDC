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
using System.Diagnostics;
using System.Globalization;
using MySql.Data.Types;

namespace RIFDC
{
    public abstract class MySqlClusterPattern : IDataCluster
    {
        //это класс-паттерн для mysql кластеров, т.к. есть разные способы подключение - через oledb, через mysql.data  и др.

        public ConnectionData connectionData { get; set; }

        //virtual
        public virtual string connectionString
        {
            //здесь для каждого типа сервера бд (aceess, mysql и др. указывается свой способ получения connectionString) 
            get;
        }

        private Lib.Filter _actualFilter;

        public List<IUniversalRowDataContainer> makeGroupQuery(IKeeper keeper, Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField)
        {
            // выполняет групповые запросы для каждого элемента данной сущности по зависимым сущностям
            // генерит запрос типа, например, Select COUNT…  UNION select Count.... 
            // todo может, разбить на несколько? или сделать асинхронно

            // 1) взять targetRelation удебдиться, что тип запроса верный, т.е. что там 1--n или 1--1
            // 1.1. удедиться, чот это вообще наш relation

            //работает только по соседним сущностям 
            //todo подвесить под технологию retailChain


            Relations.Relation.RelationSide ourSide = targetRelation.getMyRelationSide(keeper.sampleObject);
            Relations.Relation.RelationSide otherSide = targetRelation.getAnotherRelationSide(keeper.sampleObject);

            if (ourSide == null)
            {
                return null; // Lib.ObjectOperationResult.sayNo("Relation doesnt fit object");
            }


            if (groupQueryType != Lib.GroupQueryTypeEnum.Count)
            {
                return null;
            }
            //нам надо запрос вида selec count 

            string cmdText = "";
            bool eol;
            int counter = 0;
            string qTypeStr = "";
            string union;
            if (groupQueryType == Lib.GroupQueryTypeEnum.Count) qTypeStr = "count";

            //так, нужна таблица из relation
            // ну логичнее всего взять ее из fieldInfo по принципу ДНК

            foreach (IKeepable x in keeper.items)
            {
                eol = (counter == keeper.count-1);
                union = (eol ? "" : " union ");

                // select count (поле),  from [таблица] where [внешнее поле] = x.id
                // select count id, spec.markid from spaeLines where spec.markid group by spec.markid

                // todo а не проще будет собрать 

                // ну в данном случае это запрос для count
                cmdText += string.Format("select {0}(id) as reznum, {1} as extid from {2} group by {3} having {4}='{5}' {6}",
                    qTypeStr,  //0
                        otherSide.fieldInfo.fieldDbName,  //1
                            otherSide.fieldInfo.parent.tableName,  //2
                               otherSide.fieldInfo.fieldDbName,  //3
                                otherSide.fieldInfo.fieldDbName,  //4
                                    x.getMyParameter(ourSide.fieldName),  //5
                                            union  //6
                                                );

               // cmdText += "select " + qTypeStr + " (id) as reznum, " + otherSide.fieldInfo.fieldDbName + " as extid,  from " + otherSide.fieldInfo.parent.tableName + " where " +  + " group by " + otherSide.fieldName + " " + (eol ? "" : " union ");

                counter++;
            }

            MySqlCommand cmd = new MySqlCommand(cmdText, activeConnection);

            MySqlDataReader reader = null;

            object tmp;

            //Logger.log("DB", "Executing query=" + cmdText);

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Logger.log("DB", "Executing query=" + e.Message);
            }


            if (reader == null)
            {
                return null;
            }

            //Logger.log("DB", "Result is: hasRows=" + reader.HasRows);

            List<IUniversalRowDataContainer> rez=new List<IUniversalRowDataContainer>();

            Lib.UniversalDataKeeper d;

            while (reader.Read())
            {
                //ок, вот запрос есть
                //в теперь надо же как то выполнить этот запрос, да?
                //ну логично было бы иметь некий dataRoom.execSelectQuery, который бы возвращал массив udc
                
                d = new Lib.UniversalDataKeeper(); //это строка

                try
                {
                    //какитам поля, в этом запросе
                    d.addNewElement("reznum", fn.toStringNullConvertion( reader["reznum"]));
                    d.addNewElement("extid", fn.toStringNullConvertion(reader["extid"]));
                    
                }
                catch (Exception e)
                {
                    tmp = null;
                    Logger.log("DB-ERROR", e.Message.ToString());
                }
                rez.Add(d);
            }

            reader.Close();

            return rez;
        }

        public MySqlConnection activeConnection;

        public StorageType storageType
        {
            get
            {
                return StorageType.MySqlDatabase;
            }
        }

        public string dbCommonTitle //virtual
        {
            get { return ""; }
        }

        public MySqlClusterPattern()
        {

        }

        public bool isNowConnected { get { return activeConnection.State == System.Data.ConnectionState.Open; } }

        public Lib.DbOperationResult connect()
        {
            Lib.DbOperationResult cr = new Lib.DbOperationResult();

            MySqlConnection cnn = new MySqlConnection();

            cnn.ConnectionString = connectionString;

            try
            {
                cnn.Open();
                activeConnection = cnn;
                return Lib.DbOperationResult.sayOk(cnn.ConnectionString);
            }
            catch (Exception exception)
            {
                return Lib.DbOperationResult.sayNo(exception.Message);
            }
        }

        public Lib.DbOperationResult reconnect()
        {
            return null;
        }

        public void disconnect()
        {
            if (activeConnection != null)
            {
                activeConnection.Close();
            }

        }

        public Lib.DbOperationResult deleteFiteredPackege(IKeepable sample, Lib.Filter filter)
        {
            //удалить из таблицы элементы, определенные фильтром

            string commStr;
            int rez = 0;
            string errStr = "";
            commStr = "delete from " + sample.tableName + " where " + getWhereConditionFromFilter(filter);
            try
            {
                MySqlCommand com = new MySqlCommand(commStr, activeConnection);
                Logger.log("DB", "EXECUTING QUERY: " + commStr);
                rez = com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.log("DB", "ERROR: " + errStr);
                errStr = e.Message;
            }

            if (rez > 0)
            {
                return Lib.DbOperationResult.sayOk("");
            }
            else
            {
                return Lib.DbOperationResult.sayNo(errStr);
            }



        }



        public Lib.DbOperationResult checkObjectTable(IKeepable t, bool drop=false)
        {
            MySqlCommand com;
            string commStr;
            if (drop)
            {
                //если он true, надо удалить таблицу
                //TODO ОПАСНО ЭТО!
                try
                {
                    commStr = $"drop table {t.tableName}";
                    Logger.log("DB", "EXECUTING QUERY: " + commStr);
                    com = new MySqlCommand(commStr, activeConnection);
                    int rez= com.ExecuteNonQuery();
                    Logger.log("DB", $"RESULT IS: {rez}");

                }
                    catch (Exception ex)
                    {
                        Logger.log("DB", "ERRROR: " + ex.Message);
                        //   return Lib.DbOperationResult.sayNo(ex.Message);
                    }
                }
            
            TableChecker tableChecker = new TableChecker(activeConnection, t);
            return tableChecker.checkTable();
        }

        public Lib.DbOperationResult deleteItem(IKeepable t)
        {

            if (t == null)
            {
                return Lib.DbOperationResult.sayNo("Target Object is null");
            }

            if (fn.toStringNullConvertion(t.id) == "")
            {
                return Lib.DbOperationResult.sayNo("Target Object Id is null");
            }

            //TODO стоп, а если там цепочка удалений?
            //логика удаления хранится в объекте, или в групповом объекте? ну если там несколько удалений
            //видимо, в групповом объекте. объект хранит только как удалить оттуда "себя лично"
            string commStr;
            int rez = 0;
            string errStr = "";
            commStr = "delete from " + t.tableName + " where id='" + t.id.ToString() + "'";
            try
            {
                MySqlCommand com = new MySqlCommand(commStr, activeConnection);
                Logger.log("DB", "EXECUTING QUERY: " + commStr);
                rez = com.ExecuteNonQuery();
                if (rez > 0)
                {
                    return Lib.DbOperationResult.sayOk();
                }
                else
                {
                    return Lib.DbOperationResult.sayNo();
                }
            }
            catch (Exception e)
            {
                Logger.log("DB", "ERROR: " + errStr);
                errStr = e.Message;
                return Lib.DbOperationResult.sayNo(errStr);
            }
        }

        public Lib.DbOperationResult saveObject(IKeepable t)
        {
            //этот метод сохраняет объект IKeeper в базе 
            //Если есть ID, то это обновление. Если нет ID - добавление

            string commStr;
            MySqlCommand com;
            int maxCount = 10;
            bool success = false;
            bool continueTrying = false;
            int counter = 0;
            int rez = -1;
            string errText = "";
            DateTime dateTimeOfCreation = Convert.ToDateTime(null);
            DateTime dateTimeOfUpdate = Convert.ToDateTime(null);
            Lib.DbOperationResult _r;

            if (fn.toStringNullConvertion(t.id) == "")
            {
                do
                {
                    counter++;
                    continueTrying = (counter < maxCount);
                    try
                    {
                        //просто генерируем ID до тех пор, пока оно не вставится
                        //используем то, что база проверяет уникальность поля
                        dateTimeOfCreation = DateTime.Now;

                        //t.setMyParameter("id", fn.generate4blockGUID());  // 2 мая 2021
                        
                        t.setMyParameter("id", t.generateMyId()); 

                        t.setMyParameter("createdDateTime", dateTimeOfCreation);

                        t.setMyParameter("lastModifiedDateTime", null);

                        t.saveMyPhoto();

                        commStr = generateInsertCommand(t);

                        Logger.log("DB", "Executing query: " + commStr);
                        com = new MySqlCommand(commStr, activeConnection);
                        com.ExecuteNonQuery();
                        success = true;

                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.log("DB", "ERROR: " + e.Message);
                    }
                }
                while (continueTrying);

                if (!success)
                {
                    //исчерпаны попытки вставки данных
                    _r = Lib.DbOperationResult.sayNo("Unable to insert new record after 10 efforts");
                    _r.insertedDateTime = dateTimeOfCreation;
                    return _r;
                }
                else
                {
                    //успешано
                    Lib.DbOperationResult x = Lib.DbOperationResult.sayOk("");
                    x.createdObjectId = t.id;
                    return x;
                }

            }
            else
            {
                dateTimeOfUpdate = DateTime.Now;

                t.setMyParameter("lastModifiedDateTime", dateTimeOfUpdate);

                //выполнить UPDATE
                commStr = generateUpdateCommand(t);

                if (commStr == "")
                {
                    return Lib.DbOperationResult.sayOk("Nothing to update");
                }

                Logger.log("DB", "EXECUTING QUERY: " + commStr);

                try
                {
                    com = new MySqlCommand(commStr, activeConnection);
                    rez = com.ExecuteNonQuery();
                    Logger.log("DB", "RESULT IS: " + rez.ToString());

                    _r = Lib.DbOperationResult.sayOk();
                    _r.updatedDateTime = dateTimeOfUpdate;
                    return _r;
                }
                catch (Exception e)
                {
                    errText = "RESULT IS: ERRROR: " + e.Message;
                    Logger.log("DB", errText);
                    return Lib.DbOperationResult.sayNo(errText);
                }
            }
        }

        public List<IUniversalRowDataContainer> readItems(IKeepable t, Lib.Filter filter)
        {
            //читает множество объектов Т из базы
            List<IUniversalRowDataContainer> rez = new List<IUniversalRowDataContainer>();

            string cmdText;

            Lib.UniversalDataKeeper d;

            cmdText = generateSelectCommand(t, filter);

            MySqlDateTime msdt;

            //теперь читаем
            MySqlCommand cmd = new MySqlCommand(cmdText, activeConnection);
            object tmp=null;

            Logger.log("DB", "Executing query=" + cmdText);

            //TODO обработать исключение ошибки селекта
            MySqlDataReader reader = cmd.ExecuteReader();

            Logger.log("DB", "Result is: hasRows=" + reader.HasRows);
            //Stopwatch stopwatch0 = new Stopwatch();
            //Stopwatch stopwatch1 = new Stopwatch();
            //int col = 0;

            DateTime dt;
            bool dateConvertRezult;
            string dateStr = "";
            while (reader.Read())
            {
                // перебираем по порядку поля, кот. надо присвоить
                // Logger.log("Start reading line", "");
                d = new Lib.UniversalDataKeeper(); //это строка
                
               // fn.dp($"reading id {reader["id"]}");

                t.fieldsInfo.fields.Where(x =>
                    (x.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid ||
                    x.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable))
                    .ToList()
                    .ForEach(f => {
                        try
                        {

                            if (f.fieldDbName == "lastModifiedDateTime")
                            {
                              //  fn.dp("0");
                            }


                            //fn.dp($"reading field {f.fieldDbName}");

                            int index = reader.GetOrdinal(f.fieldDbName);
                           // fn.dp("1");



                            if (f.fieldType == Lib.FieldTypeEnum.DateTime || f.fieldType == Lib.FieldTypeEnum.Date || f.fieldType == Lib.FieldTypeEnum.Time)
                            {
                                //fn.dp("2");
                                if (!reader.IsDBNull(index))
                                {
                                    msdt = reader.GetMySqlDateTime(f.fieldDbName);
                                  //  fn.dp("2.1");

                                    dateConvertRezult = DateTime.TryParseExact($"{msdt.Day}.{msdt.Month}.{msdt.Year} {msdt.Hour}:{msdt.Minute}:{msdt.Second}",
                                                                                   "dd.MM.yyyy hh:mm:ss", CultureInfo.InvariantCulture,
                                                                                    DateTimeStyles.None, out dt);
                                    //fn.dp("2.2");
                                    tmp = dt;
                                }
                                else
                                {
                                    tmp = null;
                                }
                            }
                            else
                            {
                               // fn.dp("3");
                                if (!reader.IsDBNull(index))
                                {
                                 //   fn.dp("4");
                                    tmp = reader[f.fieldDbName];
                                }
                                else
                                {
                                   // fn.dp("5");
                                    tmp = null;
                                }
                            }
                        }

                            /*
                             * * DBNull.Value.Equals(reader[fieldName])
                             * 
                                datConvertRezult= DateTime.TryParseExact(fn.toStringNullConvertion(reader.GetMySqlDateTime(f.fieldDbName)), 
                                                    "MM.dd.yyyy hh:mm:ss", CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None, out dt); 

                            */
                        catch (Exception e)
                            {
                                tmp = null;
                                Logger.log("DB-ERROR", e.Message.ToString());
                            }

                            d.addNewElement(f.fieldDbName, tmp);
                    });

                rez.Add(d);
            }
            reader.Close();
            return rez;
        }

        private string getWhereConditionFromFilter(Lib.Filter filter)
        {

            //это условие where для sql - запросов, сгенерированное из фильтра с синтаксисом mysql
            //тут опять надо разбирать этот filtering expression

            //ну то есть берешь fe и начинаешь его парсить как при проверке
            // Regex regex;
            // MatchCollection matches;
            // string rgx;

            if (filter == null) return "";

            string rez = filter.filteringExpression;
            //rgx = @"([^0-9]+[0-9]{1,2}[^0-9]+)|(^[0-9]{1,2}[^0-9]+)|([^0-9]+[0-9]{1,2}$)|(^[0-9]{1,2}$)";
            //regex = new Regex(rgx, RegexOptions.IgnoreCase);
            string oldVal;
            string newVal;

            rez = filter.filteringExpression;

            foreach (Lib.Filter.FilteringRule fr in filter.filteringRuleList)
            {
                oldVal = "B" + fr.ruleOrder.ToString() + "E";
                newVal = " " + fr.fieldInfoObject.parent.tableName+"."+getFilteringRuleWhereExpr(fr)+ " ";
                rez = rez.Replace(oldVal, newVal);

                //rgx = @"([^0-9]+" + s + "+[^0-9]+)|(" + s + "+[^0-9]+)|([^0-9]+" + s + "+$)|(^" + s + "+$)";
                //regex = new Regex(rgx, RegexOptions.IgnoreCase);

                //matches = regex.Matches(filteringExpression);
                /*
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        Console.WriteLine("REGEX_MATCH_" + match.Value);
                }
                */
                // rez = regex.Replace(rez, fr.whereExpression);
            }

           // Logger.log("DB", "Returned WhrerExpr =" + rez);
            return rez;
        }

        private string getFilteringRuleWhereExpr(Lib.Filter.FilteringRule tmp)
        {
            //выдает выражение фильтрации применительно к конкретной базе, в аднном случае MySql
            // дело в том, что применительно к конкретной базе выражения имеют разный синтакс, напр, field like 'la'


            string s = "";

            if (tmp != null)
            {
                bool itsBool = (tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Bool);
                bool itsTime = tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Time;
                bool itsDate = tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Date;
                bool itsDateTime = (itsTime || itsDate);
                bool itsNumeric = (tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Int || tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Double);
                bool itsString = (tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.String || tmp.fieldInfoObject.fieldType == Lib.FieldTypeEnum.Memo);

                switch (tmp.filtrationOperator)
                {
                    case Lib.RIFDC_DataCompareOperatorEnum.contains:
                        //тут только строки
                        s = tmp.fieldInfoObject.fieldDbName + " like " + "'*" + tmp.filtrationValue + "*'";
                        break;

                    case Lib.RIFDC_DataCompareOperatorEnum.equal:

                        if (itsString) { s = tmp.fieldInfoObject.fieldDbName + " = " + "'" + tmp.filtrationValue + "'"; }
                        if (itsDate) { s = tmp.fieldInfoObject.fieldDbName + " = " + "#" + replaceDateSeparator(tmp.filtrationValue) + "#"; }
                        if (itsNumeric || itsBool) { s = tmp.fieldInfoObject.fieldDbName + " = " + tmp.filtrationValue; }
                        break;
                    case Lib.RIFDC_DataCompareOperatorEnum.greater:
                    case Lib.RIFDC_DataCompareOperatorEnum.greaterEqual:
                    case Lib.RIFDC_DataCompareOperatorEnum.lower:
                    case Lib.RIFDC_DataCompareOperatorEnum.lowerEqual:
                        if (itsNumeric || itsBool) { s = tmp.fieldInfoObject.fieldDbName + " " + getCompareOperatorName(tmp.filtrationOperator) + " " + tmp.filtrationValue; }
                        if (itsDate) { s = tmp.fieldInfoObject.fieldDbName + " " + getCompareOperatorName(tmp.filtrationOperator) + " #" + replaceDateSeparator(tmp.filtrationValue) + "#"; }
                        if (itsTime) { s = "/класс под ACCESS пока время не обрабатывает/"; }

                        break;

                    case Lib.RIFDC_DataCompareOperatorEnum.notEuqal:
                        if (itsString) { s = tmp.fieldInfoObject.fieldDbName + " <> " + "'" + tmp.filtrationValue + "'"; }
                        if (itsDate) { s = tmp.fieldInfoObject.fieldDbName + " <> " + "#" + replaceDateSeparator(tmp.filtrationValue) + "#"; }
                        if (itsNumeric || itsBool) { s = tmp.fieldInfoObject.fieldDbName + " <> " + tmp.filtrationValue; }

                        break;
                }
            }

            return s;
        }




        private string replaceDateSeparator(string source)
        {
            source = source.Replace('.', '/');
            return source;
        }

        private string getCompareOperatorName(Lib.RIFDC_DataCompareOperatorEnum op)
        {
            //
            switch (op)
            {
                case Lib.RIFDC_DataCompareOperatorEnum.equal:
                    return "=";

                case Lib.RIFDC_DataCompareOperatorEnum.greater:
                    return ">";

                case Lib.RIFDC_DataCompareOperatorEnum.greaterEqual:
                    return ">=";

                case Lib.RIFDC_DataCompareOperatorEnum.lower:
                    return "<";
                case Lib.RIFDC_DataCompareOperatorEnum.lowerEqual:
                    return "<>"; // ACCESS не равно выглядить так

                default: return "";
            }
        }

        //TODO при ошибке подключения к БД нпродолжать дальше не надо


        private string generateInsertCommand(IKeepable t)
        {
            string cmdText = "insert into " + t.tableName + " (";
            string valStr = " values (";
            int i = 0;
            string s0;
            int howMenySolidFields = t.fieldsInfo.howMenySolidFields();

            foreach (Lib.FieldInfo f in t.fieldsInfo.fields)
            {
                //добавляем поля
                if (f.parameterSignificanceInfo.significanceType!= Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid) continue;

                /*
                bool needsComma = f.isStringValue | f.fieldType == Lib.FieldTypeEnum.Date | f.fieldType == Lib.FieldTypeEnum.Time | f.fieldType == Lib.FieldTypeEnum.DateTime;
                string b = (needsComma) ? "'" : ""; //если это строка или даты, то нужны кавычки
*/
                string comma = i == howMenySolidFields - 1 ? "" : ","; //запятая
                cmdText = cmdText + f.fieldDbName + comma;
                s0 = insertUpdateValueCorrection(t, f, t.getMyParameter(f.fieldClassName));
                //valStr = valStr + b + s0 + b + comma;
                valStr = valStr + s0 + comma;
                i++;
            }
            //		cmdText	"insert into ObjectHistory (createdByUserId,entityName,objectId,oldValue,newValue,fieldClassName,dateTimeOfChange,userId)  values ('user01','NULL','','NewCoffeePoint00101020507','NewCoffeePoint00101020507','coffeePointTitle','2021.04.06 17.46.05','user01')"	string

            //стоп, а как же сохранять то, что изменилось?

            cmdText = cmdText + ") " + valStr + ")";
            return cmdText;
        }

        private string generateUpdateCommand(IKeepable t)
        {
            string cmdText = "UPDATE " + t.tableName + " SET ";
            string s0;
            int i;
            object tmp = null;

            List<Lib.FieldInfo> workingFields = new List<Lib.FieldInfo>();

            //Logger.log("", "Entering generateUpdateCommand");

            //включать только те поля, которые не ID, и в которых изменились значения
            t.fieldsInfo.fields.Where(x => 
                                    (x.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid)&&
                                    (x.fieldDbName.ToLower() != "id")&&
                                    (t.isFieldDirty(x.fieldClassName)))
                                    .ToList()
                                    .ForEach(f=> { 
                                     workingFields.Add(f);
                                });


            t.saveMyPhoto();

            //Logger.log("Working fields composed", "");
            if (workingFields.Count == 0) return "";

            //конструирование запроса
            i = 1;

            foreach (Lib.FieldInfo f in workingFields)
            {
                            //Logger.log("Collecting fields", f.fieldClassName);
                //добавляем поля
                string b = (f.isStringValue || f.isDateTileLikeValue) ? "'" : "";
                
                string comma = i == workingFields.Count ? "" : ",";

                        //Logger.log("DB", "tmp=" + tmp);

                s0 = insertUpdateValueCorrection(t, f, t.getMyParameter(f.fieldClassName));

                        //Logger.log("DB", "s0=" + s0);
                cmdText = cmdText + f.fieldDbName + "=" + s0 + comma;
                i++;
            }

            cmdText = cmdText + " where id='" + t.id.ToString() + "'";
            //Logger.log("Rezulting cmdText ", cmdText);
            return cmdText;
        }
        private string insertUpdateValueCorrection(IKeepable t, Lib.FieldInfo f, object value)
        {
            //когда мы что-то пишем в базу , надо учесть nullability, если оно nullable & null, надо в базу записать именно null, но это надо сделать так, как это понимает база

            string s = "";
            bool nullable = (f.nullabilityInfo.allowNull);

            //определяем, является ли это null
            bool isNull = (nullable && f.nullabilityInfo.considerNull); //IsNull действует только на nullable 
            bool isGetter = ObjectParameters.isItOnlyGetter(t, f.fieldClassName);
            if (isNull && isGetter) isNull = false; // геттер не может быть null

            s = valueForDbWithQuotes(f, value);

            return s;
        }

        private string generateSelectCommand(IKeepable t, Lib.Filter filter)
        {
            // тут еще условия будут всякие
            string tmp="";
            string comma = "";
            string s;
            bool boe;
            string whereCondition = getWhereConditionFromFilter(filter);

            int howMenySolidFields = t.fieldsInfo.howMenySolidFields();
            int howMenyOuterDependableFields = t.fieldsInfo.howMenyOuterDependableFields();
            int counter;

            if (t.fieldsInfo.hasOuterJoinFields)
            {
                //если есть OuterJoinFields
                s = " Select ";

                // сначала solid fields и поля для селекта
                // но ведь там еще и другие поля

                /*
                 * select 
                    bomlines.id,
                    bomlines.content,
                    speclines.id,
                    speclines.content,
                    marks.name,
                    tituls.name

                    from bomlines
                    left join speclines
                    on bomlines.relativeSpecLineId=speclines.id

                    left join marks on
                    marks.id=speclines.relativeMarkId

                    left join tituls on
                    tituls.id=marks.titulId
                 * 
                 * 
                 * */


                counter = 0;
                List<Lib.FieldInfo> dbStorableFields = t.fieldsInfo.fields
                    .Where(x => 
                    x.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid ||
                    x.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable
                    )
                    .ToList();

                foreach (Lib.FieldInfo f in dbStorableFields)
                {
                    boe = (counter == dbStorableFields.Count-1);
                    comma = boe ? "" : ",";

                    if (f.parameterSignificanceInfo.significanceType== Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid) 
                            tmp = t.fieldsInfo.tableName + "." + f.fieldDbName + " as " + f.fieldDbName + comma + " ";

                    //хвостовая таблица + последнее поле
                    if (f.parameterSignificanceInfo.significanceType == Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.OuterDependable)
                        tmp = f.parameterSignificanceInfo.relationsChain.finalFieldFullName +" as " + f.fieldDbName + comma + " ";

                    s += tmp;
                    counter++;
                }

                //теперь условие from
                // добавляем outer join к условию from, его надо собрать из relationsChain
                tmp = "";
                s += " from  " + t.tableName + " ";
                t.fieldsInfo.relationsChainUniqueElements.ForEach(x => {
                    tmp += string.Format(" left join {0} on {1}.{2}={3}.{4} ", x.nextTableName, x.myTableName, x.myKeyFieldName, x.nextTableName,x.nextKeyFieldName);
                });
                s += tmp;

                //теперь условие where
                s = s + (whereCondition == "" ? "" : " where " + whereCondition);
                fn.dp(s);
            }
            else
            {
                //если это просто запрос
                s = "Select * from " + t.tableName + " " + (whereCondition == "" ? "" : " where " + whereCondition);
            }

            return s;
        }

        public class ConnectionData
        {
            //сведения о подключении - сервер, имя базы, пароли и др.

        }

        public static string fieldTypeNullValueForThisDB(Lib.FieldTypeEnum en)
        {
            //что такое null для разных типов данных применительно именно к этой конкретной БД, т.е. что писать в запросе, чтобы это было сохранено базой как NULL
            switch (en)
            {
                case Lib.FieldTypeEnum.Bool:
                    return "0"; //non-nullable

                case Lib.FieldTypeEnum.Time:
                case Lib.FieldTypeEnum.Date:
                case Lib.FieldTypeEnum.DateTime:
                    return "NULL";

                case Lib.FieldTypeEnum.Double:
                case Lib.FieldTypeEnum.Int:
                    return "NULL";

                case Lib.FieldTypeEnum.Memo:
                case Lib.FieldTypeEnum.String:
                    return "NULL";
            }
            return "";
        }

        public static string сonvertedValueForThisDB(Lib.FieldTypeEnum fieldType, object value)
        {
            //конвертация значений из IKeeper в тот формат, который применителен именно для этой БД, для использования в запросах
            //например, мы храним даты в 07.07.2007, а в Mysql они лежат как 2007.07.07
            string s = fn.toStringNullConvertion(value);
            DateTime date;
            string s0 = "";

            switch (fieldType)
            {
                case Lib.FieldTypeEnum.Bool:
                    return s; 

                case Lib.FieldTypeEnum.Time:
                case Lib.FieldTypeEnum.Date:
                case Lib.FieldTypeEnum.DateTime:
                    //здесь надо менять местами цифры в дате
                    bool itsDate = (fieldType == Lib.FieldTypeEnum.Date);
                    bool itsTime = (fieldType == Lib.FieldTypeEnum.Time);
                    bool itsDateTime = (fieldType == Lib.FieldTypeEnum.DateTime);
                    
                    string dtTemplate = "";

                    if (itsDate) { dtTemplate = "yyyy.MM.dd"; }
                    if (itsTime) { dtTemplate = "HH.mm.ss"; }
                    if (itsDateTime) { dtTemplate = "yyyy.MM.dd HH.mm.ss"; }
                    try
                    {
                        date = Convert.ToDateTime(s);
                        s0 = date.ToString(dtTemplate);
                    }
                    catch (Exception e)
                    {
                        Logger.log("DB-Date convertion exception", e.Message.ToString());

                        //если не удалось, вернуть NULL для этого типа данных
                        s0 = fieldTypeNullValueForThisDB(Lib.FieldTypeEnum.Date);
                    }
                    return s0;


                case Lib.FieldTypeEnum.Double:
                        //исправить в числах double запятую на точку
                    if (s.Contains(',')) { s = s.Replace(',', '.'); }
                    return s;

                case Lib.FieldTypeEnum.Int:
                    return s; 


                case Lib.FieldTypeEnum.Memo:
                case Lib.FieldTypeEnum.String:
                    return s;
            }
            return "";
        }

        public static string valueForDbWithQuotes(Lib.FieldInfo f, object value)
        {
            //если тип данных строковый, то надо строковые данные почемтить в одинарные кавычки

            string comma = isCommaRequiringValue(f) ? "'" : "";

            if (value == null)
            {
                return fieldTypeNullValueForThisDB(f.fieldType);
            }
            else
            {
                return comma + сonvertedValueForThisDB(f.fieldType, value) + comma;
            }
                

           
        }

        public  static bool isCommaRequiringValue(Lib.FieldInfo f)
        {
            if (f.isStringValue || f.isDateTileLikeValue) { return true; }
            return false;
        }

        public class TableChecker
        {
            //этот класс используется тогда, когда мы готовим таблицу ля работы и выполняем разные виды проверок ее работоспособности

            MySqlConnection activeConnection;

            string tableName;

            IKeepable sampleObject;

            DataTable myDataTable
            {
                get
                {
                    try
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter("select * from " + tableName, activeConnection);
                        DataSet ds = new DataSet();
                        DataTable dt = new DataTable();
                        da.FillSchema(ds, SchemaType.Source);
                        //берем эту таблицу, которую создали / нашли только что
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Columns.Count > 0)
                            {
                                dt = ds.Tables[0];
                                return dt;
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }
                    catch
                    {

                    }
                    return null;
                }
            }

            public TableChecker(MySqlConnection _activeConnection, IKeepable _sampleObject)
            {
                activeConnection = _activeConnection;
                sampleObject = _sampleObject;
                tableName = sampleObject.tableName;

            }

            public Lib.DbOperationResult checkTableExistence()
            {
                string createTableQuery;
                string err;
                if (dbHasTableName(tableName))
                {
                    //таблица есть значит ок
                    return Lib.DbOperationResult.getInstance(true, "", 0);
                }
                else
                {
                    //таблицы нет
                    try
                    {
                        //если нет, создаем таблицу
                        createTableQuery = "CREATE TABLE " + tableName + " (id  VARCHAR(50) NOT NULL UNIQUE)";
                        MySqlCommand com = new MySqlCommand(createTableQuery, activeConnection);
                        Logger.log("DB", "EXECUTING QUERY: " + createTableQuery);
                        //A table must have at least 1 column - эту шоибку выдал MySql
                        int x = com.ExecuteNonQuery();
                        Logger.log("DB", "RESULT IS : " + x.ToString());

                        //проверяем, создана ли таблица
                        if (!dbHasTableName(tableName))
                        {
                            //не удалось создать
                            err = "RIFDC: Ошибка БД - невозможно содать таблицу";
                            Logger.log("DB", err);
                            return Lib.DbOperationResult.getInstance(false, err);
                        }
                        else
                        {
                            //удалось создать, все ок
                            return Lib.DbOperationResult.getInstance(true, "");
                        }
                    }
                    catch (Exception e)
                    {
                        //не удалось создать - ошибка при попытке создать
                        return Lib.DbOperationResult.getInstance(false, e.Message);
                    }
                }
            }

            public Lib.DbOperationResult checkTableColumnExistence(string fieldDbName, Lib.FieldInfo f = null, string tag = "", bool attemptToCreate = true, bool likeTypeCompare = false)
            {

                //это проверка существования колонки в таблице
                //она по-новой запрашивает состав таблицы для каждой колонки, но это необходимо, т.к. колонки зависят одна от другой
                string alterTableQuery;
                bool flag = false;
                string nullTxt = "";
                string defaultTxt = "";

                bool fIsNull = (f == null);
                bool tagIsEmpty = (tag == "");

                DataTable dt = myDataTable;
                if (dt == null)
                {
                    return Lib.DbOperationResult.getInstance(false, "ERROR trying to get DataTable object on " + tableName);
                }

                Lib.DbOperationResult rez0 = tableHasColumn(dt, fieldDbName, likeTypeCompare);

                flag = rez0.success;

                if (!attemptToCreate) return rez0;

                if (!flag) //если такого поля там то его надо создать
                {
                    if (!fIsNull)
                    {
                        nullTxt = f.nullabilityInfo.allowNull ? "NULL" : "NOT NULL";
                        defaultTxt = fn.toStringNullConvertion(f.nullabilityInfo.defaultValue) == "" ? "" : "DEFAULT " + valueForDbWithQuotes(f, f.nullabilityInfo.defaultValue);
                    }

                    if (fIsNull && tagIsEmpty) { tag = " TEXT NULL"; } //ну то есть если нет филдинфо и тега, то это будет просто текстовое поле

                    alterTableQuery = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldDbName + " ";

                    if (tag != "")
                    {
                        alterTableQuery += tag;
                    }
                    else
                    {
                        switch (f.fieldType)
                        {
                            case Lib.FieldTypeEnum.String:
                                alterTableQuery += string.Format(" TEXT {0} ", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Int:
                                alterTableQuery += string.Format(" INT {0} {1}", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Double:
                                alterTableQuery += string.Format(" DOUBLE {0} {1}", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Memo:
                                alterTableQuery += string.Format(" LONGTEXT {0} {1}", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Bool:
                                alterTableQuery += string.Format(" TINYINT {0} {1}", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Date:
                                alterTableQuery += string.Format(" DATETIME {0} {1}", nullTxt, defaultTxt);
                                break;

                            case Lib.FieldTypeEnum.Time:
                                alterTableQuery += string.Format(" DATETIME {0} {1}", nullTxt, defaultTxt);
                                break;
                            case Lib.FieldTypeEnum.DateTime:
                                alterTableQuery += string.Format(" DATETIME {0} {1}", nullTxt, defaultTxt);
                                break;

                        }
                    } // else

                    //когда сформировали, выполняем запрос
                    try
                    {
                        MySqlCommand com = new MySqlCommand(alterTableQuery, activeConnection);
                        Logger.log("DB", "EXECUTING QUERY: " + alterTableQuery);
                        com.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        return Lib.DbOperationResult.getInstance(false, e.Message);
                    }

                    //проверяем еще раз, точно ли создана колонка, и этот результат возвращаем

                    return checkTableColumnExistence(fieldDbName, f, tag, attemptToCreate);

                } // if (!flag)
                return Lib.DbOperationResult.getInstance(true, "");
            }

            public Lib.DbOperationResult checkTable()
            {
                Lib.DbOperationResult rte;
                //1) проверяет существование таблицы переданного класса T и нужных полей в ней

                #region checking table

                //проверяем, есть ли там tableName  в базе, если нет - создать
                rte = checkTableExistence();
                if (!rte.success) return rte;

                #endregion

                //2) таблица есть, теперь проверяем наличие столбцов в соотв. с полями объекта Т. По именам (пока) достаточно.
                //далее проверка полей //проверяем, есть ли такое поле в БД, если нет - создаем
                //TODO - сделать еще и проверку типов данных в колонках, т.к. в объекте мог поменяться тип
                #region checking fields

                rte = checkTableColumnExistence("id", null, " VARCHAR(30) PRIMARY KEY ");  // " VARCHAR(20) NOT NULL UNIQUE ");
                if (!rte.success) { return rte; }

                //последовательно проверяем поля, и если поля нет, то добавляем

                foreach (Lib.FieldInfo f in sampleObject.fieldsInfo.fields)
                {
                    if (f.parameterSignificanceInfo.significanceType != Lib.ParameterSignificanceInfo.ParameterSignificanceTypeEnum.Solid) continue;
                    if (f.fieldDbName.ToLower() == "id") continue;
                    rte = checkTableColumnExistence(f.fieldDbName, f, "");
                    if (!rte.success) { return rte; }
                }

                #endregion

                return Lib.DbOperationResult.getInstance(true, "Common table check success");
            }



            Lib.DbOperationResult tableHasColumn(DataTable dt, string colName, bool likeTypeCompare = false)
            {
                bool equal;
                string a, b;

                foreach (System.Data.DataColumn c in dt.Columns)
                {
                    a = c.ColumnName.ToLower();
                    b = colName.ToLower();
                    equal = likeTypeCompare ? a.Contains(b) : a == b;
                    if (equal)
                    {
                        return Lib.DbOperationResult.getInstance(true, "", 0, "", a);
                    }
                }
                return Lib.DbOperationResult.getInstance(false, "");
            }

            private bool dbHasTableName(string tableName)
            {
                List<string> dbTablesNames = getTablesNames();
                var selectedItems = from t in dbTablesNames // определяем каждый объект 
                                    where t.ToLower() == tableName.ToLower() //фильтрация по критерию
                                    orderby t  // упорядочиваем по возрастанию
                                    select t; // выбираем объект
                return selectedItems.Count() > 0;
            }

            private List<string> getTablesNames()
            {
                //получаем список таблиц БД

                List<string> rez = new List<string>();
                if (activeConnection != null)
                {
                    DataTable dbTables = activeConnection.GetSchema("Tables"); //список всех таблиц

                    foreach (DataRow row in dbTables.Rows)
                    {
                        rez.Add(row["TABLE_NAME"].ToString());
                    }
                }
                return rez;
            }
        }
    }
    public class MySqlCluster_MySqlConnectorNET : MySqlClusterPattern, IDataCluster
    {
        public new ConnectionData connectionData { get; set; }
        public override string connectionString
        {
            //здесь для каждого типа сервера бд (aceess, mysql и др. указывается свой способ получения connectionString) 
            get
            {
                string server = fn.sfn(connectionData.server, "server=", ";");
                string port = fn.sfn(connectionData.port, "port=", ";");
                string dbName = fn.sfn(connectionData.dbName, "database=", ";");
                string dbUser = fn.sfn(connectionData.dbUser, "user=", ";");
                string dbPassword = fn.sfn(connectionData.dbPassword, "password=", ";");
                string persistSecurityInfo = fn.sfn(connectionData.persistSecurityInfo, "persist Security Info=", ";");
                string pooling = fn.sfn(connectionData.pooling, "pooling=", ";");
                string useCompression = fn.sfn(connectionData.useCompression, "use Compression=", ";");
                string charSet = "CHARSET = utf8;";
                return server + port + dbName + dbUser + dbPassword + persistSecurityInfo + pooling + useCompression+ charSet;
            }
        }

        public new string dbCommonTitle
        {
            get { return fn.sfn(connectionData.server, "server=", ";") + fn.sfn(connectionData.dbName, "dbName=", ";"); }
        }

        public MySqlCluster_MySqlConnectorNET()
        {
            //TODO громоздко, и надо чтобы, может быть, разрешить только 1 экземпляр
            //ну пока пусть так
            //_filePath = __filePath;
            connectionData = new ConnectionData();
        }


        public new MySqlConnection activeConnection
        {
            get { return activeConnection; }
        }

        public new class ConnectionData
        {
            //сведения о подключении - сервер, имя базы, пароли и др.

            public string server = "";
            public string port = "";
            public string dbName = "";
            public string dbUser = "";
            public string dbPassword = "";
            public string persistSecurityInfo = "";
            public string pooling = "";
            public string useCompression = "";

        }
    }

}
