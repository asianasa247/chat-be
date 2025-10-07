using DinkToPdf;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Emails;
using ManageEmployee.Hanet;
using ManageEmployee.Helpers;
using ManageEmployee.JobSchedules;
using ManageEmployee.Queues;
using ManageEmployee.Services;
using ManageEmployee.Services.BillServices;
using ManageEmployee.Services.CarServices;
using ManageEmployee.Services.ChartOfAccountServices;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.Contract;
using ManageEmployee.Services.CustomerServices;
using ManageEmployee.Services.GoodsQuotaServices;
using ManageEmployee.Services.GoodsServices;
using ManageEmployee.Services.Hanet;
using ManageEmployee.Services.HotelServices;
using ManageEmployee.Services.InOutServices;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.Services.Interfaces.Allowances;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Cars;
using ManageEmployee.Services.Interfaces.Categories;
using ManageEmployee.Services.Interfaces.Certificates;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Configs;
using ManageEmployee.Services.Interfaces.Configurations;
using ManageEmployee.Services.Interfaces.Contracts;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.Decides;
using ManageEmployee.Services.Interfaces.DecisionTypes;
using ManageEmployee.Services.Interfaces.Degrees;
using ManageEmployee.Services.Interfaces.Departments;
using ManageEmployee.Services.Interfaces.Descriptions;
using ManageEmployee.Services.Interfaces.DeskFloors;
using ManageEmployee.Services.Interfaces.Documents;
using ManageEmployee.Services.Interfaces.Events;
using ManageEmployee.Services.Interfaces.Excels;
using ManageEmployee.Services.Interfaces.FaceRecognitions;
using ManageEmployee.Services.Interfaces.FinalStandards;
using ManageEmployee.Services.Interfaces.Generators;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Hanets;
using ManageEmployee.Services.Interfaces.HistoryAchievements;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.Services.Interfaces.Introduces;
using ManageEmployee.Services.Interfaces.Inventorys;
using ManageEmployee.Services.Interfaces.Invoices;
using ManageEmployee.Services.Interfaces.Jobs;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.Ledgers.V3;
using ManageEmployee.Services.Interfaces.LookupValues;
using ManageEmployee.Services.Interfaces.Mails;
using ManageEmployee.Services.Interfaces.MainColors;
using ManageEmployee.Services.Interfaces.Majors;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.Services.Interfaces.Orders;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.Positions;
using ManageEmployee.Services.Interfaces.Prints;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;
using ManageEmployee.Services.Interfaces.Projects;
using ManageEmployee.Services.Interfaces.Relatives;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Salarys;
using ManageEmployee.Services.Interfaces.Signatures;
using ManageEmployee.Services.Interfaces.Sliders;
using ManageEmployee.Services.Interfaces.Stationeries;
using ManageEmployee.Services.Interfaces.Statuses;
using ManageEmployee.Services.Interfaces.Surcharges;
using ManageEmployee.Services.Interfaces.Symbols;
using ManageEmployee.Services.Interfaces.Targets;
using ManageEmployee.Services.Interfaces.TaxRates;
using ManageEmployee.Services.Interfaces.TimeKeepings;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using ManageEmployee.Services.Interfaces.WareHouses;
using ManageEmployee.Services.Interfaces.Webs;
using ManageEmployee.Services.Interfaces.Socials;
using ManageEmployee.Services.Invoice;
using ManageEmployee.Services.Invoice.VintaxOne;
using ManageEmployee.Services.LedgerServices;
using ManageEmployee.Services.LedgerServices.V2;
using ManageEmployee.Services.LedgerServices.V3;
using ManageEmployee.Services.P_ProcedureServices;
using ManageEmployee.Services.P_ProcedureServices.ExpenditurePlanServices;
using ManageEmployee.Services.P_ProcedureServices.Services;
using ManageEmployee.Services.ProduceProductServices;
using ManageEmployee.Services.ProduceProductServices.ManufactureOrderServices;
using ManageEmployee.Services.ProduceProductServices.OrderProduceProductServices;
using ManageEmployee.Services.ProduceProductServices.PlanningProduceProductServices;
using ManageEmployee.Services.Reports;
using ManageEmployee.Services.StationerySevices;
using ManageEmployee.Services.UserServices;
using ManageEmployee.Services.UserServices.SalaryServices;
using ManageEmployee.Services.WareHouseService;
using ManageEmployee.Services.Web;
using ManageEmployee.Services.Socials;
using ManageEmployee.Validators;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ManageEmployee.JobSchedules.Interface;
using ManageEmployee.Services.GoodPrizeServices;
using ManageEmployee.Services.BillServices.BillReports.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Bills.BillReports.OperatingCosts;
using ManageEmployee.Services.BillServices.BillReports.OperatingCosts;
using ManageEmployee.Services.Email;
using ManageEmployee.Services.Interfaces.Email;
using ManageEmployee.HttpClients;
using ManageEmployee.Services.BillServices.BillReports.GoodReports;
using ManageEmployee.Services.Reports.LedgerDetailReports;
using ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports;
using ManageEmployee.Services.BillServices.BillReports.ConvertGoodReports;
using ManageEmployee.Services.Interfaces.Zalo;
using ManageEmployee.Services.ZaloServices;
using ManageEmployee.Services.Interfaces.EmployessByOrder;
using ManageEmployee.Services.Interfaces.FAQ_AIchat;
using ManageEmployee.Services.FAQ_AIchatServices;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using ManageEmployee.Services.ChatboxAI;
using ManageEmployee.Services.AdditionWebServices;
using ManageEmployee.Services.Interfaces.AdditionWebServices;

