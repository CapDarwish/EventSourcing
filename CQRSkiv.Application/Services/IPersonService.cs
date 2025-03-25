using CQRSkiv.Application.Commands;
using System.Threading.Tasks;

namespace CQRSkiv.Core.Interfaces;

public interface IPersonService
{
  Task CreatePersonAsync(CreatePersonCommand command);
  Task UpdatePersonAsync(UpdatePersonCommand command);
  Task DeletePersonAsync(DeletePersonCommand command);
  Task AddEmploymentAsync(AddEmploymentCommand command);
  Task DeleteEmploymentAsync(DeleteEmploymentCommand command);
  Task UpdateEmploymentAsync(UpdateEmploymentCommand command);
}