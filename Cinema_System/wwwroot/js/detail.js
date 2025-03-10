$(document).ready(function () {
    initializeMovieDetails();
});

// ==============================================
// INITIALIZATION
// ==============================================
function initializeMovieDetails() {
    const movieID = getMovieID();
    const { targetDate, targetCity, targetTime } = getUrlParams();

    if (movieID) {
        fetchMovieDetails(movieID, targetDate, targetCity, targetTime);
    } else {
        showErrorMessage("Movie ID not found");
    }
}

// ==============================================
// DATA FETCHING
// ==============================================
function fetchMovieDetails(movieID, targetDate, targetCity, targetTime) {
    $.ajax({
        url: buildDetailsUrl(movieID, targetDate, targetCity, targetTime),
        type: "GET",
        success: handleDetailsSuccess,
        error: handleDetailsError
    });
}

// ==============================================
// RESPONSE HANDLING
// ==============================================
function handleDetailsSuccess(response) {
    if (response.message === "Success" && response.data) {
        updateAllSections(response.data);
    } else {
        showErrorMessage(response.message || "No data available");
    }
}

function handleDetailsError(xhr, status, error) {
    console.error("API Error:", error);
    showErrorMessage("Failed to load movie details");
}

// ==============================================
// UI UPDATES
// ==============================================
function updateAllSections(data) {
    updateMovieInfoSection(data);
    updateShowtimesSection(data.showtimes);
}

function updateMovieInfoSection(data) {
    $("#moviePoster").attr("src", data.movieImage || "/default-image.jpg");
    $("#movieTitle").text(data.movieName || "Unknown Movie");
    $("#movieGenre").text(data.genre || "N/A");
    $("#movieDuration").text(data.duration || "N/A");
    $("#movieReleaseDate").text(data.date || "N/A");
    $("#movieSynopsis").text(data.synopsis || "No synopsis available.");
    $("#trailerLink").attr("href", data.trailerLink || "#");
}

function updateShowtimesSection(showtimes) {
    const showtimesHTML = showtimes && showtimes.length > 0
        ? buildShowtimesCards(showtimes)
        : "<p class='text-danger'>No showtimes available</p>";

    $("#showtimes").html(`<div class="row">${showtimesHTML}</div>`);
}

// ==============================================
// COMPONENT BUILDERS
// ==============================================
function buildShowtimesCards(showtimes) {
    return showtimes.map(showtime => `
        <div class="col-md-6 mb-3">
            <div class="card p-3">
                ${buildShowtimeHeader(showtime)}
                ${buildTicketsList(showtime.tickets)}
            </div>
        </div>`
    ).join('');
}

function buildShowtimeHeader(showtime) {
    return `
        <h5 class="text-primary">Cinema: ${showtime.cinemaName}</h5>
        <p><strong>Room:</strong> ${showtime.roomName}</p>
        <p><strong>Showtime:</strong> ${showtime.showtime}</p>
        <h6>🎟️ Available Tickets</h6>`;
}

function buildTicketsList(tickets) {
    if (!tickets || tickets.length === 0) {
        return `<ul class="list-group">
            <li class="list-group-item text-muted">No tickets available</li>
        </ul>`;
    }

    return `<ul class="list-group">
        ${tickets.map(ticket => `
            <li class="list-group-item">
                Seat: <strong>${ticket.seatNumber}</strong>
                (${ticket.seatType}) - $${ticket.price}
            </li>`
    ).join('')}
    </ul>`;
}

// ==============================================
// UTILITIES
// ==============================================
function getMovieID() {
    const urlParams = new URLSearchParams(window.location.search);
    return $("#movieId").val() || urlParams.get("MovieID");
}

function getUrlParams() {
    const urlParams = new URLSearchParams(window.location.search);
    return {
        targetDate: urlParams.get("targetDate") || "01/03/2025",
        targetCity: urlParams.get("targetCity"),
        targetTime: urlParams.get("targetTime")
    };
}

function buildDetailsUrl(movieID, targetDate, targetCity, targetTime) {
    const params = new URLSearchParams({
        MovieID: movieID,
        targetDate,
        targetCity,
        targetTime
    });
    return `/Guest/Detail/Details?${params.toString()}`;
}

function showErrorMessage(message) {
    $("#movie-info").html(`
        <div class="alert alert-danger" role="alert">
            ${message}
        </div>
    `);
}