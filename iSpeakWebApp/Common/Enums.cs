using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections.Generic;
using LIBUtil;

namespace iSpeakWebApp
{
    public enum EnumReminderStatuses : byte
    {
        New,
        [Description("In Progress")]
        InProgress,
        [Description("On Hold")]
        OnHold,
        Waiting,
        Completed,
        Cancel
    }

    public enum EnumActions
    {
        Create,
        Edit,
        Update,
        Delete,
        Print,
        Approve,
        Cancel,
        Previous,
        Next
    }

    public enum EnumActionTypes
    {
        All = 0
    }

    public enum UserAccountAccess
    {

    }

}