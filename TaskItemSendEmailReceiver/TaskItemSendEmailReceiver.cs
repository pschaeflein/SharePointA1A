using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using System.Text;
using System.Collections.Specialized;

namespace Schaeflein.Community.AdviseOneAnother.ItemReceivers
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class TaskItemSendEmailReceiver : SPItemEventReceiver
    {
      ILogger logger;
      string logCategory = "Schaeflein.Community/EventReceivers";
      
      /// <summary>
       /// An item is being updated.
       /// </summary>
       public override void ItemUpdating(SPItemEventProperties properties)
       {
         logger = SharePointServiceLocator.GetCurrent().GetInstance<ILogger>();

         try
         {
           logger.TraceToDeveloper("enter TaskItemSendEmailReceiver::ItemUpdating", logCategory);

           FieldInternalNames fldNames = FieldCreationHelper.GetFieldInternalNamesFromConfiguration(properties.List);
           ItemPropertyData data = ItemPropertyHelper.GetItemPropertyData(properties.AfterProperties, fldNames);

           // should we be sending a message?
           logger.TraceToDeveloper(String.Format("sendMail = {0}", data.SendEmail), logCategory);
           if (data.SendEmail)
           {
            // string mailToAddress = data.MailTo
             logger.TraceToDeveloper(String.Format("mailTo = '{0}'", data.MailTo), logCategory);

             // only continue if we have a recipient email address
             if (!String.IsNullOrEmpty(data.MailTo))
             {
               //string ccAddress = GetUserEmailFromLookupProperty(properties.Web, data.CC);
               logger.TraceToDeveloper(String.Format("cc = '{0}'", data.CC), logCategory);

               // only continue if we have a recipient email address
               logger.TraceToDeveloper(String.Format("message.IsNullOrEmpty = '{0}'", String.IsNullOrEmpty(data.Message)), logCategory);
               if (!String.IsNullOrEmpty(data.Message))
               {
                 logger.TraceToDeveloper(String.Format("listItem: {0,-80}", properties.ListItem["Title"]), logCategory);

                 // append the current task list item to the message
                 StringBuilder sb = new StringBuilder();
                 sb.Append(data.Message);
                 sb.Append("<br/>");
                 sb.Append("<br/>");

                 string listItemUrl = String.Format("{0}/{1}?ID={2}",
                                          properties.WebUrl,
                                          properties.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url,
                                          properties.ListItem.ID);

                 sb.Append("This message is associated with the task: ");
                 sb.AppendFormat("<a href='{1}'>{0}</a>", properties.ListItem["Title"], listItemUrl);

                 StringDictionary headers = new StringDictionary();
                 headers.Add("to", data.MailTo);
                 if (!String.IsNullOrEmpty(data.CC))
                 {
                   headers.Add("cc", data.CC);
                 }
                 headers.Add("subject", "Task message from " + properties.Web.Title);
                 headers.Add("content-type", "text/html");

                 logger.TraceToDeveloper("sending email", logCategory);
                 SPUtility.SendEmail(properties.Web, headers, sb.ToString());
                 logger.TraceToDeveloper("email sent", logCategory);

               }
             }
           }

           // dont actually update the mail fields...
           properties.AfterProperties[fldNames.SendEmailFieldname] = false;
           properties.AfterProperties[fldNames.MailToFieldname] = null;
           properties.AfterProperties[fldNames.CCFieldname] = null;
           properties.AfterProperties[fldNames.MessageFieldname] = null;

           base.ItemUpdating(properties);
         }
         catch (Exception ex)
         {
           logger.TraceToDeveloper(ex, "An error occurred", 0, SandboxTraceSeverity.Unexpected, logCategory);
         }
         finally
         {
           logger.TraceToDeveloper("exit TaskItemSendEmailReceiver::ItemUpdating", logCategory);
         }
       }


    }
}
