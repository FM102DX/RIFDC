using CoffeePointsDemo.Service;
using RIFDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeePointsDemoWpf.Service
{
    public class OperationResultConverter
    {
        public CommonOperationResult ConvetObjectOperationResultToCommonOperationResult(Lib.ObjectOperationResult source)
        {
            return new CommonOperationResult() { Success = source.success, Message = source.msg };
        }
    }
}
