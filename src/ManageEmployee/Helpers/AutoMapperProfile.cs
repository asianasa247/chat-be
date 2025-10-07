using AutoMapper;
using Common.Extensions;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.AllowanceModels;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.InOut;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.NumberOfMealModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PrintViewModelModels;
using ManageEmployee.DataTransferObject.PrizeModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.RegisterModels;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.SignatureBlockModels;
using ManageEmployee.DataTransferObject.Smtp;
using ManageEmployee.DataTransferObject.Stationery;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.SupplyModels.VehicleRepairRequestModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.DataTransferObject.V3;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.DataTransferObject.WeeklyScheduleModels;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Entities;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Entities.AllowanceEntities;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.Entities.ContractEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.DecideEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Entities.Email;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.HanetEntities;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.LotteryEntities;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.Entities.PrintEntities;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.ProcedureEntities.WeeklyScheduleEntities;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.StationeryEntities;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Entities.WareHouseEntities;
using ManageEmployee.Entities.ZaloEntities;
using ManageEmployee.Models.ZaloModel;
using ManageEmployee.Services.Web;
using Newtonsoft.Json;

namespace ManageEmployee.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        AllowNullCollections = true;
        AllowNullDestinationValues = true;
        CreateMap<RegisterModel, UserModel>();

        CreateMap<User, UserModel>()
            .ForMember(
                userModel => userModel.Images,
                opt => opt.MapFrom(
                    src => JsonConvert.DeserializeObject<List<FileDetailModel>>(src.Images.DefaultIfNullOrEmpty(""))
                ));

        CreateMap<UserModel, User>()
            .ForMember(
                userModel => userModel.Images,
                opt => opt.MapFrom(
                    src => JsonConvert.SerializeObject(src.Images)
                ));
        
        CreateMap<RegisterLaborWebModel, User>();
        CreateMap<BranchModel, Branch>();
        CreateMap<DocumentModel.MapDocument, Document>();
        CreateMap<DocumentV2Model, Document>().ReverseMap();
        CreateMap<Document, DocumentModel.MapDocument>();
        CreateMap<CustomerContactHistoryDto, CustomerContactHistory>(); 
        CreateMap<RelativeViewModel, Relative>();
        CreateMap<DocumentType1ViewModel, DocumentType1>();
        CreateMap<DocumentType2ViewModel, DocumentType2>();
        CreateMap<UserTaskModel, UserTask>();
        CreateMap<UserTaskCheckListModel, UserTaskCheckList>();
        CreateMap<UserTaskRoleDetailsModel, UserTaskRoleDetails>();

        CreateMap<UserTaskCheckList, UserTaskCheckListModel>();
        CreateMap<UserTaskRoleDetails, UserTaskRoleDetailsModel>();
        CreateMap<UserTask, UserTaskModel>();
        CreateMap<InOutHistoryViewModel, InOutHistory>();
        CreateMap<InOutHistory, InOutHistoryViewModel>();
        CreateMap<UserTaskCommentModel, UserTaskComment>();
        CreateMap<CertificateModel, Certificate>();
        CreateMap<Certificate, CertificateModel>();
        CreateMap<HistoryAchievementModel, HistoryAchievement>();
        CreateMap<HistoryAchievement, HistoryAchievementModel>(); 
        CreateMap<Province, ProvinceModel>(); 
        CreateMap<ProvinceModel, Province>();
        CreateMap<ChartOfAccountModel, ChartOfAccount>();
        CreateMap<FixedAsset, FixedAssetsModel>().ReverseMap();
        CreateMap<FixedAssetsModel, FixedAssetsModelEdit>().ReverseMap();
        CreateMap<FixedAsset242, FixedAssetsModelEdit>().ReverseMap();
        CreateMap<FixedAsset, FixedAssetsModelEdit>().ReverseMap();
        CreateMap<DecideViewModel, Decide>().ReverseMap();
        CreateMap<Ledger, LedgerDetailV2Model>();
        CreateMap<BillDetail, BillDetailViewPaging>();
        CreateMap<Menu, MenuViewModel>().ReverseMap();
        CreateMap<MenuRole, MenuRoleViewModel>().ReverseMap();
        CreateMap<GoodsDetailModel, GoodDetail>().ReverseMap();
        CreateMap<User, UserActiveModel>();
        CreateMap<UserTask, UserTaskProject>();
        CreateMap<Print, PrintViewModel>(); 
        CreateMap<LedgerCostOfGoodsModel, Ledger>();
        CreateMap<FixedAssetsModel, FixedAsset242>().ReverseMap();
        CreateMap<FixedAsset, FixedAsset242>().ReverseMap();
        CreateMap<AllowanceViewModel, Allowance>().ReverseMap();
        CreateMap<Allowance, AllowanceSelectList>();
        CreateMap<AllowanceUser, AllowanceUserDetailViewModel>().ReverseMap();
        CreateMap<P_SalaryAdvance, P_SalaryAdvanceViewModel>().ReverseMap();
        CreateMap<P_SalaryAdvance_Item, P_SalaryAdvance_ItemViewModel>().ReverseMap(); 
        CreateMap<P_Kpi, P_KpiViewModel>().ReverseMap();
        CreateMap<P_Kpi_Item, P_Kpi_Item_ViewModel>().ReverseMap();
        CreateMap<P_Inventory, P_InventoryViewModel>().ReverseMap();
        CreateMap<P_Inventory_Item, P_Inventory_Item_ViewModel>().ReverseMap();
        CreateMap<GoodWarehouses, GoodWarehousesViewModel>().ReverseMap();
        CreateMap<P_Leave, P_LeaveViewModel>().ReverseMap();
        CreateMap<P_Leave_Item, P_Leave_ItemModel>().ReverseMap();
        CreateMap<Customer, CustomerWarning>();
        CreateMap<FixedAssetViewModel, FixedAsset242>().ReverseMap();
        CreateMap<FixedAsset242, FixedAssetUser>();
        CreateMap<FixedAssetUser, FixedAssetViewModel>().ReverseMap();
        CreateMap<LedgerExport, Ledger>().ReverseMap();
        CreateMap<FixedAsset, FixedAssetViewModel>().ReverseMap();
        CreateMap<CustomerImport, Customer>();
        CreateMap<TillManagerModel, TillManager>();
        CreateMap<CategoryStatusWebPeriod, CategoryStatusWebPeriodModel>().ReverseMap();
        CreateMap<CategoryStatusWebPeriod, CategoryStatusWebPeriodGoodModel>().ReverseMap();
        CreateMap<CategoryStatusWebPeriod, CategoryStatusWebPeriodGoodShowWebModel>().ReverseMap();
        CreateMap<InvoiceDeclaration, InvoiceDeclarationModel>();
        CreateMap<Ledger, LedgerPrint>();
        CreateMap<GoodWarehouses, GoodWarehousesUpdateModel>();
        CreateMap<GoodWarehousesPosition, GoodWarehousesPositionUpdateModel>();
        CreateMap<Warehouse, WarehouseSetterModel>().ReverseMap();
        CreateMap<WarehouseShelveSetterModel, WareHouseShelves>().ReverseMap();
        CreateMap<WarehouseFloorSetterModel, WareHouseFloor>().ReverseMap();
        CreateMap<CustomerTaxInformation, CustomerTaxInformationModel>().ReverseMap();
        CreateMap<CustomerTaxInformationAccountant, CustomerTaxInformationAccountantModel>().ReverseMap();
        CreateMap<Warehouse, WarehousePaging>();
        CreateMap<WareHouseShelves, WareHouseShelvesPaging>();
        CreateMap<WareHouseFloor, WarehouseFloorPaging>();
        CreateMap<TaxRate, TaxRateModel>();
        CreateMap<Customer, CustomerGetterModel>();
        CreateMap<Customer, CustomerModelView>();
        CreateMap<SalarySocial, SalarySocialDetailModel>();
        CreateMap<FixedAssetUser, FixedAssetUserGetterModel>();
        CreateMap<Ledger, LedgerDetailV3Model>();
        CreateMap<ContractTypeModel, ContractType>();
        CreateMap<Goods, GoodslDetailModel>();
        CreateMap<LedgerV3UpdateModel, Ledger>();
        CreateMap<ChartOfAccount, ChartOfAccountForCashier>();
        CreateMap<LedgerWareHouse, LedgerWarehouseDetail>();
        CreateMap<CategoryModel, Category>().ReverseMap();
        CreateMap<ContractFile, ContractFileModel>().ReverseMap();
        CreateMap<Stationery, StationeryModel>().ReverseMap();
        CreateMap<StationeryImport, StationeryImportGetterModel>();
        CreateMap<StationeryImport, StationeryImportModel>().ReverseMap();
        CreateMap<StationeryImportItem, StationeryImportItemModel>().ReverseMap();
        CreateMap<StationeryExport, StationeryExportGetterModel>();
        CreateMap<StationeryExport, StationeryExportModel>().ReverseMap();
        CreateMap<StationeryExportItem, StationeryExportItemModel>().ReverseMap();
        CreateMap<Car, CarGetterModel>();
        CreateMap<Car, CarGetterDetailModel>();
        CreateMap<Car, CarGetterPagingModel>();
        CreateMap<Car, CarModel>().ReverseMap();
        CreateMap<PetrolConsumption, PetrolConsumptionGetterModel>();
        CreateMap<PetrolConsumption, PetrolConsumptionModel>().ReverseMap();
        CreateMap<EventWithImage, EventWithImagePagingGetterModel>();
        CreateMap<EventWithImage, EventWithImageDetailGetterModel>();
        CreateMap<CarFieldSetup, CarFieldSetupModel>().ReverseMap();
        CreateMap<ChartOfAccountImportModel, ChartOfAccount>();
        CreateMap<CustomerQuote, CustomerQuoteModel>();
        CreateMap<Ledger, LedgerModel>();
        CreateMap<CustomerQuote_DetailModel, CustomerQuote_Detail>().ReverseMap();
        CreateMap<CarFieldSetup, CarFieldSetupGetterModel>().ReverseMap();
        CreateMap<Shift, ShiftModel>().ReverseMap();
        CreateMap<ProcedureRequestOvertime, ProcedureRequestOvertimeModel>().ReverseMap();
        CreateMap<ProcedureRequestOvertime, ProcedureRequestOvertimePagingModel>();
        CreateMap<OrderProduceProductModel, OrderProduceProduct>().ReverseMap();
        CreateMap<OrderProduceProductDetail, OrderProduceProductDetailModel>().ReverseMap();
        CreateMap<OrderProduceProduct, OrderProduceProductPagingModel>();
        CreateMap<PlanningProduceProduct, PlanningProduceProductPagingModel>();
        CreateMap<PlanningProduceProduct, PlanningProduceProductListGetterModel>();
        CreateMap<PlanningProduceProduct, PlanningProduceProductModel>().ReverseMap();
        CreateMap<PlanningProduceProductDetail, PlanningProduceProductDetailModel>().ReverseMap();
        CreateMap<ProcedureChangeShift, ProcedureChangeShiftModel>().ReverseMap();
        CreateMap<ShiftUserDetail, ShiftUserDetailModel>().ReverseMap();
        CreateMap<ShiftUser, ShiftUserModel>().ReverseMap();
        CreateMap<SalaryUserVersion, SalaryUserVersionDetailModel>();
        CreateMap<SalaryUserVersionUpdateModel, SalaryUserVersion>();

        CreateMap<GoodsPromotionSetterModel, GoodsPromotion>();
        CreateMap<GoodsPromotion, GoodsPromotionGetListModel>();
        CreateMap<GoodsPromotion, GoodsPromotionModel>();
        CreateMap<GoodsPromotion, GoodsPromotionGetDetailModel>();
        CreateMap<GoodsPromotionDetail, GoodsPromotionDetailModel>();
        CreateMap<GoodsPromotionDetail, BillPromotion>();
        CreateMap<Bill, BillModel>();
        CreateMap<AdvancePaymentModel, AdvancePayment>().ReverseMap();
        CreateMap<PaymentProposal, PaymentProposalPagingModel>();
        CreateMap<PaymentProposal, PaymentProposalModel>().ReverseMap();
        CreateMap<PaymentProposalDetail, PaymentProposalDetailModel>().ReverseMap();
        CreateMap<GatePassModel, GatePass>().ReverseMap();
        CreateMap<RequestExportGood, RequestExportGoodPagingModel>();
        CreateMap<RequestExportGood, RequestExportGoodModel>();
        CreateMap<RequestExportGoodDetail, RequestExportGoodDetailModel>();
        CreateMap<WarehouseProduceProduct, WarehouseProduceProductPagingModel>();
        CreateMap<WarehouseProduceProduct, WarehouseProduceProductModel>().ReverseMap();
        CreateMap<WarehouseProduceProductDetail, WarehouseProduceProductDetailModel>().ReverseMap();
        CreateMap<PlanningProduceProduct, PlanningProduceProductGetDetailModel>();
        CreateMap<PlanningProduceProduct, WarehouseProduceProductModel>();
        CreateMap<PlanningProduceProductDetail, WarehouseProduceProductDetailModel>();
        CreateMap<WarehouseProduceProduct, WarehouseProduceProductGetDetailModel>();
        CreateMap<ManufactureOrder, ManufactureOrderPagingModel>();
        CreateMap<ManufactureOrder, ManufactureOrderModel>();
        CreateMap<ManufactureOrderDetail, ManufactureOrderDetailModel>();
        CreateMap<ManufactureOrder, ManufactureOrderGetDetailModel>();
        CreateMap<GatePass, GatePassPagingModel>();
        CreateMap<GoodsPromotionDetailSetterModel, GoodsPromotionDetail>();
        CreateMap<CarDelivery, CarDeliveryModel>().ReverseMap();
        CreateMap<RequestEquipment, RequestEquipmentPagingModel>();
        CreateMap<RequestEquipment, RequestEquipmentModel>().ReverseMap();
        CreateMap<RequestEquipmentDetail, RequestEquipmentDetailModel>().ReverseMap();
        CreateMap<NumberOfMeal, NumberOfMealModel>();
        CreateMap<ProduceProduct, ProduceProductPagingModel>();
        CreateMap<ProduceProduct, ProduceProductModel>();
        CreateMap<NumberOfMealDetail, NumberOfMealDetailModel>();
        CreateMap<ExpenditurePlan, ExpenditurePlanPagingModel>();
        CreateMap<ExpenditurePlan, ExpenditurePlanGetListModel>();
        CreateMap<ExpenditurePlan, ExpenditurePlanModel>().ReverseMap();
        CreateMap<ExpenditurePlanDetail, ExpenditurePlanDetailModel>().ReverseMap();
        CreateMap<ConfigDiscount, ConfigDiscountModel>().ReverseMap();
        CreateMap<SalaryType, SalaryTypeModel>().ReverseMap();
        CreateMap<SalaryTypeProduceProduct, SalaryTypeProduceProductModel>().ReverseMap();
        CreateMap<SalaryTypeProduceProduct, SalaryTypeProduceProductPagingModel>();
        CreateMap<LedgerProcedureProduct, LedgerProduceProductPagingModel>();
        CreateMap<LedgerProcedureProduct, LedgerProduceProductModel>().ReverseMap();
        CreateMap<LedgerProcedureProductDetail, LedgerProduceProductDetailModel>().ReverseMap();
        CreateMap<Ledger, LedgerProduceProductDetailModel>();
        CreateMap<Ledger, LedgerProcedureProductDetail>();
        CreateMap<Ledger, LedgerProduceProductDetailModel>();
        CreateMap<LedgerProcedureProductDetail, LedgerV3UpdateModel>();
        CreateMap<RoadRoute, RoadRoutePagingModel>();
        CreateMap<RoadRoute, RoadRouteModel>().ReverseMap();
        CreateMap<ProduceProduct, ProduceProductGetListModel>();
        CreateMap<CarLocation, CarLocationPagingModel>();
        CreateMap<CarLocation, CarLocationModel>().ReverseMap();
        CreateMap<CarLocationDetail, CarLocationDetailModel>().ReverseMap();
        CreateMap<DriverRouter, DriverRouterPagingModel>();
        CreateMap<DriverRouter, DriverRouterModel>().ReverseMap();
        CreateMap<DriverRouterDetail, DriverRouterDetailModel>();
        CreateMap<AdvancePayment, AdvancePaymentPagingModel>();
        CreateMap<RequestEquipmentOrder, RequestEquipmentOrderPagingModel>();
        CreateMap<RequestEquipmentOrder, RequestEquipmentOrderModel>().ReverseMap();
        CreateMap<RequestEquipmentDetail, RequestEquipmentOrderDetail>();
        CreateMap<RequestEquipmentOrderDetail, RequestEquipmentOrderDetailModel>().ReverseMap();
        CreateMap<SalaryTypeProduceProductDetail, SalaryTypeProduceProductDetailModel>().ReverseMap();
        CreateMap<LedgerProcedureProductDetail, LedgerPrint>().ReverseMap();
        CreateMap<SignatureBlock, SignatureBlockModel>().ReverseMap();
        CreateMap<SignatureBlock, SignatureBlockPagingModel>();
        CreateMap<VehicleRepairRequest, VehicleRepairRequestPagingModel>();
        CreateMap<VehicleRepairRequest, VehicleRepairRequestModel>().ReverseMap();
        CreateMap<VehicleRepairRequestDetail, VehicleRepairRequestDetailModel>().ReverseMap();
        CreateMap<WeeklySchedule, WeeklySchedulePagingModel>();
        CreateMap<WeeklySchedule, WeeklyScheduleModel>().ReverseMap();
        CreateMap<WeeklyScheduleDetail, WeeklyScheduleDetailModel>().ReverseMap();
        CreateMap<CustomerContactHistoryDto, CustomerContactHistoryDetailModel>().ReverseMap();
        CreateMap<CustomerContactHistoryDetailModel, CustomerContactHistory>().ReverseMap();
        CreateMap<SettingsSpin, WebSpinWithCustomerModel>();
        CreateMap<Prize, WebPrizeModel>();
        CreateMap<Goods, ProductsByMenuTypeResponse>();
        CreateMap<Goods, ProductsDetailResponse>();
        CreateMap<AdditionWebModel, AdditionWeb>();

        CreateMap<HanetUser, HanetUserModel>()
            .ForMember(
                hanet => hanet.UserIds,
                opt => opt.MapFrom(
                    src => JsonConvert.DeserializeObject<List<int>>(src.UserIds)
                ));
        CreateMap<HanetUserModel, HanetUser>()
            .ForMember(
                hanet => hanet.UserIds,
                opt => opt.MapFrom(
                    src => JsonConvert.SerializeObject(src.UserIds)
                ));

        CreateMap<Smtp,SmtpModel>().ReverseMap();
        CreateMap<MailLog, MailLogModel>().ReverseMap();
        CreateMap<MailSend, MailSendModel>().ReverseMap();
        CreateMap<MailTemplate, MailTemplateModel>().ReverseMap();
        CreateMap<MailTimer, MailTimerModel>().ReverseMap();
        CreateMap<ZaloAppConfig, ZaloAppConfigModel>().ReverseMap();
        CreateMap<ZaloUser, ZaloUserModel>().ReverseMap();
        //CreateMap<ZaloUserModel, ZaloUser>().ReverseMap();
        CreateMap<ZaloUserRespModel, ZaloUserModel>().ReverseMap();
        CreateMap<EmployeeByOrder, EmployeeByOrderModels>().ReverseMap();
        CreateMap<ProductOfEmployee, ProductOfEmployeeModels>().ReverseMap();
        CreateMap<Commission, CommissionModels>().ReverseMap();
        CreateMap<SearchViewModel, WebSearchViewModel>().ReverseMap();
        CreateMap<Goods, AdditionWebGoodsResult>().ReverseMap();
        CreateMap<GoodsPagingResult, AdditionWebGoodsPagingResult>()
            .ForMember(m => m.Goods, option => option.Ignore())
            .ReverseMap();
        CreateMap<Company, AdditionWebCompanyResult>().ReverseMap();
        CreateMap<AdditionWebCompanyResult, AdditionWebCompanyShortResult>().ReverseMap();
    }
}