namespace ManageEmployee.Extends;

public static class InjectionExtend
{
    public static void RegisterServiceInjection(this IServiceCollection services)
    {
        services.AddScoped<IMessengerServices, MessengerServices>();
        services.AddScoped<ITelegramBotService, TelegramBotService>();
        services.AddScoped<IFAQ_AIchatService, FAQ_AIchatService>();
        services.AddScoped<IFAQ_AIchatDetailService, FAQ_AIchatDetailService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IOtherCompanyInfoGetter, OtherCompanyInfoGetter>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICalculateSalaryService, CalculateSalaryService>();
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IWardService, WardService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IContractTypeService, ContractTypeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IPositionDetailService, PositionDetailService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<ITargetService, TargetService>();
        services.AddScoped<ISymbolService, SymbolService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IMajorService, MajorService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITaxRateService, TaxRateService>();
        services.AddScoped<IFinalStandardService, FinalStandardService>();
        #region ledger
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<ILedgerForSaleService, LedgerForSaleService>();
        services.AddScoped<ILedgerProcedureProductService, LedgerProcedureProductService>();
        services.AddScoped<ILedgerProduceService, LedgerProduceService>();
        services.AddScoped<ILedgerProduceHelper, LedgerProduceHelper>();
        services.AddScoped<ILedgerV2Service, LedgerV2Service>();
        services.AddScoped<ILedgerV3Service, LedgerV3Service>();
        services.AddScoped<ILedgerProcedureProductExporter, LedgerProcedureProductExporter>();
        services.AddScoped<ILedgerProjectReporter, LedgerProjectReporter>();
        services.AddScoped<IVitaxInvoiceGetterJob, VitaxInvoiceGetterJob>();
        services.AddScoped<IExportDataReport_SoChiTiet, ExportDataReport_SoChiTiet>();
        services.AddScoped<ISoChiTiet_FourReporter, SoChiTiet_FourReporter>();

        #endregion

        #region ChartOfAccount
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        services.AddScoped<IChartOfAccountUpdater, ChartOfAccountUpdater>();
        services.AddScoped<IChartOfAccountDetailUpdater, ChartOfAccountDetailUpdater>();
        services.AddScoped<INameCodeAccountChanger, NameCodeAccountChanger>();
        services.AddScoped<IChartOfAccountCalculateBalancer, ChartOfAccountCalculateBalancer>();
        services.AddScoped<IChartOfAccountGroupService, ChartOfAccountGroupService>();
        services.AddScoped<IChartOfAccountGroupLinkService, ChartOfAccountGroupLinkService>();
        services.AddScoped<IChartOfAccountValidator, ChartOfAccountValidator>();
        services.AddScoped<IChartOfAccountFilterService, ChartOfAccountFilterService>();
        services.AddScoped<IChartOfAccountForCashierService, ChartOfAccountForCashierService>();
        services.AddScoped<IChartOfAccountDeleter, ChartOfAccountDeleter>();
        #endregion

        #region Good
        services.AddScoped<IGoodsService, GoodsService>();
        services.AddScoped<IGoodImporter, GoodImporter>();
        services.AddScoped<IGoodSynchronizer, GoodSynchronizer>();
        services.AddScoped<IGoodsDetailService, GoodsDetailService>();
        services.AddScoped<IGoodsQuotaRecipeService, GoodsQuotaRecipeService>();
        services.AddScoped<IGoodsQuotaService, GoodsQuotaService>();
        services.AddScoped<IGoodExporter, GoodExporter>();
        services.AddScoped<IGoodSetter, GoodSetter>();
        services.AddScoped<IGoodsInWarehouseReporter, GoodsInWarehouseReporter>();

        #endregion

        services.AddScoped<IBillService, BillService>();
        services.AddScoped<IBillReporter, BillReporter>();
        services.AddScoped<IBillDetailService, BillDetailService>();
        services.AddScoped<IBillTrackingService, BillTrackingService>();
        services.AddScoped<IDeskFloorService, DeskFloorService>();
        services.AddScoped<IFixedAssetsService, FixedAssetService>();
        services.AddScoped<IDocumentType1Service, DocumentType1Service>();
        services.AddScoped<IDocumentType2Service, DocumentType2Service>();
        services.AddScoped<IBillExporter, BillExporter>();

        #region Customer
        services.AddScoped<ICustomerTaxInformationService, CustomerTaxInformationService>();
        services.AddScoped<ICustomerContactHistoryService, CustomerContactHistoryService>();
        services.AddScoped<ICustomerClassificationService, CustomerClassificationService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerReporter, CustomerReporter>();
        services.AddScoped<ICustomerQuoteService, CustomerQuoteService>();
        services.AddScoped<IReportDebitCustomerService, ReportDebitCustomerService>();
        #endregion

        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IRelationShipService, RelationShipService>();
        services.AddScoped<IRelativeService, RelativeService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IUserTaskService, UserTaskService>();
        services.AddScoped<IUserTaskCommentService, UserTaskCommentService>();
        services.AddScoped<IDescriptionService, DescriptionService>();
        services.AddScoped<IInOutTimeExtend, InOutTimeExtend>();
        services.AddScoped<IInOutService, InOutService>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ILookupValueService, LookupValueService>();
        services.AddScoped<IAccountBalanceSheetService, AccountBalanceSheetv2Service>();
        services.AddScoped<IPayerServices, PayerServices>();
        services.AddScoped<IDegreeService, DegreeService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IIntroduceService, IntroduceService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDecideService, DecideService>();
        services.AddScoped<IHistoryAchievementsService, HistoryAchievementsService>();
        services.AddScoped<IDecisionTypeService, DecisionTypeService>();
        services.AddScoped<ISalaryLevelService, SalaryLevelService>();
        
        services.AddScoped<IVoucherService, VoucherService>();
        services.AddScoped<ILedgerHelperService, LedgerHelperService>();
        services.AddScoped<IBalanceSheetService, BalanceSheetService>();
        services.AddScoped<ISavedMovedMoneyReportService, SavedMovedMoneyReportService>();
        services.AddScoped<IPlanMissionCountryTaxService, PlanMissionCountryTaxService>();
        services.AddScoped<IReportTaxService, ReportTaxService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPrintService, PrintService>();
        services.AddScoped<ITypeWorkService, TypeWorkService>();
        services.AddScoped<ISendMailService, SendMailService>();
        services.AddScoped<IAccountPayService, AccountPayService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IManagementAriesExcelService, ManagementAriesExcelService>();
        services.AddScoped<IGoodWarehousesService, GoodWarehousesService>();
        services.AddScoped<IFixedAssets242Service, FixedAssets242Service>();
        services.AddScoped<IAllowanceService, AllowanceService>();
        services.AddScoped<IInvoiceDeclarationService, InvoiceDeclarationService>();
        services.AddScoped<IProcedureService, ProcedureService>();
        services.AddScoped<IIsoftHistoryService, IsoftHistoryService>();
        services.AddScoped<IAllowanceUserService, AllowanceUserService>();
        services.AddScoped<IP_SalaryAdvanceService, P_SalaryAdvanceService>();
        services.AddScoped<IP_LeaveService, P_LeaveService>();
        services.AddScoped<IMenuKpiService, MenuKpiService>();
        services.AddScoped<IP_KpiService, P_KpiService>();
        services.AddScoped<IP_InventoryService, P_InventoryService>();
        services.AddScoped<IGoodWarehouseExportService, GoodWarehouseExportService>();
        services.AddScoped<IGeneralDiaryService, GeneralDiaryService>();
        
        services.AddScoped<IBillHistoryCollectionService, BillHistoryCollectionService>();
        services.AddScoped<IFixedAssetsUserService, FixedAssetsUserService>();
        services.AddScoped<IConfigAriseBehaviourService, ConfigAriseBehaviourService>();
        services.AddScoped<IMainColorService, MainColorService>();
        services.AddScoped<IUserSalaryService, UserSalaryService>();
        services.AddScoped<ICategoryStatusWebPeriodService, CategoryStatusWebPeriodService>();
        services.AddScoped<ISurchargeService, SurchargeService>();
        services.AddScoped<ITillManagerService, TillManagerService>();
        services.AddScoped<ISendMailBirthdayJob, SendMailBirthdayJob>();
        services.AddScoped<ICheckInService, CheckInService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IDocumentV2Service, DocumentV2Service>();
        services.AddScoped<ILedgerFixedAssetService, LedgerFixedAssetService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IUserContractService, UserContractService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IUserContractHistoryService, UserContractHistoryService>();
        services.AddScoped<ILedgerWareHouseService, LedgerWareHouseService>();
        services.AddScoped<ILedgerDetailBookService, LedgerDetailBookService>();
        services.AddScoped<IBillForSaleReporter, BillForSaleReporter>();

        #region queue

        services.AddScoped<IChartOfAccountCaculatorQueue, ChartOfAccountCaculatorQueue>();
        services.AddScoped<ILedgerImportErrorQueue, LedgerImportErrorQueue>();

        #endregion queue

        #region warehouse

        services.AddScoped<IWareHousePositionService, WareHousePositionService>();
        services.AddScoped<IWareHouseFloorService, WareHouseFloorService>();
        services.AddScoped<IWareHouseShelvesService, WareHouseShelvesService>();

        #endregion warehouse

        #region v2

        services.AddScoped<IDescriptionV2Service, DescriptionV2Service>();
        services.AddScoped<IAccountBalanceSheetV2Service, AccountBalanceSheetV2Service>();
        services.AddScoped<IChartOfAccountV2Service, ChartOfAccountV2Service>();
        services.AddScoped<ITaxRateV2Service, TaxRateV2Service>();

        #endregion v2

        #region invoice

        services.AddScoped<IInvoiceCreator, InvoiceCreator>();
        services.AddScoped<IInvoiceAuthorize, InvoiceAuthorize>();
        services.AddScoped<IVintaxOneInvoiceCreator, VintaxOneInvoiceCreator>();

        #endregion


        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        services.AddScoped<IBillTrackingForNotificationService, BillTrackingForNotificationService>();
        services.AddScoped<ILedgerUpdateChartOfAccountNameService, LedgerUpdateChartOfAccountNameService>();

        #region hotels
        services.AddScoped<IRoomConfigureTypeService, RoomConfigureTypeService>();
        services.AddScoped<IGoodRoomTypeService, GoodRoomTypeService>();
        services.AddScoped<IGoodRoomPriceService, GoodRoomPriceService>();

        #endregion
        services.AddScoped<ILedgerDangKiChungTuGhiSoService, LedgerDangKiChungTuGhiSoService>();
        services.AddScoped<IContractFileService, ContractFileService>();
        services.AddScoped<ITimeKeepingService, TimeKeepingService>();
        services.AddScoped<IStationeryService, StationeryService>();
        services.AddScoped<IStationeryImportService, StationeryImportService>();
        services.AddScoped<IStationeryExportService, StationeryExportService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IPetrolConsumptionService, PetrolConsumptionService>();
        services.AddScoped<IWebMailService, WebMailService>();
        services.AddScoped<IEventWithImageService, EventWithImageService>();
        services.AddScoped<ICarFieldService, CarFieldService>();
        services.AddScoped<IWarningNotificationService, WarningNotificationService>();
        services.AddScoped<IRoadRouteService, RoadRouteService>();
        services.AddScoped<IPoliceCheckPointService, PoliceCheckPointService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IProcedureStatusService, ProcedureStatusService>();
        services.AddScoped<IProcedureStatusStepService, ProcedureStatusStepService>();
        services.AddScoped<IProcedureRequestOvertimeService, ProcedureRequestOvertimeService>();
        services.AddScoped<IWorkingDayService, WorkingDayService>();
        services.AddScoped<ICharOfAccountSyncService, CharOfAccountSyncService>();
        services.AddScoped<IProcedureChangeShiftService, ProcedureChangeShiftService>();
        services.AddScoped<IShiftUserService, ShiftUserService>();
        services.AddScoped<IInOutReportService, InOutReportService>();
        services.AddScoped<ISalaryUserVersionService, SalaryUserVersionService>();
        services.AddScoped<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IGoodsPromotionService, GoodsPromotionService>();
        services.AddScoped<IProcedureHelperService, ProcedureHelperService>();
        services.AddScoped<IEmailLogin, EmailLogin>();
        services.AddScoped<IGoodsQuotaStepService, GoodsQuotaStepService>();
        services.AddScoped<IProcedureConditionService, ProcedureConditionService>();
        services.AddScoped<IPaymentProposalService, PaymentProposalService>();
        services.AddScoped<IAdvancePaymentService, AdvancePaymentService>();
        services.AddScoped<IGatePassService, GatePassService>();
        services.AddScoped<IRequestExportGoodService, RequestExportGoodService>();

        #region Bill
        services.AddScoped<IBillPromotionService, BillPromotionService>();
        services.AddScoped<IBusinessOverviewReporter, BusinessOverviewReporter>();
        services.AddScoped<IBusinessDetailForRevenueReporter, BusinessDetailForRevenueReporter>();
        services.AddScoped<IBusinessDetailForRefundReporter, BusinessDetailForRefundReporter>();
        services.AddScoped<IBusinessDetailForTotalBillReporter, BusinessDetailForTotalBillReporter>();
        services.AddScoped<IOperatingCostOverviewReporter, OperatingCostOverviewReporter>();
        services.AddScoped<IBusinessOverviewChartReporter, BusinessOverviewChartReporter>();
        services.AddScoped<IGoodOverviewReporter, GoodOverviewReporter>();
        services.AddScoped<IGoodConvertReporter, GoodConvertReporter>();
        services.AddScoped<IGoodOppositeReporter, GoodOppositeReporter>();
        services.AddScoped<IGoodNormalReporter, GoodNormalReporter>();
        #endregion

        #region Produce product
        services.AddScoped<IWarehouseProduceProductService, WarehouseProduceProductService>();
        services.AddScoped<IOrderProduceProductService, OrderProduceProductService>();
        services.AddScoped<IOrderProduceProductExcelService, OrderProduceProductExcelService>();
        services.AddScoped<IOrderProduceProductReporter, OrderProduceProductReporter>();
        services.AddScoped<IPlanningProduceProductService, PlanningProduceProductService>();
        services.AddScoped<IPlanningProduceProductExportService, PlanningProduceProductExportService>();
        services.AddScoped<IPlanningWithLedgerService, PlanningWithLedgerService>();
        services.AddScoped<IProcedureOrderProduceProductHelperService, ProcedureOrderProduceProductHelperService>();
        services.AddScoped<IManufactureOrderService, ManufactureOrderService>();
        services.AddScoped<IManufactureOrderExporter, ManufactureOrderExporter>();
        services.AddScoped<IProcedureExportHelper, ProcedureExportHelper>();
        services.AddScoped<ICarDeliveryService, CarDeliveryService>();
        services.AddScoped<IRequestEquipmentService, RequestEquipmentService>();
        services.AddScoped<IProduceProductService, ProduceProductService>();
        services.AddScoped<IProduceProductLedgerService, ProduceProductLedgerService>();
        services.AddScoped<IProcedurePlanningProduceProductHelper, ProcedurePlanningProduceProductHelper>();
        #endregion


        services.AddScoped<IGoodPriceListService, GoodPriceListService>();
        services.AddScoped<IIntroduceTypeService, IntroduceTypeService>();
        services.AddScoped<IHtmlToPdfConverter, HtmlToPdfConverter>();
        services.AddScoped<INumberOfMealService, NumberOfMealService>();
        services.AddScoped<IExpenditurePlanService, ExpenditurePlanService>();
        services.AddScoped<IExpenditurePlanExporter, ExpenditurePlanExporter>();
        services.AddScoped<IConfigDiscountService, ConfigDiscountService>();
        services.AddScoped<ISalaryTypeService, SalaryTypeService>();
        services.AddScoped<ISalaryTypeProduceProductService, SalaryTypeProduceProductService>();
        services.AddScoped<ICarLocationService, CarLocationService>();
        services.AddScoped<IDriverRouterService, DriverRouterService>();
        services.AddScoped<IInOutImporter, InOutImporter>();
        services.AddScoped<IRequestEquipmentOrderService, RequestEquipmentOrderService>();
        services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();
        services.AddScoped<ISignatureBlockService, SignatureBlockService>();
        services.AddScoped<IVehicleRepairRequestService, VehicleRepairRequestService>();
        services.AddScoped<IWeeklyScheduleService, WeeklyScheduleService>();

        services.AddScoped<ITimeKeepingReportHelper, TimeKeepingReportHelper>();

        #region hanet
        services.AddScoped<IHanetInOut, HanetInOut>();
        services.AddScoped<IHanetRegister, HanetRegister>();
        services.AddScoped<HttpClient, HttpClient>();
        services.AddScoped<IHanetPlace, HanetPlace>();
        services.AddScoped<IHanetUserService, HanetUserService>();
        #endregion

        #region social
        services.AddScoped<ISocials, SocialServices>();
        #endregion

        #region web
        services.AddScoped<IWebAuthService, WebAuthService>();
        services.AddScoped<IWebCartService, WebCartService>();
        services.AddScoped<IWebOrderService, WebOrderService>();
        services.AddScoped<IWebProductService, WebProductService>();
        services.AddScoped<IWebSocialService, WebSocialService>();
        services.AddScoped<IWebNewsService, WebNewsServices>();
        services.AddScoped<IWebCareerService, WebCareerServices>();
        services.AddScoped<IWebSettingsSpinService, WebSettingsSpinService>();
        services.AddScoped<IWebPrizeService, WebPrizeService>();
        services.AddScoped<IAdditionWebService, AdditionWebService>();
        #endregion

        services.AddScoped<ISpinPrizeRandomService, SpinPrizeRandomService>();
        services.AddScoped<IInvoiceInGetter, InvoiceInGetter>();
        services.AddScoped<IGoodCustomerService, GoodCustomerService>();
        
        services.AddScoped<ISmtpService, SmtpService>();//Dat - 2024-01-16
        services.AddScoped<IMailTemplateService, MailTemplateService>();//Dat - 2024-01-16
        services.AddScoped<IMailLogService, MailLogService>();//Dat - 2024-01-16
        services.AddScoped<IMailTimerService, MailTimerService>();//Dat - 2024-01-16
        services.AddScoped<IMailSendService, MailSendService>();//Dat - 2024-01-16
        services.AddScoped<IZaloAppConfigService, ZaloAppConfigService>();// Dat - 2025-02-17
        services.AddScoped<IZaloUserService, ZaloUserService>();// Dat - 2025-02-26
        services.AddScoped<IEmployeeByOrderService, EmployeeByOrderService>();
        services.AddScoped<IProductOfEmployeeService, ProductOfEmployeeService>();
        services.AddScoped<ICommissionService, CommissionService>();
        #region Client
        services.AddHttpClient<IVitaxOneClient, VitaxOneClient>();

        #endregion

        #region ChatboxAI
        services.AddScoped<IChatboxAITopicService, ChatboxAITopicService>();//Dat - 2024-01-16
        #endregion
        DataLayer.Service.Registrations.ServiceRegistration.Register(services);


        services.AddScoped<IAdditionWebGoodsService, AdditionWebGoodsService>();
        services.AddScoped<IAdditionWebOrderService, AdditionWebOrderService>();
    }
}