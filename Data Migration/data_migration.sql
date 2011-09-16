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
values ('һ����', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('������', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('���ز�Ǩ��', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('���й�����', 'EX', 0)
insert into [case_type]
([name], [domain], [deactivated])
values ('�Ͷ���ͬ��', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('��ũ��', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('�淨������', 'EX', 0);
insert into [case_type]
([name], [domain], [deactivated])
values ('������', 'EX', 0)

