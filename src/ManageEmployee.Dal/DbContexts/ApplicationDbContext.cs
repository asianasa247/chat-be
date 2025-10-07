using Common.Extensions;
using ManageEmployee.Dal.Configurations;
using ManageEmployee.Entities;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Entities.AllowanceEntities;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.Entities.ContractEntities;
using ManageEmployee.Entities.ContractorEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.DataMaster;
using ManageEmployee.Entities.DecideEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.HotelEntities;
using ManageEmployee.Entities.HotelEntities.RoomEntities;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.IntroduceEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Entities.PocoSelections;
using ManageEmployee.Entities.PrintEntities;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.ProcedureEntities.WeeklyScheduleEntities;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.StationeryEntities;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Entities.WareHouseEntities;
using ManageEmployee.Entities.WebEntities;
using ManageEmployee.Entities.LotteryEntities;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ManageEmployee.Dal.Enums;
using ManageEmployee.Entities.HanetEntities;
using ManageEmployee.Entities.ConvertProductEntities;
using ManageEmployee.Entities.LedgerEntities.VitaxEntities;
using ManageEmployee.Entities.Email;
using ManageEmployee.Entities.ZaloEntities;
using ManageEmployee.Entities.FAQ_AI_Chatbot;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Entities.Cultivation;
using ManageEmployee.Entities.ChatSupport;
using ManageEmployee.Entities.PaymentEntities;
namespace ManageEmployee.Dal.DbContexts;

