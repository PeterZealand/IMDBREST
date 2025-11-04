using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend{
    [Route("api/[controller]")]
    [ApiController]
    public class WritersController : ControllerBase{
        string? conString = Program.connectionString;

        [HttpGet("Title")]
        public ActionResult<object> Get(string title){
            SqlConnection sqlConn = new(conString);

            try{
                sqlConn.Open();

                object res = new();
                res = GetWriterByTitle(title,sqlConn);

                sqlConn.Close();
                if(res == null) return NoContent();

                return Ok(res);
            }
            catch(Exception){
                return BadRequest();
            }
        }

        List<object> GetWriterByTitle(string title, SqlConnection conn){
            string q = "select primaryname from getwriterbytitle(@title)";
            SqlCommand cmd = new(q,conn);
            cmd.Parameters.AddWithValue("@title",title);

            List<object> res = new();
            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }
    }
}
