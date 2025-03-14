using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.Shop.AppCode;

namespace WebsiteShop.Shop.Controllers
{
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        private const string TRACKED_ORDERS = "TrackedOrders";

        public List<Models.CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<Models.CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<Models.CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        [HttpPost]
        public IActionResult AddToCart(Models.CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ.");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ!" });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCart = GetShoppingCart();
            var item = shoppingCart.FirstOrDefault(m => m.ProductID == id);  // Sử dụng FirstOrDefault thay vì FindIndex
            if (item != null)
            {
                shoppingCart.Remove(item); // Xóa sản phẩm khỏi giỏ hàng
            }

            // Lưu lại giỏ hàng đã cập nhật vào session
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            // Trả về phản hồi để thông báo đã xóa thành công
            return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng!" });
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng không hợp lệ." });
            }

            var shoppingCart = GetShoppingCart();
            var item = shoppingCart.FirstOrDefault(m => m.ProductID == productId);

            if (item == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            // Cập nhật số lượng sản phẩm
            item.Quantity = quantity;
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            // Tính tổng giá trị mới của sản phẩm
            var totalItemPrice = item.Quantity * item.SalePrice;

            return Json(new { success = true, totalItemPrice });
        }

        public IActionResult ShoppingCart()
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                ViewBag.CartMessage = "Giỏ hàng của bạn đang trống!";
            }
            return View(shoppingCart);
        }


        /// <summary>
        /// Phương thức theo dõi đơn hàng
        /// </summary>
        /// <summary>
        /// Lấy danh sách các đơn hàng đã lưu vết trong Session.
        /// </summary>
        private List<int> GetTrackedOrders()
        {
            return ApplicationContext.GetSessionData<List<int>>(TRACKED_ORDERS) ?? new List<int>();
        }

        /// <summary>
        /// Lưu danh sách các đơn hàng đã lưu vết vào Session.
        /// </summary>
        private void SetTrackedOrders(List<int> orders)
        {
            ApplicationContext.SetSessionData(TRACKED_ORDERS, orders);
        }

        /// <summary>
        /// Thêm đơn hàng vào danh sách lưu vết.
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        private void AddTrackedOrder(int orderId)
        {
            var trackedOrders = GetTrackedOrders();
            if (!trackedOrders.Contains(orderId))
            {
                trackedOrders.Add(orderId);
                SetTrackedOrders(trackedOrders);
            }
        }

        public IActionResult Tracking(int? orderId)
        {
            // Lấy danh sách đơn hàng đã lưu vết từ Session
            var trackedOrders = ApplicationContext.GetSessionData<List<Models.OrderTrackingViewModel>>(TRACKED_ORDERS)
                                ?? new List<Models.OrderTrackingViewModel>();

            // Nếu có orderId, thêm đơn hàng vào danh sách lưu vết
            if (orderId.HasValue)
            {
                var order = OrderDataService.GetOrder(orderId.Value);
                if (order != null && !trackedOrders.Any(o => o.OrderID == order.OrderID))
                {
                    var orderDetails = OrderDataService.ListOrderDetails(orderId.Value);

                    var newOrder = new Models.OrderTrackingViewModel
                    {
                        OrderID = order.OrderID,
                        OrderTime = order.OrderTime,
                        Status = order.Status,
                        DeliveryProvince = order.DeliveryProvince,
                        DeliveryAddress = order.DeliveryAddress,
                        OrderDetails = orderDetails.Select(od => new Models.OrderItemViewModel
                        {
                            ItemName = od.ProductName,
                            Quantity = od.Quantity,
                            SalePrice = od.SalePrice,
                            TotalPrice = od.Quantity * od.SalePrice
                        }).ToList(),
                        TotalPrice = orderDetails.Sum(od => od.Quantity * od.SalePrice)
                    };

                    trackedOrders.Add(newOrder);
                }
            }

            // Cập nhật lại Session với danh sách mới
            ApplicationContext.SetSessionData(TRACKED_ORDERS, trackedOrders);

            // Truyền danh sách đơn hàng vào View
            ViewBag.TrackedOrders = trackedOrders;

            return View();
        }




    }
}
