using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class TitlesController : ControllerBase {
        string? conString = Program.connectionString;

        // [HttpGet]
        // public ActionResult<List<object>> Get(){
        //     try{
        //         List<object> res = new();
        //         SqlConnection sqlConn = new(conString);
        //         sqlConn.Open();
        //         res = GetTitles(sqlConn);
        //         sqlConn.Close();
        //         return Ok(res);
        //     }
        //     catch(Exception){
        //     }
        //     return NoContent();
        // }

        // GET api/<IMDBController>/5
        [HttpGet("Name")]
        public ActionResult<List<object>> Get(string titleName) {
            try{
                List<object> res = new();
                SqlConnection sqlConn = new(conString);

                sqlConn.Open();
                res = GetTitleWildCard(titleName,sqlConn);
                sqlConn.Close();
                if(res.Count > 0) return Ok(res);
                return NoContent();
            }
            catch(SqlException){
                return BadRequest();
            }
        }

        // POST api/<IMDBController>
        [HttpPost]
        public void Post([FromBody] string value) {
        }

        // PUT api/<IMDBController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<IMDBController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }

        List<object> GetTitles(SqlConnection conn){
            List<object> res = new();
            string get = $"select * from titles";
            SqlCommand cmd = new(get,conn);
            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }

        List<object> GetTitleWildCard(string titleName,SqlConnection conn){
            string get = $"select primarytitle from titles where primarytitle like @titleName"+
                " order by primarytitle";
            SqlCommand cmd = new(get,conn);

            cmd.Parameters.AddWithValue("@titleName",$"%{titleName}%");

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
