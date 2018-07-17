﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using team7_ssis.Models;

namespace team7_ssis.Repositories
{
    public class NotificationRepository : CrudRepository<Notification, int>
    {
        public NotificationRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.entity = context.Notification;
        }
    }
}