﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using team7_ssis.Models;
using team7_ssis.Services;

namespace team7_ssis.Controllers
{
    public class RequisitionController : Controller
    {
        static ApplicationDbContext context = new ApplicationDbContext();
        RequisitionService requisitionService = new RequisitionService(context);
        RetrievalService retrievalService = new RetrievalService(context);
        
        // GET: /Requisition
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Requisiton/RequisitionDetails
        public ActionResult RequisitionDetails()
        {
            return View();
        }
        // GET: /Requisiton/StationeryRetrieval
        [Route("/Requisition/StationeryRetrieval/{id}")]
        [HttpGet]
        public ActionResult StationeryRetrieval(string id)
        {
            // TODO: Remove hardcoded values
            id = "RET-201807-001";
            Retrieval r = retrievalService.FindRetrievalById(id);
            ViewBag.Retrieval = r;
            return View();
        }
        // GET: /Requisiton/StationeryDisbursement
        public ActionResult StationeryDisbursement()
        {
            // TODO: Remove hardcorded values
            ViewBag.Retrieval = "RET-201807-001";
            return View();
        }
        
    }
}