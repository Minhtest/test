using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Product
{
    public interface IProduct : IBaseServices<ProductEntity, long>
    {
        ProductEntity GetProductByIdAndShop(long id, int shopId);
    }
}
