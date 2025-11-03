create function GetNameByProfession (@profession varchar(max))
returns table
as
return(
select names.PrimaryName, Professions.Profession from names inner join NamesProfessions on names.Id = NamesProfessions.NameId inner join Professions on NamesProfessions.ProfessionId = professions.Id where Professions.Profession = @profession
);