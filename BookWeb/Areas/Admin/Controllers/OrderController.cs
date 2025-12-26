using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Models.ViewModels;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? orderId)
        {
             orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            if (orderVM.OrderHeader == null)
            {
                return NotFound();
            }

            // ensure OrderDetail is at least an empty list to prevent view null refs
            if (orderVM.OrderDetail == null)
            {
                orderVM.OrderDetail = new List<OrderDetail>();
            }

            return View(orderVM);

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest();
            }

            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);
            if (orderHeaderFromDb == null)
            {
                return NotFound();
            }

            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest();
            }

            var id = orderVM.OrderHeader.Id;
            if (id == 0)
            {
                return BadRequest();
            }

            _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated to In Process.";
            return RedirectToAction(nameof(Details), new { orderId = id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest();
            }

            var id = orderVM.OrderHeader.Id;
            if (id == 0)
            {
                return BadRequest();
            }

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            // update tracking and carrier from posted values
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            // mark as shipped
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest();
            }

            var id = orderVM.OrderHeader.Id;
            if (id == 0)
            {
                return BadRequest();
            }

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            // If payment was already approved (charged), attempt refund
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved && !string.IsNullOrEmpty(orderHeader.PaymentIntentId))
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = orderHeader.PaymentIntentId,
                    Reason = RefundReasons.RequestedByCustomer
                };

                var service = new RefundService();
                var refund = service.Create(options);

                orderHeader.OrderStatus = SD.StatusRefunded;
                orderHeader.PaymentStatus = SD.PaymentStatusRejected; // mark payment as handled
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["success"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(Details), new { orderId = id });
        }


        [ActionName("Details")]
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult PayNow(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest();
            }

            var id = orderVM.OrderHeader.Id;
            if (id == 0)
            {
                return BadRequest();
            }

            // stripe settings - generate absolute URLs based on current request so host/port are correct
            var successUrl = Url.Action("PaymentConfirmation", "Order", new { area = "Admin", orderHeaderId = id }, Request.Scheme);
            var cancelUrl = Url.Action("Details", "Order", new { area = "Admin", id = id }, Request.Scheme);

            var options = new SessionCreateOptions
            {
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id);
            foreach (var item in orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // in cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product?.Title ?? "Product"
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            var session = service.Create(options);

            // persist session info using repository helper
            _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader == null)
            {
                return NotFound();
            }

            // Only check Stripe session when payment status is not already approved
            if (orderHeader.PaymentStatus != SD.PaymentStatusApproved)
            {
                if (!string.IsNullOrEmpty(orderHeader.SessionId))
                {
                    var service = new SessionService();
                    var session = service.Get(orderHeader.SessionId);
                    if (session != null && string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
                    {
                        // update stripe ids and mark payment approved
                        _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                        _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusApproved, SD.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }
                }
            }

            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = Enumerable.Empty<OrderHeader>();

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                // safer retrieval of user id
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { data = objOrderHeaders });
                }

                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
            }





                switch (status)
                {
                    case "pending":
                        {
                            objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                            break;
                        }
                    case "inprocess":
                        {
                            objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                            break;
                        }
                    case "completed":
                        {
                            objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                            break;
                        }
                    case "approved":
                        {
                            objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                            break;
                        }
                    default:
                        {

                            break;
                        }

                }
            return Json(new { data = objOrderHeaders });

        }



        #endregion


    }
}
