namespace DUT.Constants
{
    public class Permissions
    {
        public static readonly string CanCreate = "Create";
        public static readonly string CanEdit = "Edit";
        public static readonly string CanRemove = "Remove";
        public static readonly string CanView = "View";
        public static readonly string CanViewAll = "View all";
        public static readonly string Search = "Search";
        public static readonly string All = "All";

        public static IEnumerable<string> GetBasic()
        {
            yield return CanCreate;
            yield return CanEdit;
            yield return CanRemove;
            yield return CanView;
        }

        public static IEnumerable<string> GetAll()
        {
            yield return CanCreate;
            yield return CanEdit;
            yield return CanRemove;
            yield return CanView;
            yield return CanViewAll;
            yield return Search;
        }
    }
}