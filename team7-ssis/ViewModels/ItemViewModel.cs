﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using team7_ssis.Models;

namespace team7_ssis.ViewModels
{
    public class ItemViewModel
    {
        public string ItemCode { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public string Description { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string Uom { get; set; }
        public Inventory Inventory { get; set; }
    }
}