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

        public async Task<List<Product>> GetSelectedValuesByProductId(int id, DateTime fromDate, DateTime toDate)
        {
            return  await _context.Products.Where(x => x.ProductId == id.ToString()).Where(x => x.ProductDate >= fromDate && x.ProductDate <= toDate).OrderBy(x => x.ProductDate).ToListAsync();
        }

        public async Task<List<Product>> GetSelectedValuesByProductIds(List<string> id, DateTime fromDate, DateTime toDate)
        {
            return await _context.Products.Where(x => id.Contains(x.ProductId)).Where(x => x.ProductDate >= fromDate && x.ProductDate <= toDate).OrderBy(x => x.ProductDate).ToListAsync();
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.AsNoTracking().AsQueryable().OrderBy(x => x.ProductDate).ToListAsync();
        }
    }
}
