using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public class ViettelPostOrderRequest
{
    [JsonPropertyName("ORDER_NUMBER")]
    public string OrderNumber { get; set; }

    [JsonPropertyName("SENDER_FULLNAME")]
    public string SenderFullname { get; set; }

    [JsonPropertyName("SENDER_ADDRESS")]
    public string SenderAddress { get; set; }

    [JsonPropertyName("SENDER_PHONE")]
    public string SenderPhone { get; set; }

    [JsonPropertyName("RECEIVER_FULLNAME")]
    public string ReceiverFullname { get; set; }

    [JsonPropertyName("RECEIVER_ADDRESS")]
    public string ReceiverAddress { get; set; }

    [JsonPropertyName("RECEIVER_PHONE")]
    public string ReceiverPhone { get; set; }

    [JsonPropertyName("PRODUCT_NAME")]
    public string ProductName { get; set; }

    [JsonPropertyName("PRODUCT_DESCRIPTION")]
    public string ProductDescription { get; set; }

    [JsonPropertyName("PRODUCT_QUANTITY")]
    public int ProductQuantity { get; set; }

    [JsonPropertyName("PRODUCT_PRICE")]
    public long ProductPrice { get; set; }

    [JsonPropertyName("PRODUCT_WEIGHT")]
    public long ProductWeight { get; set; }

    [JsonPropertyName("PRODUCT_LENGTH")]
    public long ProductLength { get; set; }

    [JsonPropertyName("PRODUCT_WIDTH")]
    public long ProductWidth { get; set; }

    [JsonPropertyName("PRODUCT_HEIGHT")]
    public long ProductHeight { get; set; }

    [JsonPropertyName("ORDER_PAYMENT")]
    public ViettelPostOrderPayment OrderPayment { get; set; }

    [JsonPropertyName("ORDER_SERVICE")]
    public string OrderService { get; set; }

    [JsonPropertyName("ORDER_SERVICE_ADD")]
    public string OrderServiceAdd { get; set; }

    [JsonPropertyName("ORDER_NOTE")]
    public string OrderNote { get; set; }

    [JsonPropertyName("MONEY_COLLECTION")]
    public long MoneyCollection { get; set; }

    [JsonPropertyName("EXTRA_MONEY")]
    public long ExtraMoney { get; set; }

    [JsonPropertyName("CHECK_UNIQUE")]
    public bool CheckUnique { get; set; }

    [JsonPropertyName("PRODUCT_DETAIL")]
    public List<ViettelPostOrderItem> ProductDetail { get; set; }

    [JsonPropertyName("RETURN_ADDRESS")]
    public ViettelPostReturnAddress ReturnAddress { get; set; }
}

public class ViettelPostOrderItem
{
    [JsonPropertyName("PRODUCT_NAME")]
    public string ProductName { get; set; }

    [JsonPropertyName("PRODUCT_QUANTITY")]
    public int ProductQuantity { get; set; }

    [JsonPropertyName("PRODUCT_PRICE")]
    public long ProductPrice { get; set; }

    [JsonPropertyName("PRODUCT_WEIGHT")]
    public long ProductWeight { get; set; }
}
public class ViettelPostReturnAddress
{
    [JsonPropertyName("REQUIRED")]
    public bool Required { get; set; }

    [JsonPropertyName("FULLADDRESS")]
    public string FullAddress { get; set; }

    [JsonPropertyName("PROVINCE_ID")]
    public long ProvinceId { get; set; }

    [JsonPropertyName("DISTRICT_ID")]
    public long DistrictId { get; set; }

    [JsonPropertyName("WARDS_ID")]
    public long WardsId { get; set; }
}

