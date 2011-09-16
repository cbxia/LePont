SET IDENTITY_INSERT [department] ON

insert into [department]
([id], [code], [name], [parent_department_id], [level], [list_order], [deactivated])
select id, tcode,substring(tname, 1, len(tname) - 1), case when tn = 0 then null else tn end, tj,listorder, 0
from [mdjfdb].[dbo].[department]

update department
set name=  substring(name, charindex('|', name) + 1, len(name)-  charindex('|', name))

update department
set name=  substring(name, charindex('|', name) + 1, len(name)-  charindex('|', name))

SET IDENTITY_INSERT [department] OFF

SET IDENTITY_INSERT [case_type] ON

insert into [case_type]
([id], [name], [domain], [deactivated])
select cataid, cataname, 'IN', 0
from [mdjfdb].[dbo].[admin_cata]
where catalevel = ''

SET IDENTITY_INSERT [case_type] OFF

insert into [case_type]
([name], [domain], [deactivated])
values ('一般类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('涉企类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('征地拆迁类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('城市管理类', 'EX', 0)
insert into [case_type]
([name], [domain], [deactivated])
values ('劳动合同类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('涉农类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('涉法涉诉类', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('其他类', 'EX', 0)

