using System.ComponentModel.DataAnnotations;
using DemoRazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DemoRazor.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
    public class CaptchaValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var captchaSvc = validationContext.GetService<CaptchaVerificationService>();
            var captcha = captchaSvc.IsCaptchaValid(value as string).Result;

            if (!captcha)
            {
                var localizer = validationContext.GetService<IStringLocalizer<SharedResources>>();
                return new ValidationResult (localizer["Captcha verification failed."]);
            }

            return ValidationResult.Success;
        }
    }
}
