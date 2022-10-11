using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using msm_ws.Data.DbContexts;
using msm_ws.Data.Dto;
using msm_ws.Helpers;

namespace msm_ws.Repositories
{
    public  class LotesRepository
    {
        
        #region [Variables & Constructor]
        private readonly RpvDbContext _context;
        private readonly AppSettings _appSettings;
        //private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public LotesRepository(IOptions<AppSettings> appSettings, RpvDbContext context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }
        #endregion
        
        #region [Getters]
        public async Task<string> GetLotes01(string authorization, string tipoCliente)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_app_getLotes01", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@tipoCliente", SqlDbType.NVarChar).Value = tipoCliente;
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                if (res == "")
                    throw new NotContentException("Sin datos");
                
            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize<StringDto>(new StringDto(res));
            return res;
        }
        
        public async Task<string> GetLotes02(string authorization, string centro)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_app_getLotes02", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@centro", SqlDbType.NVarChar).Value = centro;
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                if (res == "")
                    throw new NotContentException("Sin datos");
                
            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize<StringDto>(new StringDto(res));
            return res;
        }
        
        public async Task<string> GetLotes03(string authorization, string centro)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_app_getLotes03", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@centro", SqlDbType.NVarChar).Value = centro;
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();

                if (res == "")
                    throw new NotContentException("Sin datos");
                
            }
            catch (NotContentException ex)
            {   
                throw new NotContentException(ex.Message);
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize<StringDto>(new StringDto(res));
            return res;
        }
        
        #endregion
        
        #region [Setters]
        public async Task<object> SetLote01(string authorization, Lote01Dto lote)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_web_setLote01", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@paramJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(lote);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetInt32(0);
                }

                await reader.CloseAsync();
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize(flota);
            return res;
        }
        
        public async Task<object> SetLote02(string authorization, Lote02Dto lote)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_app_setLote02", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@paramJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(lote);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize(flota);
            return res;
        }
        
        public async Task<object> SetLote03(string authorization, Lote03Dto[] lote)
        {
            var res = "";
            // int rowcount = 0;
            var tok = authorization.Replace("Bearer ", "").Replace(System.Environment.NewLine, "");
            var jwttoken = new JwtSecurityTokenHandler().ReadJwtToken(tok);
            var person = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            var conn = new SqlConnection(_context.ConnRpvGestion);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_app_setLote03", conn);
                cmd.CommandTimeout = 120;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@person", SqlDbType.NVarChar).Value = person;
                cmd.Parameters.Add("@paramJson", SqlDbType.NVarChar).Value = JsonSerializer.Serialize(lote);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    res += reader.GetString(0);
                }

                await reader.CloseAsync();
            }
            catch (Exception e)
            {
                throw new NotFoundException("Error: " + e.ToString());
            }
            finally
            {
                conn.Close();
            }

            // return JsonSerializer.Serialize(flota);
            return res;
        }
       
        #endregion
        
    }
}