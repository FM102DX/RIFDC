using CoffeePointsDemo;
using CoffeePointsDemo.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActivityScheduler.Data.Managers
{
    public class DataFillManager
    {
        private CoffeePointsManager _coffeePointsManager;
        public DataFillManager(CoffeePointsManager coffeePointsManager) 
        {
            _coffeePointsManager= coffeePointsManager;
        }

        public void FillTheModel()
        {
            CommonOperationResult rez;

            var remRez= _coffeePointsManager.RemoveAllItems().Result;

            if (!remRez.Success)
            {
                return;
            }

            CoffeePoint point1 = new CoffeePoint()
            {
                Alias = "cpt1",
                Name = "CoffeePoint1",
                BigLattePrice = 120,
                Description = "CoffeePoint1 description"
            };

            _coffeePointsManager.AddNewItem(point1);

            CoffeePoint point2 = new CoffeePoint()
            {
                Alias = "cpt2",
                Name = "CoffeePoint2",
                BigLattePrice = 120,
                Description = "CoffeePoint2 description"
            };
            _coffeePointsManager.AddNewItem(point2);

            CoffeePoint point3 = new CoffeePoint()
            {
                Alias = "cpt3",
                Name = "CoffeePoint3",
                BigLattePrice = 120,
                Description = "CoffeePoint3 description"
            };
            _coffeePointsManager.AddNewItem(point3);
        }

    }
}
