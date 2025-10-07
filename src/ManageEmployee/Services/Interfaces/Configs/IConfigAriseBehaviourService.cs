using ManageEmployee.DataTransferObject.AriseModels;

namespace ManageEmployee.Services.Interfaces.Configs;

public interface IConfigAriseBehaviourService
{
    Task<List<ConfigAriseDocumentBehaviourDto>> GetAllByDocumentAsync(int documentId);
    Task UpdateNoKeepValueAsync(int ariseBehaviourId, ConfigAriseDocumentBehaviourInputDto input);
    Task UpdateFocusValueAsync(int ariseBehaviourId, ConfigAriseDocumentBehaviourInputDto input);
    Task<List<ConfigAriseDocumentBehaviourDto>> PreparationAriseDocumentBehaviourAsync(int documentId);
}
