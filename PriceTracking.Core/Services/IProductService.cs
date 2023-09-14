using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.RequestDtos;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;

namespace PriceTracking.Core.Services
{
    public interface IProductService:IService<Product>
    {
        Task< CustomResponseDto<List<ProductDto>>> GetSelectedValues(RequestDto request);
        Task<CustomResponseDto<InflationDto>> GetInfluationDifference(RequestDto request);

        Task<CustomResponseDto<InflationDto>> GetWeeklyDifference(RequestDto request);

        Task<CustomResponseDto<InflationDto>> GetMonthlyDifference(RequestDto request);
        Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation();
    }
}
