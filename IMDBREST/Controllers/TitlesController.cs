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
            SqlConnection sqlConn = new(conString);
            try{
                List<object> res = new();

                sqlConn.Open();
                res = GetTitleWildCard(titleName,sqlConn);
                sqlConn.Close();

                if(res.Count > 0) return Ok(res);
                return NoContent();
            }
            catch(SqlException){
                sqlConn.Close();
                return BadRequest();
            }
        }

        // POST api/<IMDBController>
        [HttpPost]
        public void Post([FromBody] TitleRecord value) {
            SqlConnection sqlConn = new(conString);
            sqlConn.Open();
            SqlTransaction sqlTrans = sqlConn.BeginTransaction();
            Console.WriteLine(value);

            try{
                Title? converted = RecordHelper.ConvertTitleRecord(value);
                InsertTitle(converted,sqlConn,sqlTrans);
                InsertTitleGenre(converted,sqlConn,sqlTrans);
            }
            catch(Exception){
            }

            sqlTrans.Commit();
            sqlTrans.Dispose();
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
            string get = "select primarytitle from titles where primarytitle like @titleName"+
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

        void InsertTitle(Title title, SqlConnection conn,SqlTransaction trans) {
            string insert = "insert into titles values"+
                "(@typeId,@primarytitle,@originalTitle,@isAdult,@startYear,@endYear,@runtimeMinutes)";

            SqlCommand cmd = new(insert,conn,trans);
            cmd.Parameters.AddWithValue("@typeId",title.TypeId);
            cmd.Parameters.AddWithValue("@primarytitle",title.PrimaryTitle);
            cmd.Parameters.AddWithValue("@originalTitle",title.OriginalTitle);
            cmd.Parameters.AddWithValue("@isAdult",title.IsAdult);
            cmd.Parameters.AddWithValue("@startYear",title.StartYear);
            cmd.Parameters.AddWithValue("@endYear",title.EndYear);
            cmd.Parameters.AddWithValue("@runtimeMinutes",title.RuntimeMinutes);

            cmd.ExecuteScalar();
        }

        void InsertTitleGenre(Title title, SqlConnection conn,SqlTransaction trans){
            string insert = "";
            int titleId = GetTitleId(title,conn,trans);
            List<int> genreIds = GetGenreIds(title,conn,trans);

            foreach(int gId in genreIds){
                insert = "insert into titleGenres values(@titleId,@genreId)";
                SqlCommand cmd = new(insert,conn,trans);
                cmd.Parameters.AddWithValue("@titleId",titleId);
                cmd.Parameters.AddWithValue("@genreId",gId);
                cmd.ExecuteNonQuery();
            }
        }

        List<int> GetGenreIds(Title title, SqlConnection conn, SqlTransaction trans){
            List<int> genreIds = new();

            foreach(string g in title.Genres){
                string q = "select id from genres where genre = @genre";
                SqlCommand getGenreId = new(q,conn,trans);
                getGenreId.Parameters.AddWithValue("@genre",g);
                int res = Convert.ToInt32(getGenreId.ExecuteScalar());
                genreIds.Add(res);
            }

            return genreIds;
        }

        int GetTitleId(Title title, SqlConnection conn, SqlTransaction trans){
            string getTitleId = "select id from titles where primaryTitle = @primaryTitle";
            SqlCommand ge = new(getTitleId,conn,trans);
            ge.Parameters.AddWithValue("@primaryTitle",title.PrimaryTitle);
            int titleId = Convert.ToInt32(ge.ExecuteScalar());
            return titleId;
        }
    }
}
