using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;

namespace Preguntin_ASP.NET.GraphQL.Query
{
  
    public class Query
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public Query(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }

        [Authorize(Policy = "Admin")]
        [UseFiltering, UseSorting]
        public IQueryable<CategoriaDefaultDTO> QueryAllCategorias()
        {
            return _dbContext.Categoria.Select(p => new CategoriaDefaultDTO
            {
                Id = p.IdCategoria,
                Nombre = p.Nombre,
            });
        }


        [Authorize(Policy = "Admin")]
        [UseFiltering, UseSorting]
        public IQueryable<PreguntasResponse> QueryAllPreguntas(int idCategoria) {
            return _dbContext.Preguntas.Where(p => p.IdCategoria == idCategoria).ProjectTo<PreguntasResponse>(_mapper.ConfigurationProvider);
        }

        [Authorize(Policy = "Jugador")]
        [UseSorting, UseFiltering]
        public async Task<User> QueryInfUser(ClaimsPrincipal claims, [Service] UserManager<Jugador> manager)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await manager.FindByIdAsync(id);
            return new User
            {
                Id=id,
                Name=user.UserName,
                Email=user.Email,
                Telefono=user.PhoneNumber
            };
        }

     }
}
