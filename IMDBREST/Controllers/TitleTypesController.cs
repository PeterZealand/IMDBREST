using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class TitleTypesController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet]
        public ActionResult<List<object>> Get(){
            try{
                List<object> res = new();
                SqlConnection sqlConn = new(conString);
                sqlConn.Open();
                res = GetTitleTypes(sqlConn);
                sqlConn.Close();
                return Ok(res);
            }
            catch(Exception){
            }
            return NoContent();
        }

        // GET api/<IMDBController>/5
        [HttpGet("Name")]
        public ActionResult<List<object>> GetId(string titleName) {
            try{
                SqlConnection sqlConn = new(conString);
                sqlConn.Open();

                // string getIdFunc = "declare @typeId varchar(max); exec @typeId = GetTitleTypeId @titleTypeName = @searchTerm; select @typeId;";
                string getIdFunc = "select id from titletypes where typename = @searchTerm";
                SqlCommand cmd = new(getIdFunc,sqlConn);
                cmd.Parameters.AddWithValue("@searchTerm",titleName);
                int res = Convert.ToInt32(cmd.ExecuteScalar());

                sqlConn.Close();

                if(res == 0) return NoContent();

                return Ok(res);
            }
            catch(SqlException ex){
                return BadRequest(ex.Message);
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

        List<object> GetTitleTypes(SqlConnection conn){
            List<object> res = new();
            string get = $"select typename from titletypes";
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

