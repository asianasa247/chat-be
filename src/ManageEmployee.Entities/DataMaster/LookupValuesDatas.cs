namespace ManageEmployee.Entities.DataMaster;

public static class LookupValuesDatas
{
    public static List<LookupValue> datas = new()
    {
        new LookupValue
        {
                Id = 1,
                Code = "1",
                Scope = "COA_ACC_GROUP",
                Value = "1. Thông thường"
            },
        new LookupValue
        {
                Id = 2,
            Code = "2",
            Scope = "COA_ACC_GROUP",
            Value = "2. Khách hàng"
        },
        new LookupValue
        {
                Id = 3,
            Code = "3",
            Scope = "COA_ACC_GROUP",
            Value = "3. Tồn kho"
        },
        new LookupValue
        {
            Id = 4,
            Code = "4",
            Scope = "COA_ACC_GROUP",
            Value = "4. Nhập xuất"
        },
        new LookupValue
        {
                Id = 5,
                Code = "1",
                Scope = "COA_CLASSIFICATION",
                Value = "1. Thông thường"
            },
        new LookupValue
        {
                Id = 6,
            Code = "2",
            Scope = "COA_CLASSIFICATION",
            Value = "2. Hàng hóa"
        },
        new LookupValue
        {
            Id = 7,
            Code = "3",
            Scope = "COA_CLASSIFICATION",
            Value = "3. Nguyên vật liệu"
        },
        new LookupValue
        {
            Id = 8,
            Code = "4",
            Scope = "COA_CLASSIFICATION",
            Value = "4. Công cụ dụng cụ"
        },
        new LookupValue
        {
            Id = 9,
            Code = "5",
            Scope = "COA_CLASSIFICATION",
            Value = "5. Tài sản cố định"
        },
        new LookupValue
        {
            Id = 10,
            Code = "6",
            Scope = "COA_CLASSIFICATION",
            Value = "6. Dự án"
        }
    };
}
