using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActivityScheduler.Data.Executors;
using CoffeePointsDemo.Service;
using CoffeePointsDemoWpf.Service;
using RIFDC;

namespace CoffeePointsDemo
{
    public class CoffeePointsManager
    {
        private ItemKeeper<CoffeePoint> _repo;

        public CheckExecutor<Activity> _checker = new CheckExecutor<Activity>();

        public List<CoffeePoint> _itemsList = new List<CoffeePoint>();

        private void ReadItemsList()
        {
            _repo.readItems();
            _itemsList = _repo.actualItemList.Cast<CoffeePoint>().ToList();
        }

        public CoffeePointsManager(ItemKeeper<CoffeePoint> repo)
        {
            _repo = repo;

            /*
            _checker
            .AddCheck(new List<string>() { "Update" }, "ActivityId", (Activity activity) => {

                // get all activities of this batch
                _repo.readItems();
                var _list = _repo.actualItemList.Cast<CoffeePoint>;

                // _activitiesList = 

                var rez = Validation.CheckActivityId(activity.ActivityId.ToString());
                if (!rez.Success) { return rez; }

                //ActivityId should be unique
                var uniqueActivities = _activitiesList.Where(x=>(x.Id!= activity.Id) &&(x.ActivityId== activity.ActivityId)).ToList();
                if(uniqueActivities.Count>0)
                {
                    return CommonOperationResult.SayFail($"Activity with Id = {activity.ActivityId} already exists in this batch");
                }

                return CommonOperationResult.SayOk();
            })
            .AddCheck(new List<string>() { "Insert" }, "ActivityId", (Activity activity) => {

                if (activity.ActivityId.ToString() == "000") { return CommonOperationResult.SayOk(); }

                //get all activities of this batch
                _activitiesList = _repo.GetAllAsync(x => x.BatchId == activity.BatchId).Result.ToList();

                var rez = Validation.CheckActivityId(activity.ActivityId.ToString());
                if (!rez.Success) { return rez; }

                //ActivityId should be unique
                var uniqueActivities = _activitiesList.Where(x => (x.Id != activity.Id) && (x.ActivityId != activity.ActivityId)).ToList();
                if (uniqueActivities.Count > 0)
                {
                    return CommonOperationResult.SayFail($"Activity with Id = {activity.ActivityId} already exists in this batch");
                }

                return CommonOperationResult.SayOk();


            })
            .AddCheck(new List<string>() { "Update", "Insert" }, "ActivityId", (Activity activity) => {

                            if (activity.ActivityId > 9999 || activity.ActivityId<1)
                {
                                return CommonOperationResult.SayFail($"ActivityId is a number between 1 and 9999");
                            }

                            return CommonOperationResult.SayOk();
            })
            .AddCheck(new List<string>() { "Update", "Insert" }, "Name", (Activity activity) => {

            var rez = Validation.CheckIfTransactionOrBatchNameIsCorrect(activity.Name);
                if (!rez.Success) { return rez; }

                //ActivityName should be unique
                var uniqueActivities = _activitiesList.Where(x => (x.Id != activity.Id) && (x.Name == activity.Name)).ToList();
                if (uniqueActivities.Count > 0)
                {
                    return CommonOperationResult.SayFail($"Activity with Name = {activity.Name} already exists in this batch");
                }

                return CommonOperationResult.SayOk();
            })
            .AddCheck(new List<string>() { "Update", "Insert" }, "TransactionId", (Activity activity) => {
                var rez = Validation.CheckIf6DigitTrasactionNumberIsCorrect(activity.TransactionId);
                if (!rez.Success) { return rez; }
                return CommonOperationResult.SayOk();
            })
            .AddCheck(new List<string>() { "Update", "Insert" }, "Starttime", (Activity activity) => {
                if (activity.StartTime.TotalSeconds < 0) { return CommonOperationResult.SayFail("Activity start timepoint cant be negative"); }
                return CommonOperationResult.SayOk();
             });
            */
        }

        public Task<CommonOperationResult> AddNewItem(CoffeePoint item)
        {
            var rez = _repo.saveItem(item);
            return Task.FromResult( new OperationResultConverter().ConvetObjectOperationResultToCommonOperationResult(rez));
        }

        public Task<List<CoffeePoint>> GetAll()
        {
            _repo.readItems();
            var items = _repo.actualItemList.Cast<CoffeePoint>().ToList();
            return Task.FromResult(items);
        }

        public Task<CoffeePoint> GetItemById(string id)
        {
            return Task.FromResult((CoffeePoint)_repo.getItemById(id));
        }

        public Task<CommonOperationResult> ModifyItem(CoffeePoint item)
        {
            /* var btc = _checker.PerformCheck("Update", activity);

            if (!btc.Success)
            {
                return Task.FromResult(btc);
            }
            */

            var rez = _repo.saveItem(item);

            return Task.FromResult(new OperationResultConverter().ConvetObjectOperationResultToCommonOperationResult(rez)); ;
        }

        public Task<CommonOperationResult> RemoveItem(CoffeePoint item)
        {
            var delRez = _repo.deleteItem(item);

            if (!delRez.success)
            {
                return Task.FromResult(CommonOperationResult.SayFail($"Cannot remove activity because of error: {delRez.msg}"));
            }
            return Task.FromResult(new OperationResultConverter().ConvetObjectOperationResultToCommonOperationResult(delRez));
        }

        public Task<CommonOperationResult> RemoveAllItems()  
        {
            _repo.readItems();

            var idList = _repo.actualItemList.Select(select => select.id).ToList();

            idList.ForEach(x => {

                var _item = _repo.getItemById(x);

                _repo.deleteItem(_item);

            });

            return Task.FromResult(CommonOperationResult.SayOk());
        }

        public bool Similar(CoffeePoint? original, CoffeePoint? compare)
        {
            if (original == null && compare == null)
            {
                return true;
            }

            if (original == null || compare == null) 
            { 
                return false; 
            }

            bool rez = original.id == compare.id &&
                        original.Alias == compare.Alias &&
                        original.Name == compare.Name &&
                        original.Description == compare.Description &&
                        original.LastVisitDate == compare.LastVisitDate &&
                        original.BigLattePrice == compare.BigLattePrice;
            return rez;
        }

        public CoffeePoint Clone(CoffeePoint source)
        {
            CoffeePoint acv = new CoffeePoint();
            acv.id= source.id;
            acv.Alias = source.Alias;
            acv.Name = source.Name;
            acv.Description = source.Description;
            acv.LastVisitDate = source.LastVisitDate;
            acv.BigLattePrice = source.BigLattePrice;
            return acv;
        }
    }
}
