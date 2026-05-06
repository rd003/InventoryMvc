namespace InventoryMvc.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Account = "Account";

    public static IEnumerable<string> GetAll() => [Admin, Manager, Account];
}
