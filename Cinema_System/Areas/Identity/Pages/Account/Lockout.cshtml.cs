// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cinema_System.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public LockoutModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; private set; }

        public async Task OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user);

                if (LockoutEnd.HasValue)
                {
                    var lockoutTime = LockoutEnd.Value.UtcDateTime.ToString("o"); // ISO 8601 format

                    Response.Cookies.Append("LockoutEnd", lockoutTime, new CookieOptions
                    {
                        Expires = LockoutEnd.Value.UtcDateTime,
                        HttpOnly = false // Can be accessed by JavaScript
                    });
                }
            }

        }
    }
}
