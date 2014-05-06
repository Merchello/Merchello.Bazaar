﻿using System;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using Merchello.Web.Models;
using Merchello.Web.Workflow;
using Moq;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.ItemCache
{
    [TestFixture]
    public class BasketTests : DatabaseIntegrationTestBase
    {
        
        private ICustomerBase _customer;
        private IBasket _basket; 

        [SetUp]
        public void Init()
        {

            var mockGatewayProviderResolver = new Mock<IGatewayProviderResolver>(MockBehavior.Loose);
 
            PreTestDataWorker.DeleteAllItemCaches();

            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(MerchelloContext.Current, _customer);
            

        }


        [Test]
        public void Can_Retrieve_A_Customer_Basket_Mulitple_Times()
        {
            //// Arrange
            
            //// Act
            var basket1 = Basket.GetBasket(MerchelloContext.Current, _customer);
            var basket2 = Basket.GetBasket(MerchelloContext.Current, _customer);
            var basket3 = Basket.GetBasket(MerchelloContext.Current, _customer);
            var basket4 = Basket.GetBasket(MerchelloContext.Current, _customer);


        }

        /// <summary>
        /// Test verifies that a product can be added to a basket
        /// </summary>
        [Test]
        public void Can_Add_A_Product_To_A_Basket()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();

            //// Act
            _basket.AddItem(product.GetProductVariantForPurchase());
            Basket.Save(MerchelloContext.Current, _basket);

            //// Assert
            Assert.IsFalse(_basket.Items.IsEmpty);
        }

        /// <summary>
        /// Test verifies that a products can be added and saved to the basket 
        /// </summary>
        [Test]
        public void Can_Call_Save_Multiple_Times_On_A_Basket()
        {
            //// Arrange
            var product1 = PreTestDataWorker.MakeExistingProduct();
            var product2 = PreTestDataWorker.MakeExistingProduct();
            var product3 = PreTestDataWorker.MakeExistingProduct();

            //// Act
            _basket.AddItem(product1);
            _basket.AddItem(product2);
            Basket.Save(MerchelloContext.Current, _basket);

            _basket.AddItem(product3);
            Basket.Save(MerchelloContext.Current, _basket);
            
            //// Assert
            Assert.IsFalse(_basket.IsEmpty);
            Assert.AreEqual(3, _basket.Items.Count);
        }

        /// <summary>
        /// Test verifies that an existing item can be removed from the basket
        /// </summary>
        [Test]
        public void Can_Remove_An_Existing_Item_From_A_Basket()
        {
            //// Arrange
            var product1 = PreTestDataWorker.MakeExistingProduct();
            var product2 = PreTestDataWorker.MakeExistingProduct();
            var product3 = PreTestDataWorker.MakeExistingProduct();
            _basket.AddItem(product1);
            _basket.AddItem(product2);
            _basket.AddItem(product3);
            Basket.Save(MerchelloContext.Current, _basket);
            Assert.IsTrue(3 == _basket.Items.Count);

            //// Act
            _basket.RemoveItem(product2.Sku);
            Basket.Save(MerchelloContext.Current, _basket);
            _basket = Basket.GetBasket(MerchelloContext.Current, _customer);

            var price = _basket.TotalBasketPrice;
            Console.WriteLine(price);

            //// Assert
            Assert.IsTrue(2 == _basket.Items.Count);
            
        }
    }
}
