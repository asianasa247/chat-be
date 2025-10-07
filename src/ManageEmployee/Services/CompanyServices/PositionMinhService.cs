using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Dal.Migrations;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SolrNet.Utils;
using System.Drawing.Printing;

namespace ManageEmployee.Services.CompanyServices
{
    public class PositionMinhService : IPositionMinhService
    {
        private ApplicationDbContext _context;
        public PositionMinhService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagingResult<Entities.PositionMinhs>> GetAll(int currentPage,int pagesize ,string keyword)
        {
            if (pagesize <= 0)
            { 
                pagesize = 20; 
            }
            if (currentPage <0)
            {  
                currentPage = 1; 
            }
            var result = new PagingResult<Entities.PositionMinhs>()
            {
                CurrentPage = currentPage,
                PageSize = pagesize,
            };
             
            var query = _context.PositionMinhs.Where(x => string.IsNullOrEmpty(keyword) || x.Code.Contains(keyword)
                || x.Name.Contains(keyword));

            result.TotalItems = await query.CountAsync();
            result.Data = await query.OrderBy(x => x.Order).Skip((currentPage - 1) * pagesize).Take(pagesize).ToListAsync();
            return result;
        }
        public IEnumerable<Entities.PositionMinhs> GetAll()
        { 
           var query =_context.PositionMinhs.OrderBy(x=> x.Order);
            return query.ToList();
        }
        public Entities.PositionMinhs GetByID(int id)
        {
            return _context.PositionMinhs.Find(id);
        }

        public Entities.PositionMinhs Create(Entities.PositionMinhs param)
        {
            if (string.IsNullOrWhiteSpace(param.Name))
                throw new ErrorException(ResultErrorConstants.MODEL_MISS);
            if (_context.PositionMinhs.Where(p => p.Code == param.Code).Any())
            {
                throw new ErrorException(ResultErrorConstants.CODE_EXIST);
            }
            _context.PositionMinhs.Add(param);
            _context.SaveChanges();

            return param;
        }

        public void Update(Entities.PositionMinhs param)
        {
            var position = _context.PositionMinhs.Find(param.Id);

            if (position == null)
                throw new ErrorException(ResultErrorConstants.MODEL_NULL);

            if (_context.PositionMinhs.Where(p => p.Id != param.Id && p.Code == param.Code).Any())
            {
                throw new ErrorException(ResultErrorConstants.CODE_EXIST);
            }
            var checkMemberHaveWarehouse = _context.PositionDetails.Where(x => !x.isDelete && x.PositionId == param.Id).ToList();
            if (!checkMemberHaveWarehouse.Any())
            {
                position.Code = param.Code;
            }

            position.Name = param.Name;
            position.Code = param.Code;
            position.Order = param.Order;

            _context.PositionMinhs.Update(position);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var position = _context.PositionMinhs.Find(id);
            if (position != null)
            {
                _context.PositionMinhs.Remove(position);
                _context.SaveChanges();
            }
        }
    }
}
