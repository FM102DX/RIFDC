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
using System.Text.RegularExpressions;
using System.Globalization;
using RIFDC;

namespace RIFDC
{

    public class VCCommon : Validation.IValidationCluster
    {
        public class ValidationFormats
        {
            public class Double2digit : Lib.ValidationFormat
            {
                //двухзнаковый double
                public Double2digit()
                {
                    symbolValidation.rules.Add(new VCCommon.VF.com_symbol_double(','));
                    leaveValidation.rules.Add(new VCCommon.VF.com_leave_double(',', 2));
                }

            }

            public class IntCommon : Lib.ValidationFormat
            {
                //двухзнаковый double
                public IntCommon()
                {
                    symbolValidation.rules.Add(new VCCommon.VF.com_symbol_int());
                }
            }
        }

        public class VF
        {
            public class com_leave_double : Validation.IValidationFunction
            {
                char separator;
                int howMenyDigits;
                public com_leave_double(char _separator, int _howMenyDigits = 2)
                {
                    separator = _separator;
                    howMenyDigits = _howMenyDigits;

                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _com_leave_double(f, value);
                }

                private Validation.ValidationResult _com_leave_double(Lib.FieldInfo f, object value)
                {
                    //валидатор числа double с несколькими знаками после запятой и разделителем
                    //оставить только один сепаратор, последний в строке
                    //оставить указанное кол-во знаков после запятой

                    //предполагается, что у нас там только числа и сепаратор, т.е. ввод фильтровался
                    //value = Lib.convertedObjectRIFDCTypes(f.fieldType, value);
                    bool itsSeparator;
                    bool separatorFound = false;
                    int digits;
                    int sepPosition = -1;

                    string source = Convert.ToString(value);
                    string rez = "";
                    string A = "";
                    string B = "";
                    int i;

                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    string separator = this.separator.ToString();
                    digits = this.howMenyDigits;

                    if (source.Length == 0)
                    {
                        //можно сразу выйти, т.к. нечего проверять
                        vr.validationSuccess = true;
                        return vr;
                    }

                    //мы предполагаем, что есть 1 сепаратор, он последний в строке, и его надо оставить, остальные убрать
                    //идем с конца строки

                    for (i = source.Length - 1; i >= 0; i--)
                    {
                        itsSeparator = (source[i].ToString() == separator);

                        if (!itsSeparator)
                        {
                            rez = source[i].ToString() + rez;
                        }

                        if (itsSeparator && (!separatorFound))
                        {
                            separatorFound = true;
                            rez = source[i].ToString() + rez;
                            sepPosition = i;
                        }

                        if (itsSeparator && separatorFound)
                        {
                            //ничего не делаем, пропускаем, пишу это для ясности
                        }
                    }

                    //теперь надо понять, какая часть целая, какая дробная
                    source = rez;
                    separatorFound = false;

                    for (i = 0; i < source.Length; i++)
                    {
                        itsSeparator = (source[i].ToString() == separator);

                        if (itsSeparator && (!separatorFound))
                        {
                            separatorFound = true;
                        }

                        if (!itsSeparator && (!separatorFound))
                        {
                            A += source[i].ToString();
                        }
                        if (!itsSeparator && (separatorFound))
                        {
                            B += source[i].ToString();
                        }
                    }

                    if (A == "") { A = "0"; }
                    rez = A + ((B == "") ? "" : separator + B);

                    //теперь округление

                    double d = Convert.ToDouble(rez);
                    if (digits < 0) digits = 0;
                    d = Math.Round(d, digits, MidpointRounding.ToEven);
                    rez = d.ToString();

                    //теперь такая штука: количество знаков после сепа должно быть = digits
                    //если сепа нет - добавить
                    sepPosition = rez.IndexOf(separator[0]);
                    if (sepPosition == -1)
                    {
                        rez += separator;
                        sepPosition = rez.Length - 1;
                    }

                    int n = digits - (rez.Length - sepPosition - 1);

                    if (n > 0) { rez = rez + fn.RepeatString("0", n); }

                    vr.validationSuccess = true;
                    vr.validatedValue = rez;
                    return vr;
                }
            }

