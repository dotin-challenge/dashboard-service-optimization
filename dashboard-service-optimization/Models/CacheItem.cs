using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashboard_service_optimization.Models
{
    public class CacheItem<T> 
    {
        public T Value { get; set; }
        public DateTime Expiration { get; set; }
    }
}
