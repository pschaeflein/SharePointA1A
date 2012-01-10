using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace Schaeflein.Community.AdviseOneAnother
{
  public class FieldCreationHelper
  {
    public static string CreateField(SPList list, FieldCreationInfo properties)
    {
      string result = default(string);

      if (properties.FieldType == SPFieldType.Lookup)
      {
        Guid listId = default(Guid);
        Guid webId = default(Guid);
        SPWeb web = null;
        // we need ids for the web and list...
        if (properties.ListInRootWeb)
        {
          web = list.ParentWeb.Site.RootWeb;
        }
        else
        {
          web = list.ParentWeb;
        }

        webId = web.ID;
        listId = web.Lists[properties.ListName].ID;

        result = list.Fields.AddLookup(properties.DisplayName, listId, webId, properties.Required);
        SPFieldLookup newField = (SPFieldLookup)list.Fields.GetFieldByInternalName(result);
        newField.LookupField = "Title";
        newField.Update();
      }
      else
      {
        result = list.Fields.Add(properties.DisplayName, properties.FieldType, properties.Required);
      }

      return result;
    }

    public static FieldInternalNames GetFieldInternalNamesFromConfiguration(SPList list)
    {
      IConfigManager configManager = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();
      configManager.SetWeb(list.ParentWeb);
      IPropertyBag bag = configManager.GetPropertyBag(ConfigLevel.CurrentSPWeb);

      string keyTemplate = list.ID.ToString() + ":";

      FieldInternalNames results = new FieldInternalNames();
      results.SendEmailFieldname = configManager.GetFromPropertyBag<string>(keyTemplate + Constants.ConfigurationKeys.SendEmailFieldname, bag);
      results.MailToFieldname    = configManager.GetFromPropertyBag<string>(keyTemplate + Constants.ConfigurationKeys.MailToFieldname, bag);
      results.CCFieldname        = configManager.GetFromPropertyBag<string>(keyTemplate + Constants.ConfigurationKeys.CCFieldname, bag);
      results.MessageFieldname   = configManager.GetFromPropertyBag<string>(keyTemplate + Constants.ConfigurationKeys.MessageFieldname, bag);
      return results;
    }

    public static void RemoveFieldInternalNamesFromConfiguration(SPList list)
    {
      IConfigManager configManager = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();
      configManager.SetWeb(list.ParentWeb);
      IPropertyBag bag = configManager.GetPropertyBag(ConfigLevel.CurrentSPWeb);

      string keyTemplate = list.ID.ToString() + ":";

      FieldInternalNames results = new FieldInternalNames();
      configManager.RemoveKeyFromPropertyBag(keyTemplate + Constants.ConfigurationKeys.SendEmailFieldname, bag);
      configManager.RemoveKeyFromPropertyBag(keyTemplate + Constants.ConfigurationKeys.MailToFieldname, bag);
      configManager.RemoveKeyFromPropertyBag(keyTemplate + Constants.ConfigurationKeys.CCFieldname, bag);
      configManager.RemoveKeyFromPropertyBag(keyTemplate + Constants.ConfigurationKeys.MessageFieldname, bag);

    }
  }

  public class FieldCreationInfo
  {
    public string DisplayName { get; set; }
    public SPFieldType FieldType { get; set; }
    public bool Required { get; set; }
    public string ListName { get; set; }
    public bool ListInRootWeb { get; set; }
    public string ConfigurationKey { get; set; }
  }

  /// <summary>
  /// Contains the internal field names that are retrieved from configuration
  /// </summary>
  public class FieldInternalNames
  {
    public string SendEmailFieldname { get; set; }
    public string MailToFieldname { get; set; }
    public string CCFieldname { get; set; }
    public string MessageFieldname { get; set; }
  }

}
