// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




$(document).ready(function () {
    $(".nav-btn").click(function () {
        // Lấy giá trị target từ data attribute
        var target = $(this).data("target");

        // Ẩn video và ndung
        $(".video-background video").removeClass("active");
        $(".content").removeClass("active");

        // Hiển thị video và nội dung tương ứng với target khi có active
        $("#video" + target).addClass("active");
        $("#content" + target).addClass("active");
    });
});


//show dropdown
const showDropdown = (dropdownId) => {
    const dropdown = document.getElementById(dropdownId)

    dropdown.addEventListener('click', () => {
        dropdown.classList.toggle('show-dropdown');

    })
}

showDropdown('dropdown');

//login & sign up
const container = document.querySelector(".container"),
    pwShowHide = document.querySelectorAll(".showHidePw"),
    pwFields = document.querySelectorAll(".password"),
    signUp = document.querySelector(".signup-link"),
    login = document.querySelector(".login-link");
// show/hide password and change icon
pwShowHide.forEach((eyeIcon) => {
    eyeIcon.addEventListener("click", () => {
        pwFields.forEach((pwField) => {
            if (pwField.type === "password") {
                pwField.type = "text";
                pwShowHide.forEach((icon) => {
                    icon.classList.replace("uil-eye-slash", "uil-eye");
                });
            } else {
                pwField.type = "password";
                pwShowHide.forEach((icon) => {
                    icon.classList.replace("uil-eye", "uil-eye-slash");
                });
            }
        });
    });
});
//appear signup and login form
signUp.addEventListener("click", (e) => {
    e.preventDefault();
    container.classList.add("active");
});
login.addEventListener("click", (e) => {
    e.preventDefault();
    container.classList.remove("active");
});




