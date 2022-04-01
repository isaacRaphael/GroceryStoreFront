﻿using GroceryStoreWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Core;
using Store.Core.Models;
using Store.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        private readonly ICartService _cartService;
        private readonly IAddProdToDbService _addProdToDbService;
        private readonly IStore _store;
        private readonly CartItem _cartItem;


        public HomeController(ILogger<HomeController> logger,
            ILoginService loginService,
            ICartService cartService, 
            IStore store,
            IAddProdToDbService addProdToDbService)
        {
            _logger = logger;
            _loginService = loginService;
            _store = store;
            _cartService = cartService;
            _cartItem = new CartItem() { Products = _store.Products, User =_store.User };
            _addProdToDbService = addProdToDbService;
           
        }


        //Get Index
        public IActionResult Index()
        {
            return View();
        }
        
        
        //CreateProd Get
        public IActionResult CreateProd()
        {
            return View();
        }

        //CreateProd Post
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreateProd(CartItem item)
        {
            var prod = new Product(item.Name)
            {
                Quantity = item.Quantity,
                Price = item.Price
            };
            _addProdToDbService.AddProdToDb(prod.Id, prod.Name, prod.Price, prod.Quantity);
            return RedirectToAction("StoreFront");
        }

        public IActionResult CartItems()
        {
            var CartList = new List<CartItem>();
            var Items = _cartService.GetCart();
            foreach(var item in Items)
            {
                CartList.Add(new CartItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }
            return View(CartList);
        }



        //Post Login
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var checker = _loginService.ValidateLogIn(model.UserName, model.PassWord);
                if (checker)
                {
                    _store.User = new User(model.UserName);

                    return RedirectToAction("StoreFront", _cartItem);
                }
            }
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CartAdd(CartItem cartItem)
        {
            var pass = new CartDTO()
            {
                Id = cartItem.Id,
                Name = cartItem.Name,
                Price = cartItem.Price,
                Quantity = cartItem.Quantity
            };

            if (ModelState.IsValid)
                _cartService.Add(pass);

            return RedirectToAction("StoreFront", _cartItem);
        }

        public IActionResult Back()
        {
            _cartService.ClearCart();
            return RedirectToAction("StoreFront");
        }
        public IActionResult StoreFront()
        {
            return View(_cartItem);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
