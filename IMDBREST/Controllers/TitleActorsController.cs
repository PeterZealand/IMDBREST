using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class TitleActorsController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet("Name")]
        public ActionResult<List<object>> Get(string title){
            List<object> res = new();
            SqlConnection sqlConn = new(conString);

            sqlConn.Open();
            res = GetActorsByTitle(title,sqlConn);

            sqlConn.Close();
            return res;
        }

        List<object> GetActorsByTitle(string title,SqlConnection conn){
            List<object> res = new();
            string get = "";
            SqlCommand cmd = new(get,conn);

            cmd.Parameters.AddWithValue("@name",title);

            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }
    }
}
