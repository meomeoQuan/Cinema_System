// --- 1. Khởi tạo và Cấu hình Kết nối SignalR ---
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/countdownHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// --- 2. Định nghĩa các Hàm Xử lý Sự kiện từ Server ---
connection.on("ReceiveCountdown", function (timeLeft) {
    console.log(⏳ Nhận thời gian từ server: ${ timeLeft }s);
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;
    const countdownElement = document.getElementById("countdown");
    if (countdownElement) {
        countdownElement.textContent = ${ minutes }:${ seconds < 10 ? "0" : "" }${ seconds };
    }
});

connection.on("CountdownFinished", function (selectedSeats) {
    console.log("⏰ Hết thời gian. Server đã giải phóng ghế.", selectedSeats);
    alert("Hết thời gian giữ vé!");
    location.reload();
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
            fetch(/api/showtime - seat / ${ showtimeId })
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
                        throw new Error(Lỗi: ${ response.status } - ${ response.statusText });
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
        const response = await fetch(/api/showtime - seat / ss / ${ showSeatId }); // GET
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
        try {
            // CHỈ GỌI SIGNALR
            await connection.invoke("DeselectSeat", showSeatId);

            // Cập nhật UI ngay lập- tức
            seat.classList.remove("selected");
            console.log(Đã bỏ chọn ghế ${ showSeatId });

            // XÓA BỎ FETCH PUT
            // fetch(/api/showtime-seat/${showSeatId}/0, ... ) // << ĐÃ XÓA

        } catch (err) {
            console.error("Lỗi khi bỏ chọn ghế (SignalR):", err.message);
            alert("Có lỗi xảy ra, không thể bỏ chọn ghế. Vui lòng thử lại.");
        }

    } else if (!available) {
        // --- GHẾ ĐÃ BỊ CHỌN (BỞI NGƯỜI KHÁC) ---
        alert("Ghế này vừa được người khác chọn, vui lòng chọn ghế khác.");
        location.reload();
        return;

    } else {
        // --- ĐANG CHỌN GHẾ ---
        try {
            // CHỈ GỌI SIGNALR
            await connection.invoke("SelectSeat", showSeatId);

            // Cập nhật UI ngay lập tức
            seat.classList.add("selected");
            console.log(Đã chọn ghế ${ showSeatId });

            // XÓA BỎ FETCH PUT
            // fetch(/api/showtime-seat/${showSeatId}/2, ... ) // << ĐÃ XÓA

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

    // updateTotal();
});
/**
 * 
 */


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
            totalElement.innerText = Total: ${ total };
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
            selectedFoods.push(${ count } x ${ foodName });
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

document.getElementById('book-btn').addEventListener('click', async function () {

    let bookBtn = this;
    bookBtn.disabled = true;
    let seatSelecteds = document.querySelectorAll(".seat.selected");
    let selectedSeats = [];
    seatSelecteds.forEach(seat => {
        if (!seat.classList.contains('note')) {
            selectedSeats.push({ nameSeat: String(seat.innerText).trim(), showTimeSeatId: seat.getAttribute("data-show-seat-id") });
        }
    })
    let selectedFoods = [];

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
    const apiUrl = /api/showtime - seat / ss / ${ selectedSeats[0].showTimeSeatId };
    console.log("Fetching from:", apiUrl);
    try {
        let response = await fetch(apiUrl);
        showtimeSeat = await response.json();
        console.log("Fetched data:", showtimeSeat);
    } catch (error) {
        console.log("Fetch error:", error);
        return;  // Dừng hàm nếu fetch bị lỗi
    }

    let showtime;
    try {
        let response = await fetch(/api/showtime / getById / ${ showtimeSeat.showtimeID })
        showtime = await response.json();
    } catch (e) {
        console.error(e);
        return;
    }

    let cinema;

    try {
        let response = await fetch(/api/cinemas / id / ${ cinemaId });
        cinema = await response.json();
    } catch (e) {
        console.error(e);
        return;
    }

    let bookingData = {
        Coupon: coupon,
        Seats: selectedSeats,
        Items: selectedFoods,
        TotalAmount: document.querySelector("#total-price").innerText.replace(/\D/g, ""), // Chuyển đổi số tiền
        TitleMovie: nameMovie,
        Cinema: cinema,
        ShowTimeSeat: showtimeSeat,
        Showtime: showtime,
        TiketPrice: 80000
    };
    const user = getCookie('user');
    if (user != null) {
        fetch(/Guest/Payment / CreatePayment, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(bookingData),
        }).then(response => response.json())
            .then(data => {
                if (data.paymentUrl) {
                    window.location.href = data.paymentUrl; // ✅ Redirect người dùng tới PayOS
                } else {
                    alert("Lỗi khi tạo thanh toán, vui lòng thử lại.");
                    bookBtn.disabled = false;
                }
            })
            .catch(error => bookBtn.disabled = false);
    } else {
        localStorage.setItem("bookingData", JSON.stringify(bookingData));
        window.location.href = "/Guest/Details/InformationTicket";
    }
})

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
        fetch(/api/cinemas / ${ cinemaCityName })
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
        fetch(/api/showtime / ${ cinemaId } / ${ movieId })
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
        fetch(/api/showtime / ${ cinemaId } / ${ movieId })
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