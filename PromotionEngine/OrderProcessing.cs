using PromotionEngine.Contracts;
using PromotionEngine.DataAccess.Contracts;
using PromotionEngine.DataAccess.DataModels;
using PromotionEngine.Models;
using System.Collections.Generic;
using System.Linq;


namespace PromotionEngine
{
    public class OrderProcessing : IOrderProcessing   
    {
        private readonly IPromotionProvider promotionProvider;
        private PromotionModel promotionModel;
   

        public OrderProcessing(IPromotionProvider promotionProvider)
        {
            this.promotionProvider = promotionProvider;          
            this.promotionModel = GetPromotion();
        }

        public double GetTotalPrice(List<ItemModel> items)
        {
            if (items != null && items.Count > 0)
            {
                var totalPrice = 0.0;

                //check and Add GroupSave Promotion
                var groupSavePrice = GetGroupSavePromotion(items);
                totalPrice += groupSavePrice;
              
                //check and Add Combo Promotion
                var comboPrice = GetComboPromotion(items);                
                totalPrice += comboPrice;

                //add remaining item prices
                foreach (var item in items)
                {
                    var itemPrice = GetItemPrice(item.SKU);
                    totalPrice += (item.Quantity * itemPrice);  
                }

                return totalPrice;
            }

            return 0;
        }

        private double GetGroupSavePromotion(List<ItemModel> items)
        {
            var totalGroupSavePrice = 0.0;

            foreach (var item in items.ToList())
            {
                var groupItemPrice = 0.0;

                //check if items are eligiblefor groupsave
                if (promotionModel.GroupSavePromotions.Any(x => x.SKU == item.SKU && x.Quantity <= item.Quantity))
                {
                    var groupSavePromotion = promotionModel.GroupSavePromotions.Where(x => x.SKU == item.SKU && x.Quantity <= item.Quantity).FirstOrDefault();
                    groupItemPrice = (item.Quantity / groupSavePromotion.Quantity) * groupSavePromotion.Price + (item.Quantity % groupSavePromotion.Quantity * GetItemPrice(item.SKU));
                }

                if (groupItemPrice != 0)
                {
                    totalGroupSavePrice += groupItemPrice;

                    //Remove item from list to avoid duplicate
                    items.Remove(item);
                }
            }

            return totalGroupSavePrice;
        }

        private double GetComboPromotion(List<ItemModel> items)
        {            
            var totalComboPrice = 0.0;

            foreach (var combo in promotionModel.ComboPromotions)
            {
                var totalNoOfCombo = 0;
                int SKU1Quantity = 0;
                int SKU2Quantity = 0;

                //check if items are eligible combo
                if (items.Any(x => x.SKU == combo.SKU1) && items.Any(x => x.SKU == combo.SKU2))
                {
                    SKU1Quantity = items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity;
                    SKU2Quantity = items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity;

                    if (SKU1Quantity  <= SKU2Quantity)                    
                        totalNoOfCombo = SKU1Quantity;                    
                    else                  
                        totalNoOfCombo = SKU2Quantity;                 

                    //Reduce the quantity of item that are considered for combo
                    items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity -= totalNoOfCombo;
                    items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity -= totalNoOfCombo;

                    //calculate combo save Price and any remaining quantities of both items
                    totalComboPrice += totalNoOfCombo * combo.Price + items.Where(w => w.SKU == combo.SKU1).FirstOrDefault().Quantity * GetItemPrice(combo.SKU1) + items.Where(w => w.SKU == combo.SKU2).FirstOrDefault().Quantity * GetItemPrice(combo.SKU2);
                    
                    //Remove item from list to avoid duplicate
                    items.RemoveAll(w => w.SKU == combo.SKU1);
                    items.RemoveAll(w => w.SKU == combo.SKU2);
                }
            }

            return totalComboPrice;         
        }

        private PromotionModel GetPromotion()
        {
            return promotionProvider.GetPromotion();
        }

        private double GetItemPrice(string sku)
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
