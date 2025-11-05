using IMDB2025Inserter;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace IMDBFrontend {
    [Route("api/[controller]")]
    [ApiController]
    public class TitlesController : ControllerBase {
        string? conString = Program.connectionString;

        [HttpGet("Top")]
        public ActionResult<List<object>> GetTop(int count,string titleName){
            SqlConnection sqlConn = new(conString);
            try{
                List<object>? res = new();
                sqlConn.Open();

                res = GetTitleTop(count,titleName,sqlConn);
                sqlConn.Close();

                if(res?.Count <= 0) return NoContent();

                return Ok(res);
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Id")]
        public ActionResult<List<object>> GetTitleId(string titleName) {
            SqlConnection sqlConn = new(conString);
            try{
                int res = 0;

                sqlConn.Open();
                res = GetTitleId(titleName,sqlConn);
                sqlConn.Close();

                if(res != 0){
                    return Ok(res);
                }

                return NoContent();
            }
            catch(SqlException){
                sqlConn.Close();
                return BadRequest();
            }
        }

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
        public ActionResult<Title> Post([FromBody] TitleRecord value) {
            using SqlConnection sqlConn = new(conString);

            try{
                sqlConn.Open();

                using SqlTransaction sqlTrans = sqlConn.BeginTransaction();

                Title converted = RecordHelper.ConvertTitleRecord(value);

                InsertTitle(converted,sqlConn,sqlTrans);

                if(converted?.Genres?.Count > 0){
                    InsertTitleGenre(converted,sqlConn,sqlTrans);
                }
                sqlTrans.Commit();

                return Created("/"+converted?.Id,converted);
            }
            catch(Exception){
                return BadRequest();
            }
        }

        // PUT api/<IMDBController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) {
        }

        // DELETE api/<IMDBController>/5
        [HttpDelete("{id}")]
        public ActionResult<Title> Delete(int id) {
            using SqlConnection sqlConn = new(conString);

            try{
                sqlConn.Open();

                using SqlTransaction sqlTrans = sqlConn.BeginTransaction();

                object ob = GetTitleById(id,sqlConn,sqlTrans);
                RemoveTitle((int)ob,sqlConn,sqlTrans);

                sqlTrans.Commit();

                return Ok(ob);
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
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
            cmd.Parameters.AddWithValue("@typeId",(object?)title.TypeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@primarytitle",title.PrimaryTitle);
            cmd.Parameters.AddWithValue("@originalTitle",(object?)title.OriginalTitle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@isAdult",title.IsAdult);
            cmd.Parameters.AddWithValue("@startYear",(object?)title.StartYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@endYear",(object?)title.EndYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@runtimeMinutes",(object?)title.RuntimeMinutes ?? DBNull.Value);

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

        object GetTitleById(int id, SqlConnection conn,SqlTransaction trans){
            string getTitleId = "select * from titles where id = @id";
            SqlCommand cmd = new(getTitleId,conn,trans);
            cmd.Parameters.AddWithValue("@id",id);
            return cmd.ExecuteScalar();
        }

        void RemoveTitle(int id, SqlConnection conn, SqlTransaction trans){
            string removeTitleGenres = "delete from titlegenres where titleid = @id";
            SqlCommand cmdTitleGenre = new(removeTitleGenres,conn,trans);
            cmdTitleGenre.Parameters.AddWithValue("@id",id);
            cmdTitleGenre.ExecuteNonQuery();

            string removePrincipals = "delete from principals where titleid = @id";
            SqlCommand cmdPrincipals = new(removePrincipals,conn,trans);
            cmdPrincipals.Parameters.AddWithValue("@id",id);
            cmdPrincipals.ExecuteNonQuery();

            string removeCrewDirector = "delete from crewdirector where titleid = @id";
            SqlCommand cmdCrewDirector = new(removeCrewDirector,conn,trans);
            cmdCrewDirector.Parameters.AddWithValue("@id",id);
            cmdCrewDirector.ExecuteNonQuery();

            string removeCrewWriter = "delete from crewwriter where titleid = @id";
            SqlCommand cmdCrewWriter = new(removeCrewWriter,conn,trans);
            cmdCrewWriter.Parameters.AddWithValue("@id",id);
            cmdCrewWriter.ExecuteNonQuery();

            string removeNamesKnownFor = "delete from namesknownfor where titleid = @id";
            SqlCommand cmdNamesKnownFor = new(removeNamesKnownFor,conn,trans);
            cmdNamesKnownFor.Parameters.AddWithValue("@id",id);
            cmdNamesKnownFor.ExecuteNonQuery();

            string remove = "delete from titles where id = @id";
            SqlCommand cmd = new(remove,conn,trans);
            cmd.Parameters.AddWithValue("@id",id);
            cmd.ExecuteNonQuery();
        }

        List<object>? GetTitleTop(int count,string titleName, SqlConnection conn){
            string q = "select distinct top(@count) primarytitle from titles where primarytitle like @titleName";
            SqlCommand cmd = new(q,conn);
            cmd.Parameters.AddWithValue("@count",count);
            cmd.Parameters.AddWithValue("@titleName",$"%{titleName}%");

            List<object> res = new();
            using(SqlDataReader reader = cmd.ExecuteReader()){
                while(reader.Read()){
                    res.Add(reader[0]);
                }
            }
            return res;
        }

        int GetTitleId(string titleName, SqlConnection conn){
            string q = "select id from titles where primarytitle = @titleName";
            SqlCommand cmd = new(q,conn);
            cmd.Parameters.AddWithValue("@titleName",titleName);
            int titleId = Convert.ToInt32(cmd.ExecuteScalar());
            return titleId;
        }
    }
}
