--1 - il faut aller dans Contact et selectionner les collaborateur actifs 

 SELECT 'insert into ##idsCollab values('+ Cast(c.id as varchar) +','+ Cast(c.IsActive as varchar)+')'
  FROM [contact].[Contact] c
  where ContactTypeId = 1

-- 2 - creer une table temporaire dans mandat 
 Create table ##idsCollab (
     Id int not null,
	 isActive bit not null 
	 Primary key(ID)
)
--3 - Exécuter les script d'insert dans la table temp 
--4 - Delete les roles associés 

delete from Mandate.CompanyCollaborator where CollaboratorId not in (
select o.Id from Mandate.Collaborator o
inner join ##idsCollab i on o.Id = i.Id )
--5 - Delete les clients 
delete from Mandate.Collaborator where Id not in (
select o.Id from Mandate.Collaborator o
inner join ##idsCollab i on o.Id = i.Id )

--6 - Delete la table tempo  
drop table ##idsCollab