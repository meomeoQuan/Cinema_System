using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLatestTodbs : Migration
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
                name: "Room",
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
                    table.PrimaryKey("PK_Room", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK_Room_Cinemas_CinemaID",
                        column: x => x.CinemaID,
                        principalTable: "Cinemas",
                        principalColumn: "CinemaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seat",
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
                    table.PrimaryKey("PK_Seat", x => x.SeatID);
                    table.ForeignKey(
                        name: "FK_Seat_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
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
                        name: "FK_showTimes_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "CinemaID", "Address", "AdminID", "ClosingTime", "CreatedAt", "Name", "NumberOfRooms", "OpeningTime", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Main St, City", null, "23:00", null, "Grand Cinema", 5, "09:00", "Open", null },
                    { 2, "456 Broadway Ave, City", null, "22:30", null, "Skyline Theater", 7, "10:00", "Open", null }
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
                table: "Products",
                columns: new[] { "ProductID", "Description", "Name", "Price", "ProductImage", "ProductType", "Quantity" },
                values: new object[,]
                {
                    { 1, "A large bucket of buttered popcorn.", "Popcorn", 5.9900000000000002, "", 0, 50 },
                    { 2, "Refreshing cold soda, 500ml.", "Soda", 2.9900000000000002, "", 1, 100 }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "RoomID", "Capacity", "CinemaID", "CreatedAt", "RoomNumber", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 100, 1, null, "A1", 0, null },
                    { 2, 150, 2, null, "B1", 0, null }
                });

            migrationBuilder.InsertData(
                table: "showTimes",
                columns: new[] { "ShowTimeID", "CinemaID", "MovieID", "RoomID", "ShowDates", "ShowTimes" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "01/03/2025", "18:30" },
                    { 2, 2, 2, 2, "01/03/2025", "20:15" }
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
                name: "IX_Room_CinemaID",
                table: "Room",
                column: "CinemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_RoomID",
                table: "Seat",
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
                name: "Products");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "showTimes");

            migrationBuilder.DropTable(
                name: "UserCoupon");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
