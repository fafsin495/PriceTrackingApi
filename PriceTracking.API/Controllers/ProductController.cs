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
        private readonly IService<Product> _productService;

        public ProductController(IMapper mapper, IService<Product> productService)
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


    }
}
