using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet]
        public ActionResult<List<object>> Get(){
            try{
                List<object> res = new();
                SqlConnection sqlConn = new(conString);
                sqlConn.Open();
                res = GetGenres(sqlConn);
                sqlConn.Close();
                return Ok(res);
            }
            catch(Exception){
            }
            return NoContent();
        }

        // GET api/<IMDBController>/5
        [HttpGet("Name")]
        public ActionResult<List<object>> Get(string titleName) {
            try{
                List<object> res = new();
                SqlConnection sqlConn = new(conString);
                return NoContent();
            }
            catch(SqlException){
                return BadRequest();
            }
        }

        // POST api/<IMDBController>
        [HttpPost]
        public void Post([FromBody] TitleRecord value) {
            SqlConnection sqlConn = new(conString);
            sqlConn.Open();

            Title? toConvert = RecordHelper.ConvertTitleRecord(value);

            sqlConn.Close();
        }

        // PUT api/<IMDBController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<IMDBController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }

        List<object> GetGenres(SqlConnection conn){
            List<object> res = new();
            string get = $"select genre from genres";
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

