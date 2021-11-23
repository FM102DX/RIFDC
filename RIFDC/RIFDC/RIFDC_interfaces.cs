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


// Здесь интерфейсы, используемые в рамках всего решения

namespace RIFDC
{

    public interface IDataCluster
    {
        //по каждый вид хранения создается свой кластер. Фактически, класте- это драйвер конкретного вида базы
        //Например, есть драйвер под MySql, под Acess, Excel и др.

        StorageType storageType { get; }

        bool isNowConnected { get; }

        string dbCommonTitle { get; }

        Lib.DbOperationResult connect();

        Lib.DbOperationResult reconnect();

        Lib.DbOperationResult saveObject(IKeepable t);

        List<IUniversalRowDataContainer> makeGroupQuery(IKeeper keeper, Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField);

        void disconnect();

        Lib.DbOperationResult checkObjectTable(IKeepable t, bool drop=false);
        Lib.DbOperationResult deleteItem(IKeepable t);

        List<IUniversalRowDataContainer> readItems(IKeepable t, Lib.Filter filter);

        Lib.DbOperationResult deleteFiteredPackege(IKeepable sample, Lib.Filter filter);
        
    }

    public interface IDataRoom
    {

        IDataCluster actualCluster { get; set; }
        List<IUniversalRowDataContainer> makeGroupQuery(IKeeper keeper, Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField);

        Lib.DbOperationResult saveObject(IKeepable t);
        Lib.DbOperationResult checkObjectTable(IKeepable t, bool drop=false);
        Lib.DbOperationResult deleteItem(IKeepable t);
        List<IUniversalRowDataContainer> readItems(IKeepable t, Lib.Filter filter);

        Lib.DbOperationResult connect();
        void disconnect();
        bool isConnected { get; }

        Lib.DbOperationResult deleteFiteredPackege(IKeepable sample, Lib.Filter filter);
    }

    public interface ICurrentRecord
    {
        bool isFirst();
        bool isLast();
        void moveNext();
        void movePrevious();
        string id { get; set; }
        int index { get; set; }
        IKeepable getMember();
    }
    public interface ISortMethodGroup
    {
        void applySorter(Lib.Sorter sorter);
        void resetSorter();

        Lib.Sorter myActualSorter { get; }

        void applyActualSorter();
    }
    public interface IFilteratonMethodGroup
    {
        void applyGlobalFilter(Lib.Filter filter);
        void applyLocalFilter(Lib.Filter filter);
        void resetFilter(Lib.FiltrationTypeEnum filtrationType= Lib.FiltrationTypeEnum.Global);

        Lib.Filter myActualGlobalFilter { get;}
        Lib.Filter myActualLocalFilter { get; }

        Lib.Sorter myActualSorter { get;}

        void resetSearshFilter(Lib.FiltrationTypeEnum filtrationType);

    }
    public interface IKeepable
    {
        bool isTreeViewBased { get; } // является ли это деревом
        Lib.FieldsInfo fieldsInfo { get; }
        string tableName { get; }
        string entityName { get; }
        bool storeSerialized { get; }
        string id { get; set; }
        string parentId { get; set; }
        string generateMyId();

        IKeeper getMyIkeeper();
        void setDefaultValues(bool forNonNullableOnly = true);

        List<IKeepable> getMyDependentObjectsFromRelation(Relations.Relation relation);
        bool isFieldDirty(string fieldClassName);
        bool isDataPresenceValid { get; }
        Lib.KeepableClassStructureTypeEnum structureType { get; }
        int order { get; set; }
        Validation.ValidationResult validateMyParameter(Validation.ValidationTypeEnum validationType, string parameterClassName, object value);
        Lib.ObjectOperationResult setMyParameter(string parameterClassName, object value);
        object getMyParameter(string fieldClassName);

        IKeeperAlterationPackage getMyDirtyFields4History();
        // int saveMe(); //отдано в Ikeeper

        void setMyMultipleParameters(IKeeperAlterationPackage source);

        void saveMyPhoto();
        string displayId { get; }
        string displayName { get; }
        string displayNameLong { get; }
        string entityType { get; }
        Lib.FieldInfo getFieldInfoByFieldClassName(string fieldClassName);
        string objectDump();

        Lib.Filter getIncomingFilterFromInterFormMessage(Lib.InterFormMessage msg);

        string searchable { get; }

        bool needToSaveMyHistory { get; }
        int level { get; set; }

        string getMyStringRepresentation();

    }
    public interface IKeeper
    {
        bool isTreeViewBased { get; } // является ли это деревом
        IEnumerable items { get; }
        IKeepable sampleObject { get; }
        void readItems(bool append = false);
        List<IKeepable> actualItemList { get; }
        IFilteratonMethodGroup filtration { get; }
        ISortMethodGroup sort { get; }
        IDataRoom dataRoom { get; set; }
        ICurrentRecord currentRecord { get; }
        Lib.DbOperationResult checkMyTable(bool drop=false);
        void addExistingObject(IKeepable t);
        Lib.ObjectOperationResult saveItem(IKeepable t);
        Lib.ObjectOperationResult deleteItem(IKeepable t, bool silent=false);
        IKeepable getItemById(string id);

        void arrangeTreeLevels();

        Lib.ObjectOperationResult makeGroupQuery(Lib.GroupQueryTypeEnum groupQueryType, Relations.Relation targetRelation, string targetField);

        Lib.ObjectOperationResult deleteFiteredPackege(Lib.Filter filter);
        IKeepable createNewObject_inserted();
        IKeepable createNewObject_notInserted();
        int count { get; }
        void clear();
        string entityType { get; }
        void simpleObjectDump();

        void openMyHistoryFrm(Lib.InterFormMessage ifMsg);

        List<Lib.ObjectOperationResult> deleteMultipleItems(List<IKeepable> tl, bool silent = false);

        IKeepable GetRandomObject();
    }

    public interface IUniversalRowDataContainer
    {
        //для передачи любых multi-row данных между объектами
        List<IUniversalDataElement> items { get; }

        object getValueByName(string name);
        void setValueByName(string name, object value);
        void addNewElement(string _name, object _value);
        

    }
    public interface IUniversalDataElement
    {
        string name { get; set; }
        object value { get; set; }
    }

    public interface IKeeperWrap
    {
        void readMyItems();
    }
}