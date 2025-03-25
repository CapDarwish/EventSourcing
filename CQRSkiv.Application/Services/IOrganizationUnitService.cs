using CQRSkiv.Application.Commands;
using System.Threading.Tasks;

namespace CQRSkiv.Application.Services;

public interface IOrganizationUnitService
{
  Task CreateOrganizationUnitAsync(CreateOrganizationUnitCommand command);
  Task UpdateOrganizationUnitAsync(UpdateOrganizationUnitCommand command);
  Task DeleteOrganizationUnitAsync(DeleteOrganizationUnitCommand command);
}