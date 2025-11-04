--select * from GetActorNameIdFromTitle('pirates of the caribbean')

--select titles.PrimaryTitle,names.PrimaryName from titles inner join CrewDirector on titles.Id = CrewDirector.TitleId inner join names on CrewDirector.NameId = names.Id where titles.id = 1111

select primaryname from names where id in (select id from GetActorNameIdFromTitle('pirates of the caribbean'))