﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using team7_ssis.Models;
using team7_ssis.Repositories;

namespace team7_ssis.Services
{
    public class NotificationService
    {
        ApplicationDbContext context;
        NotificationRepository notificationRepository;
        NotificationTypeRepository notificationTypeRepository;
        StatusRepository statusRepository;

        public NotificationService(ApplicationDbContext context)
        {
            this.context = context;
            notificationRepository = new NotificationRepository(context);
            notificationTypeRepository = new NotificationTypeRepository(context);
            statusRepository = new StatusRepository(context);
        }

        public Notification ReadNotification(int notificationId)
        {
            var notification = FindNotificationById(notificationId);

            if (notification.Status.StatusId == 15)
                throw new ArgumentException("Notification already read");

            notification.Status = statusRepository.FindById(15);

            return Save(notification);
        }

        public Notification FindNotificationById(int notificationId)
        {
            return notificationRepository.FindById(notificationId);
        }

        private Notification InstantiateNotification(ApplicationUser recipient)
        {
            //instantiate new notification object and populating the fields
            return new Notification {
                NotificationId = IdService.GetNewNotificationId(context),
                CreatedDateTime = DateTime.Now,
                CreatedFor = recipient,

                Status = statusRepository.FindById(14)
            };

        }

        public Notification CreateUnableToFulFillNotification(Retrieval retrieval, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);
            var requisitionRepository = new RequisitionRepository(context);

            notification.NotificationType = notificationTypeRepository.FindById(8);
            notification.Contents = String.Format("Your recent requisitions could not be fulfilled due to low stock count. We have automatically generated a new Requisition with ID: {0} on your behalf for the next retrieval cycle.",
                requisitionRepository.FindAll()
                .Where(r => r.RequisitionId.StartsWith("SRQ")
                    && r.EmployeeRemarks.Contains(retrieval.RetrievalId)
                    && r.Department.DepartmentCode == recipient.Department.DepartmentCode)
                .FirstOrDefault().RequisitionId);

            return this.Save(notification);
        }

        public Notification CreateUnableToFulfillNotification(Disbursement disbursement, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);
            var requisitionRepository = new RequisitionRepository(context);

            notification.NotificationType = notificationTypeRepository.FindById(8);
            notification.Contents = String.Format("Your recent requisitions could not be fulfilled due to low stock count. We have automatically generated a new Requisition with ID: {0} on your behalf for the next retrieval cycle.",
                requisitionRepository.FindAll()
                .Where(r => r.RequisitionId.StartsWith("SRQ")
                    && r.EmployeeRemarks.Contains(disbursement.Retrieval.RetrievalId)
                    && r.Department.DepartmentCode == disbursement.Department.DepartmentCode)
                .FirstOrDefault().RequisitionId);

            return this.Save(notification);
        }

        public Notification CreateNotification(Disbursement disbursement, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);
            var requisitionRepository = new RequisitionRepository(context);

            // If disbursement has no actual quantity retrieved
            // Send Unable to Fulfill notification
            if (disbursement.DisbursementDetails.All(dd => dd.ActualQuantity == 0))
            {
                return CreateUnableToFulfillNotification(disbursement, disbursement.Department.Representative);
            }

            // If confirm retrieval produces an autogenerated requisition
            // Send partial collection notification
            if (requisitionRepository.FindAll()
                .Where(r => r.RequisitionId.StartsWith("SRQ") 
                    && r.EmployeeRemarks.Contains(disbursement.Retrieval.RetrievalId)
                    && r.Department.DepartmentCode == disbursement.Department.DepartmentCode)
                .Count() == 1)
            {
                notification.NotificationType = notificationTypeRepository.FindById(7);
                notification.Contents = String.Format("Your Disbursement with ID: {0} is ready for collection. Please note that your order was only partially fulfilled. We will notify you when the next collection is ready.", disbursement.DisbursementId);
                return this.Save(notification);
            }

            notification.NotificationType = notificationTypeRepository.FindById(1);
            notification.Contents = String.Format("Your Disbursement with ID: {0} is ready for collection", disbursement.DisbursementId);
            return this.Save(notification);

        }

        public Notification CreateNotification(Requisition requisition, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);
           
            notification.NotificationType = notificationTypeRepository.FindById(2);
            notification.Contents = String.Format("New Stationery Requisition Request: {0} is pending approval", requisition.RequisitionId);
            return this.Save(notification);
        }

        public Notification CreateNotification(StockAdjustment SA, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);

            notification.NotificationType = notificationTypeRepository.FindById(3);
            notification.Contents = String.Format("New Stock Adjustment Request: {0} is pending your approval", SA.StockAdjustmentId);

            return this.Save(notification);
        }

        public Notification CreateChangeCollectionPointNotification(Department department, ApplicationUser recipient)
        {
            Notification notification = InstantiateNotification(recipient);

            notification.NotificationType = notificationTypeRepository.FindById(6);
            notification.Contents = $"{department.Name} Collection Point changed to {department.CollectionPoint.Name}";

            return this.Save(notification);
        }
       
        public List<Notification> FindNotificationsByUser(ApplicationUser user)
        {
            if (user == null) return new List<Notification>();

            return notificationRepository.FindByUser(user).ToList();
        }

        public List<Notification> FindNotificationByType(NotificationType type)
        {
            return notificationRepository.FindByType(type).ToList();

        }

        public Notification Save(Notification notification)
        {
            return notificationRepository.Save(notification);
        }

        public ApplicationUser GetCreatedFor(string stockadjustmentid )
        {
            if(notificationRepository.FindAll().Where(x => x.Contents.Contains(stockadjustmentid)).First() ==null)
            {
                return null;
            }
            else
            {
                Notification n = notificationRepository.FindAll().Where(x => x.Contents.Contains(stockadjustmentid)).First();
                return n.CreatedFor;
            }
         
        }

        public ApplicationUser GetCreatedForRequisition(string reqId)
        {
            if (notificationRepository.FindAll().Where(x => x.Contents.Contains(reqId)).First() == null)
            {
                return null;
            }
            else
            {
                Notification n = notificationRepository.FindAll().Where(x => x.Contents.Contains(reqId)).First();
                return n.CreatedFor;
            }

        }


    }
}