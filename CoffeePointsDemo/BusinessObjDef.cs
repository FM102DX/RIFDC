using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIFDC;

namespace CoffeePointsDemo
{
    class CoffeePoint : KeepableClass, IKeepable
    {
        public string name;
        public DateTime lastVisitDate;
        public double bigLattePrice;
        public string description;
        public string alias;

        public DateTime lastVisitDate2;
        public double bigLattePrice2;
        public string description2;


        public override string tableName { get { return "CoffeePoints"; } }
        public override string entityName { get { return "CoffeePoint"; } }

        public override Lib.FieldsInfo _get_fieldsInfo()
        {
            Lib.FieldInfo x;
            Lib.FieldsInfo f = this.getInitialFieldsInfoObject();

            x = f.addFieldInfoObject("name", "name", Lib.FieldTypeEnum.String);
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = "NewCoffeePoint";
            x.caption = "Название";
            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_strictNamingString());
            x.isSearchable = true;
            

            x = f.addFieldInfoObject("lastVisitDate", "lastVisitDate", Lib.FieldTypeEnum.Date);
            x.nullabilityInfo.allowNull = true;
            x.caption = "Посл.посещение";

            x = f.addFieldInfoObject("bigLattePrice", "bigLattePrice", Lib.FieldTypeEnum.Double);
            x.nullabilityInfo.allowNull = true;
            x.caption = "Цена большого латте";
            x.validationInfo.symbolValidation.rules.Add(new VCCommon.VF.com_symbol_double(','));
            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_double(',', 2));
            //x.validationInfo.businessValidation.rules.Add(new VCCommon.VF.biz_priceMoreThan80());
            x.saveHistory = true;

            x = f.addFieldInfoObject("description", "description", Lib.FieldTypeEnum.String);
            x.caption = "Комментарий";
            x.nullabilityInfo.allowNull = true;
            x.isSearchable = true;

            x = f.addFieldInfoObject("alias", "alias", Lib.FieldTypeEnum.String);
            x.caption = "Алиас";
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = "NewCoffeePointAlias";
            x.isSearchable = true;


            x = f.addFieldInfoObject("lastVisitDate2", "lastVisitDate2", Lib.FieldTypeEnum.Date);
            x.nullabilityInfo.allowNull = false;
            x.caption = "Посл.посещение2";
            x.nullabilityInfo.defaultValue = Convert.ToDateTime("01.01.2001");


            x = f.addFieldInfoObject("bigLattePrice2", "bigLattePrice2", Lib.FieldTypeEnum.Double);
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = 100;
            x.caption = "Цена большого латте2";
            x.validationInfo.symbolValidation.rules.Add(new VCCommon.VF.com_symbol_double(','));
            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_double(',', 2));
            x.validationInfo.businessValidation.rules.Add(new VCCommon.VF.biz_priceMoreThan80());
            x.isAvialbeForGroupOperations = true;


            x = f.addFieldInfoObject("description2", "description2", Lib.FieldTypeEnum.String);
            x.caption = "Комментарий2";
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = "Default Description";
            x.isSearchable = true;
            x.isAvialbeForGroupOperations = true;

            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_snake_style_alias());

            return f;
        }

        public class MyControlFormats
        {
            public class ShortCoffeePointGridFormat : Lib.GridBasedControlFormat, Lib.IControlFormat
            {
                ShortCoffeePointGridFormat(IKeepable _sampleObject) : base(_sampleObject)
                {

                }
                public static ShortCoffeePointGridFormat getMyInstance(IKeepable sampleObject)
                {
                    // if (parent == null) return null;
                    string tmp;
                    ShortCoffeePointGridFormat g = new ShortCoffeePointGridFormat(sampleObject);
                    Lib.FieldsInfo f = sampleObject.fieldsInfo;

                    tmp = "id"; g.addFormatLine(tmp, 0, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "name"; g.addFormatLine(tmp, 150, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "lastVisitDate"; g.addFormatLine(tmp, 100, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "bigLattePrice"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "description"; g.addFormatLine(tmp, 200, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "alias"; g.addFormatLine(tmp, 150, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "lastVisitDate2"; g.addFormatLine(tmp, 100, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "bigLattePrice2"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "description2"; g.addFormatLine(tmp, 200, f.getFieldInfoObjectByFieldClassName(tmp).caption);


                    return g;
                }

            }
        }

        public override string displayName
        {
            get
            {
                return name;
            }
        }

    }
}
