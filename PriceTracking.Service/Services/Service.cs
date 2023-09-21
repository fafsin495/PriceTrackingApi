using Microsoft.EntityFrameworkCore;
using PriceTracking.Core.DTOs;
using PriceTracking.Core.Repositories;
using PriceTracking.Core.Services;
using PriceTracking.Core.UnitOfWorks;
using PriceTracking.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Service.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IUnitOfWork _unitofWork;

        public Service(IGenericRepository<T> repository, IUnitOfWork unitofWork)
        {
            _repository = repository;
            _unitofWork = unitofWork;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            var product = await _repository.AnyAsync(expression);
            if(product == null)
            {
                throw new NotFoundException($"Product  not found.");
            }
            return product;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _repository.GetAll().ToListAsync();

        }

        public async Task<T> GetByIdAsync(int id)
        {
            var hasProduct = await _repository.GetByIdAsync(id);
            if(hasProduct == null)
            {
                throw new NotFoundException($"{typeof(T).Name} ({id}) not found.");
            }
            return hasProduct;
        }


        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
             var hasProduct = _repository.Where(expression);
            if (hasProduct == null)
            {
                throw new NotFoundException($"{typeof(T).Name} ({expression.Name}) not found.");
            }
            return hasProduct;

        }
    }
}
