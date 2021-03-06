﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team7_ssis.Models;

namespace team7_ssis.Repositories
{
    public class PurchaseOrderRepository : CrudRepository<PurchaseOrder, String>
    {
        public PurchaseOrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.entity = context.PurchaseOrder;
        }

        /// <summary>
        /// Find PurchaseOrder objects that match start and end date range inclusive by CreatedDateTime
        /// </summary>
        /// <param name="startDateRange"></param>
        /// <param name="endDateRange"></param>
        /// <returns>IQueryable of PurchaseOrder objects</returns>
        public IQueryable<PurchaseOrder> FindByCreatedDateTime(DateTime startDateRange, DateTime endDateRange)
        {
            return context.PurchaseOrder
                .Where(x => x.CreatedDateTime.CompareTo(startDateRange) >= 0 &&
                    x.CreatedDateTime.CompareTo(endDateRange) <= 0);
        }


        public IQueryable<PurchaseOrder> FindBySupplier(string supplierCode)
        {
            return context.PurchaseOrder
                .Where(x => x.SupplierCode == supplierCode);
        }

        public IQueryable<PurchaseOrder> FindByStatus(params int[] statusId)
        {
            return context.PurchaseOrder.Where(x => statusId.Contains(x.Status.StatusId));
        }

        public IQueryable<PurchaseOrderDetail> FindPurchaseOrderDetailByIdStatus(string purchaseOrderNumber,params int[] statusId)
        {
            return context.PurchaseOrderDetail.Where(x => (statusId.Contains(x.Status.StatusId) && purchaseOrderNumber.Contains(x.PurchaseOrderNo)));
        }









    }
}
