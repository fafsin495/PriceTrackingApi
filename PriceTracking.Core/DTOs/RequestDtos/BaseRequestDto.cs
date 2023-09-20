using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs.RequestDtos
{
    public class BaseRequestDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
