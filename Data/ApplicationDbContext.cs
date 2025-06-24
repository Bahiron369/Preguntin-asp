using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Preguntin_ASP.NET.Models.Preguntas;
using Preguntin_ASP.NET.Models.Usuarios;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Preguntin_ASP.NET.Models;
using Preguntin_ASP.NET.Models.Juego;

namespace Preguntin_ASP.NET.Data
{
    public class ApplicationDbContext : IdentityDbContext<Jugador, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //agregamos los modelos para el mapeo
        public DbSet<ModelCategoria> Categoria { set; get; }
        public DbSet<ModelPregunta> Preguntas { set; get; }
        public DbSet<CategoriasUsuarios> Puntos { set; get; }
        public DbSet<ModelRespuesta> Respuestas { set; get; }
        public DbSet<Jugador> jugadorUsers { set; get; }
        public DbSet<Auditoria> auditorias { set; get; }
        public DbSet<CategoriasUsuarios> categoriasUsuarios { set; get; }
        public DbSet<ConfirmarInDatos> confirmars { get; set; }

        //creamos tablas y relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//pasamos la configuracion

            //Relaciones entre las tablas****************************
            //creamos una relacion de N:M categoria:Muchas Preguntas
            modelBuilder.Entity<ModelPregunta>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.preguntas)
                .HasForeignKey(p => p.IdCategoria);

            //creamos una relacion de N:M una respuesta muchas preguntas
            modelBuilder.Entity<ModelRespuesta>()
                .HasOne(p => p.Pregunta)
                .WithMany(p => p.respuesta)
                .HasForeignKey(p => p.IdPregunta);

        }
    }
}

