using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.Models
{
    public class ProductFeature:BaseEntity
    {
        //[Key]
        public string ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductUrl { get; set; }
        public int CategoryId { get; set; }

    }
}
