using Newtonsoft.Json;
using PromotionEngine.DataAccess.Contracts;
using PromotionEngine.DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine.DataAccess
{
    public class PromotionProvider : IPromotionProvider
    {
        public PromotionModel GetPromotion()
        {
            using (StreamReader r = new StreamReader("DataJson\\Promotions.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<PromotionModel>(json);

            }
        }
    }
}
