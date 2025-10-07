using ManageEmployee.DataTransferObject.CompanyModels;
using System.Text.Json;

namespace ManageEmployee.Helpers;

public static class DataSellerHelper
{
    public static DataSellerModel GetData()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "DataSeller\\CompanyData.json");

        if (!File.Exists(path))
        {
            return new DataSellerModel();
        }

        DataSellerModel response;

        using (StreamReader r = new(path))
        {
            string json = r.ReadToEnd();
            response = JsonSerializer.Deserialize<DataSellerModel>(json);
        }
        response.Companies = response.Companies.Where(x => x.IsDisplay).ToList();
        return response;
    }
}