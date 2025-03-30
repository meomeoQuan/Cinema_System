// HI?N TH? R?P THEO THÀNH PH?
//document.addEventListener("DOMContentLoaded", function () {
//    let cinemaCityDropdown = document.getElementById("cinemaCity");
//    let cinemaDropdown = document.getElementById("cinema");
//    // S? KI?N CH?N THÀNH PH? -> L?C R?P
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
//        // ??t l?i dropdown r?p v? m?c ??nh sau khi l?c
//        cinemaDropdown.selectedIndex = 0;
//    });
//    // ?? S? KI?N CH?N R?P -> C?P NH?T THÀNH PH? ??
//    cinemaDropdown.addEventListener("change", function () {
//        let selectedTheater = this.options[this.selectedIndex];
//        // N?u ch?n "Select a Theater" thì c?ng ??t l?i thành ph?
//        if (selectedTheater.value === "") {
//            cinemaCityDropdown.selectedIndex = 0; // ??t v? "Select a City"
//        } else {
//            let city = selectedTheater.getAttribute("data-city");
//            if (city) {
//                cinemaCityDropdown.value = city;
//            }
//        }
//    });
//});
document.addEventListener("DOMContentLoaded", function() {
    // Load danh sách thành ph? khi vào trang
    fetch("/api/details/cities")
        .then(response => response.json())
        .then(data => {
            let cityDropdown = document.getElementById("cinemaCity");
            data.forEach(city => {
                //let option = document.createElement("option");
                //option.value = city.data;
                //option.textContent = city.data;
                //cityDropdown.appendChild(option);
                console.log(city);
            });
        });

    // Hi?n r?p theo thành ph? ?ã ch?n
    document.getElementById("cinemaCity").addEventListener("change", function() {
        let cinemaCityName = this.value;
        let cinemaDropdown = document.getElementById("cinema");

        cinemaDropdown.innerHTML = '<option value="">Ch?n r?p phim</option>';
        let dateDropdown = document.getElementById("date");

        dateDropdown.innerHTML = '<option value="">Ch?n ngày chi?u</option>';
        let timeDropdown = document.getElementById("time");

        timeDropdown.innerHTML = '<option value="">Ch?n gi? chi?u</option>';

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
    });

    // Hi?n ngày theo r?p ?ã ch?n
    document.getElementById("cinema").addEventListener("change", function() {
        let cinemaId = this.value;
        let dateDropdown = document.getElementById("date");
        const movieId = document.querySelector('input[name="Movie.MovieID"]').value;

        dateDropdown.innerHTML = '<option value="">Ch?n ngày chi?u</option>';

        let timeDropdown = document.getElementById("time");

        timeDropdown.innerHTML = '<option value="">Ch?n gi? chi?u</option>';

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
                .catch(error => console.error("L?i:", error));
        }
    });


    // Hi?n gi? theo ngày ?ã ch?n
    document.getElementById("date").addEventListener("change", function() {
        let cinemaId = document.getElementById("cinema").value;
        const movieId = document.querySelector('input[name="Movie.MovieID"]').value;
        let timeDropdown = document.getElementById("time");
        let dateChoose = this.value;

        timeDropdown.innerHTML = '<option value="">Ch?n gi? chi?u</option>';

        if (cinemaId) {
            fetch(`/api/showtime/${cinemaId}/${movieId}`)
                .then(response => response.json())
                .then(data => {
                    data.forEach(showtime => {
                        let dateObj = new Date(showtime.showDate);
                        let formattedDate = new Date(showtime.showDate).toISOString().split("T")[0];
                        if (dateChoose === formattedDate) {

                            // Format gi? theo "HH:mm" (24h)
                            let formattedTime = dateObj.toLocaleTimeString("vi-VN", {
                                hour: "2-digit",
                                minute: "2-digit",
                                hour12: false, // Dùng h? 24 gi?
                            });

                            let option = document.createElement("option");
                            option.textContent = formattedTime;
                            option.value = formattedTime;
                            timeDropdown.appendChild(option);
                        }
                    });
                })
                .catch(error => console.error("L?i:", error));
        }
    });
});
