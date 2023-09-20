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
    public class ProductsController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductsService _productsService;


        public ProductsController(IMapper mapper, IProductsService productService)
        {
            _mapper = mapper;
            _productsService = productService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSelectedData([FromQuery] RequestByProductIdsDto request)
        {
            return CreateActionResult(await _productsService.GetProductsByDatesAndIds(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetInflationDifference([FromQuery] RequestByProductIdsDto request)
        {
            return CreateActionResult(await _productsService.GetInfluationDifference(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeeklyDifference([FromQuery] RequestByProductIdsDto request)
        {
            return CreateActionResult(await _productsService.GetWeeklyDifference(request));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMonthlyDifference([FromQuery] RequestByProductIdsDto request)
        {
            return CreateActionResult(await _productsService.GetMonthlyDifference(request));
        }
        
    }
}
