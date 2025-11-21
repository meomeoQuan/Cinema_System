// --- 1. Khởi tạo và Cấu hình Kết nối SignalR ---
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/countdownHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// --- 2. Định nghĩa các Hàm Xử lý Sự kiện từ Server ---
connection.on("ReceiveCountdown", function (timeLeft) {
    console.log(`⏳ Nhận thời gian từ server: ${timeLeft}s`);
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;
    const countdownElement = document.getElementById("countdown");
    if (countdownElement) {
        countdownElement.textContent = `${minutes}:${seconds < 10 ? "0" : ""}${seconds}`;
    }
});

connection.on("CountdownFinished", function (selectedSeats) {
    console.log("⏰ Hết thời gian. Server đã giải phóng ghế.", selectedSeats);

    // 1. Thông báo nhẹ nhàng
    alert("Hết thời gian giữ vé! Các ghế bạn chọn đã được giải phóng. Vui lòng chọn lại.");

    // 2. Xử lý giao diện (Không Reload)
    if (selectedSeats && selectedSeats.length > 0) {
        console.log("⏰ Chiều dài.", selectedSeats.length);
        selectedSeats.forEach(seatId => {
            // Tìm phần tử HTML của ghế dựa trên data-show-seat-id
            // Lưu ý: Dấu ngoặc vuông [] dùng để select theo attribute
            const seatElement = document.querySelector(`.seat[data-show-seat-id='${seatId}']`);

            if (seatElement) {
                // Xóa class 'selected' để ghế trở về trạng thái bình thường
                seatElement.classList.remove("selected");

                // Đảm bảo nó không còn class 'booked' hay 'maintenance' nếu có lỗi hiển thị trước đó
                // seatElement.classList.remove("booked"); 
            }
        });
    }

    // 3. Cập nhật lại Tổng tiền và Ẩn thanh thanh toán
    // Gọi hàm updateTotal() có sẵn của bạn để nó tự tính lại tiền (về 0)
    if (typeof updateTotal === "function") {
        updateTotal();
    } else {
        // Fallback thủ công nếu hàm update chưa chạy
        document.getElementById("booking-summary").classList.add("d-none");
        document.getElementById("total-price").innerText = "0 VND";
    }

    // 4. Reset hiển thị đồng hồ về 00:00 hoặc ẩn đi
    const countdownElement = document.getElementById("countdown");
    if (countdownElement) {
        countdownElement.textContent = "00:00";
        // Hoặc: countdownElement.parentElement.classList.add("d-none");
    }

    // 5. Bật lại nút đặt vé (nếu nó đang disabled)
    const bookBtn = document.getElementById('book-btn');
    if (bookBtn) bookBtn.disabled = false;
});

connection.onclose(() => {
    console.warn("⚠️ Mất kết nối SignalR. Thử kết nối lại sau 5 giây...");
    setTimeout(() => startConnection(), 5000);
});

// --- 3. Hàm Bắt đầu Kết nối ---
async function startConnection() {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        try {
            await connection.start();
            console.log("✅ Kết nối SignalR thành công!");
        } catch (err) {
            console.error("❌ Lỗi kết nối SignalR:", err);
            setTimeout(() => startConnection(), 5000);
        }
    }
}

// --- 4. Khởi động Kết nối Ngay lập tức ---
startConnection();

