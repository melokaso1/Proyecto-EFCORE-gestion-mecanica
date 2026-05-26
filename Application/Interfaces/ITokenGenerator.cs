using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenGenerator
{
    TokenResponseDto Generate(Usuario usuario, string email);
}
