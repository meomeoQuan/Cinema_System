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
                    CouponImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    AgeLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUpcomingMovie = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MovieImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Theaters",
                columns: table => new
                {
                    CinemaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CinemaCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theaters", x => x.CinemaID);
                    table.ForeignKey(
                        name: "FK_Theaters_AspNetUsers_AdminID",
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
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CouponID = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTables", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_OrderTables_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_Rooms_Theaters_CinemaID",
                        column: x => x.CinemaID,
                        principalTable: "Theaters",
                        principalColumn: "CinemaID",
                        onDelete: ReferentialAction.Restrict);
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
                    ShowDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ShowTimes = table.Column<TimeSpan>(type: "time", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_showTimes", x => x.ShowTimeID);
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

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    ShowtimeSeatID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    ProductID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderTables_OrderID",
                        column: x => x.OrderID,
                        principalTable: "OrderTables",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductID1",
                        column: x => x.ProductID1,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK_OrderDetails_showTimeSeats_ShowtimeSeatID",
                        column: x => x.ShowtimeSeatID,
                        principalTable: "showTimeSeats",
                        principalColumn: "ShowtimeSeatID");
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Points", "SecurityStamp", "TwoFactorEnabled", "UserImage", "UserName" },
                values: new object[] { "a1234567-b89c-40d4-a123-456789abcdef", 0, "a1234567-b89c-40d4-a123-456789abcdef", "ApplicationUser", "daoduyquylop97@gmail.com", true, "Đào Duy Quý", true, null, "daoduyquylop97@gmail.com", "Đào Duy Quý", "AQAAAAEAACcQAAAAEJ9", "0123456789", true, 0, "a1234567-b89c-40d4-a123-456789abcdef", false, "/css/images/user.png", "Đào Duy Quý" });

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
                table: "Products",
                columns: new[] { "ProductID", "Description", "Name", "Price", "ProductImage", "ProductType", "Quantity" },
                values: new object[,]
                {
                    { 1, "mix with cheese", "Popcorn Cheese", 50000.0, "/css/images/pro1.jpg", 0, 12 },
                    { 2, "Mix with Caramel", "Popcorn Caramel", 50000.0, "/css/images/pro2.jpg", 0, 10 },
                    { 3, "Mix with Caramel and Cheese", "Popcorn Mix", 50000.0, "/css/images/pro6.jpg", 0, 20 },
                    { 4, "Traditional Popcorn", "Popcorn", 50000.0, "/css/images/pro3.jpg", 0, 8 },
                    { 5, "Refreshing drink to quench your thirst.", "Boba Tea", 20000.0, "/css/images/pro4.jpg", 1, 25 },
                    { 6, "Sweet and fruity drink for a burst of flavor.", "Orange Juice", 20000.0, "/css/images/pro5.jpg", 1, 18 },
                    { 7, "Energy drink to keep you going.", "Coke 32Oz", 20000.0, "/css/images/drink2.png", 1, 12 },
                    { 8, "Classic soda for a nostalgic taste.", "Sprite", 20000.0, "/css/images/drink1.png", 1, 30 },
                    { 9, "2 coke + 1 corn cheese + 1 corn caramel", "COMBO A", 100000.0, "/css/images/popcorn1.png", 3, 8 },
                    { 10, "4 coke + 2 corn cheese + 2 corn caramel", "COMBO B", 150000.0, "/css/images/popcorn2.png", 3, 10 },
                    { 11, "1 sprite + mix popcorn cheese caramel", "COMBO C", 70000.0, "/css/images/popcorn3.png", 3, 6 },
                    { 12, "1 Teddy Bear + 1 Bottle", "Special Gift 1", 50000.0, "/css/images/gift1.jpg", 2, 3 },
                    { 13, "1 Teddy Bear + 1 Bottle", "Special Gift 1", 75000.0, "/css/images/gift3.jpg", 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Theaters",
                columns: new[] { "CinemaID", "Address", "AdminID", "CinemaCity", "ClosingTime", "CreatedAt", "Name", "NumberOfRooms", "OpeningTime", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, "456 Broadway Ave, HCM City", null, null, new TimeSpan(0, 23, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Skyline Theater", 7, new TimeSpan(0, 9, 0, 0, 0), "Open", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "124 Main St, Danang City", null, null, new TimeSpan(0, 23, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CGV Cinema", 5, new TimeSpan(0, 9, 0, 0, 0), "Open", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "124 Main St, HCM City", null, null, new TimeSpan(0, 23, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HCM Cinestar Cinema", 5, new TimeSpan(0, 9, 0, 0, 0), "Open", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomID", "Capacity", "CinemaID", "CreatedAt", "RoomNumber", "Status", "UpdatedAt" },
                values: new object[] { 2, 150, 2, null, "B1", 0, null });

            migrationBuilder.InsertData(
                table: "Theaters",
                columns: new[] { "CinemaID", "Address", "AdminID", "CinemaCity", "ClosingTime", "CreatedAt", "Name", "NumberOfRooms", "OpeningTime", "Status", "UpdatedAt" },
                values: new object[] { 1, "123 Main St, Da Nang City", "a1234567-b89c-40d4-a123-456789abcdef", null, new TimeSpan(0, 23, 0, 0, 0), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Grand Cinema", 5, new TimeSpan(0, 9, 0, 0, 0), "Open", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomID", "Capacity", "CinemaID", "CreatedAt", "RoomNumber", "Status", "UpdatedAt" },
                values: new object[] { 1, 100, 1, null, "A1", 0, null });

            migrationBuilder.InsertData(
                table: "showTimes",
                columns: new[] { "ShowTimeID", "MovieID", "RoomID", "ShowDate", "ShowTimes" },
                values: new object[,]
                {
                    { 2, 3, 2, new DateOnly(2025, 3, 10), new TimeSpan(0, 9, 3, 0, 0) },
                    { 6, 1, 2, new DateOnly(2025, 3, 11), new TimeSpan(0, 17, 3, 0, 0) },
                    { 7, 1, 2, new DateOnly(2025, 3, 11), new TimeSpan(0, 18, 3, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "SeatID", "ColumnNumber", "RoomID", "Row", "Status" },
                values: new object[,]
                {
                    { 1, 1, 1, "A", 1 },
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
                columns: new[] { "ShowTimeID", "MovieID", "RoomID", "ShowDate", "ShowTimes" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateOnly(2025, 3, 10), new TimeSpan(0, 7, 3, 0, 0) },
                    { 3, 1, 1, new DateOnly(2025, 3, 10), new TimeSpan(0, 11, 3, 0, 0) },
                    { 4, 1, 1, new DateOnly(2025, 3, 10), new TimeSpan(0, 13, 3, 0, 0) },
                    { 5, 1, 1, new DateOnly(2025, 3, 11), new TimeSpan(0, 15, 3, 0, 0) },
                    { 8, 1, 1, new DateOnly(2025, 3, 12), new TimeSpan(0, 19, 3, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "showTimeSeats",
                columns: new[] { "ShowtimeSeatID", "Price", "SeatID", "ShowtimeID", "Status" },
                values: new object[,]
                {
                    { 1, 80000.0, 1, 1, 0 },
                    { 2, 80000.0, 2, 1, 0 },
                    { 3, 80000.0, 3, 1, 0 },
                    { 4, 80000.0, 4, 1, 0 },
                    { 5, 80000.0, 5, 1, 0 },
                    { 6, 80000.0, 6, 1, 0 },
                    { 7, 80000.0, 7, 1, 0 },
                    { 8, 80000.0, 8, 1, 0 },
                    { 9, 80000.0, 9, 1, 0 },
                    { 10, 80000.0, 10, 1, 0 },
                    { 11, 80000.0, 11, 1, 0 },
                    { 12, 80000.0, 12, 1, 0 },
                    { 13, 80000.0, 13, 1, 0 },
                    { 14, 80000.0, 14, 1, 0 },
                    { 15, 80000.0, 15, 1, 0 },
                    { 16, 80000.0, 16, 1, 0 },
                    { 17, 80000.0, 17, 1, 0 },
                    { 18, 80000.0, 18, 1, 0 },
                    { 19, 80000.0, 19, 1, 0 },
                    { 20, 80000.0, 20, 1, 0 },
                    { 21, 80000.0, 21, 1, 0 },
                    { 22, 80000.0, 22, 1, 0 },
                    { 23, 80000.0, 23, 1, 0 },
                    { 24, 80000.0, 24, 1, 0 },
                    { 25, 80000.0, 25, 1, 0 },
                    { 26, 80000.0, 26, 1, 0 },
                    { 27, 80000.0, 27, 1, 0 },
                    { 28, 80000.0, 28, 1, 0 },
                    { 29, 80000.0, 29, 1, 0 },
                    { 30, 80000.0, 30, 1, 0 },
                    { 31, 80000.0, 31, 1, 0 },
                    { 32, 80000.0, 32, 1, 0 },
                    { 33, 80000.0, 33, 1, 0 },
                    { 34, 80000.0, 34, 1, 0 },
                    { 35, 80000.0, 35, 1, 0 },
                    { 36, 80000.0, 36, 1, 0 },
                    { 37, 80000.0, 37, 1, 0 },
                    { 38, 80000.0, 38, 1, 0 },
                    { 39, 80000.0, 39, 1, 0 },
                    { 40, 80000.0, 40, 1, 0 },
                    { 41, 80000.0, 41, 1, 0 },
                    { 42, 80000.0, 42, 1, 0 },
                    { 43, 80000.0, 43, 1, 0 },
                    { 44, 80000.0, 44, 1, 0 },
                    { 45, 80000.0, 45, 1, 0 },
                    { 46, 80000.0, 46, 1, 0 },
                    { 47, 80000.0, 47, 1, 0 },
                    { 48, 80000.0, 48, 1, 0 },
                    { 49, 80000.0, 49, 1, 0 },
                    { 50, 80000.0, 50, 1, 0 }
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
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID1",
                table: "OrderDetails",
                column: "ProductID1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ShowtimeSeatID",
                table: "OrderDetails",
                column: "ShowtimeSeatID");

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
                name: "IX_Theaters_AdminID",
                table: "Theaters",
                column: "AdminID");

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
                name: "UserCoupon");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "OrderTables");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "showTimeSeats");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "showTimes");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Theaters");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
