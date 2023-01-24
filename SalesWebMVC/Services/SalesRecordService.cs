using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Data;
using SalesWebMVC.Models;

namespace SalesWebMVC.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMVCContext _context;

        public SalesRecordService(SalesWebMVCContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;

            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }
        //This method doesn't work with VS22 and MySQL8, but it's the original method presented in the course

        //public async Task<List<IGrouping<Department?,SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        //{
        //    var result = from obj in _context.SalesRecord select obj;

        //    if (minDate.HasValue)
        //    {
        //        result = result.Where(x => x.Date >= minDate.Value);
        //    }
        //    if (maxDate.HasValue)
        //    {
        //        result = result.Where(x => x.Date <= maxDate.Value);
        //    }
        //    return await result
        //        .Include(x => x.Seller)
        //        .Include(x => x.Seller.Department)
        //        .OrderByDescending(x => x.Date)
        //        .GroupBy(x => x.Seller.Department)
        //        .ToListAsync();
        //}

        //Here below is the method wich works nowadays
        public async Task<List<IGrouping<Department?, SalesRecord>>> GroupByDepartmentAsync(DateTime? minDate, DateTime? maxDate)
        {
            var filteredSales = await _context.SalesRecord
                .Where(salesRecord => salesRecord.Date >= minDate && salesRecord.Date <= maxDate)
                .Include(x => x.Seller.Department)
                .ToListAsync();

            var groupedSales = await Task.Run(() =>
                filteredSales
                .GroupBy(salesRecord => salesRecord.Seller.Department)
                .ToList());

            return groupedSales;
        }
    }
}
