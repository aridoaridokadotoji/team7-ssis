﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team7_ssis.Models;

namespace team7_ssis.Repositories
{
    public class ItemPriceRepository : CrudMultiKeyRepository<ItemPrice, String, String>
    {
        public ItemPriceRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.entity = context.ItemPrice;
        }
    }
}
