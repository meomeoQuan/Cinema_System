﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Cinema_System.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel( ILogger<LogoutModel> logger)
        {
         
            _logger = logger;
        }
        //original code // remmove both authenticate and persistent cookies
        //public async Task<IActionResult> OnPost(string returnUrl = null)
        //{
        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation("User logged out.");
        //    if (returnUrl != null)
        //    {
        //        return LocalRedirect(returnUrl);
        //    }
        //    else
        //    {
        //        // This needs to be a redirect so that the browser performs a new
        //        // request and the identity for the user gets updated.
        //        return RedirectToPage();
        //    }
        //}
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Remove only session authentication but keep persistent cookies
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme,
                    new AuthenticationProperties { IsPersistent = true });

                _logger.LogInformation("User logged out but persistent cookie remains.");
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage();
        }

    }
}
