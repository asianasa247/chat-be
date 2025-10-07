using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerClassificationService
{
    IEnumerable<CustomerClassification> GetAll();

    List<CustomerClassification> GetAll(int currentPage, int pageSize, string keyword);

    CustomerClassification GetById(int id);

    CustomerClassification Create(CustomerClassification param);

    void Update(CustomerClassification param);

    string Delete(int id);
}
