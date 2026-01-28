using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class InvoiceItemLine
{
    [JsonPropertyName("partnerId")]
    public required string PartnerId { get; set; }
    
    [JsonPropertyName("customerId")]
    public required string CustomerId { get; set; }
    
    [JsonPropertyName("customerName")]
    public required string CustomerName { get; set; }
    
    [JsonPropertyName("customerDomainName")]
    public required string CustomerDomainName { get; set; }
    
    [JsonPropertyName("customerCountry")]
    public required string CustomerCountry { get; set; }
    
    [JsonPropertyName("invoiceNumber")]
    public required string InvoiceNumber { get; set; }
    
    [JsonPropertyName("mpnId")]
    public required string MpnId { get; set; }
    
    [JsonPropertyName("orderId")]
    public required string OrderId { get; set; }
    
    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }
    
    [JsonPropertyName("productId")]
    public required string ProductId { get; set; }
    
    [JsonPropertyName("skuId")]
    public required string SkuId { get; set; }
    
    [JsonPropertyName("availabilityId")]
    public required string AvailabilityId { get; set; }
    
    [JsonPropertyName("skuName")]
    public required string SkuName { get; set; }
    
    [JsonPropertyName("productName")]
    public required string ProductName { get; set; }
    
    [JsonPropertyName("chargeType")]
    public required string ChargeType { get; set; }
    
    [JsonPropertyName("unitPrice")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal UnitPrice { get; set; }
    
    [JsonPropertyName("quantity")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal Quantity { get; set; }
    
    [JsonPropertyName("subtotal")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal Subtotal { get; set; }
    
    [JsonPropertyName("taxTotal")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal TaxTotal { get; set; }
    
    [JsonPropertyName("total")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal Total { get; set; }
    
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }
    
    [JsonPropertyName("publisherName")]
    public required string PublisherName { get; set; }
    
    [JsonPropertyName("publisherId")]
    public required string PublisherId { get; set; }
    
    [JsonPropertyName("subscriptionDescription")]
    public required string SubscriptionDescription { get; set; }
    
    [JsonPropertyName("subscriptionId")]
    public required string SubscriptionId { get; set; }
    
    [JsonPropertyName("chargeStartDate")]
    public DateTime ChargeStartDate { get; set; }
    
    [JsonPropertyName("chargeEndDate")]
    public DateTime ChargeEndDate { get; set; }
    
    [JsonPropertyName("termAndBillingCycle")]
    public required string TermAndBillingCycle { get; set; }
    
    [JsonPropertyName("effectiveUnitPrice")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal EffectiveUnitPrice { get; set; }
    
    [JsonPropertyName("unitType")]
    public required string UnitType { get; set; }
    
    [JsonPropertyName("alternateId")]
    public required string AlternateId { get; set; }
    
    [JsonPropertyName("billableQuantity")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal BillableQuantity { get; set; }
    
    [JsonPropertyName("billingFrequency")]
    public required string BillingFrequency { get; set; }
    
    [JsonPropertyName("pricingCurrency")]
    public required string PricingCurrency { get; set; }
    
    [JsonPropertyName("pcToBCExchangeRate")]
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal PcToBCExchangeRate { get; set; }
    
    [JsonPropertyName("pcToBCExchangeRateDate")]
    public DateTime PcToBCExchangeRateDate { get; set; }
    
    [JsonPropertyName("meterDescription")]
    public required string MeterDescription { get; set; }
    
    [JsonPropertyName("subscriptionStartDate")]
    public DateTime SubscriptionStartDate { get; set; }
    
    [JsonPropertyName("subscriptionEndDate")]
    public DateTime SubscriptionEndDate { get; set; }
    
    [JsonPropertyName("referenceId")]
    public required string ReferenceId { get; set; }
    
    [JsonPropertyName("productQualifiers")]
    public required string ProductQualifiers { get; set; }
    
    [JsonPropertyName("productCategory")]
    public required string ProductCategory { get; set; }
}