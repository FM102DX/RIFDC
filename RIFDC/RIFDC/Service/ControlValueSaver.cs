using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CommonFunctions;
using System.IO;


namespace RIFDC
{
    public class ControlValueSaver
    {
        //сохраняет данные с контролов в текстовый файл, чтобы при следующем запуске программы не вводить все заново
        
        private List<Control> items = new List<Control>();
        public class ValueSaver { }
        string workingDir;
        string prefix;

        public ControlValueSaver(string workingDir, string prefix)
        {
            this.workingDir = workingDir;
            this.prefix = prefix;
        }

        public string path { get { return workingDir + @"\" + prefix + "_saved_data.rifdcfile"; } }

        public void saveIt()
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);
                foreach (Control c in items)
                {
                    sw.WriteLine(c.Name + "=" + c.Text);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }


            /*              
             *              
             *              FileStream f = new FileStream(path, FileMode.Create);

                            string s = "";
                            foreach (Control c in items)
                            {
                                s += c.Name + "=" + c.Text + ((char)13).ToString();
                            }
                            byte[] input = Encoding.Default.GetBytes(s);
                            f.Write(input, 0, input.Length);
                            f.Close();
                            */

            /*   if (String.IsNullOrEmpty(workingDir)) return;
               ValueSaver vs = new ValueSaver();
               vs.cbx_BBC_BOMFile_value = frm.cbx_BBC_BOMFile.Text;

               System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ValueSaver));

               var path = this.workingDir + @"\SerializationOverview.xml";
               //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
               System.IO.FileStream file = System.IO.File.Create(path);

               writer.Serialize(file, vs);
               file.Close();
               */
        }

        private void setCtrlValue(string ctrlName, string value)
        {
            foreach (Control c in items)
            {
                if (c.Name == ctrlName)
                {
                    c.Text = value;
                    break;


                }
            }
        }

        public void addSaverCtrl (Control c)
        {
            items.Add(c);
        }

        public void loadIt()
        {
            string[] _arr = new string[2];

            try
            {

                StreamReader sr = new StreamReader(path);
                //Read the first line of text
                string line;
                List<string> _items = new List<string>();

                

                do
                {
                    line = sr.ReadLine();
                    _items.Add(line);

                } while (line != null);
                sr.Close();

                foreach (string s in _items)
                {
                    _arr = s.Split('=');
                    setCtrlValue(_arr[0], _arr[1]);

                    fn.Dp(_arr[0] + "=" + _arr[1]);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void clearItems()
        {
            items.Clear();
        }
    }
}
