//IN RA GHẾ
$(document).ready(function () {
    // Generate seats
    const seatsContainer = $('#seats');
    for (let row = 1; row <= 5; row++) {
        for (let seatNum = 1; seatNum <= 10; seatNum++) {
            const seat = $('<div>').addClass('seat').text(`${String.fromCharCode(64 + row)}${seatNum}`);
            if (Math.random() < 0.1) seat.addClass('booked');
            if (Math.random() < 0.05) seat.addClass('maintenance');
            seatsContainer.append(seat);
        }
        seatsContainer.append($('<br>'));
    }

// HIỆN LỰA CHỌN GHẾ KHI CHỌN XONG SUẤT CHIẾU
    $('#showtime').change(function () {
        const selectedShowtime = $('#showtime').val();
        console.log(selectedShowtime);
        if (selectedShowtime && selectedShowtime != "- Chọn giờ -") {
            $('#seat-selection').removeClass('d-none'); // Hiển thị section đặt ghế
        } else {
            $('#seat-selection').addClass('d-none'); // Ẩn section đặt ghế nếu không chọn suất chiếu
        }
    });

    $(document).ready(function () {
        let seatPrice = 80000; // Giá giả định cho mỗi ghế

        function updateTotal() {
            let total = 0;
            let selectedFoods = [];

            // Tính tổng tiền ghế đã chọn
            $('.seat.selected').each(function () {
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


        // Xử lý chọn ghế
        $('.seat').click(function () {
            if (!$(this).hasClass('booked') && !$(this).hasClass('maintenance')) {
                $(this).toggleClass('selected');
                if ($('.seat.selected').length > 0) {
                    $('#booking-summary').removeClass('d-none');
                } else {
                    $('#booking-summary').addClass('d-none');
                }
                updateTotal();
            }
        });

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
// HIỂN THỊ RẠP THEO THÀNH PHỐ
    //document.addEventListener("DOMContentLoaded", function () {
    //    let cinemaCityDropdown = document.getElementById("cinemaCity");
    //    let cinemaDropdown = document.getElementById("cinema");

    //    // SỰ KIỆN CHỌN THÀNH PHỐ -> LỌC RẠP
    //    cinemaCityDropdown.addEventListener("change", function () {
    //        let selectedCity = this.value;

    //        Array.from(cinemaDropdown.options).forEach(option => {
    //            let city = option.getAttribute("data-city");

    //            if (!selectedCity || city === selectedCity) {
    //                option.style.display = "block";
    //            } else {
    //                option.style.display = "none";
    //            }
    //        });

    //        // Đặt lại dropdown rạp về mặc định sau khi lọc
    //        cinemaDropdown.selectedIndex = 0;
    //    });

    //    // 🔥 SỰ KIỆN CHỌN RẠP -> CẬP NHẬT THÀNH PHỐ 🔥
    //    cinemaDropdown.addEventListener("change", function () {
    //        let selectedTheater = this.options[this.selectedIndex];

    //        // Nếu chọn "Select a Theater" thì cũng đặt lại thành phố
    //        if (selectedTheater.value === "") {
    //            cinemaCityDropdown.selectedIndex = 0; // Đặt về "Select a City"
    //        } else {
    //            let city = selectedTheater.getAttribute("data-city");
    //            if (city) {
    //                cinemaCityDropdown.value = city;
    //            }
    //        }
    //    });
    //});

// Hiện rạp theo thành phố đã chọn
document.getElementById("cinemaCity").addEventListener("change", function () {
    let cinemaCityName = this.value;
    let cinemaDropdown = document.getElementById("cinema");

    cinemaDropdown.innerHTML = '<option value="">Chọn rạp phim</option>';

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
    }
})

// Hiện ngày theo rạp đã chọn

document.getElementById("cinema").addEventListener("change", function () {
    let cinemaId = this.value;
    let dateDropdown = document.getElementById("date");
    const movieId = document.querySelector('input[name="Movie.MovieID"]').value;

    dateDropdown.innerHTML = '<option value="">Chọn ngày chiếu</option>';

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
    }
});


// Hiện giờ theo ngày đã chọn
document.getElementById("date").addEventListener("change", function () {
    let cinemaId = document.getElementById("cinema").value;
    const movieId = document.querySelector('input[name="Movie.MovieID"]').value;
    let timeDropdown = document.getElementById("time");
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
                        option.value = formattedTime;
                        timeDropdown.appendChild(option);
                    }
                });
            })
            .catch(error => console.error("Lỗi:", error));
    }
});
