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
using RIFDC;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using RIFDC.RIFDC.Service;

namespace RIFDC
{
    public static class Relations
    {
        public static List<Relation> items = new List<Relation>();
        public static Relation createRelation()
        {
            Relation r = new Relation();
            items.Add(r);
            return r;
        }
        public class Relation
        {

            public RelationSide _A;
            public RelationSide _B;

            public RelationSide A { get { return _A; } set { value.relationSideQualifier = "A"; _A = value; } }
            public RelationSide B { get { return _B; } set { value.relationSideQualifier = "B"; _B = value; } }

            public string comment;

            public RelationSide getRealtionSideByClassName(string className)
            {
                if (A.targetClassName == className) return A;
                if (B.targetClassName == className) return B;
                return null;
            }

            public class RelationSide
            {
                public string relationSideQualifier { get; set; } = "";

                Lib.FieldInfo _fieldInfo;

                public Type targetClass;

                public string entityName
                {
                    get
                    {
                        return fieldInfo.parent.entityName;
                    }
                }

                public string targetClassName { get { return Fn.GetEntityTypeFromFullTypeNameString(targetClass.Name); } }

                public string fieldName; //имя поля в классе, которым он участвует в этом relation

                public RelationSideQuantifier quantifier;
                public RelationSide()
                {

                }

                public string keyTableFieldPhrase
                {
                    get
                    {
                        return tableName + "." + fieldInfo.fieldDbName;
                    }
                }

                public string tableName
                {
                    get
                    {
                        string s = "";
                        try
                        {
                            s = myIKeeper.sampleObject.tableName;
                        }
                        catch
                        {
                            s = "";
                        }
                        return s;
                    }
                }

                public Lib.FieldInfo fieldInfo
                {
                    get
                    {
                        //чтобы лишний раз не создавать это экземпляр
                        if (_fieldInfo != null) return _fieldInfo;

                        //надо создать объект экземпляра targetClass
                        object X = Activator.CreateInstance(targetClass);
                        IRelatible Z = (IRelatible)X;
                        _fieldInfo = Z.getFieldInfoByFieldClassName(fieldName);
                        return _fieldInfo;


                        /*    //получаем конструктор
                            System.Reflection.ConstructorInfo ci = targetClass.GetConstructor(new Type[] { });
                            //вызываем конструтор
                            object Obj = ci.Invoke(new object[] { });
                            */
                    }

                }

                public static RelationSide createRelationSide(Type targetClass, string fieldName, RelationSideQuantifier quantifier)
                {
                    RelationSide rs = new RelationSide();
                    rs.targetClass = targetClass;
                    rs.fieldName = fieldName;
                    rs.quantifier = quantifier;
                    //тут еще должно быть присвоение филдинфо через создание sampleObject

                    return rs;
                }

                public bool isObligatory
                {
                    get
                    {
                        if (quantifier == RelationSideQuantifier.rel_1 || quantifier == RelationSideQuantifier.rel_1_to_n)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                public IKeeper myIKeeper
                {
                    get
                    {
                        return RIFDC_App.iKeeperSampleHolder.getIKeeperByEntityType(targetClassName);
                    }
                }

                public static bool Equals(RelationSide A, RelationSide B)
                {
                    bool AIsNull = (A == null);
                    bool BIsNull = (B == null);

                    if (AIsNull && BIsNull) return true;
                    if (AIsNull && !BIsNull) return false;
                    if (!AIsNull && BIsNull) return false;

                    return (A.targetClassName == B.targetClassName) && (A.fieldName == B.fieldName);
                }

/*
                public static bool operator ==(RelationSide A, RelationSide B)
                {
                    return (A.targetClassName == B.targetClassName) && (A.fieldName == B.fieldName);
                }

                public static bool operator !=(RelationSide left, RelationSide right)
                {
                    return !(left == right);
                }
                */
            }

            public bool isMyRelation_where_im_obligatory(IKeepable sampleObject)
            {
                //определяет, является ли sampleObject той стороной этого relation, где он обязателен

                RelationSide rs = getRealtionSideByClassName(sampleObject.entityType);

                if (rs == null) return false;

                if (rs.isObligatory) return true; else return false;
            }

            public bool isValid()
            {
                bool a = A.quantifier == RelationSideQuantifier.rel_1 && B.quantifier == RelationSideQuantifier.rel_0_to_1;
                bool b = A.quantifier == RelationSideQuantifier.rel_0_to_1 && B.quantifier == RelationSideQuantifier.rel_1;

                bool c = A.quantifier == RelationSideQuantifier.rel_1_to_n && B.quantifier == RelationSideQuantifier.rel_0_to_1;
                bool d = A.quantifier == RelationSideQuantifier.rel_0_to_1 && B.quantifier == RelationSideQuantifier.rel_1_to_n;

                bool e = A.quantifier == RelationSideQuantifier.rel_0_to_n && B.quantifier == RelationSideQuantifier.rel_1_to_n;
                bool f = A.quantifier == RelationSideQuantifier.rel_1_to_n && B.quantifier == RelationSideQuantifier.rel_0_to_n;

                bool rez = (a || b || c || d || e || f);

                return rez;
            }
            public string asLeftJoinQueryPart(string mySideEntityName)
            {
                //возвращает предикат left join для этого relation для использования в селект-запросах

                RelationSide mrs = getMyRelationSide(mySideEntityName);

                RelationSide ors = getAnotherRelationSide(mySideEntityName);

                string s = string.Format("left join {0} on {1}.{2} = {3}.{4}", ors.tableName, mrs.tableName, mrs.fieldName, ors.tableName, ors.fieldName); // + тут должна быть другая таблица, значит, надо передавать myside 

                return s;
            }

            public RelationSide getAnotherRelationSide(string mySideEntityName)
            {
                //определяет сторону, которую занимает переданный объект, и возвращает другую

                RelationSide rls = getRealtionSideByClassName(mySideEntityName);

                if (rls == null) return null;

                if (rls.relationSideQualifier == "A") return B; else return A;
            }
            public RelationSide getAnotherRelationSide(IKeepable sampleObject)
            {
                //определяет сторону, которую занимает переданный объект, и возвращает другую

                RelationSide rls = getRealtionSideByClassName(sampleObject.entityType);

                if (rls == null) return null;

                if (rls.relationSideQualifier == "A") return B; else return A;
            }
            public RelationSide getMyRelationSide(IKeepable sampleObject)
            {
                return getRealtionSideByClassName(sampleObject.entityType);
            }
            public RelationSide getMyRelationSide(string mySideEntityName)
            {
                return getRealtionSideByClassName(mySideEntityName);
            }


            public static bool operator ==(Relation left, Relation right)
            {
                bool x = (left.A == right.A) && (left.B == right.B);
                bool y = (left.A == right.B) && (left.B == right.A);

                return x || y;
            }

            public static bool operator !=(Relation left, Relation right)
            {
                return !(left == right);
            }

        }
        public enum RelationSideQuantifier
        {
            rel_0_to_1 = 1,
            rel_1 = 2,
            rel_0_to_n = 3,
            rel_1_to_n = 4
        }
        public interface IRelatible
        {
            Lib.FieldInfo getFieldInfoByFieldClassName(string fieldClassName);
        }

