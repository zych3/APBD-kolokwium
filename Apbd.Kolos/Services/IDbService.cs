using Apbd.Kolos.DTOs;

namespace Apbd.Kolos.Services;

public interface IDbService
{
    Task<GetCustomerDto> GetByIdAsync(int id);
    Task CreateAsync(PostCustomerDto dto);
}