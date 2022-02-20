using System.Collections.Generic;

namespace PaymentGateway.Library.Interface
{
    public interface IMapper<TFrom, TTo>
    {
        TTo Map(TFrom item);

        IEnumerable<TTo> Map(IEnumerable<TFrom> items);
    }
}
