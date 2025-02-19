$(document).ready(function () {
    loadMovies(1, 1, 1); // Load all movies on page load

    // Event Listeners for Previous & Next Buttons
    $(".btn-prev").click(function () {
        let category = $(this).data("category");
        let page = parseInt($(`#${category}-page`).attr("data-page")) - 1;
        if (page > 0) updateMovies(category, page);
    });

    $(".btn-next").click(function () {
        let category = $(this).data("category");
        let page = parseInt($(`#${category}-page`).attr("data-page")) + 1;
        updateMovies(category, page);
    });
});

function loadMovies(showingPage, upcomingPage, couponPage) {
    // Preserve current page values if null
    showingPage = showingPage || parseInt($("#showing-page").attr("data-page"));
    upcomingPage = upcomingPage || parseInt($("#upcoming-page").attr("data-page"));
    couponPage = couponPage || parseInt($("#coupon-page").attr("data-page"));

    $.ajax({
        url: `/Guest/Home/GetMovies?Showingpage=${showingPage}&Upcommingpage=${upcomingPage}&CouponPage=${couponPage}`,
        type: "GET",
        success: function (response) {
            if (response.message === "Success") {
                let data = response.data || {};

                let CountShowingMovies = data.showingMoviesCount;
                let CountUpcomingMovies = data.upcommingMoviesCount;
                let CountCouponMovies = data.couponCount;
                let pageSize = data.pageSize;

                let totalPagesShowing = Math.ceil(CountShowingMovies / pageSize);
                let totalPagesUpcoming = Math.ceil(CountUpcomingMovies / pageSize);
                let totalPagesCoupons = Math.ceil(CountCouponMovies / pageSize);

                // Update Movies Content
                updateMovieSection("showing", data.showingMovies, totalPagesShowing, showingPage);
                updateMovieSection("upcoming", data.upcommingMovies, totalPagesUpcoming, upcomingPage);
                updateMovieSection("coupon", data.couponMovies, totalPagesCoupons, couponPage);
            } else {
                alert("Failed to load movies.");
            }
        },
        error: function () {
            alert("Error fetching data from API.");
        }
    });
}

// Function to Update Movie Sections & Pagination
function updateMovieSection(category, movies, totalPages, currentPage) {
    let container = $(`#${category}-movies`);
    let paginationContainer = $(`#${category}-pagination`);

    // Clear old data
    container.html("");
    paginationContainer.html("");

    // Append Movies
    movies.forEach(movie => {
        container.append(`<div><h3>${movie.genre}</h3><p>${movie.title}</p></div>`);
    });

    // Update Pagination
    updatePagination(category, totalPages, currentPage);
}

// Function to Generate Pagination Buttons
function updatePagination(category, totalPages, currentPage) {
    let paginationHtml = "";

    for (let i = 1; i <= totalPages; i++) {
        let activeClass = i === currentPage ? 'active-page' : '';
        paginationHtml += `<span class="page-number ${activeClass}" data-page="${i}" data-category="${category}">${i}</span>`;
    }

    $(`#${category}-pagination`).html(paginationHtml);

    // Click Event for Page Numbers
    $(".page-number").click(function () {
        let page = parseInt($(this).attr("data-page"));
        let category = $(this).attr("data-category");
        updateMovies(category, page);
    });

    // Update Page Number Display
    $(`#${category}-page`).attr("data-page", currentPage);
}

// Function to Load Movies for a Given Category
function updateMovies(category, page) {
    if (category === "showing") loadMovies(page, null, null);
    if (category === "upcoming") loadMovies(null, page, null);
    if (category === "coupon") loadMovies(null, null, page);
}
