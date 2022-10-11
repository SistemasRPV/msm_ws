using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using msm_ws.Data.DbContexts;
using msm_ws.Data.Dto;
using msm_ws.Helpers;

namespace msm_ws.Repositories
{
    public  class MainRepository
    {
        
        #region [Variables & Constructor]
        private readonly RpvDbContext _context;
        private readonly AppSettings _appSettings;
        //private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly int repositorio = 25;
        
        public MainRepository
            (IOptions<AppSettings> appSettings, RpvDbContext context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }
        #endregion
        
        #region [Constantes SQL]
        public string Authenticate_sql = @"select IdUsuario, 
                                                IdPersona, 
                                                IdPerfil, 
                                                Nombre, 
                                                Detalle, 
                                                Funciones, 
                                                Topics, 
                                                IdPersonaResponsable, 
                                                Titular, 
                                                Login 
                                            from [RPVGESTION].[dbo].[vUsuarios] 
                                            where @login = Login and @pass = Password and APPS like '%%' 
                                            FOR JSON PATH, INCLUDE_NULL_VALUES";
        public string Authenticate_param01 = "@login";
        public string Authenticate_param02 = "@pass";
        
        #endregion
        
        #region [Auth]
        public async Task<AuthDto> Authenticate(LoginDto auth)
        {            
            // Comprobamos si viene informado el login y el password
            if (string.IsNullOrEmpty(auth.Login) || string.IsNullOrEmpty(auth.Password))
                throw new NotContentException("Usuario o password no válidos");

            // Obtenemos los usuarios del login solicitado
            var usersJson = "";
            
            var conn = new SqlConnection(_context.ConnRpvGestion);
            var  command =     conn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = Authenticate_sql;
            command.CommandTimeout = 0;
            command.Parameters.Add(Authenticate_param01, System.Data.SqlDbType.NVarChar).Value = auth.Login;
            command.Parameters.Add(Authenticate_param02, System.Data.SqlDbType.NVarChar).Value = auth.Password;

            await conn.OpenAsync();
            var dataReader =  await command.ExecuteReaderAsync();
            while (dataReader.Read())
            {
                usersJson += dataReader.GetString(0);
            }
            await conn.CloseAsync();

            if(usersJson == "")
                throw new NotContentException("El usuario no es válido");

            var users = (List<UserDto>) null;
            try
            {
                users  = JsonSerializer.Deserialize<List<UserDto>>(usersJson);
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Error de deserialización: \n" + ex.Message);
            }


            // Comprobamos si existe el usuario
            if (users == null) throw new NotContentException("El usuario no es válido");

            // Comprobamos si coincide el password encriptándolo primero
            //if (EncryptPassword(auth.Password) != user.Password) throw new BadRequestException("El password no es válido");
            
            // Convertimos la key de appsettings en un array de bytes
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            // Crea el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // 	Establece las notificaciones de salida que se van a incluir en el token emitido.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, users[0].IdPersona),
                    new Claim(ClaimTypes.Email, users[0].Detalle)
                }),

                // Establece la fecha que expira el token
                Expires = DateTime.UtcNow.AddDays(2),

