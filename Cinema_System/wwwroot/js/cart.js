document.querySelector("#payment-btn").addEventListener('click', function () {
    const bodyTable = document.querySelector(".text-table");
    const inputs = bodyTable.querySelectorAll('input[name="id"]');
    const idOrderDetails = [];
    Array.from(inputs).map(it => idOrderDetails.push(Number(it.value)));
    const coupon = document.getElementById("cart-bottom").querySelector('input[name="coupon"]').value;
    let response = [];

        fetch('/api/order-detail', {
            headers: {
                "Content-Type": 'application/json',
            },
            method: 'POST',
            body: JSON.stringify({ orderDetailsId: idOrderDetails })
        }).then(response => response.json())
            .then(data => {
                response = data;

                let total = 0;
                let selectedFoods = [];
                Array.from(response).map(item => {
                    total += item.orderDetail.totalPrice;
                    selectedFoods.push({ name: item.product.name, quantity: item.orderDetail.quantity, price: item.orderDetail.price });
                });
    const bookingData = {
        Coupon: coupon,
        Items: selectedFoods,
        TotalAmount: total, // Chuyển đổi số tiền
        OrderCode: response[0].orderDetail.orderID,
    }

    try {
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
    } catch (e) {
        console.error(e);
    }
            }).catch(error => console.error(error));
})