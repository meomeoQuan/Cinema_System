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
        if (selectedShowtime) {
            $('#seat-selection').removeClass('d-none'); // Hiển thị section đặt ghế
        } else {
            $('#seat-selection').addClass('d-none'); // Ẩn section đặt ghế nếu không chọn suất chiếu
        }
    });

    //HIÊN THANH BOOKING SUMMARY KHI CHỌN XONG GHẾ
    $('.seat').click(function () {
        if (!$(this).hasClass('booked') && !$(this).hasClass('maintenance')) {
            $(this).toggleClass('selected');
            if ($('.seat.selected').length > 0) {
                $('#booking-summary').removeClass('d-none');
            } else {
                $('#booking-summary').addClass('d-none');
            }
        }
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
$(document).ready(function () {
    $('.plus').click(function () {
        let count = $(this).siblings('.count');
        let currentCount = parseInt(count.text());
        count.text(currentCount + 1);
    });

    $('.minus').click(function () {
        let count = $(this).siblings('.count');
        let currentCount = parseInt(count.text());
        if (currentCount > 0) {
            count.text(currentCount - 1);
        }
    });
});