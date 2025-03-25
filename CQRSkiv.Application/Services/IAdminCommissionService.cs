using CQRSkiv.Application.Commands;
using System.Threading.Tasks;

namespace CQRSkiv.Application.Services;

public interface IAdminCommissionService
{
  Task CreateAdminCommissionAsync(CreateAdminCommissionCommand command);
  Task UpdateAdminCommissionAsync(UpdateAdminCommissionCommand command);
  Task DeleteAdminCommissionAsync(DeleteAdminCommissionCommand command);
}