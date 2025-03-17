$(document).ready(function () {
    // Initialize core variables
    const movieID = $("#movieID").val();
    const IdUser = $("#targetTime").val();
    let allShowtimes = [];

    // Main AJAX request
    $.ajax({
        url: `/Guest/Detail/Details?userId=${IdUser}&MovieID=${movieID}`,
        method: "GET",
        success: function (response) {
            handleSuccessResponse(response);
        },
        error: handleErrorResponse
    });
    //====================== Populate hidden input fields==================
    $("#movieID").val(movieID);
    //====================== Populate hidden input fields==================
    function handleSuccessResponse(response) {
        console.log("Response received:", response);

        if (response.message !== "Success") {
            showErrorMessage("#movieDetails", "Failed to load movie details.");
            return;
        }

        const data = response.data;
        allShowtimes = data.showtimes || [];

        initializeFilters(
            data.availableDates.map(d => d.date),
            data.availableCities.map(c => c.city)
        );
        renderFoodItems(data.foodItems);
    }

    function initializeFilters(dates, cities) {
        createDateFilters("#filterDates", dates);
        createCityFilter("#filterCities", cities);
        applyFilters(); // Initial filter application
    }

    function createDateFilters(container, dates) {
        const buttonsHtml = dates.map((date, index) => `
            <button class="btn btn-outline-warning px-3 py-2 mt-3 date-toggle ${index === 0 ? 'active' : ''}" 
                    data-value="${date}" 
                    style="width: 120px; height: 120px;">
                ${date}
            </button>
        `).join(" ");

        $(container).html(buttonsHtml)
            .find(".date-toggle").click(function () {
                $(".date-toggle").removeClass("active");
                $(this).addClass("active");
                applyFilters();
            });
    }

    function createCityFilter(container, cities) {
        const dropdownHtml = `
            <select id="citySelect" class="form-select form-select-sm mb-2">
                <option value="">All Cities</option>
                ${cities.map(c => `<option value="${c}">${c}</option>`).join("")}
            </select>`;

        $(container).html(dropdownHtml)
            .find("select").on("change", applyFilters);
    }

    function applyFilters() {
        const filters = {
            date: getActiveDateFilter(),
            city: $("#citySelect").val()
        };

        const filtered = allShowtimes.filter(show =>
            (!filters.date.length || filters.date.includes(show.date)) &&
            (!filters.city || show.city === filters.city)
        );

        updateShowtimesDisplay(filtered);
        updateHiddenInputs(filters);
    }
    //====================== Populate hidden input fields==================
    function updateHiddenInputs(filters) {
        $("#selectedDate").val(filters.date.length ? filters.date[0] : ""); // Assuming single date selection
        $("#selectedCity").val(filters.city || "");
    }

    //====================== Populate hidden input fields==================



    function getActiveDateFilter() {
        return $("#filterDates .date-toggle.active").map(function () {
            return $(this).data("value");
        }).get();
    }

    function updateShowtimesDisplay(showtimes) {
        const cinemaMap = showtimes.reduce((acc, show) => {
            acc[show.cinemaName] = acc[show.cinemaName] || [];
            acc[show.cinemaName].push(show);
            return acc;
        }, {});

        const displayHtml = Object.entries(cinemaMap).map(([cinema, shows]) => `
        <div class="card sci-fi-card mb-3 w-100">
            <h5 class="card-title">${cinema}</h5>
            <p>${shows[0].cinemaAddress}</p> 
            <div class="d-flex flex-wrap gap-2">
                ${shows.map(show => `
                    <button class="btn sci-fi-btn showtime-btn" 
                            data-showtime='${JSON.stringify(show)}'
                            data-showtimeId='${show.showtimeId}'>
                        ${show.showtime}
                    </button>
                `).join("")}
            </div>
        </div>
    `).join("") || "<p class='mb-3 text-white'>No showtimes available</p>";

        $("#movieDetails").html(displayHtml)
            .find(".showtime-btn").click(handleShowtimeSelection);
    }


    function handleShowtimeSelection() {
        $(".showtime-btn").removeClass("btn-primary").addClass("btn-outline-primary");
        const $btn = $(this).removeClass("btn-outline-primary").addClass("btn-primary");

        const showtime = JSON.parse($btn.attr("data-showtime"));

        //====================== Populate hidden input fields==================
        //$("#selectedCinema").val(showtime.cinemaId);
        //$("#RoomId").val(showtime.roomId);
        //$("#RoomName").val(showtime.roomName);
        $("#selectedShowtime").val(showtime.showtimeId);
        //=====================================================================
        //renderSeats(showtime.seatList);
        //renderTicketList(showtime.ticketList);
    }






    function handleErrorResponse() {
        showErrorMessage("#movieDetails", "Error loading movie details.");
    }

    function showErrorMessage(container, message) {
        $(container).html(`<p class="text-danger mb-3">${message}</p>`);
    }





});






function handleFormSubmission(event) {
    event.preventDefault(); // Prevent normal form submission

    // Collect all required data
    $("#MovieName").val($("#displayMovieName").text());
    $("#selectedDate").val(getActiveDateFilter());
    $("#selectedCity").val($("#citySelect").val());
    $("#selectedCinema").val($(".showtime-btn.btn-primary").closest(".card").find(".card-title").text());
    $("#selectedShowtime").val($(".showtime-btn.btn-primary").text());
    $("#RoomId").val($(".showtime-btn.btn-primary").data("showtime").roomId);
    $("#RoomName").val($(".showtime-btn.btn-primary").data("showtime").roomName);

    // Get seats, tickets, and food
    let selectedSeats = $(".seat.selected").map(function () {
        return { seatId: $(this).data("seat-id") }; // showtimeseat
    }).get();
    $("#SelectedSeatsJson").val(JSON.stringify(selectedSeats));
    //$("#TicketsJson").val(JSON.stringify(getSelectedTickets()));
    $("#FoodItemsJson").val(JSON.stringify(getSelectedFood()));

    // Update total price
    $("#TotalPrice").val(calculateTotalPrice(getSelectedTickets(), getSelectedFood()).toFixed(2));

    // Submit the form
    $("#booking-summary").submit();
}

// Event listener for form submission
$("#booking-summary").on("submit", handleFormSubmission);