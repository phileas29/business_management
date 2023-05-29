using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LittleFirmManagement.Models
{
    public class FInvoicesViewModel
    {
        public List<int> selectedInterventions { get; set; }

        public FInvoice fInvoice { get; set; }
        public bool InIsEligibleDeferredTaxCredit { get; set; }

        public FInvoicesViewModel()
        {
            fInvoice = new FInvoice();
            selectedInterventions = new List<int>();
        }
    }
}
