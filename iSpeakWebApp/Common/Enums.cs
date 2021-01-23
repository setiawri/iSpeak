
namespace iSpeakWebApp
{
    public enum EnumReminderStatuses : byte
    {
        New,
        InProgress,
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