using PromotionEngine2.DataAccess.DataModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine2.DataAccess.Contracts
{
    public interface IPromotionProvider
    {
        PromotionModel GetPromotion();

    }
}
