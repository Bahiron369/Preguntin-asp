using AutoMapper;
using Preguntin_ASP.NET.Models.Preguntas;
using Preguntin_ASP.NET.Models.Usuarios;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile() {

            CreateMap<CategoriaDTO, ModelCategoria>();
            CreateMap<ModelCategoria, CategoriaDTO>();
            CreateMap<Jugador, Login>();
            CreateMap<Jugador, Registro>();
            CreateMap<Registro, Jugador>();
            CreateMap<Jugador, User>();
            CreateMap<Jugador, PuntosJugador>();
            CreateMap<CategoriaDefaultDTO, ModelCategoria>();
            CreateMap<ModelPregunta, PreguntasResponse>();
            CreateMap<PreguntasResponse, ModelPregunta>();
        }
    }
}
