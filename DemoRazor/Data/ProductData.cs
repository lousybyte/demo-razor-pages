using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoRazor.Data
{
    public class ProductData
    {
        public long    Id { get; set; }

        [Required]
        [MaxLength(255)]
        [DefaultValue("PRODUCT_NAME")]
        public string  ProductName { get; set; }

        [Required]
        [DefaultValue("0")]
        public long    BrandId { get; set; }

        [Required]
        [DefaultValue("0")]
        public long    CategoryId { get; set; }

        [Required]
        [DefaultValue("0.00")]
        public decimal ListPrice { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public uint    xmin { get; set; }

        public virtual ProductsBrandData Brand { get; set; }
        public virtual ProductsCategoryData Category { get; set; }
    }
}
