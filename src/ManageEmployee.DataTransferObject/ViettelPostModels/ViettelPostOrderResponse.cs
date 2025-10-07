using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public class ViettelPostOrderResponse
{
    [JsonPropertyName("ORDER_NUMBER")]
    public string OrderNumber { get; set; }

    [JsonPropertyName("MONEY_COLLECTION")]
    public long MoneyCollection { get; set; }

    [JsonPropertyName("EXCHANGE_WEIGHT")]
    public long ExchangeWeight { get; set; }

    [JsonPropertyName("MONEY_TOTAL")]
    public long MoneyTotal { get; set; }

    [JsonPropertyName("MONEY_TOTAL_FEE")]
    public long MoneyTotalFee { get; set; }

    [JsonPropertyName("MONEY_FEE")]
    public long MoneyFee { get; set; }

    [JsonPropertyName("MONEY_COLLECTION_FEE")]
    public long MoneyCollectionFee { get; set; }

    [JsonPropertyName("MONEY_OTHER_FEE")]
    public long MoneyOtherFee { get; set; }

    [JsonPropertyName("MONEY_VAS")]
    public long MoneyVas { get; set; }

    [JsonPropertyName("MONEY_VAT")]
    public long MoneyVat { get; set; }

    [JsonPropertyName("KPI_HT")]
    public double KpiHt { get; set; }

    [JsonPropertyName("RECEIVER_PROVINCE")]
    public long ReceiverProvince { get; set; }

    [JsonPropertyName("RECEIVER_DISTRICT")]
    public long ReceiverDistrict { get; set; }

    [JsonPropertyName("RECEIVER_WARDS")]
    public long ReceiverWards { get; set; }
}
