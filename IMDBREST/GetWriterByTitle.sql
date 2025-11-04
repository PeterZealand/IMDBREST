create function GetWriterByTitle(@primaryTitle varchar(max))
returns table
as
return(
select titles.PrimaryTitle,names.PrimaryName from titles inner join CrewWriter on titles.id = CrewWriter.TitleId inner join names on CrewWriter.NameId = names.Id where titles.PrimaryTitle = @primaryTitle
);