            public class com_symbol_double : Validation.IValidationFunction
            {
                char separator;
                public com_symbol_double(char _separator)
                {
                    separator = _separator;
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _com_symbol_double(f, value);
                }
                Validation.ValidationResult _com_symbol_double(Lib.FieldInfo f, object value)
                {
                    //посимвольный валидатор числа double
                    //в поле можно вводить числа и сепаратор
                    bool itsDigit;
                    bool itsSeparator;
                    string rez = "";

                    string source = Convert.ToString(value);
                    string separator = Convert.ToString(this.separator);
                    for (int i = 0; i < source.Length; i++)
                    {
                        itsDigit = "0123456789".Contains(source[i]);
                        itsSeparator = (source[i].ToString() == separator);
                        if (itsDigit || itsSeparator) { rez += source[i]; }
                    }

                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    vr.validationSuccess = true; //символьная валидация всегда возвращает true  и пакет символов, которые ее прошли
                    vr.validatedValue = rez;
                    return vr;
                }

            }

            public class com_symbol_int : Validation.IValidationFunction
            {
                public com_symbol_int()
                {

                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _com_symbol_int(f, value);
                }

                Validation.ValidationResult _com_symbol_int(Lib.FieldInfo f, object value)
                {
                    //посимвольный валидатор числа double
                    //в поле можно вводить числа и сепаратор
                    bool itsDigit;
                    string rez = "";
                    string source = Convert.ToString(value);

                    for (int i = 0; i < source.Length; i++)
                    {
                        itsDigit = "0123456789".Contains(source[i]);
                        if (itsDigit) { rez += source[i]; }
                    }

                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    vr.validationSuccess = true; //символьная валидация всегда возвращает true  и пакет символов, которые ее прошли
                    vr.validatedValue = rez;
                    return vr;
                }

            }

            public class biz_priceMoreThan80 : Validation.IValidationFunction
            {
                public biz_priceMoreThan80()
                {
                    
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _priceMoreThan80(f, value);
                }

                Validation.ValidationResult _priceMoreThan80(Lib.FieldInfo f, object value)
                {
                    
                    Lib.CommonOperationResult convrez = Lib.convertedObjectRIFDCTypes(f.fieldType, value);
                    if (!convrez.success) return Validation.ValidationResult.sayNo(convrez.msg);
                    value = convrez.returnedValue;

                    //этот метод проверяет число на положительность
                    Validation.ValidationResult r = new Validation.ValidationResult();
                    r.validationSuccess = false;

                    double i = Convert.ToDouble(value);
                    if (i > 80)
                    {
                        r.validationSuccess = true;
                    }
                    else
                    {
                        r.validationMsg = "Цена большого латте должна быть не менее 80 рэ";
                    }

                    return r;
                }

            }

            public class biz_priceLessThan60 : Validation.IValidationFunction
            {
                public biz_priceLessThan60()
                {

                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _priceLessThan60(f, value);
                }

                Validation.ValidationResult _priceLessThan60(Lib.FieldInfo f, object value)
                {

                    Lib.CommonOperationResult convrez = Lib.convertedObjectRIFDCTypes(f.fieldType, value);
                    if (!convrez.success) return Validation.ValidationResult.sayNo(convrez.msg);
                    value = convrez.returnedValue;

                    //этот метод проверяет число на положительность
                    Validation.ValidationResult r = new Validation.ValidationResult();
                    r.validationSuccess = false;

                    double i = Convert.ToDouble(value);
                    if (i < 60)
                    {
                        r.validationSuccess = true;
                    }
                    else
                    {
                        r.validationMsg = "Цена большого латте должна быть не более 60 рэ";
                    }
                    return r;
                }
            }

            public class leave_correctDateEntered_ddmmyyy : Validation.IValidationFunction
            {
                public leave_correctDateEntered_ddmmyyy()
                {

                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    return _correctDateEntered_ddmmyyy(f, value);
                }

                Validation.ValidationResult _correctDateEntered_ddmmyyy(Lib.FieldInfo f, object value)
                {
                    //этот метод проверяет, что введена корректная дата dd mm yyyy
                    Validation.ValidationResult r = new Validation.ValidationResult();
                    r.validationSuccess = false;

                    if (f.fieldType != Lib.FieldTypeEnum.Date) return r;

                    DateTime dt;
                    
                    bool nullable = (f.nullabilityInfo.allowNull);
                    bool isNull = (value == null);

                    if (nullable && isNull)
                    {
                        return Validation.ValidationResult.sayOk();
                    }

                    if ((!nullable) && isNull)
                    {
                        return Validation.ValidationResult.getInstance(true, "", f.nullabilityInfo.defaultValue);
                    }

                    string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                                             "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy"};

