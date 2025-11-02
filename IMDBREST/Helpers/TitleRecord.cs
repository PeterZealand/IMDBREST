using IMDB2025Inserter;
namespace IMDBFrontend;

public record TitleRecord(int? Id,int? TypeId, string? PrimaryTitle, string? OriginalTitle, bool IsAdult, int? StartYear, int? EndYear, int? RuntimeMinutes, List<string>? Genres);

public static class RecordHelper {
    public static Title ConvertTitleRecord(TitleRecord record) {
        if (record.PrimaryTitle == null) {
            throw new ArgumentNullException("" + record.PrimaryTitle);
        }
        if(record.Genres == null){
            throw new ArgumentNullException("" + record.Genres);
        }
        return new Title {
            Id = record.Id,
            TypeId = record.TypeId,
            PrimaryTitle = record.PrimaryTitle,
            OriginalTitle = record.OriginalTitle,
            IsAdult = record.IsAdult,
            StartYear = record.StartYear,
            EndYear = record.EndYear,
            RuntimeMinutes = record.RuntimeMinutes,
            Genres = record.Genres
        };
    }
}
