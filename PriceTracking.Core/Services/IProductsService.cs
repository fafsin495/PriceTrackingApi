using PriceTracking.Core.DTOs.RequestDtos;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.Services
{
    public interface IProductsService : IService<Product>
    {
        Task<CustomResponseDto<List<ProductDto>>> GetProductsByDatesAndIds(RequestByProductIdsDto request);
        Task<CustomResponseDto<List<InflationDto>>> GetInfluationDifference(RequestByProductIdsDto request);

        Task<CustomResponseDto<List<InflationDto>>> GetWeeklyDifference(RequestByProductIdsDto request);

        Task<CustomResponseDto<List<InflationDto>>> GetMonthlyDifference(RequestByProductIdsDto request);
        Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation();
    }
}
