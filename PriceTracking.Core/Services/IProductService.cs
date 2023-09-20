using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.RequestDtos;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;

namespace PriceTracking.Core.Services
{
    public interface IProductService:IService<Product>
    {
        Task< CustomResponseDto<List<ProductDto>>> GetSelectedValues(RequestByProductIdDto request);
        Task<CustomResponseDto<InflationDto>> GetInfluationDifference(RequestByProductIdDto request);

        Task<CustomResponseDto<InflationDto>> GetWeeklyDifference(RequestByProductIdDto request);

        Task<CustomResponseDto<InflationDto>> GetMonthlyDifference(RequestByProductIdDto request);
        Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation();
    }
}
