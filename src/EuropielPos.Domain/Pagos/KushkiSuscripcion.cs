namespace EuropielPos.Domain.Pagos;

/// <summary>
/// Payloads del API de Kushki (smart links y suscripciones).
/// Port de <c>KushkiSuscripcion.vb</c> y <c>KushkiSmartLinkResponse.vb</c>.
/// Los nombres camelCase se conservan porque son el contrato JSON de Kushki.
/// </summary>
#pragma warning disable IDE1006 // nombres camelCase requeridos por el API de Kushki
public class KushkiSmartLinkResponse
{
    public string? smartLinkUrl { get; set; }
}

public class KushkiSuscripcion
{
    public string? publicMerchantId { get; set; }

    public string? merchantName { get; set; }

    public PaymentConfigClass? paymentConfig { get; set; }

    public GeneralConfigClass? generalConfig { get; set; }

    public StyleAndStructureClass? styleAndStructure { get; set; }

    public ContactClass? contact { get; set; }

    public List<FormConfigClass>? formConfig { get; set; }

    public class AmountClass
    {
        public string? currency { get; set; }

        public int subtotalIva { get; set; }

        public int subtotalIva0 { get; set; }

        public int iva { get; set; }
    }

    public class ContactClass
    {
        public string? email { get; set; }

        public string? phoneNumber { get; set; }
    }

    public class FormConfigClass
    {
        public string? label { get; set; }

        public string? type { get; set; }

        public bool split { get; set; }

        public bool required { get; set; }

        public bool disabled { get; set; }

        public string? name { get; set; }

        public string? placeholder { get; set; }

        public string? value { get; set; }
    }

    public class GeneralConfigClass
    {
        public string? productName { get; set; }

        public string? description { get; set; }

        public string? productImage { get; set; }

        public string? brandLogo { get; set; }

        public int executionLimit { get; set; }

        public bool showTimer { get; set; }

        public bool enabled { get; set; }

        public string? termsAndConditions { get; set; }

        public string? promotionalText { get; set; }

        public string? buyButtonText { get; set; }
    }

    public class MixedOptions
    {
        public int numberOfFees { get; set; }

        public int subscriptionDay { get; set; }

        public string? periodicity { get; set; }
    }

    public class PaymentConfigClass
    {
        public string? paymentType { get; set; }

        public AmountClass? amount { get; set; }

        public List<string>? paymentMethod { get; set; }

        public SubscriptionOptions? subscriptionOptions { get; set; }

        public MixedOptions? mixedOptions { get; set; }
    }

    public class StyleAndStructureClass
    {
        public string? structure { get; set; }
    }

    public class SubscriptionOptions
    {
        public AmountClass? amount { get; set; }

        public string? periodicity { get; set; }

        public string? planName { get; set; }

        public string? startDate { get; set; }

        public string? terms { get; set; }
    }
}
#pragma warning restore IDE1006
