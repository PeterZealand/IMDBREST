create function GetDirectorByTitle(@primaryTitle varchar(max))
returns table
as
return(
select titles.PrimaryTitle,names.PrimaryName from titles inner join CrewDirector on titles.id = CrewDirector.TitleId inner join names on CrewDirector.NameId = names.Id where titles.PrimaryTitle = @primaryTitle
);