// --- 5. Logic Chính của Trang (In ghế và Xử lý Chọn giờ) ---
document.getElementById("time").addEventListener("change", function () {
    const seatSelection = document.getElementById("seat-selection");
    if (this.value) {
        seatSelection.classList.remove("d-none");
        const seatsContainer = document.getElementById("seats");
        seatsContainer.innerHTML = "";
        let showtimeId = document.getElementById("time").value;

        if (showtimeId) {
            let showtimeSeatSet = new Set();
            let showtimeSeatList = [];

            // Bước 1 & 2: Fetch API và lấy chi tiết ghế
            fetch(`/api/showtime-seat/${showtimeId}`)
                .then(response => response.json())
                .then(data => {
                    showtimeSeatList = data;
                    data.forEach(showtimeSeat => {
                        showtimeSeatSet.add(showtimeSeat.showtimeSeatID);
                    });
                    let seatIdList = Array.from(showtimeSeatSet);
                    return fetch("/api/seats", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(seatIdList)
                    });
                })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Lỗi: ${response.status} - ${response.statusText}`);
                    }
                    return response.json();
                })
                .then(data => {
                    // Bước 3: In ghế ra màn hình
                    let rowSeatSet = new Set();
                    Array.from(data).forEach(seat => {
                        if (!rowSeatSet.has(seat.row)) {
                            rowSeatSet.add(seat.row);
                        }
                    });

                    let identitySeat = 0;
                    rowSeatSet.forEach(row => {
                        for (let seatNum = 1; seatNum <= 10 && identitySeat < data.length; seatNum++) {
                            let seatCur = Array.from(data)[identitySeat];
                            let showtimeSeatCur = showtimeSeatList.find(showtimeSeat => showtimeSeat.seatID === seatCur.seatID);

                            if (showtimeSeatCur) {
                                const seat = `<div class='seat${showtimeSeatCur.status === 1 || seatCur.status === 1 ? " maintenance" : showtimeSeatCur.status === 2 ? " booked" : ""}' 
                                     data-show-seat-id='${showtimeSeatCur.showtimeSeatID}' 
                                     data-seat-id='${seatCur.seatID}'>
                                     ${String.fromCharCode(64 + row)}${seatCur.row}${seatCur.columnNumber}
                                 </div>`;
                                seatsContainer.insertAdjacentHTML("beforeend", seat);
                            }
                            identitySeat++;
                        }
                        seatsContainer.insertAdjacentHTML("beforeend", "<br>");
                    });

                    //// Bước 4: Khởi tạo MutationObserver để bắt đầu đếm ngược
                    //// (Giữ nguyên, không thay đổi)
                    //const targetNode = document.getElementById("booking-summary");
                    //if (!targetNode) return;

                    //const observer = new MutationObserver((mutationsList) => {
                    //    mutationsList.forEach(mutation => {
                    //        if (mutation.attributeName === "class") {
                    //            if (!targetNode.classList.contains("d-none")) {
                    //                console.log("Phần tử #booking-summary đã hiển thị! Bắt đầu đếm ngược...");
                    //                observer.disconnect();
                    //                if (connection.state === signalR.HubConnectionState.Connected) {
                    //                    connection.invoke("StartCountdown").then(() => {
                    //                        console.log("📡 Gửi lệnh StartCountdown thành công!");
                    //                    }).catch(err => console.error("❌ Lỗi khi gửi lệnh StartCountdown:", err));
                    //                } else {
                    //                    console.error("❌ SignalR chưa kết nối, không thể bắt đầu timer!");
                    //                }
                    //            }
                    //        }
                    //    });
                    //});
                    //observer.observe(targetNode, { attributes: true });

                })
                .catch(error => console.error("Lỗi khi fetch và in ghế:", error));
        }
    } else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }
});


/**
 * TRÌNH NGHE SỰ KIỆN TỔNG (ĐÃ SỬA ĐỔI)
 * XÓA BỎ HOÀN TOÀN logic fetch PUT, chỉ gọi SignalR
*/
document.getElementById("seats").addEventListener("click", async function (event) {
    let seat = event.target;
    if (!seat.classList.contains("seat") || seat.classList.contains("booked") || seat.classList.contains("maintenance")) {
        return; // Bỏ qua nếu không phải ghế hợp lệ
    }

    const showSeatId = Number(seat.getAttribute("data-show-seat-id"));

    // 1. Kiểm tra trạng thái ghế (VẪN CẦN THIẾT)
    let available;
    try {
        const response = await fetch(`/api/showtime-seat/ss/${showSeatId}`); // GET
        if (!response.ok) throw new Error("Server không phản hồi");
        let data = await response.json();
        available = data.status === 0; // true nếu status là 0 (trống)
    } catch (e) {
        console.error("Lỗi khi kiểm tra ghế:", e);
        alert("Không thể kiểm tra trạng thái ghế, vui lòng thử lại.");
        return;
    }

    // 2. Xử lý logic chọn/bỏ chọn
    if (seat.classList.contains("selected")) {
        // --- ĐANG BỎ CHỌN ---
        // (Logic bỏ chọn giữ nguyên, bỏ chọn thì thường không cần check gap chặt chẽ, 
        // hoặc nếu cần thì thêm checkGap ở đây nếu rạp khó tính)
        try {
            await connection.invoke("DeselectSeat", showSeatId);
            seat.classList.remove("selected");
            // ...
        } catch (err) { /*...*/ }

    } else if (!available) {
        // ... (Code xử lý ghế đã bán giữ nguyên) ...
    } else {
        // --- ĐANG CHỌN GHẾ (Thêm Logic Check Gap ở đây) ---

        // [THÊM MỚI] Gọi hàm kiểm tra
        const isValidSelection = checkGap(seat);

        if (!isValidSelection) {
            alert("Vui lòng chọn ghế liên tiếp, không để trống 1 ghế ở giữa!");
            return; // Dừng lại, không gọi SignalR
        }

        try {
            // CHỈ GỌI SIGNALR
            await connection.invoke("SelectSeat", showSeatId);
            seat.classList.add("selected");
            console.log(`Đã chọn ghế ${showSeatId}`);
        } catch (err) {
            console.error("Lỗi khi chọn ghế (SignalR):", err.message);
            alert("Có lỗi xảy ra, không thể chọn ghế. Vui lòng thử lại.");
        }
    }

    // 3. Cập nhật summary (luôn chạy sau khi thử chọn/bỏ chọn)
    if (document.querySelectorAll(".seat.selected").length > 0) {
        document.getElementById("booking-summary").classList.remove("d-none");
    } else {
        document.getElementById("booking-summary").classList.add("d-none");
    }

    updateTotal();
});
function checkGap(seatElement) {
    // Lấy thông tin hàng và số cột của ghế hiện tại
    // Giả sử text ghế là "A5" -> Row: A, Col: 5
    const seatText = seatElement.innerText.trim();
    const rowChar = seatText.charAt(0); // "A"
    const colNum = parseInt(seatText.substring(1)); // 5

    // Lấy tất cả các ghế trong CÙNG MỘT HÀNG
    // (Giả sử HTML của bạn có cấu trúc để lọc được ghế theo hàng, 
    // hoặc ta lọc thủ công từ text của tất cả ghế)
    const allSeats = Array.from(document.querySelectorAll('.seat'));
    const rowSeats = allSeats.filter(s => s.innerText.trim().startsWith(rowChar))
        .sort((a, b) => {
            const colA = parseInt(a.innerText.trim().substring(1));
            const colB = parseInt(b.innerText.trim().substring(1));
            return colA - colB;
        });

    // Tạo một mảng trạng thái: 0=Trống, 1=Đã bán/Bảo trì, 2=Đang chọn (Selected)
    // Ta giả lập: nếu ghế hiện tại được chọn thì nó sẽ là 2
    const seatMap = rowSeats.map(s => {
        const col = parseInt(s.innerText.trim().substring(1));
        let status = 0; // Trống

        if (s.classList.contains('booked') || s.classList.contains('maintenance')) {
            status = 1; // Không thể chọn
        } else if (s.classList.contains('selected') || s === seatElement) {
            status = 2; // Đang chọn (bao gồm cả ghế đang click)
        }

        return { col, status, element: s };
    });

    // --- LOGIC KIỂM TRA ---
    // Tìm tất cả các ghế ĐANG CHỌN (status 2) trong hàng này
    const selectedIndices = seatMap.map((s, i) => s.status === 2 ? i : -1).filter(i => i !== -1);

    if (selectedIndices.length === 0) return true; // Chưa chọn ghế nào -> OK

    // Kiểm tra từng ghế đang chọn xem có tạo gap không
    for (let i of selectedIndices) {
        // Kiểm tra bên trái
        if (i > 0) {
            // Nếu ghế bên trái (i-1) là TRỐNG (0) 
            // VÀ ghế bên trái nữa (i-2) là CÓ NGƯỜI (1 hoặc 2) hoặc là đầu hàng
            // -> Thì cái ghế (i-1) là một cái lỗ hổng 1 ghế.
            const left = seatMap[i - 1];

            if (left.status === 0) {
                // Nếu bên trái là trống, kiểm tra tiếp bên trái nó nữa
                if (i - 1 === 0) {
                    // Lỗ hổng ngay đầu hàng -> Chặn
                    // return false; // (Tùy rạp, thường rạp cho phép để trống đầu hàng)
                } else {
                    const leftLeft = seatMap[i - 2];
                    if (leftLeft.status !== 0) {
                        // [X] [Trống] [Đang Chọn] -> LỖI
                        return false;
                    }
                }
            }
        }

        // Kiểm tra bên phải (tương tự)
        if (i < seatMap.length - 1) {
            const right = seatMap[i + 1];
            if (right.status === 0) {
                if (i + 1 === seatMap.length - 1) {
                    // Lỗ hổng cuối hàng
                } else {
                    const rightRight = seatMap[i + 2];
                    if (rightRight.status !== 0) {
                        // [Đang Chọn] [Trống] [X] -> LỖI
                        return false;
                    }
                }
            }
        }
    }

    // Kiểm tra xem các ghế đang chọn có liên tiếp không (nếu chọn nhiều ghế rời rạc)
    // Logic đơn giản: Khoảng cách giữa min và max index phải bằng số lượng - 1
    // (Chỉ áp dụng nếu rạp bắt buộc chọn liên tiếp trong 1 lần đặt)
    if (selectedIndices.length > 1) {
        const min = Math.min(...selectedIndices);
        const max = Math.max(...selectedIndices);
        // Kiểm tra xem giữa min và max có ghế trống nào không
        for (let k = min; k <= max; k++) {
            if (seatMap[k].status === 0) return false; // Có ghế trống xen giữa các ghế chọn
        }
    }

    return true;
}


async function updateTotal() {
    let total = 0;
    let selectedFoods = [];

    let selectedSeats = document.querySelectorAll(".seat.selected");
    let showTimeId = document.getElementById("time").value;

    let selectedSeatIds = [];

    for (let sls of selectedSeats) {
        if (!sls.classList.contains("note")) {
            selectedSeatIds.push(sls.getAttribute("data-seat-id"));
        }
    }

    try {
        let response = await fetch("/api/showtime-seat", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ showTimeId: showTimeId, seatIds: selectedSeatIds })
        });

        let data = await response.json();
        console.log(data);
        total = data.reduce((acc, seat) => acc + Number(seat.price || 0), 0);

        let totalElement = document.getElementById("total");
        if (totalElement) {
            totalElement.innerText = `Total: ${total}`;
        }
    } catch (error) {
        console.error("Lỗi khi gọi API:", error);
    }

    // Tính tổng tiền thức ăn đã chọn
    $('.product-card').each(function () {
        let count = parseInt($(this).find('.count').text());
        let foodName = $(this).find('h4').text();
        let price = parseInt($(this).find('.price').text().replace(/\D/g, ''));

        if (count > 0) {
            selectedFoods.push(`${count} x ${foodName}`);
            total += count * price;
        }
    });

    // Cập nhật tổng giá trên giao diện
    $('#total-price').text(total.toLocaleString() + ' VND');

    // Cập nhật danh sách món ăn đã chọn
    $('#selected-foods').text(selectedFoods.length > 0 ? selectedFoods.join(', ') : 'No food selected');

    // Cập nhật giá trị của input hidden
    $('#totalAmountInput').val(total);
    if (total > 0) {
        document.getElementById("booking-summary").classList.remove("d-none");
    } else {
        document.getElementById("booking-summary").classList.add("d-none");
    }
}


document.getElementById('book-btn').addEventListener('click', async function () {

    event.preventDefault();

    let bookBtn = this;
    bookBtn.disabled = true;

    //// --- 1. KIỂM TRA KẾT NỐI CHẶT CHẼ ---
    //if (connection.state !== signalR.HubConnectionState.Connected) {
    //    //alert("⚠️ Mất kết nối với máy chủ! Đang thử kết nối lại, vui lòng đợi...");
    //    console.warn("SignalR không kết nối. Trạng thái hiện tại: ", connection.state);

    //    // Thử kết nối lại ngay lập tức
    //    try {
    //        await connection.start();
    //        console.log("Đã kết nối lại thành công!");
    //    } catch (err) {
    //        //alert("❌ Không thể kết nối lại. Vui lòng tải lại trang (F5) để đặt vé.");
    //        bookBtn.disabled = false;
    //        return; // Dừng ngay lập tức
    //    }
    //}

    // --- 2. GỌI SERVER VÀ CHỜ PHẢN HỒI ---
    try {
        // Gọi hàm ConfirmBooking và LẤY KẾT QUẢ TRẢ VỀ
        // Server trả về true nếu xóa state thành công, false nếu không tìm thấy
        const isConfirmed = await connection.invoke("ConfirmBooking");

        if (isConfirmed === true) {
            console.log("✅ Server xác nhận: OK. Ghế đã được bảo vệ.");
        } else {
            console.warn("❌ Server báo: Không tìm thấy phiên đặt vé. Có thể ghế đã bị hủy do hết giờ.");
            alert("Phiên đặt vé không hợp lệ hoặc đã hết giờ. Vui lòng chọn lại ghế.");
            location.reload();
            return; // Dừng ngay
        }
    } catch (err) {
        console.error("❌ Lỗi khi gọi ConfirmBooking:", err);
        alert("Lỗi hệ thống khi xác nhận ghế. Vui lòng thử lại.");
        bookBtn.disabled = false;
        return;
    }

    let seatSelecteds = document.querySelectorAll(".seat.selected");
    let selectedSeats = [];
    seatSelecteds.forEach(seat => {
        if (!seat.classList.contains('note')) {
            selectedSeats.push({ nameSeat: String(seat.innerText).trim(), showTimeSeatId: seat.getAttribute("data-show-seat-id") });
        }
    })

    // Kiểm tra an toàn
    if (selectedSeats.length === 0) {
        alert("Vui lòng chọn ghế!");
        bookBtn.disabled = false;
        return;
    }

    let selectedFoods = [];
    // ... (Code lấy selectedFoods giữ nguyên) ...
    let productCard = document.querySelectorAll(".product-card");
    productCard.forEach(product => {
        let count = product.querySelector(".count").innerText;
        if (count > 0) {
            let foodName = product.querySelector("h4").innerText;
            let price = product.querySelector(".price").getAttribute("product-price").replace(/\D/g, "");
            selectedFoods.push({ name: foodName, price: price, quantity: count });
        }
    })

    let nameMovie = document.querySelector("#title-movie").innerHTML;
    let coupon = document.querySelector(".coupon").value;
    let cinemaId = document.querySelector("#cinema").value;
    let showtimeSeat;
    const apiUrl = `/api/showtime-seat/ss/${selectedSeats[0].showTimeSeatId}`;

    try {
        let response = await fetch(apiUrl);
        if (!response.ok) throw new Error("Network response was not ok");
        showtimeSeat = await response.json();
    } catch (error) {
        console.log("Fetch error:", error);
        bookBtn.disabled = false;
        return;
    }

    let showtime;
    try {
        let response = await fetch(`/api/showtime/getById/${showtimeSeat.showtimeID}`)
        showtime = await response.json();
    } catch (e) {
        console.error(e);
        bookBtn.disabled = false;
        return;
    }

    let cinema;
    try {
        let response = await fetch(`/api/cinemas/id/${cinemaId}`);
        cinema = await response.json();
    } catch (e) {
        console.error(e);
        bookBtn.disabled = false;
        return;
    }

    let bookingData = {
        Coupon: coupon,
        Seats: selectedSeats,
        Items: selectedFoods,
        TotalAmount: document.querySelector("#total-price").innerText.replace(/\D/g, ""),
        TitleMovie: nameMovie,
        Cinema: cinema,
        ShowTimeSeat: showtimeSeat,
        Showtime: showtime,
        TiketPrice: 80000
    };

    const user = getCookie('user');

    // --- 4. NGẮT KẾT NỐI THỦ CÔNG (AN TOÀN) ---
    // Lúc này Server đã xác nhận (bước 2), nên ngắt kết nối ở đây là an toàn.
    // OnDisconnectedAsync sẽ chạy nhưng không tìm thấy state -> không hủy ghế.
    try {
        await connection.stop();
        console.log("🛑 Đã ngắt kết nối SignalR để chuyển trang.");
    } catch (e) {
        console.warn("Lỗi khi ngắt kết nối:", e);
    }

    if (user != null) {
        fetch(`/Guest/Payment/CreatePayment`, {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(bookingData),
        }).then(response => response.json())
            .then(data => {
                if (data.paymentUrl) {
                    window.location.href = data.paymentUrl;
                } else {
                    alert("Lỗi khi tạo thanh toán.");
                    bookBtn.disabled = false;
                    // Nếu lỗi thanh toán, cần kết nối lại để giữ ghế tiếp
                    startConnection();
                }
            })
            .catch(error => {
                console.error(error);
                bookBtn.disabled = false;
                startConnection();
            });
    } else {
        localStorage.setItem("bookingData", JSON.stringify(bookingData));
        window.location.href = "/Guest/Details/InformationTicket";
    }
});

document.addEventListener("DOMContentLoaded", function () {
    let listProduct;
    fetch("/api/products")
        .then(response => response.json())
        .then(data => {
            Array.from(data).map(product => {
                let content = `<div class="col-md-6">
						<div class="product-card d-flex align-items-center">
							<img src="${product.productImage}" alt="${product.name}" class="product-img">
							<div class="product-info">
								<h4>${product.name}</h4>
								<p>${product.description}</p>
								<span class="price" product-price="${product.price}">${product.price} VNĐ</span>
								<div class="quantity">
									<button class="btn minus">-</button>
									<span class="count">0</span>
									<button class="btn plus">+</button>
								</div>
							</div>
						</div>
					</div>`;
                let productHtml;
                if (product.productType === 0) {
                    productHtml = document.querySelector("#food-selection .food");

                } else if (product.productType === 1) {
                    productHtml = document.querySelector("#food-selection .drink");

                } else if (product.productType === 2) {
                    productHtml = document.querySelector("#food-selection .gift");
                } else if (product.productType === 3) {
                    productHtml = document.querySelector("#food-selection .comboo");
                }
                productHtml.innerHTML += content;
            })
            addEventListenersForButtons();

        })
});

function addEventListenersForButtons() {

    document.querySelectorAll(".plus").forEach(plus => {
        plus.addEventListener("click", function () {
            let count = this.previousElementSibling;
            count.textContent = parseInt(count.textContent) + 1;
            updateTotal();
        })
    })

    document.querySelectorAll(".minus").forEach(plus => {
        plus.addEventListener("click", function () {
            let count = this.nextElementSibling;
            if (parseInt(count.textContent) > 0) {
                count.textContent = parseInt(count.textContent) - 1;
                updateTotal();
            }
        })
    })
}

function getCookie(name) {
    let match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    localStorage.setItem("match", match);
    if (match) {
        return match[2];  // Return the cookie value
    }
    return null;  // Return null if the cookie is not found
}
//$(document).ready(function () {
    
//});




document.addEventListener("DOMContentLoaded", function () {
    // Load danh sách thành phố khi vào trang
    fetch("/api/details/cities")
        .then(response => response.json())
        .then(data => {
            let cityDropdown = document.getElementById("cinemaCity");
            data.data.forEach(city => {
                let option = document.createElement("option");
                option.value = city;
                option.textContent = city;
                cityDropdown.appendChild(option);
            });
        });

});

// Hiện rạp theo thành phố đã chọn
document.getElementById("cinemaCity").addEventListener("change", function () {
    let cinemaCityName = this.value;
    let cinemaDropdown = document.getElementById("cinema");
    const seatSelection = document.getElementById("seat-selection");

    cinemaDropdown.innerHTML = '<option value="">-- Select a Theater --</option>';
    let dateDropdown = document.getElementById("date");

    dateDropdown.innerHTML = '<option value="">-- Select a Date --</option>';
    let timeDropdown = document.getElementById("time");

    timeDropdown.innerHTML = '<option value="">-- Select a Time --</option>';

    if (cinemaCityName) {
        fetch(`/api/cinemas/${cinemaCityName}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(cinema => {
                    let option = document.createElement("option");
                    option.value = cinema.cinemaID;
                    option.textContent = cinema.name;
                    cinemaDropdown.appendChild(option);
                });
            });
    } else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }
})

