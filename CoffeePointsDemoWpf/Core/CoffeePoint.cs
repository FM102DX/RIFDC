using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIFDC;

namespace CoffeePointsDemo
{
    public class CoffeePoint : KeepableClass, IKeepable
    {
        public string Name { get; set; }
        public DateTime LastVisitDate { get; set; }
        public double BigLattePrice { get; set; }
        public string Description { get; set; }
        public string Alias { get; set; }

        public override string TableName { get { return "CoffeePoints"; } }
        public override string EntityName { get { return "CoffeePoint"; } }

        public override Lib.FieldsInfo _get_fieldsInfo()
        {
            Lib.FieldInfo x;
            Lib.FieldsInfo f = this.getInitialFieldsInfoObject();

            x = f.addFieldInfoObject("Name", "Name", Lib.FieldTypeEnum.String);
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = "NewCoffeePoint";
            x.caption = "Название";
            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_strictNamingString());
            x.isSearchable = true;
            

            x = f.addFieldInfoObject("LastVisitDate", "LastVisitDate", Lib.FieldTypeEnum.Date);
            x.nullabilityInfo.allowNull = true;
            x.caption = "Посл.посещение";

            x = f.addFieldInfoObject("BigLattePrice", "BigLattePrice", Lib.FieldTypeEnum.Double);
            x.nullabilityInfo.allowNull = true;
            x.caption = "Цена большого латте";
            x.validationInfo.symbolValidation.rules.Add(new VCCommon.VF.com_symbol_double(','));
            x.validationInfo.leaveValidation.rules.Add(new VCCommon.VF.com_leave_double(',', 2));
            //x.validationInfo.businessValidation.rules.Add(new VCCommon.VF.biz_priceMoreThan80());
            x.saveHistory = true;

            x = f.addFieldInfoObject("Description", "Description", Lib.FieldTypeEnum.String);
            x.caption = "Комментарий";
            x.nullabilityInfo.allowNull = true;
            x.isSearchable = true;

            x = f.addFieldInfoObject("Alias", "Alias", Lib.FieldTypeEnum.String);
            x.caption = "Алиас";
            x.nullabilityInfo.allowNull = false;
            x.nullabilityInfo.defaultValue = "NewCoffeePointAlias";
            x.isSearchable = true;

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
                    tmp = "Name"; g.addFormatLine(tmp, 150, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "LastVisitDate"; g.addFormatLine(tmp, 100, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "BigLattePrice"; g.addFormatLine(tmp, 50, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "Description"; g.addFormatLine(tmp, 200, f.getFieldInfoObjectByFieldClassName(tmp).caption);
                    tmp = "Alias"; g.addFormatLine(tmp, 150, f.getFieldInfoObjectByFieldClassName(tmp).caption);

                    return g;
                }

            }
        }

        public override string DisplayName
        {
            get
            {
                return Name;
            }
        }

    }
}
