select titles.PrimaryTitle,names.PrimaryName from titles inner join CrewDirector on titles.Id = CrewDirector.TitleId inner join names on CrewDirector.NameId = names.Id where titles.id = 1111

select titles.PrimaryTitle,names.PrimaryName from titles inner join CrewWriter on titles.Id = CrewWriter.TitleId inner join names on CrewWriter.NameId = names.Id where titles.id = 11111

select names.PrimaryName, Professions.Profession from names inner join NamesProfessions on names.Id = NamesProfessions.NameId inner join Professions on NamesProfessions.ProfessionId = professions.Id where names.PrimaryName = 'Jackie Chan'