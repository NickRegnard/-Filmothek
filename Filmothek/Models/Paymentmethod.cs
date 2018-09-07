using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class Paymentmethod
    {        
        public int ClientId { get; set; }
        public string PaypalPassword { get; set; }
        public string PaypalLogin { get; set; }
        public string CreditcardTyp { get; set; }
        public string CreditcardOwner { get; set; }
        public long CreditcardNumber { get; set; }
        public int CreditcardSecret { get; set; }
        public string CreditcardExpire { get; set; }
        public string BankaccOwner { get; set; }
        public string BankaccIBAN { get; set; }
    }
}
