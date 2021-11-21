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
using RICOMPANY.CommonFunctions;
using System.Text.RegularExpressions;

namespace RIFDC
{


    public static class Validation
    {
        public interface IValidationFunction
        {
            //это конкретная функция, которая выполняет валидацию, она предоставляет собой класс
            Validation.ValidationResult validate(Lib.FieldInfo f, object value);
        }
        public interface IValidationCluster
        {
            //это просто группировка валидационых функций и форматов

        }

        public interface IValidationFormat
        {
            //формат, который представляет собой группировку валидационных функций
            List<IValidationFunction> symbolValidationRules { get; }
            List<IValidationFunction> leaveValidationRules { get; }
            List<IValidationFunction> businessValidationRules { get; }
        }
        public static List<Validation.IValidationFunction> mergeVrls (List<Validation.IValidationFunction> vrl1, List<Validation.IValidationFunction> vrl2)
        {
            //просто смерджить 2 листа
            List<Validation.IValidationFunction> rez = new List<IValidationFunction>();
            if (vrl1!=null) foreach (IValidationFunction f in vrl1) { rez.Add(f); }
            if (vrl2 != null) foreach (IValidationFunction f in vrl2) { rez.Add(f); }
            return rez;

        }
        public static ValidationResult validate(Lib.FieldInfo f, ValidationTypeEnum validationType, object value)
        {
            ValidationResult vr = new ValidationResult();
            List<IValidationFunction> tmp=null;
            switch (validationType)
            {
                case ValidationTypeEnum.business:
                    tmp = f.validationInfo.businessValidationRuleFullList;
                    break;
                case ValidationTypeEnum.leave:
                    tmp = f.validationInfo.leaveValidationRuleFullList;
                    break;
                case ValidationTypeEnum.symbol:
                    tmp = f.validationInfo.symbolValidationRuleFullList;
                    break;
            }
            vr.validatedValue = value;
            vr.validationSuccess = true;

            foreach (IValidationFunction ivf in tmp)
            {
                vr = ivf.validate(f, vr.validatedValue);
                if (vr.validationSuccess == false)
                {
                    return vr; //если какая -то из валидационных функций не пропустит, то выход
                }
            }
            return vr;
        }

        public class ValidationResult
        {
            public bool validationSuccess;
            public string validationMsg;
            public object validatedValue;

            public static ValidationResult getInstance(bool _validationSuccess, string _validationMsg="", object _validatedValue=null)
            {
                ValidationResult vr = new ValidationResult
                {
                    validationSuccess = _validationSuccess,
                    validationMsg = _validationMsg,
                    validatedValue = _validatedValue
                };
                return vr;
            }
            public static ValidationResult sayOk(string _validationMsg="")
            {
                return getInstance(true, _validationMsg);
            }
            public static ValidationResult sayNo(string _validationMsg = "")
            {
                return getInstance(false, _validationMsg);
            }

        }

        public enum ValidationTypeEnum
        {
            symbol = 1,
            leave = 2,
            business = 3
        }
    }


}