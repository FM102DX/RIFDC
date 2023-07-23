using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIFDC.RIFDC.Service
{
    public static class Logger
    {

        static DynamicLogger logger = null;

        public static Fn.CommonOperationResult log(object domain, string text)
        {
            initLogger();
            return logger.log(domain, text);
        }

        public static Fn.CommonOperationResult log(string text)
        {
            initLogger();
            return logger.log(text);
        }

        public static void prepare()
        {
            initLogger(); logger.prepare();
        }

        private static void initLogger()
        {
            if (logger == null)
            {
                logger = new DynamicLogger();
            }
        }
        public static string fileName
        {
            get { initLogger(); return logger.fileName; }
            set { initLogger(); logger.fileName = value; }
        }

        public static DynamicLogger.logDirectionEnum logDirection
        {
            get { initLogger(); return logger.logDirection; }
            set { initLogger(); logger.logDirection = value; }
        }

        public static bool logIsOn
        {
            get { initLogger(); return logger.logIsOn; }
            set { initLogger(); logger.logIsOn = value; }
        }

    }


    public class DynamicLogger
    {
        public DynamicLogger()
        {

        }

        public static DynamicLogger getConsoleLoggerInstance()
        {
            DynamicLogger dl = new DynamicLogger();
            dl.logDirection = logDirectionEnum.toConsole;
            return dl;
        }

        public static DynamicLogger getFileLoggerInstance(string fileName)
        {
            DynamicLogger dl = new DynamicLogger();
            dl.logDirection = logDirectionEnum.toFile;
            dl.fileName = fileName;
            return dl;
        }
        public static DynamicLogger getFileAndConsoleLoggerInstance(string fileName)
        {
            DynamicLogger dl = new DynamicLogger();
            dl.logDirection = logDirectionEnum.bothToConAndFile;
            dl.fileName = fileName;
            return dl;
        }

        //Класс, занимающийся логгированием
        public string fileName = "";
        public bool logIsOn = true;
        public logDirectionEnum logDirection = logDirectionEnum.toConsole;
        public bool imTheAspNetService = false;
        public void prepare(bool killLogs = false)
        {
            if (logDirection == logDirectionEnum.bothToConAndFile || logDirection == logDirectionEnum.toFile)
            {
                FileInfo file = new FileInfo(fileName);
                if (killLogs || !file.Exists)
                {
                    StreamWriter sw = file.CreateText();
                    sw.Close();
                }
            }
        }

        void writeToConsole(string s)
        {
            if (imTheAspNetService)
            {
                Debug.WriteLine(s);
            }
            else
            {
                Console.WriteLine(s);
            }
        }
        void writeToFile(string s)
        {
            string writePath = fileName;
            StreamWriter sw = new StreamWriter(writePath, true, Encoding.Default);
            sw.WriteLine(s);
            sw.Close();
        }
        public Fn.CommonOperationResult log(object domain, object text)
        {
            try
            {
                string s = Convert.ToString(domain).ToUpper() + "_" + Fn.ConvertObjectToString(text);
                if (logIsOn)
                {
                    switch (logDirection)
                    {
                        case logDirectionEnum.toFile:
                            writeToFile(s);
                            break;
                        case logDirectionEnum.toConsole:
                            writeToConsole(s);
                            break;
                        case logDirectionEnum.bothToConAndFile:
                            writeToFile(s);
                            writeToConsole(s);
                            break;
                    }
                }
                return Fn.CommonOperationResult.SayOk();
            }
            catch (Exception ex)
            {
                return Fn.CommonOperationResult.SayFail(ex.Message);
            }
        }
        public Fn.CommonOperationResult log(object text)
        {
            return log("", text);
        }

        public enum logDirectionEnum
        {
            toConsole = 1,
            toFile = 2,
            bothToConAndFile = 3

        }
    }
}
