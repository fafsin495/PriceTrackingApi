using Microsoft.EntityFrameworkCore;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.Models;
using PriceTracking.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRespository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetSelectedValues(int id, DateTime startDate, DateTime endDate)
        {
            return  await _context.Products.Where(x => x.ProductId == id.ToString()).Where(x => x.ProductDate >= startDate && x.ProductDate <= endDate).OrderBy(x => x.ProductDate).ToListAsync();
        }
    }
}
