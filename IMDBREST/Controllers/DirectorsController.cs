using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase{
        string? conString = Program.connectionString;

        [HttpGet("Title")]
        public ActionResult<object> Get(string title){
            SqlConnection sqlConn = new(conString);

            try{
                sqlConn.Open();

                object res = new();
                res = GetDirectorByTitle(title,sqlConn);

                sqlConn.Close();
                if(res == null) return NoContent();

                return Ok(res);
            }
            catch(Exception){
                return BadRequest();
            }
        }

        List<object> GetDirectorByTitle(string title, SqlConnection conn){
            string q = "select primaryname from getdirectorbytitle(@title)";
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

        List<object> GetDirectors(string profession,SqlConnection conn){
            string query = "select top 1000 primaryname from GetNameByProfession(@profession)";
            SqlCommand cmd = new(query,conn);
            cmd.Parameters.AddWithValue("@profession",profession);

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
