using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace Schaeflein.Community.AdviseOneAnother
{
  public class ItemPropertyHelper
  {
    public static ItemPropertyData GetItemPropertyData(SPItemEventDataCollection properties, FieldInternalNames fldNames)
    {
      ItemPropertyData results = new ItemPropertyData();
      results.SendEmail = ParseBoolProperty(properties[fldNames.SendEmailFieldname]);
      results.MailTo = (string)properties[fldNames.MailToFieldname];
      results.CC = (string)properties[fldNames.CCFieldname];
      results.Message = (string)properties[fldNames.MessageFieldname];

      return results;
    }

    private static bool ParseBoolProperty(object property)
    {
      bool results = false;
      if (property != null)
      {
        bool work = false;
        if (bool.TryParse((string)property, out work))
        {
          results = work;
        }
      }
      return results;
    }

    private static int ParseIntProperty(object property)
    {
      int results = 0;
      if (property != null)
      {
        int work = default(int);
        if (Int32.TryParse((string)property, out work))
        {
          results = work;
        }
      }
      return results;
    }

    private static string ParseTaxProperty(object property)
    {
      string results = String.Empty;
      if (property != null)
      {
        string[] values = ((string)property).Split('|');
        if (values.Length > 0)
          results = values[0];
      }
      return results;
    }
  }

  public class ItemPropertyData
  {
    public bool SendEmail { get; set; }
    public string MailTo { get; set; }
    public string CC { get; set; }
    public string Message { get; set; }
  }
}
