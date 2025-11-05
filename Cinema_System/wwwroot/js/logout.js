//function deleteAllCookies() {
//    // Lấy tất cả các cookie hiện tại
//    const cookies = document.cookie.split(";");
//    localStorage.setItem("cookies", cookies);

//    // Duyệt qua tất cả cookies và thiết lập thời gian hết hạn đã qua
//    for (let i = 0; i < cookies.length; i++) {
//        let cookie = cookies[i];
//        let equalsPos = cookie.indexOf("=");
//        let name = equalsPos > -1 ? cookie.substr(0, equalsPos) : cookie;

//        // Xóa cookie bằng cách thiết lập thời gian hết hạn là quá khứ
//        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
//    }
//}
//document.getElementById("logout").addEventListener('click', function () {
//    deleteAllCookies();
//});