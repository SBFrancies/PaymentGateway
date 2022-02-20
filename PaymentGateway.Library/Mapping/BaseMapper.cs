using PaymentGateway.Library.Interface;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Library.Mapping
{
    public abstract class BaseMapper<TFrom, TTo> : IMapper<TFrom, TTo>
    {
        public abstract TTo Map(TFrom item);

        public IEnumerable<TTo> Map(IEnumerable<TFrom> items)
        {
            if (items == null)
            {
                return null;
            }

            return items.Select(Map);
        }
    }
}
