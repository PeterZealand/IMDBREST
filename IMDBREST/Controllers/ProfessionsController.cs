using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionsController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet]
        public ActionResult<List<object>> Get(){
            List<object> res = new();
            SqlConnection sqlConn = new(conString);

            sqlConn.Open();
            res = GetProfessions(sqlConn);

            sqlConn.Close();
            return res;
        }

        List<object> GetProfessions(SqlConnection conn){
            List<object> res = new();
            string get = "select profession from professions";
            SqlCommand cmd = new(get,conn);

            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }
    }
}
