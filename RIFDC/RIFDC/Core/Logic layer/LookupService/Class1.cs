using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIFDC
{
    public class LookupMachine
    {
        //та главная штука, которая все ищет
        public void doLookup()
        {

        }


    }
    public class LookupService
    {
        //сервис, который отвечает за поиск по конкретному типу объектов
        public IKeeper dataSource;

        public void doLookup()
        {

        }
        
        public List<IKeepable> lookuResult { get; }


    }


}
