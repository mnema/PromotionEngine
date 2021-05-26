using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.DataAccess.DataModels
{
    public class GroupSavePromotion
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
