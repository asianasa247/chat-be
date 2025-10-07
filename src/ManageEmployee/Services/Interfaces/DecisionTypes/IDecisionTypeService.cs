using ManageEmployee.Entities.DecideEntities;

namespace ManageEmployee.Services.Interfaces.DecisionTypes;

public interface IDecisionTypeService
{
    IEnumerable<DecisionType> GetAll(int currentPage, int pageSize, string keyword);

    DecisionType Create(DecisionType param);

    void Update(DecisionType param);

    void Delete(int id);

    int Count(string keyword);

    IEnumerable<DecisionType> GetAllList();
}
