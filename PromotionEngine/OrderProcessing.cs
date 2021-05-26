using Newtonsoft.Json;
using PromotionEngine2.Contracts;
using PromotionEngine2.DataAccess.Contracts;
using PromotionEngine2.DataAccess.DataModels;
using PromotionEngine2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngine2
{
    public class OrderProcessing : IOrderProcessing   
    {
        private readonly IPromotionProvider promotionProvider;
        private PromotionModel promotion;

        public OrderProcessing(IPromotionProvider promotionProvider)
        {
            this.promotionProvider = promotionProvider;
            this.promotion = GetComboPromotion();
        }

        private PromotionModel GetComboPromotion()
        {
            return promotionProvider.GetPromotion();
        }

        public double GetTotalPrice(List<Item> items)
        {
            if (items != null && items.Count > 0)
            {
                var totalPrice = 0.0;

                //check and Add GroupSave Promotion
                var groupsavePrice = GetGroupSavePromotion(items);
                totalPrice += groupsavePrice;
              
                //check and Add Combo Promotion
                var comboItemsPrice = GetComboPromotion(items);                
                totalPrice += comboItemsPrice;

                //add remaining item prices
                foreach (var item in items)
                {
                    totalPrice += item.Quantity * GetPrice(item.SKU);  
                }

                return totalPrice;
            }

            return 0;

        }

        private double GetGroupSavePromotion(List<Item> items)
        {
            var totalPrice = 0.0;

            foreach (var item in items.ToList())
            {
                var groupItemPrice = 0.0;
                if (promotion.GroupSavePromotions.Any(x => x.SKU == item.SKU && x.Quantity <= item.Quantity))
                {
                    var groupSavePromotion = promotion.GroupSavePromotions.Where(x => x.SKU == item.SKU && x.Quantity <= item.Quantity).FirstOrDefault();
                    groupItemPrice = (item.Quantity / groupSavePromotion.Quantity) * groupSavePromotion.Price + (item.Quantity % groupSavePromotion.Quantity * GetPrice(item.SKU));
                }
               
                if (groupItemPrice != 0)
                {
                    totalPrice += groupItemPrice;
                    items.Remove(item);
                }
            }
        
            return totalPrice;        
        }

        private double GetComboPromotion(List<Item> items)
        {            
            var totalPrice = 0.0;

            foreach (var combo in promotion.ComboPromotions)
            {
                var totalcomboOfer = 0;
                int SKU1Quantity = 0;
                int SKU2Quantity = 0;

                if (items.Any(x => x.SKU == combo.SKU1) && items.Any(x => x.SKU == combo.SKU2))
                {
                    SKU1Quantity = items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity;
                    SKU2Quantity = items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity;

                    if (SKU1Quantity  <= SKU2Quantity)
                    {
                        totalcomboOfer = SKU1Quantity;
                    }
                    else
                    {
                        totalcomboOfer = SKU2Quantity;
                    }

                    items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity -= totalcomboOfer;
                    items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity -= totalcomboOfer;

                    totalPrice += totalcomboOfer * combo.Price + items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity * GetPrice(combo.SKU1) + items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity * GetPrice(combo.SKU2);
                    
                    items.RemoveAll(w => w.SKU == combo.SKU1);
                    items.RemoveAll(w => w.SKU == combo.SKU2);
                }
            }

            return totalPrice;
         
        }

        private double GetPrice(string sku)
        {
            switch (sku)
            {
                case "A":
                    return 50;                   
                case "B":
                    return 30;                
                case "C":
                    return 20;                 
                case "D":
                    return 15;
                default:
                    return 0;
                    
            }
        }
    }
}
