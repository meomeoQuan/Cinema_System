$(document).ready(function () {
    const seatsContainer = $('#seats');
    const userId = "User123"; // 🔥 Replace with actual user login ID
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/seatBookingHub")
        .build();

    // 🎭 Generate seats dynamically
    for (let row = 1; row <= 5; row++) {
        for (let seatNum = 1; seatNum <= 10; seatNum++) {
            const seatId = `${String.fromCharCode(64 + row)}${seatNum}`;
            const seat = $('<div>')
                .addClass('seat available') // Default class
                .text(seatId)
                .attr('data-seat-id', seatId);
            seatsContainer.append(seat);
        }
        seatsContainer.append($('<br>'));
    }

    // 🔄 Fetch booked seats from server on page load
    connection.start().then(() => {
        connection.invoke("GetBookedSeats")
            .then(bookedSeats => {
                bookedSeats.forEach(seatId => {
                    $(`[data-seat-id="${seatId}"]`).removeClass('available').addClass('booked');
                });
            })
            .catch(err => console.error(err));
    }).catch(err => console.error("SignalR Connection Error:", err));

    // 🖱️ Click event for booking seats
    $(document).on('click', '.seat.available', function () {
        const seatId = $(this).data('seat-id');
        connection.invoke("BookSeat", seatId, userId)
            .catch(err => console.error(err));
    });

    // ✅ Handle real-time seat booking update
    connection.on("SeatBooked", function (seatId, user) {
        $(`[data-seat-id="${seatId}"]`).removeClass('available').addClass('booked');
    });

    // 🟢 Handle real-time seat release update
    connection.on("SeatReleased", function (seatId) {
        $(`[data-seat-id="${seatId}"]`).removeClass('booked').addClass('available');
    });

    // ❌ Handle booking failure
    connection.on("SeatBookingFailed", function (seatId, message) {
        alert(`Seat ${seatId} booking failed: ${message}`);
    });

    // 🎭 Toggle seat selection and booking summary when selecting a showtime
    $('#showtime').change(function () {
        const selectedShowtime = $('#showtime').val();
        $('#seat-selection').toggleClass('d-none', !selectedShowtime);
    });

    // 📝 Show booking summary when selecting seats
    $(document).on('click', '.seat:not(.booked, .maintenance)', function () {
        $(this).toggleClass('selected');
        if ($('.seat.selected').length > 0) {
            $('#booking-summary').removeClass('d-none');
        } else {
            $('#booking-summary').addClass('d-none');
        }
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





