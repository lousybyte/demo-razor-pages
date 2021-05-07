using System;
using System.ComponentModel.DataAnnotations;
using DemoRazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using static DemoRazor.Pages.ProductsModel;

namespace DemoRazor.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ValidCategoryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            ProductInputModel product = (ProductInputModel)context.ObjectInstance;

            var dataRepository = context.GetService<ProductService>();

            if (!dataRepository.IsCategoryValidAsync(product.Category).Result)
            {
                var localizer = context.GetService<IStringLocalizer<SharedResources>>();
                return new ValidationResult(localizer["Invalid product category."]);
            }

            return ValidationResult.Success;
        }
    }
}
