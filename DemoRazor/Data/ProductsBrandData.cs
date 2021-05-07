using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoRazor.Data
{
    public class ProductsBrandData
    {
        public ProductsBrandData()
        {
            Products = new HashSet<ProductData>();
        }

        public long   Id { get; set; }

        [Required]
        [MaxLength(255)]
        [DefaultValue("BRAND_NAME")]
        public string BrandName { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public uint   xmin { get; set; }

        public virtual ICollection<ProductData> Products { get; set; }
    }
}
