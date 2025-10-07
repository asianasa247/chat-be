namespace ManageEmployee.Entities.SalaryEntities;

public class Salary
{
    public int Userid { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public double NumberOfWorkingDays { get; set; }
    public double BaseSalary { get; set; }
    public double ContractualSalaryAmount { get; set; }
    public double AllowanceAmount { get; set; }
    public double DeduceMealCost { get; set; }
    public double RemainingAmount { get; set; }
    public double SaleCommission { get; set; }
    public void CalculateRemainingSalary()
    {
        RemainingAmount = BaseSalary + ContractualSalaryAmount + AllowanceAmount + SaleCommission - DeduceMealCost;
    }
}