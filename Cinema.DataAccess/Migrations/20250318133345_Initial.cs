using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPercentage = table.Column<double>(type: "float", nullable: false),
                    UsageLimit = table.Column<double>(type: "float", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CouponImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponID);
                });

            migrationBuilder.CreateTable(
                name: "FoodSelectionVM",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    FoodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrailerLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgeLimit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUpcomingMovie = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MovieImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    CinemaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosingTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CinemaCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.CinemaID);
                    table.ForeignKey(
                        name: "FK_Cinemas_AspNetUsers_AdminID",
                        column: x => x.AdminID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderTables",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CouponID = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTables", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_OrderTables_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderTables_Coupons_CouponID",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "CouponID");
                });

            migrationBuilder.CreateTable(
                name: "UserCoupon",
                columns: table => new
                {
                    UserCouponID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CouponID = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupon", x => x.UserCouponID);
                    table.ForeignKey(
                        name: "FK_UserCoupon_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupon_Coupons_CouponID",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "CouponID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CinemaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK_Rooms_Cinemas_CinemaID",
                        column: x => x.CinemaID,
                        principalTable: "Cinemas",
                        principalColumn: "CinemaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    MovieName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cinema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Showtime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderTables_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTables",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    SeatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Row = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ColumnNumber = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.SeatID);
                    table.ForeignKey(
                        name: "FK_Seats_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "showTimes",
                columns: table => new
                {
                    ShowTimeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowDates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowTimes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CinemaID = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_showTimes", x => x.ShowTimeID);
                    table.ForeignKey(
                        name: "FK_showTimes_Cinemas_CinemaID",
                        column: x => x.CinemaID,
                        principalTable: "Cinemas",
                        principalColumn: "CinemaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_showTimes_Movies_MovieID",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "MovieID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_showTimes_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "showTimeSeats",
                columns: table => new
                {
                    ShowtimeSeatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowtimeID = table.Column<int>(type: "int", nullable: false),
                    SeatID = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    SeatType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_showTimeSeats", x => x.ShowtimeSeatID);
                    table.ForeignKey(
                        name: "FK_showTimeSeats_Seats_SeatID",
                        column: x => x.SeatID,
                        principalTable: "Seats",
                        principalColumn: "SeatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_showTimeSeats_showTimes_ShowtimeID",
                        column: x => x.ShowtimeID,
                        principalTable: "showTimes",
                        principalColumn: "ShowTimeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "CinemaID", "Address", "AdminID", "CinemaCity", "ClosingTime", "CreatedAt", "Name", "NumberOfRooms", "OpeningTime", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Main St, Da Nang City", null, "Danang", "23:00", null, "Grand Cinema", 5, "09:00", "Open", null },
                    { 2, "456 Broadway Ave, HCM City", null, "Ho Chi Minh", "22:30", null, "Skyline Theater", 7, "10:00", "Open", null },
                    { 3, "124 Main St, Danang City", null, "Danang", "23:00", null, "CGV Cinema", 5, "09:00", "Open", null },
                    { 4, "124 Main St, HCM City", null, "Ho Chi Minh", "23:00", null, "HCM Cinestar Cinema", 5, "09:00", "Open", null }
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponID", "Code", "CouponImage", "DiscountPercentage", "ExpireDate", "UsageLimit", "UsedCount" },
                values: new object[] { 1, "TEST10", "", 0.10000000000000001, null, 10.0, 1 });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieID", "AgeLimit", "CreatedAt", "Duration", "Genre", "IsUpcomingMovie", "MovieImage", "ReleaseDate", "Synopsis", "Title", "TrailerLink", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "13+", null, 148, "Sci-Fi", false, "https://m.media-amazon.com/images/I/51oBxmV-dML._AC_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A thief who enters the dreams of others to steal secrets.", "Inception", "https://www.youtube.com/watch?v=YoHD9XEInc0", null },
                    { 2, "16+", null, 152, "Action", false, "https://m.media-amazon.com/images/I/A1exRxgHRRL.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batman faces the Joker, a criminal mastermind who brings chaos to Gotham.", "The Dark Knight", "https://www.youtube.com/watch?v=EXeTwQWrcwY", null },
                    { 3, "10+", null, 169, "Sci-Fi", false, "https://m.media-amazon.com/images/I/91kFYg4fX3L._AC_SL1500_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A team of explorers travel through a wormhole in space in an attempt to save humanity.", "Interstellar", "https://www.youtube.com/watch?v=zSWdZVtXT7E", null },
                    { 4, "12+", null, 192, "Adventure", false, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTfTiEWQYqVZHQuVHy6G9PQIUfa5ujUpy0e7fZ-t6TwN19glQiAuhNS3PkWt-v48Lr9pIE&usqp=CAU", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jake Sully and Neytiri must protect their family from an old enemy on Pandora.", "Avatar: The Way of Water", "https://www.youtube.com/watch?v=d9MyW72ELq0", null },
                    { 5, "13+", null, 155, "Sci-Fi", false, "https://m.media-amazon.com/images/I/81MUHYLUf6L._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A noble family's son leads a rebellion on a desert planet.", "Dune", "https://www.youtube.com/watch?v=n9xhJrPXop4", null },
                    { 6, "18+", null, 169, "Action", false, "https://m.media-amazon.com/images/I/71tIm0Xxr2L._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John Wick takes on the High Table in his most dangerous fight.", "John Wick 4", "https://www.youtube.com/watch?v=qEVUtrk8_B4", null },
                    { 7, "16+", null, 180, "Biography", false, "https://m.media-amazon.com/images/I/71qu4p5bnDL._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The story of J. Robert Oppenheimer and the atomic bomb.", "Oppenheimer", "https://www.youtube.com/watch?v=bK6ldnjE3Y0", null },
                    { 8, "13+", null, 148, "Superhero", false, "https://m.media-amazon.com/images/I/71niXI3lxlL._AC_SL1500_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spider-Man fights villains from multiple universes.", "Spider-Man: No Way Home", "https://www.youtube.com/watch?v=JfVOs4VSpmA", null },
                    { 9, "16+", null, 148, "Sci-Fi", false, "https://m.media-amazon.com/images/I/71PQje4I99L.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Neo returns to the Matrix for a new journey.", "The Matrix Resurrections", "https://www.youtube.com/watch?v=9ix7TUGVYIo", null },
                    { 10, "18+", null, 120, "Action/Comedy", true, "https://m.media-amazon.com/images/I/71wNKMs+CvL._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Deadpool returns with more fourth-wall-breaking humor.", "Deadpool 3", "", null },
                    { 11, "16+", null, 150, "Action", true, "https://m.media-amazon.com/images/I/61NCZ4VQ8EL._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Dark Knight faces a new enemy in Gotham.", "The Batman 2", "", null },
                    { 12, "12+", null, 180, "Adventure", true, "https://m.media-amazon.com/images/I/61SNSxk3RNL._AC_UF894,1000_QL80_.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Na'vi continue their fight against human invaders.", "Avatar 3", "", null },
                    { 13, "13+", null, 140, "Superhero", true, "https://m.media-amazon.com/images/I/81TBhA6kgBL.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marvel's First Family joins the MCU.", "Fantastic Four", "", null },
                    { 14, "All Ages", null, 100, "Animation/Comedy", true, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTJgULr6iPFNLydknD-UKqWcCsfyUZVmJrjuw&s", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shrek and friends return for a new adventure.", "Shrek 5", "", null }
                });

            migrationBuilder.InsertData(
                table: "OrderTables",
                columns: new[] { "OrderID", "CouponID", "CreatedAt", "Status", "TotalAmount", "UpdatedAt", "UserID" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 124235.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, null, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 747237.65399999998, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 3, null, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 50000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 4, null, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 60000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 5, null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 70000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 6, null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 80000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 7, null, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 90000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 8, null, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 100000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 9, null, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 110000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 10, null, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 120000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 11, null, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 130000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 12, null, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 140000.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "Description", "Name", "Price", "ProductImage", "ProductType", "Quantity" },
                values: new object[,]
                {
                    { 1, "A large bucket of buttered popcorn.", "Popcorn", 5.9900000000000002, "", 0, 50 },
                    { 2, "Refreshing cold soda, 500ml.", "Soda", 2.9900000000000002, "", 1, 100 }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "OrderDetailID", "Cinema", "City", "Date", "MovieId", "MovieName", "OrderID", "Price", "Quantity", "RoomId", "RoomName", "Showtime" },
                values: new object[,]
                {
                    { 1, "Grand Cinema", "Danang", "01/03/2025", 1, "Inception", 1, 10.0, 2, 1, "A1", "18:30" },
                    { 2, "Skyline Theater", "Ho Chi Minh", "01/03/2025", 2, "The Dark Knight", 2, 15.0, 3, 2, "B1", "20:15" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomID", "Capacity", "CinemaID", "CreatedAt", "RoomNumber", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 100, 1, null, "A1", 0, null },
                    { 2, 150, 2, null, "B1", 0, null }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "SeatID", "ColumnNumber", "RoomID", "Row", "Status" },
                values: new object[,]
                {
                    { 1, 1, 1, "A", 0 },
                    { 2, 2, 1, "A", 0 },
                    { 3, 3, 1, "A", 0 },
                    { 4, 4, 1, "A", 0 },
                    { 5, 5, 1, "A", 0 },
                    { 6, 6, 1, "A", 0 },
                    { 7, 7, 1, "A", 0 },
                    { 8, 8, 1, "A", 0 },
                    { 9, 9, 1, "A", 0 },
                    { 10, 10, 1, "A", 0 },
                    { 11, 1, 1, "B", 0 },
                    { 12, 2, 1, "B", 0 },
                    { 13, 3, 1, "B", 0 },
                    { 14, 4, 1, "B", 0 },
                    { 15, 5, 1, "B", 0 },
                    { 16, 6, 1, "B", 0 },
                    { 17, 7, 1, "B", 0 },
                    { 18, 8, 1, "B", 0 },
                    { 19, 9, 1, "B", 0 },
                    { 20, 10, 1, "B", 0 },
                    { 21, 1, 1, "C", 0 },
                    { 22, 2, 1, "C", 0 },
                    { 23, 3, 1, "C", 0 },
                    { 24, 4, 1, "C", 0 },
                    { 25, 5, 1, "C", 0 },
                    { 26, 6, 1, "C", 0 },
                    { 27, 7, 1, "C", 0 },
                    { 28, 8, 1, "C", 0 },
                    { 29, 9, 1, "C", 0 },
                    { 30, 10, 1, "C", 0 },
                    { 31, 1, 1, "D", 0 },
                    { 32, 2, 1, "D", 0 },
                    { 33, 3, 1, "D", 0 },
                    { 34, 4, 1, "D", 0 },
                    { 35, 5, 1, "D", 0 },
                    { 36, 6, 1, "D", 0 },
                    { 37, 7, 1, "D", 0 },
                    { 38, 8, 1, "D", 0 },
                    { 39, 9, 1, "D", 0 },
                    { 40, 10, 1, "D", 0 },
                    { 41, 1, 1, "E", 0 },
                    { 42, 2, 1, "E", 0 },
                    { 43, 3, 1, "E", 0 },
                    { 44, 4, 1, "E", 0 },
                    { 45, 5, 1, "E", 0 },
                    { 46, 6, 1, "E", 0 },
                    { 47, 7, 1, "E", 0 },
                    { 48, 8, 1, "E", 0 },
                    { 49, 9, 1, "E", 0 },
                    { 50, 10, 1, "E", 0 }
                });

            migrationBuilder.InsertData(
                table: "showTimes",
                columns: new[] { "ShowTimeID", "CinemaID", "MovieID", "RoomID", "ShowDates", "ShowTimes" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "01/03/2025", "18:30" },
                    { 2, 2, 2, 2, "01/03/2025", "20:15" },
                    { 3, 1, 1, 1, "01/03/2025", "23:00" },
                    { 4, 3, 1, 1, "08/03/2025", "21:00" },
                    { 5, 3, 1, 1, "10/03/2025", "23:00" },
                    { 6, 4, 1, 2, "19/03/2025", "17:00" },
                    { 7, 3, 1, 2, "01/03/2025", "17:00" },
                    { 8, 4, 1, 1, "01/03/2025", "19:00" }
                });

            migrationBuilder.InsertData(
                table: "showTimeSeats",
                columns: new[] { "ShowtimeSeatID", "Price", "SeatID", "SeatType", "ShowtimeID", "Status" },
                values: new object[,]
                {
                    { 1, 10.0, 1, 0, 1, 0 },
                    { 2, 10.0, 2, 0, 1, 0 },
                    { 3, 10.0, 3, 0, 1, 0 },
                    { 4, 10.0, 4, 0, 1, 0 },
                    { 5, 10.0, 5, 0, 1, 0 },
                    { 6, 10.0, 6, 0, 1, 0 },
                    { 7, 10.0, 7, 0, 1, 0 },
                    { 8, 10.0, 8, 0, 1, 0 },
                    { 9, 10.0, 9, 0, 1, 0 },
                    { 10, 10.0, 10, 0, 1, 0 },
                    { 11, 10.0, 11, 0, 1, 0 },
                    { 12, 10.0, 12, 0, 1, 0 },
                    { 13, 10.0, 13, 0, 1, 0 },
                    { 14, 10.0, 14, 0, 1, 0 },
                    { 15, 10.0, 15, 0, 1, 0 },
                    { 16, 10.0, 16, 0, 1, 0 },
                    { 17, 10.0, 17, 0, 1, 0 },
                    { 18, 10.0, 18, 0, 1, 0 },
                    { 19, 10.0, 19, 0, 1, 0 },
                    { 20, 10.0, 20, 0, 1, 0 },
                    { 21, 10.0, 21, 0, 1, 0 },
                    { 22, 10.0, 22, 0, 1, 0 },
                    { 23, 10.0, 23, 0, 1, 0 },
                    { 24, 10.0, 24, 0, 1, 0 },
                    { 25, 10.0, 25, 0, 1, 0 },
                    { 26, 10.0, 26, 0, 1, 0 },
                    { 27, 10.0, 27, 0, 1, 0 },
                    { 28, 10.0, 28, 0, 1, 0 },
                    { 29, 10.0, 29, 0, 1, 0 },
                    { 30, 10.0, 30, 0, 1, 0 },
                    { 31, 10.0, 31, 0, 1, 0 },
                    { 32, 10.0, 32, 0, 1, 0 },
                    { 33, 10.0, 33, 0, 1, 0 },
                    { 34, 10.0, 34, 0, 1, 0 },
                    { 35, 10.0, 35, 0, 1, 0 },
                    { 36, 10.0, 36, 0, 1, 0 },
                    { 37, 10.0, 37, 0, 1, 0 },
                    { 38, 10.0, 38, 0, 1, 0 },
                    { 39, 10.0, 39, 0, 1, 0 },
                    { 40, 10.0, 40, 0, 1, 0 },
                    { 41, 10.0, 41, 0, 1, 0 },
                    { 42, 10.0, 42, 0, 1, 0 },
                    { 43, 10.0, 43, 0, 1, 0 },
                    { 44, 10.0, 44, 0, 1, 0 },
                    { 45, 10.0, 45, 0, 1, 0 },
                    { 46, 10.0, 46, 0, 1, 0 },
                    { 47, 10.0, 47, 0, 1, 0 },
                    { 48, 10.0, 48, 0, 1, 0 },
                    { 49, 10.0, 49, 0, 1, 0 },
                    { 50, 10.0, 50, 0, 1, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cinemas_AdminID",
                table: "Cinemas",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTables_CouponID",
                table: "OrderTables",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTables_UserID",
                table: "OrderTables",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CinemaID",
                table: "Rooms",
                column: "CinemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_RoomID",
                table: "Seats",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_showTimes_CinemaID",
                table: "showTimes",
                column: "CinemaID");

            migrationBuilder.CreateIndex(
                name: "IX_showTimes_MovieID",
                table: "showTimes",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_showTimes_RoomID",
                table: "showTimes",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_showTimeSeats_SeatID",
                table: "showTimeSeats",
                column: "SeatID");

            migrationBuilder.CreateIndex(
                name: "IX_showTimeSeats_ShowtimeID",
                table: "showTimeSeats",
                column: "ShowtimeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupon_CouponID",
                table: "UserCoupon",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupon_UserID",
                table: "UserCoupon",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FoodSelectionVM");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "showTimeSeats");

            migrationBuilder.DropTable(
                name: "UserCoupon");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "OrderTables");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "showTimes");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
