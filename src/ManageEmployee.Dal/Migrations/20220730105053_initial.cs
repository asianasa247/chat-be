using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Branchs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Branchs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Companies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MST = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                WebsiteName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NameOfCEO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NoteOfCEO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NameOfChiefAccountant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NoteOfChiefAccountant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NameOfTreasurer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NameOfStorekeeper = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NameOfChiefSupplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NoteOfChiefSupplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AssignPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CharterCapital = table.Column<double>(type: "float", nullable: false),
                FileOfBusinessRegistrationCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                FileLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BusinessType = table.Column<int>(type: "int", nullable: false),
                AccordingAccountingRegime = table.Column<int>(type: "int", nullable: false),
                MethodCalcExportPrice = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Companies", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ContractTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContractTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Departments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                isDelete = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Departments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Districts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProvinceId = table.Column<int>(type: "int", nullable: false),
                SortCode = table.Column<int>(type: "int", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Districts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                IdentityNumber = table.Column<double>(type: "float", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PalceOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PalceResidence = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Majors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                isDelete = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Majors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PositionDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PositionId = table.Column<int>(type: "int", nullable: false),
                isDelete = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PositionDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Positions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Order = table.Column<int>(type: "int", nullable: true),
                isDelete = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Positions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Provinces",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SortCode = table.Column<int>(type: "int", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Provinces", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Symbols",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TimeIn = table.Column<TimeSpan>(type: "time", nullable: false),
                TimeOut = table.Column<TimeSpan>(type: "time", nullable: false),
                TimeTotal = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Symbols", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Targets",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ArmyNumber = table.Column<int>(type: "int", nullable: false),
                Present = table.Column<int>(type: "int", nullable: false),
                NameContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DateInvoice = table.Column<DateTime>(type: "datetime2", nullable: true),
                UnitPrice = table.Column<double>(type: "float", nullable: false),
                Total = table.Column<double>(type: "float", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentityCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Targets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserRoles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: true),
                WarehouseId = table.Column<int>(type: "int", nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                PositionDetailId = table.Column<int>(type: "int", nullable: true),
                TargetId = table.Column<int>(type: "int", nullable: true),
                SymbolId = table.Column<int>(type: "int", nullable: true),
                Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                Gender = table.Column<short>(type: "smallint", nullable: false),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProvinceId = table.Column<int>(type: "int", nullable: true),
                DistrictId = table.Column<int>(type: "int", nullable: true),
                WardId = table.Column<int>(type: "int", nullable: true),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Identify = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentifyCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                IdentifyCreatedPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentifyExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                NativeProvinceId = table.Column<int>(type: "int", nullable: true),
                NativeDistrictId = table.Column<int>(type: "int", nullable: true),
                NativeWardId = table.Column<int>(type: "int", nullable: true),
                PlaceOfPermanent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Nation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Religion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EthnicGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UnionMember = table.Column<int>(type: "int", nullable: false),
                LicensePlates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                isDemobilized = table.Column<bool>(type: "bit", nullable: false),
                Literacy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LiteracyDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MajorId = table.Column<int>(type: "int", nullable: false),
                CertificateOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Bank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShareHolderCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NoOfLeaveDate = table.Column<int>(type: "int", nullable: false),
                SendSalaryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ContractTypeId = table.Column<int>(type: "int", nullable: true),
                Salary = table.Column<double>(type: "float", nullable: false),
                SocialInsuranceSalary = table.Column<double>(type: "float", nullable: false),
                NumberWorkdays = table.Column<double>(type: "float", nullable: false),
                DayOff = table.Column<int>(type: "int", nullable: true),
                PersonalTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SocialInsuranceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SocialInsuranceCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                Timekeeper = table.Column<int>(type: "int", nullable: false),
                Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserRoleId = table.Column<int>(type: "int", nullable: true),
                Status = table.Column<bool>(type: "bit", nullable: false),
                LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                RequestPassword = table.Column<bool>(type: "bit", nullable: false),
                Quit = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                UserUpdated = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Wards",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DistrictId = table.Column<int>(type: "int", nullable: false),
                SortCode = table.Column<int>(type: "int", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Wards", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Warehouses",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                UserUpdated = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Warehouses", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Branchs");

        migrationBuilder.DropTable(
            name: "Companies");

        migrationBuilder.DropTable(
            name: "ContractTypes");

        migrationBuilder.DropTable(
            name: "Departments");

        migrationBuilder.DropTable(
            name: "Districts");

        migrationBuilder.DropTable(
            name: "Employees");

        migrationBuilder.DropTable(
            name: "Majors");

        migrationBuilder.DropTable(
            name: "PositionDetails");

        migrationBuilder.DropTable(
            name: "Positions");

        migrationBuilder.DropTable(
            name: "Provinces");

        migrationBuilder.DropTable(
            name: "Symbols");

        migrationBuilder.DropTable(
            name: "Targets");

        migrationBuilder.DropTable(
            name: "UserRoles");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Wards");

        migrationBuilder.DropTable(
            name: "Warehouses");
    }
}
