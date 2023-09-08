using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.Services
{
    public interface IProductService:IService<Product>
    {
        Task< CustomResponseDto<List<ProductDto>>> GetSelectedValues(int id, DateTime fromDate, DateTime toDate);
        Task<CustomResponseDto<InflationDto>> GetInfluationDifference(int id, DateTime fromDate, DateTime toDate);

        Task<CustomResponseDto<InflationDto>> GetWeeklyDifference(int id, DateTime fromDate, DateTime toDate);

        Task<CustomResponseDto<InflationDto>> GetMonthlyDifference(int id, DateTime fromDate, DateTime toDate);
        Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation();
    }
}
