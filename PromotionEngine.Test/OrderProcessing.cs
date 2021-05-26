using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

using PromotionEngine.DataAccess.Contracts;
using PromotionEngine.DataAccess.DataModels;
using PromotionEngine.Models;
using System.Collections.Generic;
using System.IO;

namespace PromotionEngine.Test
{
    [TestFixture]
    public class Tests
    {
        Mock<IPromotionProvider> mockPromotionProvider= null;
        private OrderProcessing orderProcessing = null;
        private PromotionModel promotionModel = null;


        [SetUp]
        public void Setup()
        {
            mockPromotionProvider = new Mock<IPromotionProvider>();
            using (StreamReader r = new StreamReader("DataJson\\Promotions.json"))
            {
                string json = r.ReadToEnd();
                promotionModel = JsonConvert.DeserializeObject<PromotionModel>(json);
            }
       
            mockPromotionProvider.Setup(m => m.GetPromotion()).Returns(promotionModel);
            orderProcessing = new OrderProcessing(mockPromotionProvider.Object);
        }

        [TearDown]
        public void TestCleanUp()
        {
            mockPromotionProvider = null;
            orderProcessing = null;
            promotionModel = null;
        }

        [Test]
        
        public void Basket_WhenProducts0_Returns0()
        {
            //Arrange
          
            //Act

            var totalPrice =  orderProcessing.GetTotalPrice(null);

            //Assert

            Assert.AreEqual(0, totalPrice);
           
        }
      
        [TestCase("A",1,50)]
        [TestCase("B", 1, 30)]
        [TestCase("C", 1, 20)]
        [TestCase("D", 1, 15)]
        [TestCase("A", 2, 100)]
        [TestCase("C", 2, 40)]
        [TestCase("D", 2, 30)]
        public void Basket_WhenProductIsA_Returns50(string SKU, int quantity, double result)
        {
            //Arrange
          
            List<Item> items = new List<Item> 
            { 
                new Item { SKU = SKU, Quantity = quantity }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(result, totalPrice);
        }  
  

        //multiple items without Promotion
        [Test]
        public void Basket_WhenProductIsMultiple_ReturnsResult()
        {
            //Arrange
            List<Item> items = new List<Item> 
            {
                new Item { SKU = "D", Quantity = 2 },
                new Item { SKU = "A", Quantity = 5 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(260, totalPrice);
        }

        [Test]
        public void Basket_WhenProductIsMultiple2_ReturnsResult()
        {
            //Arrange
            List<Item> items = new List<Item> 
            { 
                new Item { SKU = "A", Quantity = 2 },
                new Item { SKU = "C", Quantity = 1 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(120, totalPrice);

        }

        // Group Save Promotions
       
        [TestCase("A", 3, 130)]
        [TestCase("B", 2, 45)]
        [TestCase("B", 5, 120)]
        public void Basket_WhenProductIs3A_Returns130(string SKU, int quantity, double result)
        {
            //Arrange
            List<Item> items = new List<Item>
            {
                new Item { SKU = SKU, Quantity = quantity }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(result, totalPrice);
        }

        [Test]
        public void Basket_WhenMultiGroupSave_Returns()
        {
            //Arrange
            List<Item> items = new List<Item> 
            {
                new Item { SKU = "B", Quantity = 5 },
                new Item { SKU = "A",Quantity=5 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(350, totalPrice);
        }

        //ComboPromotion     
        [TestCase("C", 1,"D", 1, 30)]
        [TestCase("C", 2, "D", 1, 50)]
        public void Basket_WhenDefault_ReturnsResult(string SKU1, int quantity1, string SKU2, int quantity2, double result)
        {
            //Arrange
            List<Item> items = new List<Item> 
            { 
                new Item { SKU = SKU1, Quantity = quantity1 },
                new Item{ SKU = SKU2,Quantity=quantity2} 
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(result, totalPrice);

        }  

        [Test]
        public void Basket_WhenMultiple_ReturnResult()
        {
            //Arrange
            List<Item> items = new List<Item> 
            { 
                new Item { SKU = "C", Quantity = 2 },
                new Item{ SKU = "D",Quantity=1}, 
                new Item { SKU = "A", Quantity = 4 } ,
                new Item { SKU = "B", Quantity = 3 } 
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(50+130+50+45+30, totalPrice);

        }

    }
}