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
using RIFDC;using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace RIFDC
{
    //RIFDC - Research inc ERM-движок 
    //предназначен для того, чтобы при создании приложений не писать для каждого типа объектов операции - чтение/запись/удаление в БД
    //когда программируем, мы делаем только объект с бизнес-полями, наследуюем его от keepable class
    //для упраления коллекциями объектов создается класс ItemKeeper<KeepableClass>

    //TODO - 6.12.2020 - читать из базы не все объекты, а часть. напр что-то типа механизма страниц, т.к. объектов там может быть оч. много, чтобы память не забивать. Ну пока он читает все.

    //TODO - не только сериализация объектов, но и автогенерация форм


    //TODO - объект готовит под себя базу, под свои поля == значит, будет список полей, то есть fieldlist
    //ну то есть я ввел название таблиц и забыл про это дело
    //это можно и позже сделать

    //TODO - кейс, когда у тебя объекты одного типа но с разной аналитикой хранятся в одной и той же таблице
    //решено-- вводим объект parentObject (хотя они и так не будут пересекаться, т.к. там есть отличающие их поля)


    public static class Manager
    {

    }



    public static class RTFSaver
    {
        //класс, который занимается сохранением RTF

        public static string fileName { get; set; }



        public static void save(string text)
        {
            writeToFile(text);
        }
        private static void writeToFile(string s)
        {
            string writePath = fileName;

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(s);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }


    public static class ClassValidator
    {
        //объект, который проверяет классы сразу после запуская приложения на предмет правильного сбора fieldInfo
        public static List<Type> items = new List<Type>();

        public static bool registerClass(Type targetClass)
        {
            object a = Activator.CreateInstance(targetClass);
            IKeepable k;
            bool rez;

            try
            {
                k = (IKeepable)a;
                rez = k.fieldsInfo.validateMySelf();
                if (!rez) return false;
            }
            catch
            {

            }

            items.Add(targetClass);
            return true;
        }
    }


    public static class ObjectMessenger
    {
        //позволяет объектам обмениваться между собой сообщеняими
        //например, одна форма может послать другой форме сообщение изменить фильтр.

        public delegate void MessageReceiverProc(List<ObjectMessage> messages);

        // список нужен для того, чтобы объект мог забрать сообщения не сразу, а когда будет запущен, например
        public static List<ObjectMessage> items = new List<ObjectMessage>();

        public static void sendMessage(ObjectMessage msg)
        {
            items.Add(msg);
            msg.receiver.messenger.getMail();
        }

        public static List<ObjectMessage> getMyMail(IObjectMessengerParty receiver)
        {
            //через этот метод объект читает свои сообщения -он просто получает List<IObjectMessage>

            List<ObjectMessage> messages = items.Where(f => f.receiver == receiver).ToList();

            foreach (ObjectMessage m in messages)
            {
                items.Remove(m);
            }
            return messages;
        }

        public interface IObjectMessengerParty
        {
            //объект, который может принимать / отправлять сообщения
            //void readMyMail(); //  сходи сюда и прочитай почту, ну а как конкретны объект будет это делать - это его личное дело

            ObjectMessengerClient messenger { get; set; }

            //string className { get; }

        }

        public interface IObjectMessageArgs
        {
            ObjectMessageTypeNeum myMsgType { get; }
        }

        public enum ObjectMessageTypeNeum
        {
            FormManagersCommunicationMessage = 1
        }

        public enum FormmManagerCommunicationMessageTypeEnum
        {
            changeFilter = 1
        }

        public class FormmManagerCommunicationMessageArgs : IObjectMessageArgs
        {
            public ObjectMessageTypeNeum myMsgType { get { return ObjectMessageTypeNeum.FormManagersCommunicationMessage; } }
            public FormmManagerCommunicationMessageTypeEnum msgType2;
            public Lib.Filter filter;

        }

        public class ObjectMessage
        {
            //  это само сообщение
            //  формат сообщения универсален, поскольку для отправки и получения используется одна и та же функция
            public IObjectMessengerParty sender { get; set; }
            public IObjectMessengerParty receiver { get; set; }
            public IObjectMessageArgs args { get; set; }

            public ObjectMessageTypeNeum msgType { get; set; }

            public bool isCorrect()
            {
                if (args == null) { return false; }
                //if (msgType == null) { return false; }
                if (args.myMsgType != msgType) { return false; }
                return true;
            }
        }

        public class ObjectMessengerClient
        {
            //это класс, который создается на объекте.
            // он инкапсулирует все методы работы с сообщениями, 
            //а т.ж. гарантирует, что каждый объект может отпарвлять/получать только свою почту


            MessageReceiverProc msgProc;
            IObjectMessengerParty me;

            public ObjectMessengerClient(IObjectMessengerParty _me, MessageReceiverProc _msgProc)
            {
                me = _me;
                msgProc = _msgProc;
            }

            public void sendMessage(IObjectMessengerParty receiver, ObjectMessageTypeNeum msgType, IObjectMessageArgs args)
            {

                ObjectMessage msg = new ObjectMessage();

                msg.sender = me;  //вот для этого и делалась единя процедура отправки - чтобы объект мог отправлять/получать только свою почту
                msg.receiver = receiver;
                msg.msgType = msgType; //мы не ограничиваем типы сообщений, которые объекты могут передавать друг другу.
                msg.args = args;
                if (!msg.isCorrect())
                {
                    fn.dp("OBJECT MESSENGER ERROR - неверный формат объектного сообщения");
                    return;
                }

                ObjectMessenger.sendMessage(msg);
            }

            public void getMail()
            {
                List<ObjectMessage> messages = ObjectMessenger.getMyMail(me);
                me.messenger.msgProc(messages);
            }
        }
    }


    public static class DependencyManager
    {

        public static List<Dependency> items = new List<Dependency>();

        public static Dependency createDependency(
                                            IDependable master,
                                            IDependable dependent,
                                            Relations.Relation relation,
                                            DependencyTypeEnum dependencyType
            )
        {

            Dependency d = new Dependency();

            d.master = master;
            d.dependent = dependent;
            d.relation = relation;
            d.dependencyType = dependencyType;

            // тут надо проверить, что эти объекты и правда соответствуют этому relation
            string val1 = master.entityType.ToLower();
            string val2 = dependent.entityType.ToLower();

            string val3 = relation.A.targetClassName.ToLower();
            string val4 = relation.B.targetClassName.ToLower();

            bool fit_1 = (val1 == val3) && (val2 == val4);
            bool fit_2 = (val1 == val4) && (val2 == val3);
            bool fit = fit_1 || fit_2;

            if (fit)
            {
                items.Add(d);
                return d;
            }
            else
            {
                fn.dp("ERROR creation dependency: objects don't fit relation");
                return null;
            }
        }

        public interface IDependable : ObjectMessenger.IObjectMessengerParty
        {
            string entityType { get; } //название KK
            string getFieldValueByFieldClassName(string fieldClassName);
        }

        public static List<Dependency> getMyDependecies(IDependable master)
        {
            return items.Where(f => f.master == master).ToList();
        }

        public static List<Dependency> getMyActualMasters(IDependable me)
        {
            return items.Where(f => (f.dependent == me && f.dependencyType == DependencyTypeEnum.setFilter)).ToList();
        }



        public enum DependencyTypeEnum
        {
            setFilter = 1
        }

        public class Dependency
        {
            public IDependable master;
            public IDependable dependent;
            public Relations.Relation relation;
            public DependencyTypeEnum dependencyType;
        }

        public class DependencyManagerClient
        {
            IDependable parent;
            public DependencyManagerClient(IDependable _parent)
            {
                parent = _parent;
            }

            public void processMyDependencies(DependencyArgs args = null)
            {
                //перебрать все депендесы где он мастер и сделать соотв. дествия

                List<Dependency> items0 = items.Where(f => f.master == parent).ToList();
                foreach (Dependency d in items0)
                {
                    if (d.dependencyType == DependencyTypeEnum.setFilter)
                    {
                        //это команда слейву от мастера применить к себе фильтр


                        //теперь надо передать, что такое-то поле у слейва должно принять значение x
                        // какое именно поле - мы берем из relation
                        // в relation должен быть некий метод, который возвращает поле, по которому слейв зависит от мастера

                        Relations.Relation.RelationSide X0 = d.relation.getRealtionSideByClassName(d.master.entityType);
                        Relations.Relation.RelationSide X = d.relation.getRealtionSideByClassName(d.dependent.entityType);

                        string masterValue = d.master.getFieldValueByFieldClassName(X0.fieldName);

                        Lib.Filter filter = new Lib.Filter();

                        //теперь надо взять поле, которое в релейшен у мастера

                        filter.addNewFilteringRule(X.fieldInfo, Lib.RIFDC_DataCompareOperatorEnum.equal, masterValue, Lib.Filter.FilteringRuleTypeEnum.ParentDFCFilteringRule, X0.targetClass);

                        ObjectMessenger.FormmManagerCommunicationMessageArgs args1 = new ObjectMessenger.FormmManagerCommunicationMessageArgs();

                        args1.filter = filter;

                        args1.msgType2 = ObjectMessenger.FormmManagerCommunicationMessageTypeEnum.changeFilter;

                        d.master.messenger.sendMessage(d.dependent, ObjectMessenger.ObjectMessageTypeNeum.FormManagersCommunicationMessage, args1);
                    }
                }
            }
        }

        public class DependencyArgs
        {
            //класс-пакет для передачи аргументов
            public string  targetFieldId = "";
        }
    }

}