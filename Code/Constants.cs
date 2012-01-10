using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Schaeflein.Community.AdviseOneAnother
{
  class Constants
  {
    public static List<FieldCreationInfo> RequiredFields = new List<FieldCreationInfo> {
      new FieldCreationInfo{
        DisplayName     =FieldDisplayNames.SendEmailDisplayName, 
        FieldType       =SPFieldType.Boolean ,
        Required        =false,
        ConfigurationKey=ConfigurationKeys.SendEmailFieldname
      },
      new FieldCreationInfo{
        DisplayName     =FieldDisplayNames.MailToDisplayName,
        FieldType       =SPFieldType.Text,
        Required        =false,
        ConfigurationKey=ConfigurationKeys.MailToFieldname
      },
      new FieldCreationInfo{
        DisplayName     =FieldDisplayNames.CCDisplayName,
        FieldType       =SPFieldType.Text,
        Required        =false,
        ConfigurationKey=ConfigurationKeys.CCFieldname
      },
      new FieldCreationInfo{
        DisplayName     ="Message",
        FieldType       =SPFieldType.Note,
        Required        =false,
        ConfigurationKey=ConfigurationKeys.MessageFieldname
      }
    };

    public static class ConfigurationKeys
    {
      public const string SendEmailFieldname = "SendEmailFieldname";
      public const string MailToFieldname = "MailToFieldname";
      public const string CCFieldname = "CCFieldname";
      public const string MessageFieldname = "MessageFieldname";
    }

    public static class FieldDisplayNames
    {
      public const string SendEmailDisplayName = "Send email";
      public const string MailToDisplayName = "Mail to";
      public const string CCDisplayName = "CC";
      public const string MessageDisplayName = "Message";
    }

  }
}