                // Establece las credenciales que se utilizan para firmar el token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)               
            };

            // Creamos un manejador para el token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Creamos el token en base al descriptor del token especificado
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serializa el token
            var tokenString = tokenHandler.WriteToken(token);

            // Creamos el AuthDto de salida
            AuthDto response = new AuthDto
            {
                Users = usersJson,
                Token = tokenString
            };          

            return response;
        }
        
        #endregion

        #region [Getters]
        public async Task<string> GetFotoByOrdenAndCentro(int idOrden, int idCentro)
        {
            var res = "";
            int rowcount = 0;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getFotosByIdOrden&IdCentro", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idCentro", SqlDbType.Int).Value = idCentro;
                cmd.Parameters.Add("@idOrden", SqlDbType.Int).Value = idOrden;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<String> GetCentros(int idUser)
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getCentrosEnRutaByUser", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idUser", SqlDbType.Int).Value = idUser;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<String> GetTodosCentros(int idUser)
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getTodosCentrosByUser", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idUser", SqlDbType.Int).Value = idUser;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<int> GetVisita(int idUsuario, string idPersona, int idCentro)
        {
            int visita = 0;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getLastVisitOrCreateNew", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                cmd.Parameters.Add("@idPersona", SqlDbType.NVarChar).Value = idPersona;
                cmd.Parameters.Add("@idCentro", SqlDbType.Int).Value = idCentro;
                cmd.Parameters.Add("@res", SqlDbType.Int);
                cmd.Parameters["@res"].Direction = ParameterDirection.Output;

                await cmd.ExecuteNonQueryAsync();

                visita = (int) cmd.Parameters["@res"].Value;
                
                if(visita <= 0)
                    throw new Exception();

            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return visita;

        }
        
        public async Task<String> GetContactosCentroByIdCentro(int idCentro)
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getContactosCentroByIdCentro", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idCentro", SqlDbType.Int).Value = idCentro;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        public async Task<String> GetOrdenesByCentro(int idCentro)
        {
            var res = "";
            int rowcount = 0;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getOrdenesByCentro", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idCentro", SqlDbType.Int).Value = idCentro;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<String> GetTipoByIdTipo(int idTipo)
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getTipoById", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idTipo", SqlDbType.Int).Value = idTipo;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;
                
                if(rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (Exception e)
            { 
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<String> GetTipos()
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getTipos", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;
                
                if(rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<String> GetMateriales()
        {
            var res = "";
            int rowcount;
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_getMateriales", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@rowcount", SqlDbType.Int);
                cmd.Parameters["@rowcount"].Direction = ParameterDirection.Output;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                rowcount = (int) cmd.Parameters["@rowcount"].Value;

                if (rowcount <= 0)
                    throw new NotContentException("Sin datos");

            }
            catch (NotContentException ex)
            {
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        #endregion
        
        #region [Setters]
        // public async Task<string> SetFotos(FotoDto[] fotos)
        // {
        //     var res = "";
        //
        //     
        //     conn.Open();
        //     try
        //     {
        //         SqlCommand cmd = new SqlCommand("sp_setFotos", conn);
        //         cmd.CommandTimeout = 120;
        //         cmd.CommandType = CommandType.StoredProcedure;
        //         cmd.Parameters.Add("@fotosJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(fotos);
        //         
        //         SqlDataReader reader = await cmd.ExecuteReaderAsync();
        //
        //         while (reader.Read())
        //         {
        //             res += reader.GetString(0);
        //         }
        //
        //         await reader.CloseAsync();
        //
        //         if (res == "")
        //             throw new NotFoundException("SQL problems");
        //
        //     }
        //     catch (Exception e)
        //     {
        //         throw new NotFoundException("Error: " + e.Message);
        //     }
        //     finally
        //     {
        //         conn.Close();
        //     }
        //     
        //     return res;
        // }
        
        public async Task<string> DeleteFoto(int idFoto)
        {
            var res = "-1";
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_deleteFoto", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idFoto", SqlDbType.Int).Value = idFoto;
                
                await cmd.ExecuteNonQueryAsync();
                res = "1";
            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<string> SetContacto(ContactoDto contacto)
        {
            var res = "";
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_setContacto", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@contactoJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(contacto);
                
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                if (res == "")
                    throw new NotFoundException("SQL problems");

            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<string> SetOrden(OrdenFotoDto orden)
        {
            var res = "";
            List<FotoDto> listaFoto;
            try
            {
                listaFoto = JsonSerializer.Deserialize<List<FotoDto>>(orden.Fotos);
                
                foreach (FotoDto t in listaFoto)
                {
                
                    if (t.Id_Foto != 0 ||t.Foto64.Length < 1)    
                    {
                        continue;
                    }
                    
                    string pixName = F.Normalize(t.Nombre
                                                 + "_" + F.Normalize(t.Direccion)
                                                 + "_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
                                                 // + "_" + DateTime.Now.ToString("dd-MM-yyyy")
                                                 + ".jpg");

                    //Subir foto...
                    string anio = DateTime.Now.ToLocalTime().ToString("yyyy");
                    string mes = DateTime.Now.ToLocalTime().ToString("MM");
                    Directory.CreateDirectory(@"F:\inetpub\fotos.rpv.es\repositorio\" + repositorio + "\\" + anio + "\\" + mes + "\\");
                    Directory.CreateDirectory(@"F:\inetpub\fotos.rpv.es\repositorio\" + repositorio + "\\thumbs\\" + anio + "\\" + mes + "\\");
                    pixName = anio + "/" + mes + "/" + pixName;
                    MemoryStream msImage = new MemoryStream(Convert.FromBase64String(t.Foto64));
                    Bitmap bmpImage = new Bitmap(msImage);
                    Bitmap resizedImage = F.CreateThumbnail(bmpImage, 1200, 900);
                    Bitmap resizedThumb = F.CreateThumbnail(bmpImage, 267, 200);

                    resizedImage.Save(@"F:\inetpub\fotos.rpv.es\repositorio\"+ repositorio + "\\" + pixName, ImageFormat.Jpeg);
                    resizedThumb.Save(@"F:\inetpub\fotos.rpv.es\repositorio\" + repositorio + "\\thumbs\\" + pixName, ImageFormat.Jpeg);

                    resizedThumb.Dispose();
                    resizedImage.Dispose();
                    bmpImage.Dispose();
                    msImage.Close();

                    t.Foto64 = @"fotos.rpv.es/repositorio/" + repositorio + "/" + pixName;
                    t.Nombre = pixName;
                }
            }
            catch (Exception e)
            {
                throw new BadRequestException("Error: " + e.Message);
            }
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_setOrdenes", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ordenJson", SqlDbType.NVarChar).Value = orden.Ordenes;
                cmd.Parameters.Add("@fotosJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(listaFoto);

                // await cmd.ExecuteNonQueryAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    res += reader.GetString(0);
                }
                await reader.CloseAsync();

                if (res == "")
                    throw new NotFoundException("SQL problems");
            }
            catch (Exception e)
            {
                throw new BadRequestException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        
        public async Task<int> SetNewOrden(OrdenMsmDto orden)
        {
            var res = 0;
            
            var conn = new SqlConnection(_context.ConnMsm);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_web_setNewOrden", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@paramJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(orden);

                await cmd.ExecuteNonQueryAsync();

                // SqlDataReader reader = await cmd.ExecuteReaderAsync();
                //
                // while (reader.Read())
                // {
                //     res += reader.GetInt32(0);
                // }
                //
                // await reader.CloseAsync();

                // if (res == "")
                //     throw new NotFoundException("SQL Problems");

            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
            }
            
            return res;
        }
        
        #endregion
        
    }
}