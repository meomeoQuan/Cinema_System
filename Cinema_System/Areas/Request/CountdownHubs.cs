// Thêm các using này vào đầu file
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.Models;
using Cinema_System.Areas.Guest.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema_System.Areas.Request
{
    public class CountdownHub : Hub
    {
        private class ConnectionState
        {
            public int CountdownTime { get; set; } = 30; // 30 giây
            public HashSet<int> SelectedSeats { get; } = new HashSet<int>();
            public CancellationTokenSource TimerTokenSource { get; set; }
        }

        private static readonly ConcurrentDictionary<string, ConnectionState> _connectionStates = new ConcurrentDictionary<string, ConnectionState>();

        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<CountdownHub> _hubContext;

        public CountdownHub(IServiceProvider serviceProvider, IHubContext<CountdownHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        // === BỎ HÀM StartCountdown() ===
        // public async Task StartCountdown() { ... } // << XÓA HÀM NÀY

        private async Task RunCountdownTimer(string connectionId, ConnectionState state, CancellationToken token)
        {
            try
            {
                while (state.CountdownTime > 0)
                {
                    if (token.IsCancellationRequested) break;

                    // Gửi thời gian về cho CHỈ CLIENT GỌI
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveCountdown", state.CountdownTime);
                    await Task.Delay(1000, token);
                    state.CountdownTime--;
                }

                if (!token.IsCancellationRequested)
                {
                    // Timer kết thúc bình thường
                    await _hubContext.Clients.Client(connectionId).SendAsync("CountdownFinished", state.SelectedSeats);

                    // Lấy danh sách ghế để giải phóng
                    HashSet<int> seatsToRelease;
                    lock (state.SelectedSeats) // Khóa lại để đọc
                    {
                        seatsToRelease = new HashSet<int>(state.SelectedSeats); // Tạo bản copy
                        state.SelectedSeats.Clear(); // Xóa ngay lập tức
                    }
                    await ReleaseSeats(seatsToRelease); // Giải phóng bản copy
                }
            }
            catch (TaskCanceledException) { /* Bị hủy, không làm gì */ }
            finally
            {
                // Luôn xóa trạng thái khi timer kết thúc/hủy
                _connectionStates.TryRemove(connectionId, out _);
            }
        }

        // === LOGIC SỬA ĐỔI QUAN TRỌNG NHẤT ===
        public async Task SelectSeat(int seatId)
        {
            var connectionId = Context.ConnectionId;

            // 1. Tự động Lấy state cũ HOẶC Tạo Mới state nếu chưa có
            // Đây là hàm "nguyên tử", đảm bảo không bị lỗi tranh chấp
            var state = _connectionStates.GetOrAdd(connectionId, (id) => new ConnectionState());

            bool startTimer = false;
            bool added = false;

            // 2. Khóa (lock) state lại để sửa đổi
            lock (state)
            {
                // 3. Thêm ghế vào danh sách
                added = state.SelectedSeats.Add(seatId);

                // 4. KIỂM TRA: Nếu timer chưa chạy (lần đầu) thì đánh dấu để chạy
                if (state.TimerTokenSource == null)
                {
                    state.TimerTokenSource = new CancellationTokenSource();
                    startTimer = true; // Đánh dấu để bắt đầu timer
                }
            }

            // 5. Chạy logic DB (tốn thời gian) BÊN NGOÀI lock
            if (added)
            {
                // Cập nhật DB trạng thái "đang giữ" (status = 2)
                await UpdateSeatStatus(seatId, 2);
            }

            // 6. Nếu đã đánh dấu, thì bắt đầu chạy timer
            if (startTimer)
            {
                _ = RunCountdownTimer(connectionId, state, state.TimerTokenSource.Token);
            }
        }

        public async Task DeselectSeat(int seatId)
        {
            // Nếu không tìm thấy state, nghĩa là không có gì để Deselect
            if (_connectionStates.TryGetValue(Context.ConnectionId, out var state))
            {
                bool removed;
                lock (state.SelectedSeats) // Khóa lại để sửa
                {
                    removed = state.SelectedSeats.Remove(seatId);
                }

                if (removed)
                {
                    // Cập nhật DB trạng thái "trống" (status = 0)
                    await UpdateSeatStatus(seatId, 0);
                }
            }
        }

        public Task<bool> ConfirmBooking()
        {
            var cid = Context.ConnectionId;
            Console.WriteLine($"[SignalR - {cid}] ConfirmBooking: Đang yêu cầu xác nhận...");

            // Cố gắng xóa trạng thái
            if (_connectionStates.TryRemove(cid, out var state))
            {
                state.TimerTokenSource?.Cancel();
                Console.WriteLine($"[SignalR - {cid}] ConfirmBooking: ✅ THÀNH CÔNG! State đã xóa. Ghế sẽ được giữ.");
                return Task.FromResult(true);
            }
            else
            {
                Console.WriteLine($"[SignalR - {cid}] ConfirmBooking: ❌ THẤT BẠI! Không tìm thấy State (có thể đã bị xóa trước đó).");
                return Task.FromResult(false);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var cid = Context.ConnectionId;
            // Nếu tìm thấy state ở đây, nghĩa là ConfirmBooking CHƯA chạy hoặc chạy thất bại
            if (_connectionStates.TryRemove(cid, out var state))
            {
                Console.WriteLine($"[SignalR - {cid}] Disconnected: ⚠️ Phát hiện ngắt kết nối mà chưa Confirm -> Đang hủy ghế.");

                state.TimerTokenSource?.Cancel();
                HashSet<int> seatsToRelease;
                lock (state.SelectedSeats)
                {
                    seatsToRelease = new HashSet<int>(state.SelectedSeats);
                }
                await ReleaseSeats(seatsToRelease);
            }
            else
            {
                // Đây là trường hợp mong muốn khi chuyển trang thanh toán
                Console.WriteLine($"[SignalR - {cid}] Disconnected: ℹ️ Ngắt kết nối an toàn (State đã trống).");
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Hàm này giải phóng NHIỀU ghế
        private async Task ReleaseSeats(HashSet<int> seatsToRelease)
        {
            if (seatsToRelease.Count == 0) return; // Dòng này là lý do code bạn không chạy

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var showtimeApi = new ShowtimeSeatApiController(dbContext);

                foreach (int seat in seatsToRelease)
                {
                    try
                    {
                        await showtimeApi.PutSTSeatStatus(seat, 0);
                    }
                    catch (Exception ex) { /* PHẢI GHI LOG LỖI Ở ĐÂY */ }
                }
            }
        }

        // Hàm này cập nhật MỘT ghế
        private async Task UpdateSeatStatus(int seatId, int status)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var showtimeApi = new ShowtimeSeatApiController(dbContext);
                try
                {
                    await showtimeApi.PutSTSeatStatus(seatId, status);
                }
                catch (Exception ex) { Console.WriteLine("Lỗi không thể thay đổi ở database"); }
            }
        }
    }
}