using PromotionEngine.DataAccess.DataModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.DataAccess.Contracts
{
    public interface IPromotionProvider
    {
        PromotionModel GetPromotion();

    }
}
