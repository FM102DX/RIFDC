using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIFDC;
using System.Windows.Forms;


namespace RIFDC
{
    //класс приложения, который используется фреймворком 
    public static class RIFDC_App
    {
        private static string _currentUserId = null;
        private static IDataRoom _mainDataRoom = null;
        public static RelationsHolder _relationsHolder;
        public static IKeeperSampleHolder _iKeeperSampleHolder;
        public static string workingDir;
        public static Form mainWindowFrmInstance;

        public static string currentUserId
        {
            get
            {
                if (_currentUserId == null) return "user01"; else return _currentUserId;
            }
            set
            {
                _currentUserId = value;
            }
        }
        public static IDataRoom mainDataRoom
        {
            get
            {
                if (_mainDataRoom == null) return new DataRoom(); else return _mainDataRoom;
            }
            set
            {
                _mainDataRoom = value;
            }
        }
        public static IKeeperSampleHolder iKeeperSampleHolder
        {
            get
            {
                if (_iKeeperSampleHolder == null)
                {
                    _iKeeperSampleHolder = new IKeeperSampleHolder();
                }
                return _iKeeperSampleHolder;
            }
        }

        public static RelationsHolder relationsHolder
        {
            get
            {
                if (_relationsHolder==null)
                {
                    _relationsHolder = new RelationsHolder();
                }
                return _relationsHolder;
            }
        }

    }
    public class RelationsHolder
    {
        //класс, который хранит все relations, чтобы в рантайме каждый объект мог найти свои
        List<Relations.Relation> relations = new List<Relations.Relation>();

        public void registerRelation(Relations.Relation rel)
        {
            relations.Add(rel);
        }

        public List<Relations.Relation> getMyRelations_where_Im_obligatory(IKeepable sampleObject)
        {
            List<Relations.Relation> rez = new List<Relations.Relation>();
            foreach (Relations.Relation rel in relations)
            {
                if (rel.isMyRelation_where_im_obligatory(sampleObject)) rez.Add(rel);
            }
            return rez;
        }

    }

    public class IKeeperSampleHolder
    {
        //класс, который хранит инстансы Ikeeper всех типов бизнес-объектов, т.к. дженерик в рантейме не объявишь

        List<IKeeper> iKeepers = new List<IKeeper>();
        
        public void registerIKeeper(IKeeper keeper)
        {
            iKeepers.Add(keeper);
        }

        public IKeeper getIKeeperByEntityType(string entityTypeName)
        {
            foreach (IKeeper k in iKeepers)
            {
                if (k.entityType == entityTypeName) return k;
            }
            return null;
        }


    }
}
