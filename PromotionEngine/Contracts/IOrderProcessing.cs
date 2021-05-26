using PromotionEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.Contracts
{
    public interface IOrderProcessing
    {
        double GetTotalPrice(List<ItemModel> items);
    }
}
