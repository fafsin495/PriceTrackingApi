using PriceTracking.Core.DTOs;
using PriceTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Core.Repositories
{
    public interface IProductRespository:IGenericRepository<Product>
    {
        Task<List<Product>> GetSelectedValuesByProductId(int id, DateTime fromDate, DateTime toDate);
        Task<List<Product>> GetSelectedValuesByProductIds(List<string> id, DateTime fromDate, DateTime toDate);
    }
}
