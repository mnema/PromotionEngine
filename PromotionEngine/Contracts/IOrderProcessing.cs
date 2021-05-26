using PromotionEngine2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine2.Contracts
{
    public interface IOrderProcessing
    {
        double GetTotalPrice(List<Item> items);
    }
}
