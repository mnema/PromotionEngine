using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PromotionEngine2.Contracts;
using PromotionEngine2.DataAccess.Contracts;
using PromotionEngine2.DataAccess.DataModels;
using PromotionEngine2.Models;
using System.Collections.Generic;
using System.IO;

namespace PromotionEngine2.Test
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

        [Test]
        public void Basket_WhenProductIsA_Returns50()
        {
            //Arrange
          
            List<Item> items = new List<Item> { new Item { SKU = "A", Quantity = 1 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(50, totalPrice);

        }

        [Test]
        public void Basket_WhenProductIsB_Returns30()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "B", Quantity = 1 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(30, totalPrice);

        }


        [Test]
        public void Basket_WhenProductIsC_Returns20()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "C", Quantity = 1 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(20, totalPrice);

        }

        [Test]
        public void Basket_WhenProductIsD_Returns15()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "D", Quantity = 1 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(15, totalPrice);

        }

        // N quantity
        [Test]
        public void Basket_WhenProductIs2A_Returns100()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "A", Quantity = 2 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(100, totalPrice);

        }

 

        [Test]
        public void Basket_WhenProductIs2C_Returns40()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "C", Quantity = 2 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(40, totalPrice);

        }

        [Test]
        public void Basket_WhenProductIs2D_Returns30()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "D", Quantity = 2 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(30, totalPrice);

        }

        //multiple items
        [Test]
        public void Basket_WhenProductIsMultiple_ReturnsResult()
        {

            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "D", Quantity = 2 },
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
            List<Item> items = new List<Item> { new Item { SKU = "A", Quantity = 2 },
                new Item { SKU = "C", Quantity = 1 }
            };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(120, totalPrice);

        }

        //Promotions
        [Test]
        public void Basket_WhenProductIs3A_Returns130()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "A", Quantity = 3 }};

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(130, totalPrice);

        }

        [Test]
        public void Basket_WhenProductIs2B_Returns45()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "B", Quantity = 2 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(45, totalPrice);

        }

        [Test]
        public void Basket_WhenProductIs2Bmultiple_Returns()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "B", Quantity = 5 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(120, totalPrice);
        }

        [Test]
        public void Basket_WhenProductIs2B5A_Returns()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "B", Quantity = 5 },
            new Item{ SKU = "A",Quantity=5} };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(120+230, totalPrice);

        }

        //ComboPromotion
        [Test]
        public void Basket_WhenProduct1C1D_Returns30()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "C", Quantity = 1 },
            new Item{ SKU = "D",Quantity=1} };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(30, totalPrice);

        }

        [Test]
        public void Basket_WhenProduct2C1D_Returns30()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "C", Quantity = 2 },
            new Item{ SKU = "D",Quantity=1} };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(50, totalPrice);

        }

        [Test]
        public void Basket_WhenProductGeneric()
        {
            //Arrange
            List<Item> items = new List<Item> { new Item { SKU = "C", Quantity = 2 },
            new Item{ SKU = "D",Quantity=1}, new Item { SKU = "A", Quantity = 4 } , new Item { SKU = "B", Quantity = 3 } };

            //Act

            var totalPrice = orderProcessing.GetTotalPrice(items);

            //Assert

            Assert.AreEqual(50+130+50+45+30, totalPrice);

        }

    }
}