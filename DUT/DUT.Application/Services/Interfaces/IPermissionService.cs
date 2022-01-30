namespace DUT.Application.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(PermissionAction action, object data = null);
    }

    public enum PermissionAction
    {
        CreateUniversity,
        EditUniversity,
        CreateFaculty,
        EditFaculty,
        RemoveFaculty,
        CreateSpecialty,
        EditSpecialty,
        RemoveSpecialty,
        CreateGroup,
        EditGroup,
        RemoveGroup
    }
}
