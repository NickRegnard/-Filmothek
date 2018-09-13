using Filmothek.Models;

namespace Filmothek.Controllers
{
    public class PaymentMask
    {
        public string PaypalPassword { get; set; }
        public string PaypalLogin { get; set; }
        public string CreditcardTyp { get; set; }
        public string CreditcardOwner { get; set; }
        public long? CreditcardNumber { get; set; }
        public int? CreditcardSecret { get; set; }
        public string CreditcardExpire { get; set; }
        public string BankaccOwner { get; set; }
        public string BankaccIBAN { get; set; }

        public void fromPaymentMethod(PaymentMethod value)
        {
            this.BankaccIBAN = value.BankaccIBAN;
            this.BankaccOwner = value.BankaccOwner;
            this.CreditcardExpire = value.CreditcardExpire;
            this.CreditcardOwner = value.CreditcardOwner;
            this.CreditcardSecret = value.CreditcardSecret;
            this.CreditcardTyp = value.CreditcardTyp;
            this.CreditcardNumber = value.CreditcardNumber;
            this.PaypalLogin = value.PaypalLogin;
            this.PaypalPassword = value.PaypalPassword;

        }
    }
}