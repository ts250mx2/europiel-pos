using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class KushkiSmartLink
{
    public int KushkiSmartLinkId { get; set; }

    public string? PublicMerchantId { get; set; }

    public string? MerchantName { get; set; }

    public string? PaymentType { get; set; }

    public string? Currency { get; set; }

    public decimal? SubtotalIva { get; set; }

    public decimal? SubtotalIva0 { get; set; }

    public decimal? Iva { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Periodicity { get; set; }

    public DateTime? StartDate { get; set; }

    public string? Terms { get; set; }

    public int? NumberOfFees { get; set; }

    public int? SubscriptionDay { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public string? ProductImage { get; set; }

    public string? BrandLogo { get; set; }

    public int? ExecutionLimit { get; set; }

    public string? TermAndConditions { get; set; }

    public string? PromotionalText { get; set; }

    public string? BuyButtonText { get; set; }

    public string? Structure { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }

    public string? Link { get; set; }

    public string? SmartLinkId { get; set; }

    public int? IdLocalPaquete { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdLocalPaciente { get; set; }

    public int? IdPaciente { get; set; }

    public string? PlanName { get; set; }

    public DateTime? CreateDate { get; set; }
}
