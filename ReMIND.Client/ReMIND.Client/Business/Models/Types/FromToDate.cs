using System;

namespace ReMIND.Client.Business.Models.Types
{
    public class FromToDate
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public FromToDate()
        {
            //piksi proveri da li ovo mozda treba ovako da radi, ili treba da se kreira interval sa null vrednostima, ne zelim da menjam intent koda
            //DateFrom = new();
            //DateTo = new();
        }
    }

}
