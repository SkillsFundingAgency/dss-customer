﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text.Json;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace NCS.DSS.Customer.Helpers
{
    public class DynamicHelper : IDynamicHelper
    {
        public ExpandoObject ExcludeProperty(Exception exception, string[] names)
        {
            dynamic updatedObject = new ExpandoObject();
            foreach (var item in typeof(Exception).GetProperties())
            {
                if (names.Contains(item.Name))
                    continue;

                AddProperty(updatedObject, item.Name, item.GetValue(exception));
            }
            return updatedObject;
        }

        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
