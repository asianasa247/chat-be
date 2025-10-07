namespace ManageEmployee.DataTransferObject.Contractors;

public record class UpdateUserToContractorDto(
    Guid UserToContractorId,
    string Domain,
    bool IsDeleted
);
