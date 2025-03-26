namespace Cinema_System.Areas.Request
{
    public class SeatSelectedRequest
    {
        public int showTimeSeatId { get; set; }  // Mã đơn hàng từ PayOS
        public string nameSeat { get; set; }  // Trạng thái thanh toán (PAID, FAILED, PENDING, ...)
    }
}
