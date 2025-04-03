// Tìm kiếm không cần reload trang 
document.getElementById('searchProduct').addEventListener('keyup', function (e) {
    if (e.key === 'Enter') {
        searchProduct();
    }
});

function searchProduct() {
    const searchString = document.getElementById('searchProduct').value;
    window.location.href = `/Guest/Product/Product?searchString=${encodeURIComponent(searchString)}`;
}