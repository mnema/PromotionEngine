using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.DataAccess.DataModels
{
    public class PromotionModel
    {
        public List<GroupSavePromotion> GroupSavePromotions { get; set; }

        public List<ComboPromotion> ComboPromotions { get; set; }

    }
}
