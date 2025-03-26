//IN RA GHẾ
document.getElementById("time").addEventListener("change", function () {
    const seatSelection = document.getElementById("seat-selection");
    if (this.value) {
        seatSelection.classList.remove("d-none");
        const seatsContainer = document.getElementById("seats");
        seatsContainer.innerHTML = "";
        let showtimeId = document.getElementById("time").value;

        if (showtimeId) {
            let showtimeSeatSet = new Set();

            // Gọi API để lấy danh sách ghế
            fetch(`/api/showtime-seat/${showtimeId}`)
                .then(response => response.json())
                .then(data => {
                    let showtimeSeatList = data; // Lưu danh sách ghế từ API
                    data.forEach(showtimeSeat => {
                        showtimeSeatSet.add(showtimeSeat.showtimeSeatID);
                    });

                    // Chuyển Set thành Array
                    let seatIdList = Array.from(showtimeSeatSet);

                    // Gửi danh sách ID ghế đến API để lấy chi tiết ghế
                    return fetch("/api/seats", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(seatIdList)
                    }).then(response => {
                        if (!response.ok) {
                            throw new Error(`Lỗi: ${response.status} - ${response.statusText}`);
                        }
                        return response.json();
                    }).then(data => {
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

                                const seat = `<div class='seat${showtimeSeatCur.status === 1 || seatCur.status === 1 ? " maintenance" : showtimeSeatCur.status === 2 ? " booked" : ""}' 
                                    data-show-seat-id='${showtimeSeatCur.showtimeSeatID}'  
                                    data-seat-id='${seatCur.seatID}'>
                                    ${String.fromCharCode(64 + row)}${seatCur.row}${seatCur.columnNumber}
                                </div>`;

                                seatsContainer.insertAdjacentHTML("beforeend", seat);
                                identitySeat++;
                            }
                            seatsContainer.insertAdjacentHTML("beforeend", "<br>");
                        });
                    });
                })
                .catch(error => console.error("Lỗi:", error));
        }
    } else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }
});


// chọn ghế
document.getElementById("seats").addEventListener("click", function (event) {
    let seat = event.target;
    if (seat.classList.contains("seat") && !seat.classList.contains("booked") && !seat.classList.contains("maintenance")) {
        let status;
        if (seat.classList.contains("selected")) {
            seat.classList.remove("selected");
            status = 0;
        } else {
            seat.classList.add("selected");
            status = 2;
        }

        fetch(`/api/showtime-seat/${seat.getAttribute("data-show-seat-id")}/${status}`, {
            method: "PUT",
            headers: {
                contentType: "application/json",
            }
        })

        if (document.querySelectorAll(".seat.selected").length > 0) {
            document.getElementById("booking-summary").classList.remove("d-none");
        } else {
            document.getElementById("booking-summary").classList.add("d-none");
        }

        updateTotal();
    }
});

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

document.getElementById('book-btn').addEventListener('click', function () {
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

    let coupon = document.querySelector(".coupon").value;

    let bookingData = {
        Coupon: coupon,
                Seats: selectedSeats,
        Items: selectedFoods,
        TotalAmount: document.querySelector("#total-price").innerText.replace(/\D/g, "") // Chuyển đổi số tiền
    };

    fetch(`/Guest/Payment/CreatePayment`, {
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
            }
        })
        .catch(error => console.error("Lỗi khi gọi API:", error));
})

$(document).ready(function () {
    // Submit the form
    $("#booking-summary").submit();
});


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
