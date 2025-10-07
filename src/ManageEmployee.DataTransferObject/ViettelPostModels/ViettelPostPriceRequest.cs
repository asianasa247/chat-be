using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public class ViettelPostPriceRequest
{
    [JsonPropertyName("SENDER_PROVINCE")]
    public long SenderProvince { get; set; }

    [JsonPropertyName("SENDER_DISTRICT")]
    public long SenderDistrict { get; set; }

    [JsonPropertyName("SENDER_WARD")]
    public long SenderWard { get; set; }

    [JsonPropertyName("RECEIVER_PROVINCE")]
    public long ReceiverProvince { get; set; }

    [JsonPropertyName("RECEIVER_DISTRICT")]
    public long ReceiverDistrict { get; set; }

    [JsonPropertyName("RECEIVER_WARD")]
    public long ReceiverWard { get; set; }

    [JsonPropertyName("PRODUCT_TYPE")]
    public ViettelPostProductType ProductType { get; set; }

    [JsonPropertyName("PRODUCT_WEIGHT")]
    public long ProductWeight { get; set; }

    [JsonPropertyName("PRODUCT_PRICE")]
    public long ProductPrice { get; set; }

    [JsonPropertyName("MONEY_COLLECTION")]
    public long MoneyCollection { get; set; }

    [JsonPropertyName("NATIONAL_TYPE")]
    public ViettelPostNationalType NationalType { get; set; }

    [JsonPropertyName("PRODUCT_LENGTH")]
    public long ProductLength { get; set; }

    [JsonPropertyName("PRODUCT_WIDTH")]
    public long ProductWidth { get; set; }

    [JsonPropertyName("PRODUCT_HEIGHT")]
    public long ProductHeight { get; set; }

    [JsonPropertyName("ORDER_SERVICE")]
    public string OrderService { get; set; }

    [JsonPropertyName("ORDER_SERVICE_ADD")]
    public string OrderServiceAdd { get; set; }
}
