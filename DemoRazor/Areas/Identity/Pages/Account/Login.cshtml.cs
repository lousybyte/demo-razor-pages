using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using DemoRazor.Attributes;
using DemoRazor.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DemoRazor.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly UserManager<IdentityUser>         _userManager;
        private readonly SignInManager<IdentityUser>       _signInManager;
        private readonly ILogger<LoginModel>               _logger;
        private readonly CaptchaVerificationService        _captchaSvc;

        public LoginModel(
            IStringLocalizer<SharedResources> localizer,
            SignInManager<IdentityUser>       signInManager,
            ILogger<LoginModel>               logger,
            UserManager<IdentityUser>         userManager,
            CaptchaVerificationService        captchaSvc)
        {
            _localizer     = localizer;
            _userManager   = userManager;
            _signInManager = signInManager;
            _logger        = logger;
            _captchaSvc    = captchaSvc;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [Required(ErrorMessage = "Captcha verification is required.")]
        [BindProperty(Name = "g-recaptcha-response")]
        [CaptchaValidation]
        public string CaptchaResponse { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "The {0} field is required.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "The {0} field is required.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //var captchaIsValid = await _captchaSvc.IsCaptchaValid(CaptchaResponse);

            //if (!captchaIsValid)
            //{
            //    ModelState.AddModelError(string.Empty, _localizer["Captcha check failed."]);
            //    return Page();
            //}

            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
