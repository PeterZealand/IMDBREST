using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend{
    public class NamesController : ControllerBase{
        string? conString = Program.connectionString;

        [HttpGet("Name")]
        public ActionResult<List<object>> Get(string actorName){
            List<object> res = new();
            SqlConnection sqlConn = new(conString);

            sqlConn.Open();

            sqlConn.Close();
            return res;
        }

        void GetNames(SqlConnection conn, SqlTransaction trans){
            string query = "select primaryname from names"
        }
    }
}
