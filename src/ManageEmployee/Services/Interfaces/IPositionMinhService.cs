using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces
   
{
    public interface IPositionMinhService
    {
        IEnumerable<PositionMinhs> GetAll();
        Task<PagingResult<PositionMinhs>> GetAll(int currentPage, int pagesize, string keyword);
        PositionMinhs GetByID(int id);
        PositionMinhs Create(PositionMinhs parma);
        void Update(PositionMinhs parma);
        void Delete(int id);
    }
}
