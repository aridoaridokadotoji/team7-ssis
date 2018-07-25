﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using team7_ssis.Models;
using team7_ssis.Services;
using team7_ssis.ViewModels;

namespace team7_ssis.Controllers
{
    public class SupplierController : Controller
    {
        ApplicationDbContext context;
        SupplierService supplierService;
        StatusService statusService;
        UserService userService;
        ItemPriceService itempriceService;
        public SupplierController()
        {
            context = new ApplicationDbContext();
            supplierService = new SupplierService(context);
            statusService = new StatusService(context);
            userService = new UserService(context);
            itempriceService = new ItemPriceService(context);

        }
        // GET: Supplier
        public ActionResult Index()
        {
            List<Status> list = new List<Status>();
            list.Add(statusService.FindStatusByStatusId(0));
            list.Add(statusService.FindStatusByStatusId(1));
            return View(new SupplierViewModel
            {
                Statuses = new SelectList(
                    list.Select(x => new { Value = x.StatusId, Text = x.Name }),
                     "Value",
                    "Text"

                )
            });
        }


        //Save new or update existing supplier
        [HttpPost]
        public ActionResult Save(SupplierViewModel model)
        {
            bool status = false;
            Supplier s = new Supplier();

            if (supplierService.FindSupplierById(model.SupplierCode) == null)
            {
                //new supplier 
                s.SupplierCode = model.SupplierCode;
                //assign user info
                s.CreatedDateTime = DateTime.Now;
                s.CreatedBy = userService.FindUserByEmail(System.Web.HttpContext.Current.User.Identity.GetUserName());

            }

            else
            {
                //existing supplier
                s= supplierService.FindSupplierById(model.SupplierCode);

                //assign user info into update fields
                s.UpdatedDateTime = DateTime.Now;
                s.UpdatedBy = userService.FindUserByEmail(System.Web.HttpContext.Current.User.Identity.GetUserName());

            }

            //assign supplier info
            s.Name = model.Name;
            s.ContactName = model.ContactName;
            s.PhoneNumber = model.PhoneNumber;
            s.FaxNumber = model.PhoneNumber;
            s.Address = model.Address;
            s.GstRegistrationNo = model.GSTNumber;
            s.Status = statusService.FindStatusByStatusId(model.Status);

            //save info to database
            if (supplierService.Save(s) !=null) status = true;

            //return RedirectToAction("Index", "Supplier");
            return new JsonResult { Data = new { status = status } };
        }

        
        [HttpGet]
        public ActionResult SupplierPriceList(string id)
        {
            Supplier supplier = supplierService.FindSupplierById(id);
            //assemble view model

            return View(new SupplierViewModel {
                SupplierCode = supplier.SupplierCode,
                Name = supplier.Name,
                ContactName = supplier.ContactName

            });
        }

        [HttpPost]
        public ActionResult UpdateItemPrice(ItemPriceViewModel model)
        {
            bool status = false;

            //find item price object and assign new price
            ItemPrice itemPrice = itempriceService.FindItemPriceByItemCode(model.ItemCode).Where(x=>x.SupplierCode==model.SupplierCode).First();
            itemPrice.Price = model.Price;

            //assign user info into update fields
            itemPrice.UpdatedDateTime = DateTime.Now;
            itemPrice.UpdatedBy = userService.FindUserByEmail(System.Web.HttpContext.Current.User.Identity.GetUserName());

            if (itempriceService.Save(itemPrice)!= null) status = true;

            return new JsonResult { Data = new { status = status } };
        }
   
    }
}
