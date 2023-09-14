using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PriceTracking.API.Filters;
using PriceTracking.Core.DTOs.RequestDtos;
using PriceTracking.Core.Services;

namespace PriceTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateFilterAttribute]
    public class ProductController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;


        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }
        


        [HttpGet("[action]")]
        public async Task<IActionResult> GetSelectedData([FromQuery]  RequestDto request)
        {
            return CreateActionResult(await _productService.GetSelectedValues(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetInflationDifference([FromQuery] RequestDto request)
        {
            return CreateActionResult(await _productService.GetInfluationDifference(request));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeeklyDifference([FromQuery] RequestDto request)
        {
            return CreateActionResult(await _productService.GetWeeklyDifference(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMonthlyDifference([FromQuery] RequestDto request)
        {
            return CreateActionResult(await _productService.GetMonthlyDifference(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetTotalInflation()
        {
            return CreateActionResult(await _productService.GetTotalInflation());
        }
        
    }
}
