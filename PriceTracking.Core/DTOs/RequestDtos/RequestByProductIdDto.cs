using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.DTOs.RequestDtos
{
    public class RequestByProductIdDto:BaseRequestDto
    {
        public int ProductId { get; set; }

    }
}