                    bool b1 = DateTime.TryParseExact(value.ToString(), formats,
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None, out dt);


                    //   DateTime.TryParseExact(value.ToString(), "dd.MM.yyyy HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    //bool b2 = DateTime.TryParseExact(value.ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt1);
                    if (b1)
                    {
                        return Validation.ValidationResult.getInstance(true, "", dt);
                    }
                    else
                    {
                        //Logger.log("DATA", "");
                        return Validation.ValidationResult.getInstance(false, "Необходимо ввести корректную дату");
                    }
                }

            }

            public class strMinLengthCheck : Validation.IValidationFunction
            {
                int minLength;
                public strMinLengthCheck(int _minLength)
                {
                    minLength = _minLength;
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    Validation.ValidationResult vr = new Validation.ValidationResult();

                    vr.validationSuccess = false;

                    string s = "";
                    try
                    {
                        s = Convert.ToString(value);
                        if (s.Length >= minLength)
                        {
                            vr.validationSuccess = true;
                        }
                        else
                        {
                            vr.validationMsg = "Строка должны быть длинее " + minLength.ToString() + " символов";
                        }
                    }
                    catch
                    {
                        vr.validationMsg = "Ошибка проверки строки: функция strMinLengthCheck";
                    }
                    return vr;

                }
            }

            public class strMaxLengthCheck : Validation.IValidationFunction
            {
                int maxLength;
                public strMaxLengthCheck(int _maxLength)
                {
                    maxLength = _maxLength;
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    Validation.ValidationResult vr = new Validation.ValidationResult();

                    vr.validationSuccess = false;

                    string s = "";
                    try
                    {
                        s = Convert.ToString(value);
                        if (s.Length <= maxLength)
                        {
                            vr.validationSuccess = true;
                        }
                        else
                        {
                            vr.validationMsg = "Строка не должны быть длинее " + maxLength.ToString() + " символов";
                        }
                    }
                    catch
                    {
                        vr.validationMsg = "Ошибка проверки строки: функция strMaxLengthCheck";
                    }
                    return vr;

                }
            }

            public class com_leave_commonNamingString : Validation.IValidationFunction
            {
                public com_leave_commonNamingString()
                {
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    //оставляет в строке только буквы, " @ # № ? * + - _ и пробелы
                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    Regex regex;
                    string source = Convert.ToString(value);
                    source = fn.RemoveArraySymbolsFromString(source, @"\|/^");
                    regex = new Regex("[^a-zA-Zа-яА-Я0-9() +-_:;!?@#.*]", RegexOptions.IgnoreCase);
                    source = regex.Replace(source, "");

                    vr.validatedValue = source;
                    vr.validationSuccess = true;
                    return vr;

                }
            }

            public class com_leave_snake_style_alias : Validation.IValidationFunction
            {
                public com_leave_snake_style_alias()
                {
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    //оставляет в строке только буквы, " @ # № ? * + - _ и пробелы
                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    Regex regex;
                    string source = Convert.ToString(value);
                    source = fn.RemoveArraySymbolsFromString(source, @"\|/^");
                    regex = new Regex("[^a-zA-Z0-9_*]", RegexOptions.IgnoreCase);
                    source = regex.Replace(source, "");

                    vr.validatedValue = source.ToLower();
                    vr.validationSuccess = true;
                    return vr;

                }
            }

            public class com_leave_strictNamingString : Validation.IValidationFunction
            {
                public com_leave_strictNamingString()
                {
                }
                public Validation.ValidationResult validate(Lib.FieldInfo f, object value)
                {
                    //оставляет в строке только буквы и пробелы
                    Validation.ValidationResult vr = new Validation.ValidationResult();
                    Regex regex;
                    string source = Convert.ToString(value);
                    source = fn.RemoveArraySymbolsFromString(source, @"\|/^");
                    regex = new Regex("[^a-zA-Zа-яА-Я0-9 ]", RegexOptions.IgnoreCase);
                    source = regex.Replace(source, "");

                    vr.validatedValue = source;
                    vr.validationSuccess = true;
                    return vr;

                }
            }
        }
    }


}