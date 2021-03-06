﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RmlOnlineShop.Application.DataServices;
using RmlOnlineShop.Application.DataServices.Interfaces;
using RmlOnlineShop.Data.Models;
using RmlOnlineShop.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using RmlOnlineShop.Application.SessionModels;
using RmlOnlineShop.Extensions;
using RmlOnlineShop.Application.LogicServices.Interfaces;
using Newtonsoft.Json;

namespace RmlOnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> loggerProductsController;
        private readonly IProductManager productManager;
        private readonly ICartLogic cartLogic;
        private readonly IClientLogic clientLogic;

        public ProductsController
            (
                ILogger<ProductsController> loggerProductsController,
                IProductManager productManager,
                ICartLogic cartLogic,
                IClientLogic clientLogic
            )
        {
            this.loggerProductsController = loggerProductsController;
            this.productManager = productManager;
            this.cartLogic = cartLogic;
            this.clientLogic = clientLogic;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(productManager.GetAllProductsAsViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Product(int id)
        {
            var product = await productManager.GetProductViewModelById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ProductToCart(ProductCart productCart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var res = await cartLogic.AddToCart(HttpContext.Session, productCart);
            if (!res)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var productsInCartViewModel = cartLogic.GetProductInCartAsViewModel(HttpContext.Session);

            
            return View(productsInCartViewModel);
        }

        [HttpGet]
        public IActionResult Conditions()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ClientCheckout()
        {
            var ProductsAndClientOrderInfo = clientLogic.GetProductsAndOrderInfo(HttpContext.Session);

            if (ProductsAndClientOrderInfo == null)
            {
                return RedirectToAction("Index", "Products");
            }

            return View(ProductsAndClientOrderInfo);
        }

        [HttpPost]
        public IActionResult ClientCheckout(ClientOrderInformatiomViewModel clientOrderInformatiomViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            
            clientLogic.SaveClientOrderInfo(HttpContext.Session, clientOrderInformatiomViewModel);
           

            return RedirectToAction("Payment", "PaymentStripe");
        }

        public IActionResult Payment()
        {
            var clientOrderInfo = clientLogic.GetClientOrderInfo(HttpContext.Session);
            if (clientOrderInfo==null)
            {
                return RedirectToAction("ClientCheckout");
            }


            return View();
        }

        public IActionResult Order(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return NotFound(orderId);

            }

            var order = clientLogic.GetOrderByUniqueId(orderId);

            if (order==null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
