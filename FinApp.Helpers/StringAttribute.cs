﻿using System;
using System.Linq;
using System.Reflection;

namespace FinApp.MiddleWare
{
    public enum YqlTables
    {
        [StringValue("yahoo.finance.sectors")]
        YahooFinanceSectors,
        [StringValue("yahoo.finance.industry")]
        YahooFinanceIndustries,
        [StringValue("yahoo.finance.quotes")]
        YahooFinanceQuotes,
        [StringValue("yahoo.finance.historicaldata")]
        YahooFinanceHistoricalData
    }


    public class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value { get { return _value; } }

        public static string GetString(Enum value)
        {
            if (value == null) return null;
            var type = value.GetType();
            var fi = type.GetRuntimeField(value.ToString());
            return
                (fi.GetCustomAttributes(typeof(StringValueAttribute), false).FirstOrDefault() as StringValueAttribute).Value;
        }
    }
}