        public class RelationsChain
        {
            public List<RelationsChainElement> items = new List<RelationsChainElement>();

            public string finalField;
            public void addElement(RelationsChainElement el) { items.Add(el); }
            public void addElement(Relation r, string myEntityName) { addElement(new RelationsChainElement(r, myEntityName)); }
            public void addElement(Relation r, RelationsChainElement prevElement)
            {
                //инициализация последующего элемента из предыдущего

                //AddElement(new RelationsChainElement(r, myEntityName)); 
            }

            public string tailTableName
            {
                get 
                {
                    //взять хвостовую таблицу
                    if (items.Count == 0) return "";

                    //это элемент, который содержит хвостовубю таблицу
                    RelationsChainElement el = items[items.Count - 1];
                    return el.nextTableName;
                }
            }
            public string finalFieldFullName
            {
                get
                {
                    return tailTableName+"."+ finalField;
                }
            }



            public class RelationsChainElement
            {
                public string myEntityName; //сущность, которая головой к запрашивающей сущности - она передается в контрукторе

                public string myKeyFieldName
                {
                    get
                    {
                        Relation.RelationSide mrs = r.getMyRelationSide(myEntityName);
                        if (mrs == null) return "";
                        return mrs.fieldName;
                    }
                }
                public string nextKeyFieldName
                {
                    get
                    {
                        Relation.RelationSide ors = r.getAnotherRelationSide(myEntityName);
                        if (ors == null) return "";
                        return ors.fieldName;
                    }
                }
                public string myTableName
                {
                    get
                    {
                        Relation.RelationSide mrs = r.getMyRelationSide(myEntityName);
                        if (mrs == null) return "";
                        return mrs.tableName;
                    }
                }

                public string nextTableName
                {
                    get
                    {
                        Relation.RelationSide ors = r.getAnotherRelationSide(myEntityName);
                        if (ors == null) return "";
                        return ors.tableName;
                    }
                }
                public string nextEntityName //хвостовая сущность
                {
                    get
                    {
                        Relation.RelationSide ors = r.getAnotherRelationSide(myEntityName);
                        if (ors == null) return "";
                        return ors.entityName;
                    }
                }

                public Relation r;
                public RelationsChainElement(Relation _r, string _myEntityName)
                {
                    myEntityName = _myEntityName;
                    r = _r;
                }
                public RelationsChainElement(Relation _r, RelationsChainElement prevElement)
                {
                    myEntityName = prevElement.nextEntityName;
                    r = _r;
                }

                public bool Equals (RelationsChainElement left, RelationsChainElement right)
                {
                    bool rez = left.myEntityName == right.myEntityName &&
                       left.nextEntityName == right.nextEntityName &&
                       left.myKeyFieldName == right.myKeyFieldName &&
                       left.nextKeyFieldName == right.nextKeyFieldName;
                    /*
                    Fn.Dp("");
                    Fn.Dp("Comparing entities:");
                    Fn.Dp(left.ToString());
                    Fn.Dp(right.ToString());
                    Fn.Dp("Rezult is: " + ( rez ? "EQUAL" : "NOT EQUAL"));
                    */
                   

                    return rez;
                }

                public static bool operator == (RelationsChainElement left, RelationsChainElement right)
                {
                    //считаем, что RelationsChainElement равны, если равны relations, которые там
                    return left.r == right.r;
                }

                public static bool operator !=(RelationsChainElement left, RelationsChainElement right)
                {
                    return !(left == right);
                }

                public override string ToString()
                {
                    return string.Format("myEntityName={0} nextEntityName={1}, myKeyFieldName={2}, nextKeyFieldName={3} ", myEntityName, nextEntityName, myKeyFieldName, nextKeyFieldName);
                }



            }



        }
    }
}
