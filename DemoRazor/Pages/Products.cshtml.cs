using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DemoRazor.Attributes;
using DemoRazor.Data;
using DemoRazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace DemoRazor.Pages
{
    [Authorize]
    public class ProductsModel : PageModel
    {
        private readonly ILogger<ProductsModel> Logger;
        private readonly ProductService         ProductSvc;

        public ProductsModel(ILogger<ProductsModel> logger, ProductService productSvc)
        {
            Logger     = logger;
            ProductSvc = productSvc;
        }

        [BindProperty]
        public ProductInputModel Input { get; set; }

        public class ProductInputModel
        {
            [Display(Name = "Product Name")]
            [Required(ErrorMessage = "The {0} field is required.")]
            [MinLength(3, ErrorMessage = "The {0} field must have at least 3 characters.")]
            [MaxLength(255, ErrorMessage = "The {0} field must have at most 255 characters.")]
            public string ProductName { get; set; }

            [Display(Name = "Brand")]
            [Required(ErrorMessage = "The {0} field is required.")]
            [Range(0, 999, ErrorMessage = "The {0} field must be between {1} and {2} characters.")]
            [ValidBrand]
            public long Brand { get; set; }

            [Display(Name = "Category")]
            [Required(ErrorMessage = "The {0} field is required.")]
            [Range(0, 999, ErrorMessage = "The {0} field must be between {1} and {2} characters.")]
            [ValidCategory]
            public long Category { get; set; }

            [Display(Name = "Price")]
            [Required(ErrorMessage = "The {0} field is required.")]
            [Range(0.0, 999999, ErrorMessage = "The {0} field must be between {1} and {2} characters.")]
            public decimal Price { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<ProductData> Products { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }


        public async Task OnGetAsync()
        {
            Products = await ProductSvc.GetProducts();

            Brands = (await ProductSvc.GetBrands()).ConvertAll(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text  = item.BrandName
            });

            Categories = (await ProductSvc.GetCategories()).ConvertAll(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.CategoryName
            });
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                Products = await ProductSvc.GetProducts();

                Brands = (await ProductSvc.GetBrands()).ConvertAll(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.BrandName
                });

                Categories = (await ProductSvc.GetCategories()).ConvertAll(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.CategoryName
                });

                return Page();
            }

            var result = await ProductSvc.AddProduct(
                new ProductData
                {
                    ProductName = Input.ProductName,
                    BrandId     = Input.Brand,
                    CategoryId  = Input.Category,
                    ListPrice   = Input.Price
                }
            );

            StatusMessage = result ? "Product was successfully added." : "Could not add product.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(long id)
        {
            var product = await ProductSvc.FindProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var result = await ProductSvc.RemoveProduct(product);

            StatusMessage = result > 0 ? "Product was successfully deleted." : "Could not delete product.";

            return RedirectToPage();
        }
    }
}
