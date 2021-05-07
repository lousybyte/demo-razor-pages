using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoRazor.Data
{
    public class ProductsCategoryData
    {
        public ProductsCategoryData()
        {
            Products = new HashSet<ProductData>();
        }

        public long   Id { get; set; }

        [Required]
        [MaxLength(255)]
        [DefaultValue("CATEGORY_NAME")]
        public string CategoryName { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public uint   xmin { get; set; }

        public virtual ICollection<ProductData> Products { get; set; }
    }
}
