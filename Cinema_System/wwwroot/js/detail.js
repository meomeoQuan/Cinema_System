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

// Xử lý nút tăng/giảm số lượng đồ ăn
