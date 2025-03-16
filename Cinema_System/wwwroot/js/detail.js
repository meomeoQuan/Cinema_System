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
                    data.forEach(showtimeSeat => {
                        showtimeSeatSet.add(showtimeSeat.showtimeSeatID);
                    });

                    // Chuyển Set thành Array
                    let seatIdList = Array.from(showtimeSeatSet);

                    // Gửi danh sách ID ghế đến API
                    return fetch("/api/seats", { // Cập nhật URL API
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(seatIdList) // Chuyển Set thành mảng trước khi gửi
                    });
                })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Lỗi: ${response.status} - ${response.statusText}`);
                    }
                    return response.json();
                })
                .then(data => {
                    let rowSeatSet = new Set();
                    Array.from(data).forEach(seat => {
                        if (!rowSeatSet.has(seat.row)) {
                            rowSeatSet.add(seat.row);
                        }
                    })
                    let identitySeat = 0;
                    rowSeatSet.forEach(row => {
                        for (let seatNum = 1; seatNum <= 10 && identitySeat < data.length; seatNum++) {
                            seatCur = Array.from(data)[identitySeat];
                            const seat = `<div class='seat${seatCur.status === 1 ? " maintenance" : seatCur.status === 2 ? " booked" : ""}'>${String.fromCharCode(64 + row)}${seatCur.seatID}</div>`;
                            seatsContainer.insertAdjacentHTML("beforeend", seat);
                            identitySeat++;
                        }
                        seatsContainer.insertAdjacentHTML("beforeend", "<br>");
                    })
                })
                .catch(error => console.error("Lỗi:", error));
        }
    }
    else if (!seatSelection.classList.contains("d-none")) {
        seatSelection.classList.add("d-none");
    }

});

// chọn ghế
document.getElementById("seats").addEventListener("click", function (event) {
    let seat = event.target;
    if (seat.classList.contains("seat") && !seat.classList.contains("maintenance")) {
        if (seat.classList.contains("selected")) {
            seat.classList.remove("selected");
        } else {
            seat.classList.add("selected");
        }

        console.log(seat.classList); // Kiểm tra class của ghế sau khi click

        if (document.querySelectorAll(".seat.selected").length > 0) {
            document.getElementById("booking-summary").classList.remove("d-none");
        } else {
            document.getElementById("booking-summary").classList.add("d-none");
        }

        updateTotal();
    }
});




$(document).ready(function () {
    
// HIỆN LỰA CHỌN GHẾ KHI CHỌN XONG SUẤT CHIẾU

    $(document).ready(function () {
        let seatPrice = 80000; // Giá giả định cho mỗi ghế

        function updateTotal() {
            let total = 0;
            let selectedFoods = [];

            // Tính tổng tiền ghế đã chọn
            $('.seat.booked').each(function () {
                total += 80000; // Giá mỗi ghế
            });

            total -= 80000;

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


       

        // Xử lý tăng giảm số lượng thức ăn
        $('.plus').click(function () {
            let count = $(this).siblings('.count');
            count.text(parseInt(count.text()) + 1);
            updateTotal();
        });

        $('.minus').click(function () {
            let count = $(this).siblings('.count');
            if (parseInt(count.text()) > 0) {
                count.text(parseInt(count.text()) - 1);
                updateTotal();
            }
        });
    });

    $(document).ready(function () {
        $("#book-btn").click(function () {
            let selectedSeats = [];
            $(".seat.selected").each(function () {
                selectedSeats.push($(this).text().trim()); // Lấy tên ghế (VD: A1, B2)
            });

            let selectedFoods = [];
            $(".product-card").each(function () {
                let count = parseInt($(this).find(".count").text());
                if (count > 0) {
                    let foodName = $(this).find("h4").text();
                    let price = parseInt($(this).find(".price").text().replace(/\D/g, ""));
                    selectedFoods.push({ name: foodName, price: price, quantity: count });
                }
            });

            let bookingData = {
                seats: selectedSeats,
                items: selectedFoods,
                totalAmount: $("#total-price").text().replace(/\D/g, "") // Chuyển đổi số tiền
            };

            $.ajax({
                url: "/Guest/Payment/CreatePayment",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(bookingData),
                success: function (response) {
                    if (response.paymentUrl) {
                        window.location.href = response.paymentUrl; // Chuyển hướng đến cổng thanh toán
                    } else {
                        alert("Payment failed, please try again.");
                    }
                },
                error: function (xhr) {
                    alert("Error processing payment: " + xhr.responseText);
                }
            });
        });
    });




 // ĐẾM NGƯỢC 5 PHÚT GIỮ VÉ
    let timeLeft = 300;
    const countdown = setInterval(function () {
        timeLeft--;
        const minutes = Math.floor(timeLeft / 60);
        const seconds = timeLeft % 60;
        $('#countdown').text(`${minutes}:${seconds < 10 ? '0' : ''}${seconds}`);
        if (timeLeft <= 0) {
            clearInterval(countdown);
            alert('Hết thời gian giữ vé!');
            location.reload();
        }
    }, 1000);
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

    cinemaDropdown.innerHTML = '<option value="">Chọn rạp phim</option>';
    let dateDropdown = document.getElementById("date");

    dateDropdown.innerHTML = '<option value="">Chọn ngày chiếu</option>';
    let timeDropdown = document.getElementById("time");

    timeDropdown.innerHTML = '<option value="">Chọn giờ chiếu</option>';

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

    dateDropdown.innerHTML = '<option value="">Chọn ngày chiếu</option>';

    let timeDropdown = document.getElementById("time");

    timeDropdown.innerHTML = '<option value="">Chọn giờ chiếu</option>';

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

    timeDropdown.innerHTML = '<option value="">Chọn giờ chiếu</option>';

    if (cinemaId) {
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
