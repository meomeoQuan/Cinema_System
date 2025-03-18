namespace Cinema_System.Areas.Request
{
    public class PaymentWebhookRequest
    {
        public int OrderCode { get; set; }  // Mã đơn hàng từ PayOS
        public string Status { get; set; }  // Trạng thái thanh toán (PAID, FAILED, PENDING, ...)
        public string TransactionId { get; set; }  // Mã giao dịch từ PayOS
    }

}
