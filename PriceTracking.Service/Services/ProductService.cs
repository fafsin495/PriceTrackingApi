using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.DTOs.RequestDtos;
using PriceTracking.Core.DTOs.ResponseDtos;
using PriceTracking.Core.Models;
using PriceTracking.Core.Repositories;
using PriceTracking.Core.Services;
using PriceTracking.Core.UnitOfWorks;
using PriceTracking.Service.Exceptions;
using System.Globalization;
using System.Linq.Expressions;

namespace PriceTracking.Service.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IProductRespository _productRespository;
        private readonly IMapper _mapper;
        private readonly object _lock = new object();
        public ProductService(IGenericRepository<Product> repository, IUnitOfWork unitofWork, IMapper mapper, IProductRespository productRespository) : base(repository, unitofWork)
        {
            _mapper = mapper;
            _productRespository = productRespository;
        }

        /// <summary>
        /// Bu metot, db üzerinde bulunan başlangıç ve son tarih arasındaki ürünlerin fiyat farkını hesaplar.
        /// Metodun döndürdüğü değer, fiyat farkıdır.
        /// </summary>
        /// <returns>Fiyat farkı</returns>
        public async Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation()
        {
            DateTime firstDate = new DateTime(2022, 8, 1).ToUniversalTime().AddDays(1);
            DateTime lastDate = new DateTime(2023, 8, 1).ToUniversalTime().AddDays(1);

            var firstProducts = _productRespository.Where(x => x.ProductDate == firstDate);
            var lastProducts = _productRespository.Where(x => x.ProductDate == lastDate);


            var inflationDtos = firstProducts.Select(x => new InflationDto
            {
                ProductId = x.ProductId,
                ProductTitle = x.ProductTitle,
                ProductCategory = x.ProductCategory,
                DurationDate = (lastDate - firstDate).Days,
                StartingPrice = x.ProductPrice,
                EndingPrice = lastProducts.Where(x => x.ProductDate == lastDate && x.ProductId == x.ProductId).Select(x => x.ProductPrice).FirstOrDefault(),
            }).ToList();

            inflationDtos.ForEach(x => x.InflationPercentageDifference = (x.EndingPrice - x.StartingPrice) / x.StartingPrice* 100);
            inflationDtos.ForEach(x => x.InflationDifference = x.EndingPrice - x.StartingPrice);
            var productDto = _mapper.Map<List<InflationDto>>(inflationDtos);

            return CustomResponseDto<List<InflationDto>>.Succes(200, productDto);
        }
        /// <summary>
        /// Bu metot, kullanıcı tarafından girilen iki farklı tarih arasındaki ürünlerin fiyatlarının aylık farkını hesaplar.
        ///
        /// Metodu kullanmak için, aşağıdaki parametreleri sağlamanız gerekir:
        ///
        /// * fromDate: İlk tarih
        /// * toDate: Son tarih
        /// * id: Ürüne ait productId bilgisi
        ///
        /// Metodun döndürdüğü değer, fiyat farkıdır.
        /// </summary>
        /// <param name="fromDate">İlk tarih</param>
        /// <param name="toDate">Son tarih</param>
        /// <param name="id">Son tarih</param>
        /// <returns>Aylık olarak Fiyat farkı</returns>
        public async Task<CustomResponseDto<InflationDto>> GetMonthlyDifference(RequestByProductIdDto request)
        {
            var fromDate = request.FromDate.ToUniversalTime().AddDays(1);
            var toDate = request.ToDate.ToUniversalTime().AddDays(1);
            
            var monthDate = FindMonth(fromDate, toDate);
            checkId(request.ProductId.ToString());
            var products = await _productRespository.GetSelectedValuesByProductId(request.ProductId, fromDate, toDate);
            var specificProducts = GetSpecificValues(products, monthDate);


            var lastPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).LastOrDefault();
            var firstPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).FirstOrDefault();


            InflationDto deneme = new InflationDto();
            deneme.ProductId = specificProducts.FirstOrDefault().ProductId;
            deneme.ProductTitle= specificProducts.FirstOrDefault().ProductTitle;
            deneme.ProductCategory = specificProducts.FirstOrDefault().ProductCategory;
            deneme.InflationDifference = lastPrice - firstPrice;
            deneme.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100, 2);
            deneme.DurationDate = (toDate - fromDate).Days;
            deneme.StartingPrice = firstPrice;
            deneme.EndingPrice = lastPrice;
            deneme.Products = _mapper.Map<List<ProductDto>>(specificProducts);


            var productDto = _mapper.Map<InflationDto>(deneme);
            return CustomResponseDto<InflationDto>.Succes(200, productDto);
        }
        /// <summary>
        /// Bu metot, kullanıcı tarafından girilen iki farklı tarih arasındaki ürünlerin fiyatlarının haftalık olarak farkını hesaplar.
        ///
        /// Metodu kullanmak için, aşağıdaki parametreleri sağlamanız gerekir:
        ///
        /// * fromDate: İlk tarih
        /// * toDate: Son tarih
        /// * id: Ürüne ait productId bilgisi
        ///
        /// Metodun döndürdüğü değer, fiyat farkıdır.
        /// </summary>
        /// <param name="fromDate">İlk tarih</param>
        /// <param name="toDate">Son tarih</param>
        /// <param name="id">Son tarih</param>
        /// <returns>Haftalık olarak Fiyat farkı</returns>
        public async Task<CustomResponseDto<InflationDto>> GetWeeklyDifference (RequestByProductIdDto request)
        {
            var fromDate = request.FromDate.ToUniversalTime().AddDays(1);
            var toDate = request.ToDate.ToUniversalTime().AddDays(1);
            
            var weekDate = FindWeek(fromDate, toDate);
            checkId(request.ProductId.ToString());
            var products = await _productRespository.GetSelectedValuesByProductId(request.ProductId, fromDate, toDate);
            var specificProducts = GetSpecificValues(products, weekDate).ToList();
            specificProducts.OrderBy(x => x.ProductDate);
            var lastPrice = specificProducts.Select(x => x.ProductPrice).LastOrDefault();
            var firstPrice = specificProducts.Select(x => x.ProductPrice).FirstOrDefault();


            InflationDto deneme = new InflationDto();
            deneme.ProductId = specificProducts.FirstOrDefault().ProductId;
            deneme.ProductTitle = specificProducts.FirstOrDefault().ProductTitle;
            deneme.ProductCategory = specificProducts.FirstOrDefault().ProductCategory;
            deneme.InflationDifference = lastPrice - firstPrice;
            deneme.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100, 2);
            deneme.DurationDate = (toDate - fromDate).Days;
            deneme.StartingPrice = firstPrice;
            deneme.EndingPrice = lastPrice;
            deneme.Products = _mapper.Map<List<ProductDto>>(specificProducts);


            var productDto = _mapper.Map<InflationDto>(deneme);
            return CustomResponseDto<InflationDto>.Succes(200, productDto);
        }
        /// <summary>
        /// Bu metot, kullanıcı tarafından girilen iki farklı tarih arasındaki ürünlerin fiyatların farkını hesaplar.
        ///
        /// Metodu kullanmak için, aşağıdaki parametreleri sağlamanız gerekir:
        ///
        /// * fromDate: İlk tarih
        /// * toDate: Son tarih
        /// * id: Ürüne ait productId bilgisi
        ///
        /// Metodun döndürdüğü değer, fiyat farkıdır.
        /// </summary>
        /// <param name="fromDate">İlk tarih</param>
        /// <param name="toDate">Son tarih</param>
        /// <param name="id">Son tarih</param>
        /// <returns>Fiyat farkı</returns>
        public async Task<CustomResponseDto<InflationDto>> GetInfluationDifference(RequestByProductIdDto request)
        {
            var fromDate = request.FromDate.ToUniversalTime().AddDays(1);
            var toDate = request.ToDate.ToUniversalTime().AddDays(1);
            var id = request.ProductId;

            checkId(request.ProductId.ToString());
            var prices = await _productRespository.Where(x => x.ProductId == id.ToString()).Where(y => y.ProductDate >= fromDate && y.ProductDate <= toDate).OrderBy(x=>x.ProductDate).Select(p => p.ProductPrice).ToListAsync();
            var product = await _productRespository.GetSelectedValuesByProductId(id, fromDate, toDate);
            var lastPrice = prices.LastOrDefault();
            var firstPrice = prices.FirstOrDefault();

            var inflationPercentage = new InflationDto();
            inflationPercentage.InflationDifference = lastPrice - firstPrice;
            inflationPercentage.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100 , 2);
            inflationPercentage.DurationDate = (toDate - fromDate).Days;
            inflationPercentage.StartingPrice = firstPrice;
            inflationPercentage.EndingPrice = lastPrice;
            inflationPercentage.Products = _mapper.Map<List<ProductDto>>(product);
            inflationPercentage.ProductId = product.FirstOrDefault().ProductId;
            inflationPercentage.ProductTitle = product.FirstOrDefault().ProductTitle;
            inflationPercentage.ProductCategory = product.FirstOrDefault().ProductCategory;

            var productDto = _mapper.Map<InflationDto>(inflationPercentage);
            return  CustomResponseDto<InflationDto>.Succes(200, productDto);

        }
        /// <summary>
        /// Bu metot, kullanıcı tarafından girilen iki farklı tarih arasındaki ürünleri getirir.
        ///
        /// Metodu kullanmak için, aşağıdaki parametreleri sağlamanız gerekir:
        ///
        /// * fromDate: İlk tarih
        /// * toDate: Son tarih
        /// * id: Ürüne ait productId bilgisi
        ///
        /// Metodun döndürdüğü değer, Ürünlerin kendisini döndürür.
        /// </summary>
        /// <param name="fromDate">İlk tarih</param>
        /// <param name="toDate">Son tarih</param>
        /// <param name="id">Son tarih</param>
        /// <returns>Ürünler</returns>
        public async Task<CustomResponseDto<List<ProductDto>>> GetSelectedValues(RequestByProductIdDto request)
        {
            var fromDate = request.FromDate.ToUniversalTime().AddDays(1);
            var toDate = request.ToDate.ToUniversalTime().AddDays(1);
            checkId(request.ProductId.ToString());
            var product = await _productRespository.GetSelectedValuesByProductId(request.ProductId, fromDate, toDate);
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
        public List<DateTime> FindWeek(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> result = new List<DateTime>();

            CultureInfo trCulture = new CultureInfo("tr-TR"); 
            Calendar takvim = trCulture.Calendar;
            DateTime suankiTarih = fromDate;
            DateTimeFormatInfo dtfi = trCulture.DateTimeFormat;
            while (suankiTarih <= toDate)
            {
                result.Add(suankiTarih);
                suankiTarih = suankiTarih.AddDays(7);
            }

            return result;
        }
        public List<DateTime> FindMonth(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> result = new List<DateTime>();
            
            while (fromDate <= toDate)
            {
                result.Add(fromDate);
                fromDate = fromDate.AddMonths(1);
            }

            return result;
        }
        public void checkId(string id)
        {
            var result = _productRespository.AnyAsync(x => x.ProductId == id).Result;
            if (result == false)
            {
                throw new NotFoundException($"Product Id {id} not found.");
            }
        }
    }
}
