using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PromotionEngine.Contracts;
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
        
        public void Basket_WhenItemsNull_Returns0()
        {
            //Arrange
          
            //Act

            var totalPrice =  orderProcessing.GetTotalPrice(null);

            //Assert

            Assert.AreEqual(0, totalPrice);
           
        }

        [Test]
        public void Basket_WhenItems0_Returns0()
        {
            //Arrange

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(new List<ItemModel>());

            //Assert

            Assert.AreEqual(0, totalPrice);
        }

        [TestCase("A", 1, 50)]
        [TestCase("B", 1, 30)]
        [TestCase("C", 1, 20)]
        [TestCase("D", 1, 15)]
        [TestCase("A", 2, 100)]
        [TestCase("C", 2, 40)]
        [TestCase("D", 2, 30)]
        public void Basket_WhenSingleItem_ReturnsResult(string SKU, int quantity, double result)
        {
            //Arrange
          
            List<ItemModel> items = new List<ItemModel> 
            { 
                new ItemModel { SKU = SKU, Quantity = quantity }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(result, totalPrice);
        }  
  

        //multiple items without Promotion
        [Test]
        public void Basket_WhenMultiple_ReturnsResult()
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel> 
            {
                new ItemModel { SKU = "D", Quantity = 2 },
                new ItemModel { SKU = "A", Quantity = 5 }
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
            List<ItemModel> items = new List<ItemModel> 
            { 
                new ItemModel { SKU = "A", Quantity = 2 },
                new ItemModel { SKU = "C", Quantity = 1 }
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
        public void Basket_WhenGroupSave_ReturnsExpectedResult(string SKU, int quantity, double result)
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel>
            {
                new ItemModel { SKU = SKU, Quantity = quantity }
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
            List<ItemModel> items = new List<ItemModel> 
            {
                new ItemModel { SKU = "B", Quantity = 5 },
                new ItemModel { SKU = "A",Quantity=5 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(350, totalPrice);
        }

        //ComboPromotion     
        [TestCase("C", 1,"D", 1, 30)]
        [TestCase("C", 2, "D", 1, 50)]
        public void Basket_WhenCombo_ReturnsExpectedResult(string SKU1, int quantity1, string SKU2, int quantity2, double result)
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel> 
            { 
                new ItemModel { SKU = SKU1, Quantity = quantity1 },
                new ItemModel{ SKU = SKU2,Quantity=quantity2} 
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(result, totalPrice);
        }  

        //Multiple promotion 
        [Test]
        public void Basket_WhenMultiple_ReturnResult()
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel> 
            { 
                new ItemModel { SKU = "C", Quantity = 2 },
                new ItemModel{ SKU = "D",Quantity=1}, 
                new ItemModel { SKU = "A", Quantity = 4 } ,
                new ItemModel { SKU = "B", Quantity = 3 } 
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(50+130+50+45+30, totalPrice);
        }

        //CodingTestScenario A
        [Test]
        public void Basket_WhenScenario_A_Return100()
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel>
            {
                new ItemModel { SKU = "A", Quantity = 1 },
                new ItemModel{ SKU = "B",Quantity=1 },
                new ItemModel { SKU = "C", Quantity = 1 }                 
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(100, totalPrice);
        }

        //CodingTestScenario B
        [Test]
        public void Basket_WhenScenario_B_Returns370()
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel>
            {
                new ItemModel { SKU = "A", Quantity = 5 },
                new ItemModel{ SKU = "B",Quantity= 5},
                new ItemModel { SKU = "C", Quantity = 1 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(370, totalPrice);

        }

        //CodingTestScenario C
        [Test]
        public void Basket_WhenScenario_C_Returns280()
        {
            //Arrange
            List<ItemModel> items = new List<ItemModel>
            {
                new ItemModel { SKU = "A", Quantity = 3 },
                new ItemModel{ SKU = "B",Quantity= 5 },
                new ItemModel { SKU = "C", Quantity = 1 },
                new ItemModel { SKU = "D", Quantity = 1 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(280, totalPrice);
        }
    }
}