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
    public interface IFiltrationMachine
    {
        List<IKeepable> getResult(List<IKeepable> source);
    }

    public class StdFiltrationMachine : FiltrationMachinePattern , IFiltrationMachine
    {
      public StdFiltrationMachine(Lib.Filter _filter) : base (_filter)
        {

        }
    }

    public abstract class FiltrationMachinePattern
    {
        //класс, который фильтрует объекты
        //идея фильтрации в том, чтобы применить фильтр к каждому объекту в исходной коллекции и вернуть результат
        //в самом просто мварианте оно просто включает в выход те элементы, что выдают true при вычислении filteringExpression

        public FiltrationMachinePattern(Lib.Filter _filter)
        {
            filter = _filter;
            //ComparerRule comparer = new ComparerRule();
            foreach (Lib.Filter.FilteringRule fr in filter.filteringRuleList)
            {
                ComparerRule c = new ComparerRule();
                c.filteringRule = fr;
                comparerRuleList.Add(c);
            }
        }
        
        Lib.Filter filter;

        List<ComparerRule> comparerRuleList = new List<ComparerRule>();

        public List<IKeepable> getResult(List<IKeepable> source)
    {
        return source.Where(x => fit(x)).ToList();

        /*
         * 
        List<IKeepable> result = new List<IKeepable>();
        foreach (IKeepable t in source)
        {
            if (fit(t)) { result.Add(t); }
        }
        */
    }

        private bool fit(IKeepable t)
        {

            //итак, фильтрация

            // сначала вычислить все правила 1 уровня для этого объекта
            foreach (ComparerRule c in comparerRuleList)
            {
                c.val1 = t.getMyParameter(c.filteringRule.fieldClassName);
                c.val2 = c.filteringRule.filtrationValue;
                //теперь надо это вычислить
                c.filteringRuleValue = evauateAtomarExpression_level0(c.val1, c.val2, c.filteringRule.filtrationOperator, c.filteringRule.fieldInfoObject.fieldType);
            }

            //ок, вот я вычислил простые выражения. Теперь надо вычислить сложно, подставляя их значения в простое.
            //сначала надо заменить в filteringExpression все цифры на соотв. значения
            string s = filter.filteringExpression;
            Regex regex;
            bool rez;
            MatchEvaluator eval = new MatchEvaluator(getFilteringRuleValueByOrder_matchEval);
            foreach (ComparerRule cr in comparerRuleList)
            {
                regex = new Regex(@"B" + cr.filteringRule.ruleOrder + "E");
                s = regex.Replace(s, eval);
                //fn.dp("Fit, repalcements, s="+s);
            }

            //теперь надо вычислить полученное выражение (true and false and true...)

            rez = evaluateAtomarExpression_level3(s);

            return rez;
        }

        private bool evaluateAtomarExpression_level3(string source)
        {
            //вычисляет логиеские выражения вида ((x)[op](x) ...[op](x)))...), где x= [op] [not] 2 [op] [not] ... n
            string rez = "";

            source = source.ToLower();

            bool hasMatches = false;

            bool atomarL2Value;
            //bool atomarCorrect;

            string rgx = @" *\({1}[^\(\)]*\){1} *";

            Regex regex;

            int iterationNo = 1;

            do
            {
                bool exprContainsBrackets = (source.Contains("(") && source.Contains(")"));

                //regex = new Regex("[^a-zA-Zа-яА-Я()+-_:;!?@#.*]", RegexOptions.None);

                if (!exprContainsBrackets)
                {
                    //признак того, что он позаменял все скобки и там осталось простое выражение 2 уровня
                    //просто вычилсяем его и возвращаем значение

                    atomarL2Value = evaluateAtomarExpression_level2(source);
                    return atomarL2Value;
                }
                else
                {
                    MatchEvaluator eval = new MatchEvaluator(evaluateAtomarExpression_level2_matchEval);
                    regex = new Regex(rgx, RegexOptions.IgnoreCase);
                    rez = regex.Replace(source, eval);
                    rez = rez.ToLower();
                    hasMatches = (rez != source);
                    source = rez;
                    iterationNo += 1;
                }
            }
            while (hasMatches);

            source = source.TrimEnd(' ');
            source = source.TrimStart(' ');

            bool rez0 = (source == "( expr )" || source == "expr");

            return rez0;
        }

        private string evaluateAtomarExpression_level2_matchEval(Match m)
        {
            //функция на базе evaluateAtomarExpression_level2, которая используется регулярным выражением
            return evaluateAtomarExpression_level2(m.Value).ToString();
        }

        private bool evaluateAtomarExpression_level2(string source)
        {
            //вычисляет выражения вида 1 [op] [not] 2 [op] [not] ... n например true and false and not true and false
            string rez = "";
            bool hasMatches = false;
            MatchCollection mc;
            Regex regex;
            //сначала проверяем на простое выражение, т.е. выражение вида, например, 1 или not 1
            regex = new Regex(@"^ *\({0,1} *(not)? *((true)|(false)){1} *\){0,1} *$", RegexOptions.IgnoreCase);
            mc = regex.Matches(source);

            if (mc.Count > 0)
            {
                //если это простое выражение, просто возвращаем рез-т его вычисления
                return evaluateAtomarExpression_level1(mc[0].Value);
            }
            else
            {
                //если это выражение не простое, т.е. там 2 и более переменных
                MatchEvaluator eval = new MatchEvaluator(evaluateAtomarExpression_level1_matchEval);
                bool isItOrExpr = source.Contains("or");
                string digits = " *(not)? *((true)|(false)){1} *";
                string op = isItOrExpr ? "(or){1}" : "(and){1}";
                string rgx = @"" + digits + op + digits;
                do
                {
                    regex = new Regex(rgx, RegexOptions.IgnoreCase);
                    rez = regex.Replace(source, eval);
                    hasMatches = (rez != source);
                    rez = rez.ToLower();
                    source = rez;
                }
                while (hasMatches);


                source = source.Replace("(", "");
                source = source.Replace(")", "");

                source = source.TrimEnd(' ');
                source = source.TrimStart(' ');

                bool rez1 = Convert.ToBoolean(source);
                return rez1;
            }
        }

        string evaluateAtomarExpression_level1_matchEval(Match m)
        {
            //функция на базе evaluateAtomarExpression_level2, которая используется регулярным выражением
            return evaluateAtomarExpression_level1(m.Value).ToString();
        }

        private bool evaluateAtomarExpression_level1(string source)
        {
            //21.01.2021 это я б-м потестил, оно пашет

            //вычисляет выражения вида [not] [true/false] [operator] [not] [true/false] или вида [not] [true/false]
            //может приходить со скобками, может - без
            /*
             * вариантов тут всего 5
             * 
not val1    op not val2 кол-во строк

    val1                       1
not val1                       2

    val1 op       val2         3

    val1 op  not  val2         4
not val1    op    val2         4
not val1    op not val2        5

            */
            Regex regex;
            MatchCollection mc;
            string s1 = "(not)+";
            string s2 = "((true)|(false))+";
            string s3 = "(and|or)+";

            bool val1 = false;
            bool val2 = false;
            string op = "";
            string tmp;
            string tmp2;
            int i;
            regex = new Regex(@"(" + s1 + "|" + s2 + "|" + s3 + "|" + s1 + "|" + s2 + ")", RegexOptions.IgnoreCase);
            mc = regex.Matches(source);

            if (mc.Count <= 0) return false;

            switch (mc.Count)
            {
                case 1:
                    tmp = mc[0].Value;

                    if (tmp != "true" && tmp != "false") return false;
                    if (tmp == "true") return true;
                    if (tmp == "false") return false;
                    break;

                case 2:
                    tmp = mc[1].Value;
                    //это значит, перед val1 есть not
                    if (tmp != "true" && tmp != "false") return false;
                    if (tmp == "true") return false;
                    if (tmp == "false") return true;
                    break;

                case 3:
                    tmp = mc[0].Value;
                    op = mc[1].Value;
                    tmp2 = mc[2].Value;

                    if (tmp != "true" && tmp != "false") return false;
                    if (tmp2 != "true" && tmp2 != "false") return false;
                    if (op != "and" && op != "or") return false;

                    val1 = (tmp == "true");
                    val2 = (tmp2 == "true");

                    if (op == "and") { return (val1 && val2); }
                    if (op == "or") { return (val1 || val2); }

                    break;

                case 4:
                    bool firstNegative = false;

                    if (mc[0].Value == "not")
                    {
                        firstNegative = true;
                        tmp = mc[1].Value;
                        op = mc[2].Value;
                        tmp2 = mc[3].Value;
                    }
                    else
                    {
                        tmp = mc[0].Value;
                        op = mc[1].Value;
                        tmp2 = mc[3].Value;
                    }


                    if (tmp != "true" && tmp != "false") return false;
                    if (tmp2 != "true" && tmp2 != "false") return false;
                    if (op != "and" && op != "or") return false;

                    val1 = (tmp == "true");
                    val2 = (tmp2 == "true");

                    if (firstNegative) { val1 = !val1; } else { val2 = !val2; }

                    if (op == "and") { return (val1 && val2); }
                    if (op == "or") { return (val1 || val2); }

                    break;

                case 5:
                    //оба отрицания
                    tmp = mc[1].Value;
                    op = mc[2].Value;
                    tmp2 = mc[4].Value;

                    if (tmp != "true" && tmp != "false") return false;
                    if (tmp2 != "true" && tmp2 != "false") return false;
                    if (op != "and" && op != "or") return false;

                    val1 = !(tmp == "true"); //учитываем отрицания ! перед выражением
                    val2 = !(tmp2 == "true");

                    if (op == "and") { return (val1 && val2); }
                    if (op == "or") { return (val1 || val2); }

                    break;
            }

            return false; //если уж оно до сюда доехало
        }
        

        
    

        private string getFilteringRuleValueByOrder_matchEval(Match m)
        {
            char[] charsToTrim = { 'B', 'E' };
            int no = Convert.ToInt32(m.Value.Trim(charsToTrim));
            //возвращает значение true/false по номеру правила
            ComparerRule tmp = comparerRuleList.Where(obj => obj.filteringRule.ruleOrder == no).First();
            if (tmp != null) { return " " + tmp.filteringRuleValue.ToString() + " "; } else { return " false "; }

        }

        private bool evauateAtomarExpression_level0(object val1, object val2, Lib.RIFDC_DataCompareOperatorEnum op, Lib.FieldTypeEnum dataType)
        {
            //вычислить атомарное выражение [класснейм вэлью] [оператор] [значение]
            switch (dataType)
            {
                case Lib.FieldTypeEnum.Memo:
                case Lib.FieldTypeEnum.String:
                    string s1 = Convert.ToString(val1).ToLower();
                    string s2 = Convert.ToString(val2).ToLower();

                    switch (op)
                    {
                        case Lib.RIFDC_DataCompareOperatorEnum.equal:
                            {
                                //строки равны
                                if (s1 == s2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.notEuqal:
                            {
                                //строки не равны
                                if (s1 != s2) { return true; } else { return false; }
                            }

                        case Lib.RIFDC_DataCompareOperatorEnum.contains:
                            {
                                //одна строка содержит другугю
                                if (s1.Contains(s2)) { return true; } else { return false; }
                            }
                        default: return false;
                    }

                case Lib.FieldTypeEnum.Double:
                case Lib.FieldTypeEnum.Int:

                    double d1 = Convert.ToDouble(val1);
                    double d2 = Convert.ToDouble(val1);

                    switch (op)
                    {
                        case Lib.RIFDC_DataCompareOperatorEnum.equal:
                            {
                                if (d1 == d2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.notEuqal:
                            {
                                if (d1 != d2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.greater:
                            {
                                if (d1 > d2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.greaterEqual:
                            {
                                if (d1 >= d2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.lower:
                            {
                                if (d1 < d2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.lowerEqual:
                            {
                                if (d1 <= d2) { return true; } else { return false; }
                            }
                        default: return false;
                    }

                case Lib.FieldTypeEnum.Date:
                case Lib.FieldTypeEnum.Time:
                    DateTime dt1 = Convert.ToDateTime(val1);
                    DateTime dt2 = Convert.ToDateTime(val2);

                    switch (op)
                    {
                        case Lib.RIFDC_DataCompareOperatorEnum.equal:
                            {
                                if (dt1 == dt2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.notEuqal:
                            {
                                if (dt1 != dt2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.greater:
                            {
                                if (dt1 > dt2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.greaterEqual:
                            {
                                if (dt1 >= dt2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.lower:
                            {
                                if (dt1 < dt2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.lowerEqual:
                            {
                                if (dt1 <= dt2) { return true; } else { return false; }
                            }
                        default: return false;
                    }
                case Lib.FieldTypeEnum.Bool:
                    bool bool1 = Convert.ToBoolean(val1);
                    bool bool2 = Convert.ToBoolean(val2);
                    switch (op)
                    {
                        case Lib.RIFDC_DataCompareOperatorEnum.equal:
                            {
                                if (bool1 == bool2) { return true; } else { return false; }
                            }
                        case Lib.RIFDC_DataCompareOperatorEnum.notEuqal:
                            {
                                if (bool1 != bool2) { return true; } else { return false; }
                            }
                        default: return false;
                    }
            }

            return false;
        }

        class ComparerRule
        {
            public Lib.Filter.FilteringRule filteringRule;
            public bool filteringRuleValue; // это то же правило, только вычисленное
            public object val1;
            public object val2;
        }
    }
}
