namespace Cinema_System.Areas.Request
{
    public class ShowTimeSearchRequest
    {
        public int showTimeId { get; set; }  // ✅ Mã đơn hàng
        public List<int> seatIds { get; set; }  // ✅ Tổng tiền
    }
}
