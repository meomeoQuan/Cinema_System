$(document).ready(function () {
    var movieID = $("#movieID").val();
    var IdUser = $("#targetTime").val();
    var allShowtimes = []; // Store all showtimes for filtering

    $.ajax({
        url: `/Guest/Detail/Details?userId=${IdUser}&MovieID=${movieID}`,
        method: "GET",
        success: function (response) {
            console.log("Response received:", response);

            if (response.message !== "Success") {
                $("#movieDetails").html("<p>Failed to load movie details.</p>");
                return;
            }

            var data = response.data;
            var availableDates = data.availableDates.map(d => d.date);
            var availableCities = data.availableCities.map(c => c.city);
            allShowtimes = data.showtimes || [];

            createToggleButtons("#filterDates", availableDates);
            createDropdown("#filterCities", availableCities);
            updateShowtimes(allShowtimes);

            $(".date-toggle").click(function () {
                $(this).toggleClass("active");
                applyFilters();
            });

            // Ensure the city selection triggers filtering
            $("#filterCities").on("change", function () {
                applyFilters();
            });
        },
        error: function () {
            $("#movieDetails").html("<p>Error loading movie details.</p>");
        }
    });

    function applyFilters() {
        var selectedDates = getActiveToggleValues("#filterDates");
        var selectedCity = $("#filterCities").val(); // Changed to get value from the correct dropdown

        var filteredShowtimes = filterShowtimes(allShowtimes, {
            date: selectedDates,
            city: selectedCity ? [selectedCity] : [] // Ensure city filter works
        });

        updateShowtimes(filteredShowtimes);
    }

    function filterShowtimes(showtimes, filters) {
        return showtimes.filter(show =>
            (!filters.date.length || filters.date.includes(show.date)) &&
            (!filters.city.length || show.city === filters.city[0]) // Fix city filtering
        );
    }

    function updateShowtimes(showtimes) {
        var cinemaMap = {};

        showtimes.forEach(show => {
            if (!cinemaMap[show.cinemaName]) {
                cinemaMap[show.cinemaName] = [];
            }
            cinemaMap[show.cinemaName].push(show);
        });

        var formattedShowtimes = "<p><strong>Showtimes:</strong></p>";

        formattedShowtimes += Object.entries(cinemaMap)
            .map(([cinemaName, shows]) =>
                `<p><strong>${cinemaName}</strong> - ` +
                shows.map(show => `<button class="btn btn-outline-primary showtime-btn" data-showtime='${JSON.stringify(show)}'>${show.showtime}</button>`).join(" ") +
                `</p>`
            )
            .join("") || "<p>No showtimes available</p>";

        $("#movieDetails").html(formattedShowtimes);

        $(".showtime-btn").click(function () {
            $(".showtime-btn").removeClass("btn-primary").addClass("btn-outline-primary");
            $(this).removeClass("btn-outline-primary").addClass("btn-primary");
            let selectedShowtime = JSON.parse($(this).attr("data-showtime"));
            renderSeats(selectedShowtime.seatList);
        });
    }

    function renderSeats(seats) {
        var seatsContainer = $("#seatsContainer").empty().addClass("seat-grid");

        seats.forEach(seat => {
            let seatClass = "seat";
            if (seat.selected) seatClass += " booked";
            if (seat.maintenance) seatClass += " maintenance";

            let seatDiv = $(`<div class="${seatClass}" data-seat-id="${seat.seatId}">${seat.seatNumber}</div>`);
            seatDiv.click(function () {
                if (!$(this).hasClass("booked") && !$(this).hasClass("maintenance")) {
                    $(this).toggleClass("selected");
                    updateBookingSummary();
                }
            });

            seatsContainer.append(seatDiv);
        });
    }

    function updateBookingSummary() {
        if ($(".seat.selected").length > 0) {
            $("#booking-summary").removeClass("d-none");
        } else {
            $("#booking-summary").addClass("d-none");
        }
    }

    function createToggleButtons(container, values) {
        var buttonsHtml = values.map(value =>
            `<button class="btn btn-outline-primary date-toggle" data-value="${value}">${value}</button>`
        ).join(" ");
        $(container).html(buttonsHtml);
    }

    function createDropdown(container, values) {
        var dropdownHtml = `<select id="citySelect" class="form-select">
            <option value="">All Cities</option>
            ${values.map(value => `<option value="${value}">${value}</option>`).join("")}
        </select>`;
        $(container).html(dropdownHtml);
    }

    function getActiveToggleValues(container) {
        return $(`${container} .date-toggle.active`).map(function () {
            return $(this).attr("data-value");
        }).get();
    }
});