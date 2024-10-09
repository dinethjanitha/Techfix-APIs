using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using techfixAPI.Models;

namespace techfixAPI.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> login([FromBody] User user)
        {
            string conString = _configuration.GetConnectionString("dconn");

            SqlConnection conn = new SqlConnection(conString);

            try
            {
                string sql = "SELECT * FROM users WHERE email = @Email AND password = @Password";


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Password", user.password);

                await conn.OpenAsync();
               SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows) { 
                    await reader.ReadAsync();

                    var userid = reader.GetInt32(0);
                    var username = reader.GetString(1);
                    var userType = reader.GetString(6);

                    return Ok(new { message = "Login Success!" , user_id = userid , user_name = username , userType = userType  });

                }
                else
                {
                    return Unauthorized(new { message = "Login Faild" });
                }

            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);

            }
            finally
            {
                await conn.CloseAsync();    
            }
        }

        
    }
}
