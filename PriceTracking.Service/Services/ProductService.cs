using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;
using PriceTracking.Core.Repositories;
using PriceTracking.Core.Services;
using PriceTracking.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Service.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IProductRespository _productRespository;
        private readonly IMapper _mapper;
        public ProductService(IGenericRepository<Product> repository, IUnitOfWork unitofWork, IMapper mapper, IProductRespository productRespository) : base(repository, unitofWork)
        {
            _mapper = mapper;
            _productRespository = productRespository;
        }

        public async Task<CustomResponseDto<InflationDto>> GetInfluationDifference(int id, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime().AddDays(1);
            endDate = endDate.ToUniversalTime().AddDays(1);
            var prices = await _productRespository.Where(x => x.ProductId == id.ToString()).Where(y => y.ProductDate >= startDate && y.ProductDate <= endDate).OrderBy(x=>x.ProductDate).Select(p => p.ProductPrice).ToListAsync();
            var lastPrice = prices.LastOrDefault();
            var firstPrice = prices.FirstOrDefault();
            InflationDto deneme = new InflationDto();
            deneme.InflationDifference = lastPrice - firstPrice;
            deneme.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100 , 2);
            deneme.DurationDate = (endDate - startDate).Days;
            deneme.StartingPrice = firstPrice;
            deneme.EndingPrice = lastPrice;

            deneme.Products = new List<ProductDto>();

            var productDto = _mapper.Map<InflationDto>(deneme);
            return  CustomResponseDto<InflationDto>.Succes(200, productDto);

        }

        public async Task<CustomResponseDto<List<ProductDto>>> GetSelectedValues(int id, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime().AddDays(1);
            endDate = endDate.ToUniversalTime().AddDays(1);

            var product = await _productRespository.GetSelectedValues(id, startDate, endDate);
            var productDto = _mapper.Map<List<ProductDto>>(product);

            return CustomResponseDto<List<ProductDto>>.Succes(200, productDto);


        }
    }
}
