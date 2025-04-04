// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cinema_System.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;


        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment webHostEnvironment,
            IUnitOfWork unitOfWork,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string? userImage { get; set; }
            public string UserId { get; set; }

            public int? Point { get; set; }


            public string? Fullname { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Email == user.Email);
            var userImage = applicationUser?.UserImage;
            var point = applicationUser?.Points;
            var fullname = applicationUser?.FullName;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                UserId = user.Id,
                userImage = userImage,
                Point = point,
                Fullname = fullname

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Email == user.Email);

            // Update FullName in the custom user model (ApplicationUser)
            applicationUser.FullName = Input.Fullname;
            _unitOfWork.ApplicationUser.Update(applicationUser);
            await _unitOfWork.SaveAsync();

            // Update profile image logic here (if there's a file uploaded)
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\user");

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(applicationUser.UserImage))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, applicationUser.UserImage.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Update image path
                applicationUser.UserImage = @"\images\user\" + fileName;
                _unitOfWork.ApplicationUser.Update(applicationUser);
                await _unitOfWork.SaveAsync();
            }

            // Update phone number logic here
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var identity = (ClaimsIdentity)User.Identity;

            // Check if claims exist
            var existingFullNameClaim = identity.FindFirst("FullName");
            var existingImageClaim = identity.FindFirst("UserImage");

            // Remove existing claims if they exist
            if (existingFullNameClaim != null)
            {
                identity.RemoveClaim(existingFullNameClaim);
            }

            if (existingImageClaim != null)
            {
                identity.RemoveClaim(existingImageClaim);
            }

            // Create new claims
            var newFullNameClaim = new Claim("FullName", applicationUser.FullName ?? "");
            var newImageClaim = new Claim("UserImage", applicationUser.UserImage ?? "/images/default-avatar.png");

            // Update claims in the database
            if (existingFullNameClaim != null)
            {
                await _userManager.ReplaceClaimAsync(user, existingFullNameClaim, newFullNameClaim);
            }
            else
            {
                await _userManager.AddClaimAsync(user, newFullNameClaim);
            }

            if (existingImageClaim != null)
            {
                await _userManager.ReplaceClaimAsync(user, existingImageClaim, newImageClaim);
            }
            else
            {
                await _userManager.AddClaimAsync(user, newImageClaim);
            }

            // Refresh sign-in
            await _signInManager.RefreshSignInAsync(user);


            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }




    }
}
