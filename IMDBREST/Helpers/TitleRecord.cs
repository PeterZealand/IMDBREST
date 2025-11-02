using IMDB2025Inserter;
namespace IMDBFrontend;

public record TitleRecord(int? Id,int? TypeId, string? PrimaryTitle, string? OriginalTitle, bool IsAdult, int? StartYear, int? EndYear, int? RuntimeMinutes, List<string>? Genres);

public static class RecordHelper {
    public static Title ConvertTitleRecord(TitleRecord record) {
        Console.WriteLine(record);
        if (record.PrimaryTitle == null) {
            throw new ArgumentNullException("" + record.PrimaryTitle);
        }
        return new Title {
            Id = record.Id ?? null,
            TypeId = record.TypeId ?? null,
            PrimaryTitle = record.PrimaryTitle ?? null,
            OriginalTitle = record.OriginalTitle ?? null,
            IsAdult = record.IsAdult,
            StartYear = record.StartYear ?? null,
            EndYear = record.EndYear ?? null,
            RuntimeMinutes = record.RuntimeMinutes ?? null,
            Genres = record.Genres ?? null
        };
    }
}
