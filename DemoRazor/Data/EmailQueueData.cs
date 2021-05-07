using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DemoRazor.Helpers.Accessor;

namespace DemoRazor.Data
{
    public class EmailQueueData
    {
        public long        Id          { get; set; }

        [Required]
        [MaxLength(100)]
        public string      Account     { get; set; }

        [Required]
        [MaxLength(100)]
        public string      Email       { get; set; }

        [Required]
        [MaxLength(50)]
        [DefaultValue("0.0.0.0")]
        public string      Ip          { get; set; }

        [Required]
        [MaxLength(1000)]
        [DefaultValue("NO SUBJECT")]
        public string      Subject     { get; set; }

        [Required]
        [MaxLength(100000)]
        [DefaultValue("")]
        public string      ContentHtml { get; set; }

        [Required]
        [DefaultValue(EmailStatus.Pending)]
        public EmailStatus Status      { get; set; }

        [Required]
        [DefaultValue("CURRENT_TIMESTAMP")]
        public DateTime    RequestedOn { get; set; }

        [Required]
        [DefaultValue("CURRENT_TIMESTAMP")]
        public DateTime    UpdatedOn   { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public uint        xmin        { get; set; }
    }
}

