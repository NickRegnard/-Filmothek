using Filmothek.Models;

namespace Filmothek.Controllers
{
    public class Paymentmask
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
            /*Paymentmask returnValue = new Paymentmask();
            returnValue.BankaccIBAN = value.BankaccIBAN;
            returnValue.BankaccOwner = value.BankaccOwner;
            returnValue.CreditcardExpire = value.CreditcardExpire;
            returnValue.CreditcardOwner = value.CreditcardOwner;
            returnValue.CreditcardSecret = value.CreditcardSecret;
            returnValue.CreditcardTyp = value.CreditcardTyp;
            returnValue.CreditcardNumber = value.CreditcardNumber;
            returnValue.PaypalLogin = value.PaypalLogin;
            returnValue.PaypalPassword = value.PaypalPassword;*/

     
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