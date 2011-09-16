using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePont.Business
{
    public class DataPage<T>
    {
        public T[] Data;
        public long TotalRecords; 
    }
}
