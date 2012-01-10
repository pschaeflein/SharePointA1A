using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace Schaeflein.Community.AdviseOneAnother.Features
{

  [Guid("18473a2a-2bbe-4aa1-ad71-ed782aca08c1")]
  public class BindToTasksEventReceiver : SPFeatureReceiver
  {
    ILogger logger;
    IConfigManager configManager;
    IPropertyBag bag;
    string logCategory = "Schaeflein.Community/FeatureReceivers";

    public override void FeatureActivated(SPFeatureReceiverProperties properties)
    {
      logger = SharePointServiceLocator.GetCurrent().GetInstance<ILogger>();
      configManager = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();

      try
      {
        logger.TraceToDeveloper("enter BindToTasksEventReceiver::FeatureActivated", logCategory);
        SPWeb web = properties.Feature.Parent as SPWeb;

        configManager.SetWeb(web);
        bag = configManager.GetPropertyBag(ConfigLevel.CurrentSPWeb);

        // ensure that task lists have the required fields
        foreach (SPList list in web.Lists)
        {
          if (list.BaseTemplate == SPListTemplateType.Tasks)
          {
            logger.TraceToDeveloper(String.Format("found list {0}", list.Title), logCategory);
            EnsureColumnnsPresent(list);
          }
        }
      }
      catch (Exception ex)
      {
        logger.TraceToDeveloper(ex, "An error occurred", 0, SandboxTraceSeverity.Unexpected, logCategory);
      }
      finally
      {
        logger.TraceToDeveloper("exit BindToTasksEventReceiver::FeatureActivated", logCategory);
      }
    }

    private void EnsureColumnnsPresent(SPList list)
    {
      string key = default(string);
      string fldInternalName = default(string);

      foreach (FieldCreationInfo fldInfo in Constants.RequiredFields)
      {
        key = list.ID.ToString() + ":" + fldInfo.ConfigurationKey;
        if (!configManager.ContainsKeyInPropertyBag(key, bag))
        {
          fldInternalName = FieldCreationHelper.CreateField(list, fldInfo);
          logger.TraceToDeveloper(String.Format("added field {0}:{1}", fldInfo.DisplayName, fldInternalName), logCategory);
          configManager.SetInPropertyBag(key, fldInternalName, bag);
        }
        else
        {
          fldInternalName = configManager.GetFromPropertyBag<string>(key, bag);
          logger.TraceToDeveloper(String.Format("field exists {0}:{1}", fldInfo.DisplayName, fldInternalName), logCategory);
        }
      }
    }

    public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
    {
      logger = SharePointServiceLocator.GetCurrent().GetInstance<ILogger>();

      // You should decide whether removing columns from list is appropriate...
      try
      {
        logger.TraceToDeveloper("enter BindToTasksEventReceiver::FeatureDeactivating", logCategory);
        SPWeb web = properties.Feature.Parent as SPWeb;

        // ensure that task lists have the required fields
        foreach (SPList list in web.Lists)
        {
          if (list.BaseTemplate == SPListTemplateType.Tasks)
          {
            logger.TraceToDeveloper(String.Format("found list {0}", list.Title), logCategory);
            FieldInternalNames fldNames = FieldCreationHelper.GetFieldInternalNamesFromConfiguration(list);
            logger.TraceToDeveloper(RemoveColumn(list, fldNames.SendEmailFieldname), logCategory);
            logger.TraceToDeveloper(RemoveColumn(list, fldNames.MailToFieldname), logCategory);
            logger.TraceToDeveloper(RemoveColumn(list, fldNames.CCFieldname), logCategory);
            logger.TraceToDeveloper(RemoveColumn(list, fldNames.MessageFieldname), logCategory);
            FieldCreationHelper.RemoveFieldInternalNamesFromConfiguration(list);
          }
        }
      }
      catch (Exception ex)
      {
        logger.TraceToDeveloper(ex, "An error occurred", 0, SandboxTraceSeverity.Unexpected, logCategory);
      }
      finally
      {
        logger.TraceToDeveloper("exit BindToTasksEventReceiver::FeatureDeactivating", logCategory);
      }
    }

    private string RemoveColumn(SPList list, string fieldName)
    {
      string results = string.Empty;
      try
      {
        list.Fields.Delete(fieldName);
        results = String.Format("field '{0}' deleted", fieldName);
      }
      catch (ArgumentException)
      {
        results = String.Format("field '{0}' not found", fieldName);
      }
      catch (Exception)
      {
        results = String.Format("field '{0}' cannot be deleted.");
      }
      return results;
    }


    // Uncomment the method below to handle the event raised after a feature has been installed.

    //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
    //{
    //}


    // Uncomment the method below to handle the event raised before a feature is uninstalled.

    //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
    //{
    //}

    // Uncomment the method below to handle the event raised when a feature is upgrading.

    //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
    //{
    //}
  }
}