// Hiện ngày theo rạp đã chọn

document.getElementById("cinema").addEventListener("change", function () {
    let cinemaId = this.value;
    let dateDropdown = document.getElementById("date");
    const movieId = document.querySelector('input[name="Movie.MovieID"]').value;
    const seatSelection = document.getElementById("seat-selection");

    dateDropdown.innerHTML = '<option value="">-- Select a Date --</option>';

    let timeDropdown = document.getElementById("time");

    timeDropdown.innerHTML = '<option value="">-- Select a Time --</option>';

    if (cinemaId) {
        fetch(`/api/showtime/${cinemaId}/${movieId}`)
            .then(response => response.json())
            .then(data => {
                let uniqueDates = new Set();

                data.forEach(showtime => {
                    let formattedDate = new Date(showtime.showDate).toISOString().split("T")[0];

                    if (!uniqueDates.has(formattedDate)) {
                        uniqueDates.add(formattedDate);

                        let option = document.createElement("option");
                        option.textContent = formattedDate;
                        option.value = formattedDate;
                        dateDropdown.appendChild(option);
                    }
                });
            })
            .catch(error => console.error("Lỗi:", error));
    } else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }
});


// Hiện giờ theo ngày đã chọn
document.getElementById("date").addEventListener("change", function () {
    let cinemaId = document.getElementById("cinema").value;
    const movieId = document.querySelector('input[name="Movie.MovieID"]').value;
    let timeDropdown = document.getElementById("time");
    const seatSelection = document.getElementById("seat-selection");
    let dateChoose = this.value;

    timeDropdown.innerHTML = '<option value="">-- Select a Time --</option>';

    if (dateChoose) {
        fetch(`/api/showtime/${cinemaId}/${movieId}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(showtime => {
                    let dateObj = new Date(showtime.showDate);
                    let formattedDate = new Date(showtime.showDate).toISOString().split("T")[0];
                    if (dateChoose === formattedDate) {

                        // Format giờ theo "HH:mm" (24h)
                        let formattedTime = dateObj.toLocaleTimeString("vi-VN", {
                            hour: "2-digit",
                            minute: "2-digit",
                            hour12: false, // Dùng hệ 24 giờ
                        });

                        let option = document.createElement("option");
                        option.textContent = formattedTime;
                        option.value = showtime.showTimeID;
                        timeDropdown.appendChild(option);
                    }
                });
            })
            .catch(error => console.error("Lỗi:", error));
    } else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }
});
