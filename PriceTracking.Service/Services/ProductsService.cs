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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PriceTracking.Service.Services
{
    public class ProductsService : Service<Product>, IProductsService
    {
        private readonly IProductRespository _productRespository;
        private readonly IMapper _mapper;
        private readonly object _lock = new object();
        public ProductsService(IGenericRepository<Product> repository, IUnitOfWork unitofWork, IMapper mapper, IProductRespository productRespository) : base(repository, unitofWork)
        {
            _mapper = mapper;
            _productRespository = productRespository;
        }


        public async Task<CustomResponseDto<List<ProductDto>>> GetProductsByDatesAndIds(RequestByProductIdsDto request)
        {
            var idList = new List<string>();
            request.ProductIds.ForEach(x => { idList.Add(x.ToString()); });

            var fromDate = request.FromDate.ToUniversalTime().AddHours(3);
            var toDate = request.ToDate.ToUniversalTime().AddHours(3);

            checkIds(idList);
            var products = await _productRespository.Where(product => product.ProductDate >= fromDate && product.ProductDate <= toDate).Where(x => idList.Contains(x.ProductId)).OrderBy(date=>date.ProductDate).ToListAsync();
            var productDto = _mapper.Map<List<ProductDto>>(products);

            return CustomResponseDto<List<ProductDto>>.Succes(200, productDto);
        }

        /// <summary>
        /// Bu metot, db üzerinde bulunan başlangıç ve son tarih arasındaki ürünlerin fiyat farkını hesaplar.
        /// Metodun döndürdüğü değer, fiyat farkıdır.
        /// </summary>
        /// <returns>Fiyat farkı</returns>
        public async Task<CustomResponseDto<List<InflationDto>>> GetTotalInflation()
        {
            DateTime firstDate = new DateTime(2022, 8, 1).ToUniversalTime().AddHours(3);
            DateTime lastDate = new DateTime(2023, 8, 1).ToUniversalTime().AddHours(3);

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

            inflationDtos.ForEach(x => x.InflationPercentageDifference = (x.EndingPrice - x.StartingPrice) / x.StartingPrice * 100);
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
        public async Task<CustomResponseDto<List<InflationDto>>> GetMonthlyDifference(RequestByProductIdsDto request)
        {
            var idList = new List<string>();
            request.ProductIds.ForEach(x => { idList.Add(x.ToString()); });

            var fromDate = request.FromDate.ToUniversalTime().AddHours(3);
            var toDate = request.ToDate.ToUniversalTime().AddHours(3);

            var monthDate = FindMonth(fromDate, toDate);

            List<InflationDto> allResult = new List<InflationDto>();
            checkIds(idList);
            foreach (var id in idList) 
            {
                var products = await _productRespository.GetSelectedValuesByProductId(Convert.ToInt32(id), fromDate, toDate);
                var specificProducts = GetSpecificValues(products, monthDate);

                var lastPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).LastOrDefault();
                var firstPrice = specificProducts.OrderBy(x => x.ProductDate).Select(x => x.ProductPrice).FirstOrDefault();

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

                allResult.Add(deneme);
                
            }


            var productDto = _mapper.Map<List<InflationDto>>(allResult);
            return CustomResponseDto<List<InflationDto>>.Succes(200, productDto);
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
        public async Task<CustomResponseDto<List<InflationDto>>> GetWeeklyDifference(RequestByProductIdsDto request)
        {
            var idList = new List<string>();
            request.ProductIds.ForEach(x => { idList.Add(x.ToString()); });

            var fromDate = request.FromDate.ToUniversalTime().AddHours(3);
            var toDate = request.ToDate.ToUniversalTime().AddHours(3);
            var weekDate = FindWeek(fromDate, toDate);

            List<InflationDto> allResult = new List<InflationDto>();
            checkIds(idList);
            foreach (var id in idList)
            {
                var products = await _productRespository.GetSelectedValuesByProductId(Convert.ToInt32(id), fromDate, toDate);
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

                allResult.Add(deneme);
            }

            var productDto = _mapper.Map<List<InflationDto>>(allResult);
            return CustomResponseDto<List<InflationDto>>.Succes(200, productDto);
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
        public async Task<CustomResponseDto<List<InflationDto>>> GetInfluationDifference(RequestByProductIdsDto request)
        {
            var idList = new List<string>();
            request.ProductIds.ForEach(x => { idList.Add(x.ToString()); });

            var fromDate = request.FromDate.ToUniversalTime().AddHours(3);
            var toDate = request.ToDate.ToUniversalTime().AddHours(3);
            List<InflationDto> allResult = new List<InflationDto>();
            checkIds(idList);

            foreach (var id in idList)
            {
                var prices = await _productRespository.Where(x => x.ProductId == id).Where(y => y.ProductDate >= fromDate && y.ProductDate <= toDate).OrderBy(x => x.ProductDate).Select(p => p.ProductPrice).ToListAsync();
                var product = await _productRespository.GetSelectedValuesByProductId(Convert.ToInt32(id), fromDate, toDate);

                var lastPrice = prices.LastOrDefault();
                var firstPrice = prices.FirstOrDefault();

                var inflationPercentage = new InflationDto();
                inflationPercentage.InflationDifference = lastPrice - firstPrice;
                inflationPercentage.InflationPercentageDifference = Math.Round(((lastPrice - firstPrice) / firstPrice) * 100, 2);
                inflationPercentage.DurationDate = (toDate - fromDate).Days;
                inflationPercentage.StartingPrice = firstPrice;
                inflationPercentage.EndingPrice = lastPrice;
                inflationPercentage.Products = _mapper.Map<List<ProductDto>>(product);
                inflationPercentage.ProductId = product.FirstOrDefault().ProductId;
                inflationPercentage.ProductTitle = product.FirstOrDefault().ProductTitle;

                allResult.Add(inflationPercentage);
            }

            var productDto = _mapper.Map<List<InflationDto>>(allResult);
            return CustomResponseDto<List<InflationDto>>.Succes(200, productDto);

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
        public async Task<CustomResponseDto<List<ProductDto>>> GetSelectedValues(RequestByProductIdsDto request)
        {
            var idList = new List<string>();
            request.ProductIds.ForEach(x => { idList.Add(x.ToString()); });

            var fromDate = request.FromDate.ToUniversalTime().AddHours(3);
            var toDate = request.ToDate.ToUniversalTime().AddHours(3);
            checkIds(idList);
            var product = await _productRespository.GetSelectedValuesByProductIds(idList, fromDate, toDate);
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
        public  void  checkIds(List<string> idList)
        {
            var commonIdList = new List<string>();
            foreach (var id in idList)
            {
                var asdw =  _productRespository.AnyAsync(x => x.ProductId == id).Result;
                if (asdw == true)
                {
                    commonIdList.Add(id);
                }
            }
            if (commonIdList.Count()!= idList.Count()) 
            {
                string ids = "";
                idList.Except(commonIdList).ToList().ForEach(x => ids += ", " + x);
                throw new NotFoundException($"Product Id {ids} not found.");
            }
        }
    }
}
