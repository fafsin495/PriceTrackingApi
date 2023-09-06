using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;
using PriceTracking.Core.Services;

namespace PriceTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;


        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var products = await _productService.GetAll();
            var productDtos = _mapper.Map<List<ProductDto>>(products.ToList());
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Succes(200,productDtos));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Succes(200, productDto));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetSelectedData(int id , DateTime startDate , DateTime endDate)
        {
            return CreateActionResult(await _productService.GetSelectedValues(id, startDate, endDate));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetInflationDifference(int id, DateTime startDate, DateTime endDate)
        {
            return CreateActionResult(await _productService.GetInfluationDifference(id, startDate, endDate));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeeklyDifference(int id, DateTime startDate, DateTime endDate)
        {
            return CreateActionResult(await _productService.GetWeeklyDifference(id, startDate, endDate));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMonthlyDifference(int id, DateTime startDate, DateTime endDate)
        {
            return CreateActionResult(await _productService.GetMonthlyDifference(id, startDate, endDate));
        }

    }
}
