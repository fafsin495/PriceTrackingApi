using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;
using PriceTracking.Core.Repositories;
using PriceTracking.Core.Services;
using PriceTracking.Core.UnitOfWorks;
using System.Globalization;

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
        public async Task<CustomResponseDto<InflationDto>> GetMonthlyDifference(int id, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime().AddDays(1);
            endDate = endDate.ToUniversalTime().AddDays(1);

            var monthDate = FindMonth(startDate, endDate);
            var products = await _productRespository.GetSelectedValues(id, startDate, endDate);
            var specificProducts = GetSpecificValues(products, monthDate);


            var lastPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).LastOrDefault();
            var firstPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).FirstOrDefault();


            InflationDto deneme = new InflationDto();
            deneme.ProductId = specificProducts.FirstOrDefault().ProductId;
            deneme.ProductTitle= specificProducts.FirstOrDefault().ProductTitle;
            deneme.ProductCategory = specificProducts.FirstOrDefault().ProductCategory;
            deneme.InflationDifference = lastPrice - firstPrice;
            deneme.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100, 2);
            deneme.DurationDate = (endDate - startDate).Days;
            deneme.StartingPrice = firstPrice;
            deneme.EndingPrice = lastPrice;
            deneme.Products = _mapper.Map<List<ProductDto>>(specificProducts);


            var productDto = _mapper.Map<InflationDto>(deneme);
            return CustomResponseDto<InflationDto>.Succes(200, productDto);
        }
        public async Task<CustomResponseDto<InflationDto>> GetWeeklyDifference (int id, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime().AddDays(1);
            endDate = endDate.ToUniversalTime().AddDays(1);
            
            var weekDate = FindWeek(startDate, endDate);
            var products = await _productRespository.GetSelectedValues(id, startDate, endDate);
            var specificProducts = GetSpecificValues(products, weekDate);

            var lastPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).LastOrDefault();
            var firstPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).FirstOrDefault();


            InflationDto deneme = new InflationDto();
            deneme.ProductId = specificProducts.FirstOrDefault().ProductId;
            deneme.ProductTitle = specificProducts.FirstOrDefault().ProductTitle;
            deneme.ProductCategory = specificProducts.FirstOrDefault().ProductCategory;
            deneme.InflationDifference = lastPrice - firstPrice;
            deneme.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100, 2);
            deneme.DurationDate = (endDate - startDate).Days;
            deneme.StartingPrice = firstPrice;
            deneme.EndingPrice = lastPrice;
            deneme.Products = _mapper.Map<List<ProductDto>>(specificProducts);


            var productDto = _mapper.Map<InflationDto>(deneme);
            return CustomResponseDto<InflationDto>.Succes(200, productDto);
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

            var product = await _productRespository.GetSelectedValues(id, startDate, endDate);
            deneme.Products = _mapper.Map<List<ProductDto>>(product);
            deneme.ProductId = product.FirstOrDefault().ProductId;
            deneme.ProductTitle = product.FirstOrDefault().ProductTitle;
            deneme.ProductCategory = product.FirstOrDefault().ProductCategory;

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
        public List<Product> GetSpecificValues(List<Product> products , List<DateTime> dates)
        {
            List<Product> result = new List<Product>();
            foreach (var date in dates) 
            {
                result.Add(products.Where(x => x.ProductDate == date).FirstOrDefault());
            }
            return result;
        }
        public List<DateTime> FindWeek(DateTime startDate, DateTime endDate)
        {
            List<DateTime> result = new List<DateTime>();

            CultureInfo trCulture = new CultureInfo("tr-TR"); 
            Calendar takvim = trCulture.Calendar;
            DateTime suankiTarih = startDate;
            DateTimeFormatInfo dtfi = trCulture.DateTimeFormat;
            while (suankiTarih <= endDate)
            {
                result.Add(suankiTarih);
                suankiTarih = suankiTarih.AddDays(7);
            }

            return result;
        }
        public List<DateTime> FindMonth(DateTime startDate, DateTime endDate)
        {
            List<DateTime> result = new List<DateTime>();
            
            while (startDate <= endDate)
            {
                result.Add(startDate);
                startDate = startDate.AddMonths(1);
            }

            return result;
        }
    }
}
