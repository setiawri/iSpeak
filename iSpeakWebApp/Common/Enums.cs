using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iSpeakWebApp
{
    public enum EnumReminderStatuses : byte
    {
        New,
        [Description("In Progress")]
        [Display(Name = "In Progress")]
        InProgress,
        [Description("On Hold")]
        [Display(Name = "On Hold")]
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

}