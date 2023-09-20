using FluentValidation;
using PriceTracking.Core.DTOs.RequestDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracking.Service.Validations
{
    public class ProductsDtoValidator : AbstractValidator<RequestByProductIdsDto>
    {
        public ProductsDtoValidator()
        {
            var beginDate = new DateTime(2022, 08, 01);
            var now = DateTime.Today;

            RuleFor(x => x.FromDate).InclusiveBetween(beginDate, DateTime.Now).WithMessage($"FromDate must be greater than {beginDate} and less than {now}.");
            RuleFor(x => x.ToDate).InclusiveBetween(beginDate, DateTime.Now).WithMessage($"ToDate must be greater than {beginDate} and less than {now}.");
            RuleFor(x => x.FromDate).LessThan(x => x.ToDate).WithMessage(" ToDate must be greater FromDate.");
            RuleFor(x => x.ProductIds.Count()).LessThan(5).WithMessage(" ProductIds numbers must be less than 4");
            RuleFor(x => x.ProductIds).ForEach(x => x.InclusiveBetween(1, int.MaxValue).WithMessage("ProductId must be greater 0."));
        }
    }
}