public class ApplicationDbContext : DbContext
{
    private readonly int[] InternalValues = new[] { 1, (int)LedgerInternal.Accounting };

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<OrderSuccessful> OrderSuccessfuls { get; set; }
    public virtual DbSet<ReminderParticipant> ReminderParticipants { get; set; }
    public virtual DbSet<ReminderModel> Reminders { get; set; }
    public virtual DbSet<PositionMinhs> PositionMinhs { get; set; }
    public virtual DbSet<ChatHistory> ChatHistories { get; set; }
    public virtual DbSet<Conversation> Conversations { get; set; }
    public virtual DbSet<FaceAccess> FaceAccesses { get; set; }
    public virtual DbSet<ScheduledPost> ScheduledPosts { get; set; }
    public virtual DbSet<FAQ_AIchat> FAQ_AIchats { get; set; }
    public virtual DbSet<FAQ_AIchatDetail> FAQ_AIchatDetails { get; set; }
    public virtual DbSet<ConvertProduct> ConvertProducts { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<SurchargeHotel> SurchargeHotels { get; set; }
    public virtual DbSet<PriceHour> PriceHours { get; set; }
    public virtual DbSet<PriceDay> PriceDay { get; set; }
    public virtual DbSet<StatusHotel> StatusHotels { get; set; }
    public virtual DbSet<Floor> Floors { get; set; }
    public virtual DbSet<ContentMail> ContentMails { get; set; }
    public virtual DbSet<PrizeGood> PrizeGoods { get; set; }
    public virtual DbSet<HistorySpin> HistorySpins { get; set; }
    public virtual DbSet<HistorySpinDetail> HistorySpinDetails { get; set; }
    public virtual DbSet<Prize> Prizes { get; set; }
    public virtual DbSet<SettingsSpin> SettingsSpins { get; set; }
    public virtual DbSet<Area> Areas { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Province> Provinces { get; set; }
    public virtual DbSet<District> Districts { get; set; }
    public virtual DbSet<Ward> Wards { get; set; }
    public virtual DbSet<Branch> Branchs { get; set; }
    public virtual DbSet<Warehouse> Warehouses { get; set; }
    public virtual DbSet<ContractType> ContractTypes { get; set; }
    public virtual DbSet<Major> Majors { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Position> Positions { get; set; }
    public virtual DbSet<PositionDetail> PositionDetails { get; set; }
    public virtual DbSet<Target> Targets { get; set; }
    public virtual DbSet<Symbol> Symbols { get; set; }
    public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
    public virtual DbSet<ChartOfAccountGroup> ChartOfAccountGroups { get; set; }
    public virtual DbSet<ChartOfAccountGroupLink> ChartOfAccountGroupLinks { get; set; }
    public virtual DbSet<ChartOfAcountTest> ChartOfAcountTests { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<TaxRate> TaxRates { get; set; }
    public virtual DbSet<FinalStandard> FinalStandards { get; set; }
    public virtual DbSet<Bill> Bills { get; set; }
    public virtual DbSet<BillDetail> BillDetails { get; set; }
    public virtual DbSet<BillDetailRefund> BillDetailRefunds { get; set; }
    public virtual DbSet<BillTracking> BillTrackings { get; set; }

    #region Goods
    public virtual DbSet<Goods> Goods { get; set; }
    public virtual DbSet<GoodsWebs> GoodsWebs { get; set; }
    public virtual DbSet<GoodDetail> GoodDetails { get; set; }
    public virtual DbSet<DeskFloor> DeskFloors { get; set; }
    public virtual DbSet<GoodCustomer> GoodCustomers { get; set; }
    public virtual DbSet<GoodsPriceList> GoodsPriceList { get; set; }
    public virtual DbSet<GoodsPromotion> GoodsPromotions { get; set; }
    public virtual DbSet<GoodsPromotionDetail> GoodsPromotionDetails { get; set; }
    public virtual DbSet<GoodsQuotaStep> GoodsQuotaSteps { get; set; }
    public virtual DbSet<GoodsQuota> GoodsQuotas { get; set; }
    public virtual DbSet<GoodsQuotaDetail> GoodsQuotaDetails { get; set; }
    public virtual DbSet<GoodsQuotaRecipe> GoodsQuotaRecipes { get; set; }
    #endregion

    public virtual DbSet<FixedAsset> FixedAssets { get; set; }
    public virtual DbSet<DocumentType1> DocumentType1 { get; set; }
    public virtual DbSet<DocumentType2> DocumentType2 { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerTaxInformation> CustomerTaxInformations { get; set; }
    public virtual DbSet<CustomerContactHistory> CustomerContactHistories { get; set; }
    public virtual DbSet<Job> Jobs { get; set; }
    public virtual DbSet<Status> Status { get; set; }
    public virtual DbSet<CustomerClassification> CustomerClassifications { get; set; }
    public virtual DbSet<Relative> Relatives { get; set; }
    public virtual DbSet<RelationShip> RelationShips { get; set; }
    public virtual DbSet<DocumentType> DocumentTypes { get; set; }
    public virtual DbSet<UserTask> UserTasks { get; set; }
    public virtual DbSet<UserTaskCheckList> UserTaskCheckLists { get; set; }
    public virtual DbSet<UserTaskRoleDetails> UserTaskRoleDetails { get; set; }
    public virtual DbSet<UserTaskComment> UserTaskComments { get; set; }
    public virtual DbSet<UserTaskTracking> UserTaskTrackings { get; set; }
    public virtual DbSet<UserTaskPin> UserTaskPins { get; set; }
    public virtual DbSet<Payer> Payers { get; set; }
    public virtual DbSet<Description> Descriptions { get; set; }
    public virtual DbSet<InOutHistory> InOutHistories { get; set; }
    public virtual DbSet<LookupValue> LookupValues { get; set; }
    public virtual DbSet<SalarySocial> SalarySocials { get; set; }
    public virtual DbSet<Degree> Degrees { get; set; }
    public virtual DbSet<Slider> Sliders { get; set; }
    public virtual DbSet<Certificate> Certificates { get; set; }
    public virtual DbSet<Introduce> Introduces { get; set; }
    public virtual DbSet<IntroduceType> IntroduceTypes { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Decide> Decide { get; set; }
    public virtual DbSet<HistoryAchievement> HistoryAchievements { get; set; }
    public virtual DbSet<DecisionType> DecisionType { get; set; }
    public virtual DbSet<Salary> Salaries { get; set; }
    public virtual DbSet<SalaryLevel> SalaryLevel { get; set; }
    public virtual DbSet<Subsidize> Subsidizes { get; set; }
    public virtual DbSet<UserSubsidize> UserSubsidizes { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Career> Career { get; set; }
    public virtual DbSet<CustomerQuote> CustomerQuote { get; set; }
    public virtual DbSet<CustomerQuote_Detail> CustomerQuote_Detail { get; set; }
    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<MenuRole> MenuRoles { get; set; }
    public virtual DbSet<Order> Order { get; set; }
    public virtual DbSet<OrderDetail> OrderDetail { get; set; }
    public virtual DbSet<PagePrint> PagePrints { get; set; }
    public virtual DbSet<Print> Prints { get; set; }
    public virtual DbSet<TypeWork> TypeWorks { get; set; }
    public virtual DbSet<AccountPay> AccountPays { get; set; }
    public virtual DbSet<Inventory> Inventory { get; set; }
    public virtual DbSet<GoodWarehouses> GoodWarehouses { get; set; }
    public virtual DbSet<FixedAsset242> FixedAsset242 { get; set; }
    public virtual DbSet<Allowance> Allowances { get; set; }
    public virtual DbSet<InvoiceDeclaration> InvoiceDeclarations { get; set; }
    public virtual DbSet<AllowanceUser> AllowanceUsers { get; set; }
    public virtual DbSet<IsoftHistory> IsoftHistory { get; set; }
    public virtual DbSet<P_Procedure> P_Procedure { get; set; }
    public virtual DbSet<P_ProcedureStatus> P_ProcedureStatus { get; set; }
    public virtual DbSet<P_ProcedureStatusStep> P_ProcedureStatusSteps { get; set; }
    public virtual DbSet<P_ProcedureStatusRole> P_ProcedureStatusRole { get; set; }
    public virtual DbSet<P_Leave> P_Leave { get; set; }
    public virtual DbSet<P_Leave_Item> P_Leave_Items { get; set; }
    public virtual DbSet<P_SalaryAdvance> P_SalaryAdvance { get; set; }
    public virtual DbSet<P_SalaryAdvance_Item> P_SalaryAdvance_Item { get; set; }
    public virtual DbSet<MenuKpi> MenuKpis { get; set; }
    public virtual DbSet<P_Kpi> P_Kpis { get; set; }
    public virtual DbSet<P_Kpi_Item> P_Kpi_Items { get; set; }
    public virtual DbSet<GoodWarehouseExport> GoodWarehouseExport { get; set; }
    public virtual DbSet<P_Inventory> P_Inventories { get; set; }
    public virtual DbSet<P_Inventory_Item> P_Inventory_Items { get; set; }
    public virtual DbSet<Cart> Cart { get; set; }
    public DbSet<Social> Social { get; set; }
    public virtual DbSet<BillHistoryCollection> BillHistoryCollections { get; set; }
    public virtual DbSet<BillHistoryCollectionStatus> BillHistoryCollectionStatus { get; set; }
    public virtual DbSet<FixedAssetUser> FixedAssetUser { get; set; }
    public virtual DbSet<SendMail> SendMails { get; set; }
    public virtual DbSet<ConfigAriseBehaviour> ConfigAriseBehaviour { get; set; }
    public virtual DbSet<ConfigAriseDocumentBehaviour> ConfigAriseDocumentBehaviour { get; set; }
    public virtual DbSet<MainColor> MainColors { get; set; }
    public virtual DbSet<CategoryStatusWebPeriod> CategoryStatusWebPeriods { get; set; }
    public virtual DbSet<CategoryStatusWebPeriodGood> CategoryStatusWebPeriodGoods { get; set; }
    public virtual DbSet<Surcharge> Surcharges { get; set; }
    public virtual DbSet<TillManager> TillManagers { get; set; }
    public virtual DbSet<GoodWarehousesPosition> GoodWarehousesPositions { get; set; }
    #region warehouse
    public virtual DbSet<WareHouseShelves> WareHouseShelves { get; set; }
    public virtual DbSet<WareHouseShelvesWithFloor> WareHouseShelvesWithFloors { get; set; }
    public virtual DbSet<WareHouseFloor> WareHouseFloors { get; set; }
    public virtual DbSet<WareHouseFloorWithPosition> WareHouseFloorWithPositions { get; set; }
    public virtual DbSet<WareHousePosition> WareHousePositions { get; set; }
    public virtual DbSet<WareHouseWithShelves> WareHouseWithShelves { get; set; }
    #endregion
    public virtual DbSet<UpdateArisingAccountQueue> UpdateArisingAccountQueue { get; set; }
    #region Ledger
    public virtual DbSet<Ledger> Ledgers { get; set; }
    public virtual DbSet<LedgerErrorImport> LedgerErrorImports { get; set; }
    public virtual DbSet<LedgerFixedAsset> LedgerFixedAssets { get; set; }
    public virtual DbSet<LedgerWareHouse> LedgerWareHouses { get; set; }
    public virtual DbSet<LedgerProcedureProduct> LedgerProcedureProducts { get; set; }
    public virtual DbSet<LedgerProcedureProductDetail> LedgerProcedureProductDetails { get; set; }
    public virtual DbSet<VintaxInvoiceIn> VintaxInvoiceIns { get; set; }

    #endregion

    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<YearSale> YearSales { get; set; }
    public virtual DbSet<CustomerTaxInformationAccountant> CustomerTaxInformationAccountants { get; set; }
    public virtual DbSet<Configuration> Configurations { get; set; }
    public virtual DbSet<ConfigurationUser> ConfigurationUsers { get; set; }
    public virtual DbSet<UserContractHistory> UserContractHistories { get; set; }
    public virtual DbSet<ConfigurationView> ConfigurationViews { get; set; }
    public virtual DbSet<ChartOfAccountFilter> ChartOfAccountFilters { get; set; }
    public virtual DbSet<ContractFile> ContractFiles { get; set; }
    public virtual DbSet<Stationery> Stationeries { get; set; }
    public virtual DbSet<StationeryImport> StationeryImports { get; set; }
    public virtual DbSet<StationeryImportItem> StationeryImportItems { get; set; }
    public virtual DbSet<StationeryExport> StationeryExports { get; set; }
    public virtual DbSet<StationeryExportItem> StationeryExportItems { get; set; }

    #region Car
    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<CarLocation> CarLocations { get; set; }
    public virtual DbSet<CarLocationDetail> CarLocationDetails { get; set; }
    public virtual DbSet<PetrolConsumption> PetrolConsumptions { get; set; }
    public virtual DbSet<DriverRouter> DriverRouters { get; set; }
    public virtual DbSet<DriverRouterDetail> DriverRouterDetails { get; set; }
    #endregion

    public virtual DbSet<WebMail> WebMails { get; set; }
    public virtual DbSet<EventWithImage> EventWithImages { get; set; }
    public virtual DbSet<WarningNotification> WarningNotifications { get; set; }
    public virtual DbSet<CarField> CarFields { get; set; }
    public virtual DbSet<CarFieldSetup> CarFieldSetups { get; set; }
    public virtual DbSet<TimeKeepingInOutLogging> TimeKeepingInOutLogging { get; set; }
    public virtual DbSet<RoadRoute> RoadRoutes { get; set; }
    public virtual DbSet<PoliceCheckPoint> PoliceCheckPoints { get; set; }
    public virtual DbSet<PetrolConsumptionPoliceCheckPoint> PetrolConsumptionPoliceCheckPoints { get; set; }
    public virtual DbSet<Shift> Shifts { get; set; }
    public virtual DbSet<ProcedureRequestOvertime> ProcedureRequestOvertimes { get; set; }
    public virtual DbSet<WorkingDay> WorkingDays { get; set; }
    public virtual DbSet<ProcedureChangeShift> ProcedureChangeShifts { get; set; }
    public virtual DbSet<ShiftUserDetail> ShiftUserDetails { get; set; }
    public virtual DbSet<ShiftUser> ShiftUsers { get; set; }
    public virtual DbSet<InOutReport> InOutReports { get; set; }
    public virtual DbSet<InOutLoggingFromHanet> InOutLoggingFromHanets { get; set; }
    public virtual DbSet<HanetUser> HanetUsers { get; set; }
    public virtual DbSet<NumberOfMeal> NumberOfMeals { get; set; }
    public virtual DbSet<NumberOfMealsTracking> NumberOfMealsTracking { get; set; }
    public virtual DbSet<BillPromotion> BillPromotions { get; set; }
    public virtual DbSet<ProcedureCondition> ProcedureConditions { get; set; }
    public virtual DbSet<ProcedureLog> ProcedureLogs { get; set; }
    public virtual DbSet<NumberOfMealDetail> NumberOfMealDetails { get; set; }
    public virtual DbSet<SignatureBlock> SignatureBlocks { get; set; }
    public virtual DbSet<WeeklySchedule> WeeklySchedules { get; set; }
    public virtual DbSet<WeeklyScheduleDetail> WeeklyScheduleDetails { get; set; }
    public virtual DbSet<Smtp> Smtps { get; set; }//By: DatNguyen
    public virtual DbSet<MailLog> MailLogs { get; set; }//By: DatNguyen
    public virtual DbSet<MailTemplate> MailTemplates { get; set; }//By: DatNguyen
    public virtual DbSet<MailTimer> MailTimers { get; set; } //By: DatNguyen
    public virtual DbSet<MailSend> MailSends { get; set; } //By: DatNguyen
    public virtual DbSet<ZaloAppConfig> ZaloAppConfigs { get; set; } //By: DatNguyen
    public virtual DbSet<ZaloUser> ZaloUsers { get; set; } //By: DatNguyen
    public virtual DbSet<Commission> Commissions { get; set; }
    public virtual DbSet<AdditionWeb> AdditionWebs { get; set; }
    // ChatSupport
    public virtual DbSet<ChatSupportRoom> ChatSupportRooms { get; set; }
    public virtual DbSet<ChatSupportParticipant> ChatSupportParticipants { get; set; }
    public virtual DbSet<ChatSupportMessage> ChatSupportMessages { get; set; }
    public DbSet<EventCustomer> EventCustomers { get; set; }

    public virtual DbSet<AlepayConfig> AlepayConfigs { get; set; }
    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public virtual DbSet<PaymentWebhookLog> PaymentWebhookLogs { get; set; }



    public DbSet<AdditionWebGoods> AdditionWebGoods { get; set; } = default!;
    public virtual DbSet<PostAi> PostAis { get; set; }


    #region Supply
    public virtual DbSet<AdvancePayment> AdvancePayments { get; set; }
    public virtual DbSet<AdvancePaymentDetail> AdvancePaymentDetails { get; set; }
    public virtual DbSet<PaymentProposal> PaymentProposals { get; set; }
    public virtual DbSet<PaymentProposalDetail> PaymentProposalDetails { get; set; }
    public virtual DbSet<GatePass> GatePasses { get; set; }
    public virtual DbSet<RequestExportGood> RequestExportGoods { get; set; }
    public virtual DbSet<RequestExportGoodDetail> RequestExportGoodDetails { get; set; }
    public virtual DbSet<RequestEquipment> RequestEquipments { get; set; }
    public virtual DbSet<RequestEquipmentDetail> RequestEquipmentDetails { get; set; }
    public virtual DbSet<ExpenditurePlan> ExpenditurePlans { get; set; }
    public virtual DbSet<ExpenditurePlanDetail> ExpenditurePlanDetails { get; set; }
    public virtual DbSet<ConfigDiscount> ConfigDiscounts { get; set; }
    public virtual DbSet<SalaryType> SalaryTypes { get; set; }
    public virtual DbSet<SalaryTypeProduceProduct> SalaryTypeProduceProducts { get; set; }
    public virtual DbSet<SalaryTypeProduceProductDetail> SalaryTypeProduceProductDetails { get; set; }
    public virtual DbSet<RequestEquipmentOrder> RequestEquipmentOrders { get; set; }
    public virtual DbSet<RequestEquipmentOrderDetail> RequestEquipmentOrderDetails { get; set; }
    public virtual DbSet<VehicleRepairRequest> VehicleRepairRequests { get; set; }
    public virtual DbSet<VehicleRepairRequestDetail> VehicleRepairRequestDetails { get; set; }
    #endregion

    #region hotel
    public virtual DbSet<RoomConfigure> RoomConfigures { get; set; }
    public virtual DbSet<RoomConfigureType> RoomConfigureTypes { get; set; }
    public virtual DbSet<GoodRoomType> GoodRoomTypes { get; set; }
    public virtual DbSet<GoodRoomPrice> GoodRoomPrices { get; set; }
    public virtual DbSet<GoodRoomBed> GoodRoomBeds { get; set; }
    public virtual DbSet<SalaryUserVersion> SalaryUserVersions { get; set; }

    #endregion

    #region Produce product

    public virtual DbSet<OrderProduceProduct> OrderProduceProducts { get; set; }
    public virtual DbSet<OrderProduceProductDetail> OrderProduceProductDetails { get; set; }
    public virtual DbSet<PlanningProduceProduct> PlanningProduceProducts { get; set; }
    public virtual DbSet<PlanningProduceProductDetail> PlanningProduceProductDetails { get; set; }
    public virtual DbSet<WarehouseProduceProduct> WarehouseProduceProducts { get; set; }
    public virtual DbSet<WarehouseProduceProductDetail> WarehouseProduceProductDetails { get; set; }
    public virtual DbSet<ManufactureOrder> ManufactureOrders { get; set; }
    public virtual DbSet<ManufactureOrderDetail> ManufactureOrderDetails { get; set; }
    public virtual DbSet<CarDelivery> CarDeliveries { get; set; }
    public virtual DbSet<ProduceProduct> ProduceProducts { get; set; }
    public virtual DbSet<ProduceProductDetail> ProduceProductDetails { get; set; }
    #endregion

    #region Contractor
    public virtual DbSet<ContractorToCategory> ContractorToCategory { get; set; }
    public virtual DbSet<ContractorToCategoryToProduct> ContractorToCategoryToProduct { get; set; }
    public virtual DbSet<UserToContractor> UserToContractor { get; set; }

    #endregion

    public virtual DbSet<EmployeeByOrder> EmployeeByOrders { get; set; }
    public virtual DbSet<ProductOfEmployee> ProductOfEmployees { get; set; }

    public virtual DbSet<VintaxInvoiceInDetail> VintaxInvoiceInDetails { get; set; }

    #region ChatboxAI
    public virtual DbSet<ChatboxAITopic> ChatboxAITopics { get; set; }
    public virtual DbSet<ChatboxAIQA> ChatboxAIQAs { get; set; }
    public virtual DbSet<ChatboxAIScheduledMessage> ChatboxAIScheduledMessages { get; set; }
    #endregion

    #region Cultivation
    public virtual DbSet<PlantingType> PlantingTypes { get; set; }
    public virtual DbSet<PlantingRegion> PlantingRegions { get; set; }
    public virtual DbSet<PlantingBed> PlantingBeds { get; set; }
    #endregion

    public string RemoveAcents(string @String) => throw new InvalidOperationException();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(
            typeof(ApplicationDbContext).GetMethod(nameof(RemoveAcents), new[] { typeof(string) }),
            b =>
            {
                b.HasName("RemoveAcents");
                b.HasParameter("String");
            });

        RegisterPocoSelections(modelBuilder);
        // ==== ChatSupport ====
        modelBuilder.Entity<ChatSupportRoom>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(128);
            b.Property(x => x.Kind).HasMaxLength(16);
            b.HasMany(x => x.Participants).WithOne(x => x.Room).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Messages).WithOne(x => x.Room).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatSupportParticipant>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId).HasMaxLength(64);
            b.Property(x => x.DisplayName).HasMaxLength(128);
            b.HasIndex(x => new { x.RoomId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<ChatSupportMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.SenderId).HasMaxLength(64);
            b.Property(x => x.SenderName).HasMaxLength(128);
            b.Property(x => x.Content).HasMaxLength(4096);
            b.Property(x => x.ContentType).HasMaxLength(64);
            b.HasIndex(x => new { x.RoomId, x.SentAt });
        });


        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SalaryConfiguration());

        modelBuilder.ApplyConfiguration(new CategoryStatusWebPeriodConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryStatusWebPeriodGoodConfiguration());

        modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedureConditionConfiguration());

        modelBuilder.ApplyConfiguration(new ChatboxAITopicConfiguration());

        // ==== Cultivation ====
        modelBuilder.ApplyConfiguration(new PlantingTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PlantingRegionConfiguration());
        modelBuilder.ApplyConfiguration(new PlantingBedConfiguration());


        modelBuilder.ApplyContractorConfiguration();

        modelBuilder.Seed();
        modelBuilder.Entity<ReminderParticipant>()
            .HasKey(rp => new { rp.ReminderId, rp.UserId });

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Register POCO classes for custom columns on SQL select
    /// </summary>
    /// <param name="modelBuilder"></param>
    private void RegisterPocoSelections(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountBalanceSheetPoco>().HasNoKey().ToView(null);
    }

    public IQueryable<ChartOfAccount> GetChartOfAccount(int year)
    {
        return ChartOfAccounts.Where(a => a.Year == year);
    }
    public IQueryable<Ledger> GetLedger(int year, int isInternal = 1)
    {
        Expression<Func<Ledger, bool>> predicate = a => a.Year == year;

        predicate = Array.Exists(InternalValues, x => x == isInternal)
                ? predicate.And(x => InternalValues.Any(a => a == x.IsInternal))
                : predicate.And(x => x.IsInternal == isInternal);

        return Ledgers.Where(predicate);
    }

    public IQueryable<Ledger> GetLedgerNotForYear(int isInternal)
    {
        Expression<Func<Ledger, bool>> predicate = Array.Exists(InternalValues, x => x == isInternal)
               ? x => InternalValues.Any(a => a == x.IsInternal)
               : x => x.IsInternal == isInternal;

        return Ledgers.Where(predicate);
    }
    public IQueryable<ChartOfAccountGroup> GetChartOfAccountGroup(int year)
    {
        return ChartOfAccountGroups.Where(a => a.Year == year);
    }
    public IQueryable<ChartOfAccountGroupLink> GetChartOfAccountGroupLink(int year)
    {
        return ChartOfAccountGroupLinks.Where(a => a.Year == year);
    }
}