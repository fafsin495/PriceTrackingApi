using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs
{
    public  class ProductBaseDto : BaseDto
    {
        public string ProductTitle { get; set; }
        public string ProductCategory { get; set; }
        public string ProductId { get; set; }

    }
}
