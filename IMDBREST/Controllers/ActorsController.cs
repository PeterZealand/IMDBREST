using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet("Name")]
        public ActionResult<List<object>> Get(string actorName){
            List<object> res = new();
            SqlConnection sqlConn = new(conString);

            sqlConn.Open();
            res = GetPersonWildCard(actorName,sqlConn);

            sqlConn.Close();
            return res;
        }

        // POST api/<ActorsController>
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/<ActorsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<ActorsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }

        List<object> GetPersonWildCard(string name,SqlConnection conn){
            List<object> res = new();
            string get = "select primaryname from names where primaryname like @name"+
                " order by primaryname";
            SqlCommand cmd = new(get,conn);

            cmd.Parameters.AddWithValue("@name",$"%{name}%");

            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }

        void InsertPerson(Title title, SqlConnection conn){
            string insert = "insert into titles values" +
                "(@primaryTitle,@originalTitle,@isAdult,@startYear,@endYear,@runtimeMinutes)";
            SqlCommand cmd = new(insert,conn);

            cmd.Parameters.AddWithValue("@primaryTitle",title.PrimaryTitle);
            cmd.Parameters.AddWithValue("@originalTitle",title.OriginalTitle);
            cmd.Parameters.AddWithValue("@isAdult",title.IsAdult);
            cmd.Parameters.AddWithValue("@startYear",title.StartYear);
            cmd.Parameters.AddWithValue("@endYear",title.EndYear);
            cmd.Parameters.AddWithValue("@runtimeMinutes",title.RuntimeMinutes);
            cmd.ExecuteNonQuery();
        }
    }
}
