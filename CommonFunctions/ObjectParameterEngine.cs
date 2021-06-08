using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CommonFunctions;

namespace ObjectParameterEngine
{
    public static class ObjectParameters
    {
        //дело в том, что у объекта есть field и property, и все его поля данных - они либо такие, либо такие
        //часть данных объекта храняться как филд, часть - как проперти
        //чтобы не перебирать филд и проепрти каждый раз в коде, делается objectParameter внутри которого и перебирается филд и проперти

        //TODO 

        public static void setObjectParameter(object x, string name, object value)
        {

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (FieldInfo f0 in newObjectFields)
            {
                
                if (f0.Name.ToLower() == name.ToLower() )
                {
                    try
                    {
                        value = fn.convertedObject(f0.FieldType.ToString(), value); //чтобы внутри object было значение нужно типа
                        f0.SetValue(x, value);
                    }
                    catch
                    {

                    }
                }
            }

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == name.ToLower())
                {
                    if (isItOnlyGetter(x, name)) return;
                    try
                    {
                        value = fn.convertedObject(f1.GetType().ToString(), value);
                        f1.SetValue(x, value);
                    }
                    catch
                    { 

                    }
                }
            }
        }
        public static ObjectParameter getObjectParameterByName(object x, string name)
        {
            ObjectParameter op = new ObjectParameter();
            op.name = name;

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (FieldInfo f0 in newObjectFields)
            {

                if (f0.Name.ToLower() == name.ToLower())
                {
                    op.value = f0.GetValue(x);
                    return op;
                }
            }

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == name.ToLower())
                {
                    op.value = f1.GetValue(x);
                    return op;
                }
            }
            return null;
        }
        public static List<ObjectParameter> getObjectParameters(object x)
        {
            List<ObjectParameter> rez = new List<ObjectParameter>();

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            // 10/12/2020 - таким образом, экземпляр класса T, созданный в дженерике в рантайме, это экземпляр наследника, а не самого Т
            // их можно достать через fieldInfo и SetValue

            foreach (FieldInfo f0 in newObjectFields)  //это поля реального объекта, который только что был создан и будет добавлен в коллекцию
            {
                ObjectParameter op = new ObjectParameter();
                op.name = f0.Name;
                op.value = f0.GetValue(x);
                rez.Add(op);

            }

            foreach (PropertyInfo f1 in newObjectProperties)  //это поля реального объекта, который только что был создан и будет добавлен в коллекцию
            {
                ObjectParameter op = new ObjectParameter();
                op.name = f1.Name;
                op.value = f1.GetValue(x);
                rez.Add(op);
            }

            return rez;
        }

        public class ObjectParameter
        {
            public string name;
            public object value;
        }


        public static bool isItOnlyGetter (object x, string fieldName)
        {
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == fieldName.ToLower())
                {
                    //вот он нашел это поле
                    return (f1.CanRead  && (!f1.CanWrite));
                }
            }
            return false;

        }
    }
}
