using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProjectModels;

namespace ManageEmployee.Services.Interfaces.Projects;

public interface IProjectService
{
    Task<IEnumerable<ProjectGetListModel>> GetAll();
    Task<PagingResult<ProjectPagingModel>> GetAll(int pageIndex, int pageSize, string keyword);
    Task<string> Create(ProjectModel request);
    Task<ProjectDetaillModel> GetById(int id);
    Task<string> Update(ProjectModel request);
    Task<string> Delete(int id);
}
