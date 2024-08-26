using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Application/Services/IMotoService.cs
using DesafioRentDelivery.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public interface IMotoService
    {
        Task AddMotoAsync(MotoDTO moto);
        Task<MotoDTO> GetMotoByIdAsync(int id);
        Task<IEnumerable<MotoDTO>> GetAllMotosAsync();
        Task UpdateMotoAsync(MotoDTO moto);
        Task RemoveMotoAsync(int id);
    }
}