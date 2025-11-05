using IMDB2025Inserter;
namespace IMDBFrontend;

public record TitleRecord(int? Id,int? TypeId, string? PrimaryTitle, string? OriginalTitle, bool IsAdult, int? StartYear, int? EndYear, int? RuntimeMinutes,List<string>? Genres);
public record NameRecord(int? Id, string? PrimaryName, int? BirthYear, int? DeathYear, List<string>? PrimaryProfessions, List<string>? KnownForTitles);

public static class RecordHelper {
    public static Title ConvertTitleRecord(TitleRecord record) {
        if(record.Id == null){
            throw new ArgumentNullException("" + record.Id);
        }
        if (record.PrimaryTitle == null){
            throw new ArgumentNullException("" + record.PrimaryTitle);
        }
        if(record.TypeId == null){
            throw new ArgumentNullException("" + record.TypeId);
        }
        // if(record.Genres == null){
        //     throw new ArgumentNullException("" + record.Genres);
        // }
        return new Title(0,0,"","",false,0,0,0,null) {
            Id = record.Id ?? null,
            TypeId = (int)record.TypeId,
            PrimaryTitle = record.PrimaryTitle,
            OriginalTitle = record.OriginalTitle ?? null,
            IsAdult = record.IsAdult,
            StartYear = record.StartYear ?? null,
            EndYear = record.EndYear ?? null,
            RuntimeMinutes = record.RuntimeMinutes ?? null,
            Genres = record.Genres ?? null,
        };
    }

    public static Name ConvertNameRecord(NameRecord record) {
        if (record.PrimaryName == null){
            throw new ArgumentNullException("" + record.PrimaryName);
        }
        return new Name(0,"",0,0,null,null){
            Id = record.Id ?? null,
            PrimaryName = record.PrimaryName,
            BirthYear = record.BirthYear ?? null,
            DeathYear = record.DeathYear ?? null,
            PrimaryProfessions = record.PrimaryProfessions ?? null,
            KnownForTitles = record.KnownForTitles ?? null
        };
    }
}
