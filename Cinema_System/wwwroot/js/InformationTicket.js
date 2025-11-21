document.addEventListener("DOMContentLoaded", function () {
    let bookingData = JSON.parse(localStorage.getItem("bookingData"));
    console.log("Dữ liệu bookingData từ localStorage:", bookingData);

    if (!bookingData) {
        console.warn("Không có dữ liệu bookingData.");
        return;
    }

    let movieTitle = document.getElementById("movie-title");
    let cinemaName = document.getElementById("cinema-name");
    let showTime = document.getElementById("show-time");   
    let selectedSeats = document.getElementById("selected-seats");
    let ticketPrice = document.getElementById("ticket-price");
    let foodList = document.getElementById("food-list");
    let totalPriceElement = document.getElementById("total-price");

    console.log("Các phần tử DOM:", { movieTitle, cinemaName, showTime, selectedSeats, ticketPrice, foodList, totalPriceElement });

    if (!movieTitle || !cinemaName || !showTime || !selectedSeats || !ticketPrice || !foodList || !totalPriceElement) {
        console.error("Lỗi: Một hoặc nhiều phần tử không tồn tại trong DOM.");
        return;
    }
    /**
     *  let bookingData = {
        Coupon: coupon,
                Seats: selectedSeats,
        Items: selectedFoods,
        TotalAmount: document.querySelector("#total-price").innerText.replace(/\D/g, ""), // Chuyển đổi số tiền
        TitleMovie: nameMovie,
        Cinema: cinema,
        Date: date,
        Time: time,
    };
     */

    movieTitle.innerText = bookingData.TitleMovie;
    cinemaName.innerText = bookingData.Cinema.name;
    showTime.innerText = formatDateTime(bookingData.Showtime.showDate);
    selectedSeats.innerText = bookingData.Seats.map(it => it.nameSeat).join(", ");
    ticketPrice.innerText = bookingData.ShowTimeSeat.price.toLocaleString() + " VNĐ/vé";

    let totalPrice = bookingData.Seats.length * bookingData.ShowTimeSeat.price;

    foodList.innerHTML = "";
    bookingData.Items.forEach(food => {
        let item = document.createElement("li");
        item.classList.add("list-group-item");
        let totalFoodPrice = food.quantity * parseInt(food.price);
        totalPrice += totalFoodPrice;
        item.innerText = `${food.name} x${food.quantity} - ${totalFoodPrice.toLocaleString()} VNĐ`;
        foodList.appendChild(item);
    });

    totalPriceElement.innerText = totalPrice.toLocaleString() + " VNĐ";
});

function formatDateTime(isoString) {
    let date = new Date(isoString);
    let day = String(date.getDate()).padStart(2, '0');
    let month = String(date.getMonth() + 1).padStart(2, '0');
    let year = date.getFullYear();
    let hours = String(date.getHours()).padStart(2, '0');
    let minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

document.getElementById("bookingForm").addEventListener('submit', function (e) {
    e.preventDefault();
    let bookingData = JSON.parse(localStorage.getItem("bookingData"));
    const fullname = document.getElementsByName("fullname")[0].value;
    const email = document.getElementsByName("email")[0].value;
    const phone = document.getElementsByName("phone")[0].value;

    bookingData = {
        ...bookingData,
        Guest: {
            fullname: fullname,
            email: email,
            phone: phone,
        },
    }

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
                    bookBtn.disabled = false;
                }
            })
            .catch(error => bookBtn.disabled = false);
})