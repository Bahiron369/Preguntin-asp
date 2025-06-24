using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preguntin_ASP.NET.Models;
using System.Text;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.Admin;
using Preguntin_ASP.NET.Services.ManagerRoles;
using Preguntin_ASP.NET.Services.ManagerUsuarios;
using Microsoft.IdentityModel.Tokens;
using Preguntin_ASP.NET.Services.Login;
using Preguntin_ASP.NET.Services.Token;
using System.Security.Claims;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Services.Juego;
using Preguntin_ASP.NET.Services.HttpCliente;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.GraphQL.Query;
using Preguntin_ASP.NET.GraphQL.Mutations;
using Preguntin_ASP.NET.Services.Informacion_Usuario;
using Preguntin_ASP.NET.Services.confirmacion_usuario;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); //añadir controladores

//configuramos jwt
builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JwtSetting")); //crea una inyeccion de dependencia para que se pueda usar en la aplicacion, la configuracion con el modelo
var JwtSetting = builder.Configuration.GetSection("JwtSetting").Get<JwtModel>();//obtenemos los datos del modelo

//configuramos la autenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //pertime que el token provenga de un emisor autorizado
        ValidateAudience = true, //permite que el token sea valido por una publico correcto
        ValidateLifetime = true, //evita token caducados
        ValidateIssuerSigningKey = true, //permite que el token sea firmado con la clave correcta
        ValidIssuer = JwtSetting.Issuer, 
        ValidAudience = JwtSetting.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting.Secret)),
        RoleClaimType = ClaimTypes.Role //verifica roles
    };
    options.IncludeErrorDetails = true;

});

//configuramos el cros para el acceso
builder.Services.AddCors(opction =>
{
    //añadir politica en pruebas
    opction.AddPolicy("prueba", opction =>
    {
        opction.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });

    opction.AddPolicy("desarrollo", opction =>
    {
        opction.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });

    opction.AddPolicy("produccion", opction =>
    {
        opction.WithOrigins("https://ambitious-field-0da23b90f.6.azurestaticapps.net")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

//Servicios de GraphQl
builder.Services
    .AddGraphQLServer()//servicios
    .AddAuthorization()//añadir autorizacion
    .ModifyRequestOptions(p=>p.IncludeExceptionDetails=true) //ver detalles si hay error
    .AddQueryType<Query>()//consultas en API
    .AddMutationType<Mutations>()//agregamos mutaciones
    .AddFiltering()//usar filtros en consultas
    .AddSorting(); //usar Ordecacion en consultas


//Servicio Http Cliente
builder.Services.AddHttpClient<IApiHttpService, ApiHttpService>(options =>
{
    options.BaseAddress = new Uri("https://opentdb.com/api.php"); //url base
    options.DefaultRequestHeaders.Add("TypeContent", "application/json"); //requisitos de la cabecera
});

//configuramos la authorizacion
builder.Services.AddAuthorization(option =>
{
    //administradores
    option.AddPolicy("Admin", option => {
        option.RequireRole("Admin");
    });

    //jugadores
    option.AddPolicy("Jugador", option => {
        option.RequireRole("Jugador");
    });

});


//configuramos base de datos
builder.Services.AddDbContext<ApplicationDbContext>(opctions =>
{
    opctions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//configuramos identity para la creacion de usuarios y roles 
builder.Services.AddIdentityCore<Jugador>()
    .AddRoles<IdentityRole>() //agregar roles
    .AddEntityFrameworkStores<ApplicationDbContext>() //agregar Ef
    .AddSignInManager() //agregar administrador inicio sesion
    .AddDefaultTokenProviders(); //provicionador de token por defecto

//agregamos servicio personalizado para la manipulacion de usuarios, roles, token, Admin, juego
builder.Services.AddScoped<Mutations>();
builder.Services.AddScoped<IRolesUsers, RolesUsers>();
builder.Services.AddScoped<IManagerUsuarios, ManagerUsuarios>();
builder.Services.AddScoped<ILoginUser,LoginUser>();
builder.Services.AddScoped<ITokenJwt, TokenJwt>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IConfirmInformation, ConfirmInformation>();
builder.Services.AddScoped<IInfUsers,InfUsers>();
builder.Services.AddScoped<IJuegoService, JuegoService>();
//añadimos mapeo de objetos con AutoMapper
builder.Services.AddAutoMapper(typeof(ApplicationProfile));
//configuramos el modelo con la sesion del usuario
builder.Services.Configure<LoginAdmin>(builder.Configuration.GetSection("CredentialEmailAdmin"));
//configuramos el email para los modelos predeterminados
builder.Services.Configure<RegistroAdmin>(builder.Configuration.GetSection("UserAdminCredential"));

var app = builder.Build();
 
//creacion por defecto 
using(var scoped = app.Services.CreateScope())//creamos un scope para usar los servicios
{
    var services = scoped.ServiceProvider;
    
    try
    {
        //requerimos los servicios
        var _roles = services.GetRequiredService<IRolesUsers>(); 
        await _roles.CrearRolesAsyncDefault();  

    }
    catch (Exception e) { Console.WriteLine(e.Message); }

    try
    {
        var categoriasDefault = services.GetRequiredService<IJuegoService>();
        await categoriasDefault.CreateCategoriaDefault();

    }catch(Exception e) { Console.WriteLine(e.Message); }
     
}

app.UseRouting();
app.UseCors("produccion");
app.UseAuthentication();//usar autentificacion
app.UseAuthorization();//usar autorizacion
app.MapControllers();//usar controladores
app.MapGraphQL(); //unico endPonit para todas las consultas y peticiones de graphQl

app.Run();
