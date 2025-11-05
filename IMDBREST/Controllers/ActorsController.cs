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
        public ActionResult<Name> Post([FromBody] NameRecord value) {
            Console.WriteLine(value);
            SqlConnection sqlConn = new(conString);
            try{
                sqlConn.Open();
                using SqlTransaction sqlTrans = sqlConn.BeginTransaction();

                Name converted = RecordHelper.ConvertNameRecord(value);

                InsertPerson(converted,sqlConn,sqlTrans);

                if(converted?.PrimaryProfessions?.Count > 0){
                    InsertNamesProfessions(converted,sqlConn,sqlTrans);
                }

                if(converted?.KnownForTitles?.Count > 0){
                    InsertNamesKnownFor(converted,sqlConn,sqlTrans);
                }

                sqlTrans.Commit();
                sqlConn.Close();
                return Created("/"+converted?.Id,converted);
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
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

        void InsertPerson(Name name, SqlConnection conn, SqlTransaction trans){
            string insert = "insert into names values" +
                "(@primaryName,@birthYear,@deathYear)";
            SqlCommand cmd = new(insert,conn,trans);

            cmd.Parameters.AddWithValue("@primaryName",name.PrimaryName);
            cmd.Parameters.AddWithValue("@birthYear",(object?)name.BirthYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deathYear",(object?)name.DeathYear ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        void InsertNamesProfessions(Name name, SqlConnection conn,SqlTransaction trans){
            if(name?.PrimaryProfessions?.Count <= 0) return;

            string insert = "";
            int nameId = GetNameId(name,conn,trans);
            List<int>? professionsId = GetProfessionsId(name,conn,trans);

            if(professionsId != null){
                foreach(int gId in professionsId){
                    insert = "insert into namesProfessions values(@nameId,@professionId)";
                    SqlCommand cmd = new(insert,conn,trans);
                    cmd.Parameters.AddWithValue("@nameId",nameId);
                    cmd.Parameters.AddWithValue("@professionId",gId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void InsertNamesKnownFor(Name name, SqlConnection conn,SqlTransaction trans){
            string insert = "";
            int nameId = GetNameId(name,conn,trans);
            List<int>? titlesId = GetTitlesId(name,conn,trans);

            if(titlesId != null){
                foreach(int gId in titlesId){
                    insert = "insert into namesknownfor values(@nameId,@titleid)";
                    SqlCommand cmd = new(insert,conn,trans);
                    cmd.Parameters.AddWithValue("@nameId",nameId);
                    cmd.Parameters.AddWithValue("@titleid",gId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        List<int>? GetTitlesId(Name name, SqlConnection conn, SqlTransaction trans){
            if(name.KnownForTitles == null) return null;

            List<int> titlesId = new();

            foreach(string n in name.KnownForTitles){
                string q = "select id from titles where primaryTitle = @titleName";
                SqlCommand getTitlesId = new(q,conn,trans);

                getTitlesId.Parameters.AddWithValue("@titleName",n);

                int res = Convert.ToInt32(getTitlesId.ExecuteScalar());
                Console.WriteLine(res);
                titlesId.Add(res);
            }

            return titlesId;
        }

        List<int>? GetProfessionsId(Name name, SqlConnection conn, SqlTransaction trans){
            if(name.PrimaryProfessions == null) return null;
            List<int> professionIds = new();

            foreach(string p in name.PrimaryProfessions){
                string q = "select id from professions where profession = @profession";
                SqlCommand getProfessionsId = new(q,conn,trans);

                getProfessionsId.Parameters.AddWithValue("@profession",p);

                int res = Convert.ToInt32(getProfessionsId.ExecuteScalar());
                professionIds.Add(res);
            }

            return professionIds;
        }

        int GetNameId(Name name, SqlConnection conn, SqlTransaction trans){
            string getNameId = "select id from names where primaryName = @primaryName";
            SqlCommand ge = new(getNameId,conn,trans);
            ge.Parameters.AddWithValue("@primaryName",name.PrimaryName);
            int nameId = Convert.ToInt32(ge.ExecuteScalar());
            Console.WriteLine(nameId);
            return nameId;
        }
    }
}
