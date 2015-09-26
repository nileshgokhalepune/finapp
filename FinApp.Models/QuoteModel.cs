using System;

namespace FinApp.Models
{
    public class QuoteModel
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public long Volumne { get; set; }
        public float Adj_Close { get; set; }
    }
}
