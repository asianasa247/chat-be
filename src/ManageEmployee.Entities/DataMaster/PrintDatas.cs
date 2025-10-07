using ManageEmployee.Entities.PrintEntities;

namespace ManageEmployee.Entities.DataMaster;

public static class PrintDatas
{
    public static List<Print> datas = new()
    {
        new Print
            {
                Id = 1,
                Name = "QrCode",
                Size = 16,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                MarginTop = 10
            },
        new Print
            {
                Id = 2,
                Name = "Barcode",
                Height = 10,
                Width = 10,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                MarginTop = 10
            },
    };
}
