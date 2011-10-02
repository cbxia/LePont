create table [department]
(
	[id] int not null identity(1,1) primary key ,
	[code] varchar(100) not null, -- hierarchical
	[name] varchar(40) not null, -- personal/org. name
	[parent_department_id] int null references [department]([id]),
	[level] smallint not null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

create table [password_question]
(
	[id] int not null identity(1,1) primary key,
	[content] varchar(100) not null,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [password_question] on;

insert into [password_question] ([id], [content])
values (1, '������������죿(�ش���: 1980��02��25��)');
insert into [password_question] ([id], [content])
values (2, '��ĳ��������ĸ���(��)��(�ش���: ������)');
insert into [password_question] ([id], [content])
values (3, '��������μӹ�����(�ش���: 2000��)');

set IDENTITY_INSERT [password_question] off;
  	
create table [user]
(
	[id] int not null identity(1,1) primary key,
	[login_id] varchar(40) not null, 
	[name] varchar(40) null,
	[password] varchar(20) not null,
	[password_question_id] int null references [password_question]([id]),
	[password_answer] varchar(40) null,
	[department_id] int null references [department]([id]),
	[phone] varchar(20) null,
	[email] varchar(100) null,
	[create_time] smalldatetime not null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [user] on;

insert into [user]([id], [login_id], [name], [password], [department_id])
values (1, 'admin', 'ϵͳ����Ա', 'admin', 1);

insert into [user]([id], [login_id], [name], [password], [department_id], [phone])
values (2, 'xcb', '�Ĺ�', 'xcb',2,'13764438712');

set IDENTITY_INSERT [user] off;

create table [user_session]
(
	[id]  int not null identity(1,1) primary key ,
	[user_id] int not null references [user]([id]),
	[ip] varchar(15) not null,
	[logon_date_time] smalldatetime not null,
	[logoff_date_time] smalldatetime null,
	[browser] varchar(20) null
);

create table [role]
(
	[id] int not null identity(1,1) primary key ,
	[name] varchar(20),
	[description] varchar(40),
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [role] on;

insert into [role] ([id], [name])
values (1, 'ϵͳ����');
insert into [role] ([id], [name])
values (2, '���ݹ���');
insert into [role] ([id], [name])
values (3, '��������');
insert into [role] ([id], [name])
values (4, '������ʾ');
insert into [role] ([id], [name])
values (5, 'Ԥ������');
insert into [role] ([id], [name])
values (6, '���ò���');

set IDENTITY_INSERT [role] off;

create table [user_x_role]
(
	[user_id] int not null references [user]([id]),
	[role_id] int not null references [role]([id])
);

insert into [user_x_role]
values(1, 1)
insert into [user_x_role]
values(1, 2)
insert into [user_x_role]
values(1, 3)

insert into [user_x_role]
values(2, 1)
insert into [user_x_role]
values(2, 2)
insert into [user_x_role]
values(2, 3)

insert into [user_x_role]
values(3, 1)
insert into [user_x_role]
values(3, 2)
insert into [user_x_role]
values(3, 5)

create table [case_type]
(
	[id] int not null identity(1,1) primary key ,
	[name] varchar(20) not null,
	[domain] varchar(20) not null,
	[description] varchar(40) null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

create table [relation_type]
(
	[id] int not null identity(1,1) primary key ,
	[name] varchar(40) not null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [relation_type] on;

insert into [relation_type] ([id], [name])
values (1, '�����빫��֮�����');

insert into [relation_type] ([id], [name])
values (2, '�����뷨�˼������֮֯�����');

set IDENTITY_INSERT [relation_type] off;

create table [case]
(
    [id] int not null identity(1,1) primary key ,
	[title] varchar(100) null,
	[locality] varchar(100) null,
	[department_id] int not null references [department]([id]),
	[internal_case_type_id] int not null references [case_type]([id]),
	[external_case_type_id] int not null references [case_type]([id]),
	[content] varchar(max) not null,
	[money_involved] decimal(10,2) null,
	[people_involved] smallint null,
	[flag_1] bit null, -- ��ֹ������������ɱ
	[flag_2] bit null, -- ��ֹ��ת�̰���  
	[flag_3] bit null, -- ��ֹȺ�����¼�
	[flag_4] bit null, -- ��ֹȺ�����Ϸ�
	[flag_5] bit null, -- �о����������ȷ����Ч
	[flag_6] bit null, -- ����֧����
	[flag_7] bit null, -- �γ�Ⱥ�����¼�
	[flag_8] bit null, -- �γ�Ⱥ�����Ϸ�
	[flag_9] bit null, -- reserved for future use
	[flag_10] bit null, -- reserved for future use
	[status] tinyint null default 0, -- 0 = �Ų� 1 = Ԥ�� 2 = ���� 3 = ����
	[parties_relation_type_id] int not null references [relation_type]([id]),
	[mediator_advice] varchar(1000) null,
	[instructions] varchar(1000) null,
	[progress] varchar(200) null,
	[disposal] varchar(200) null,
	[responsable] varchar(20) not null,
	[responsable_phone] varchar(20) null,
    [is_concluded] bit null, -- �Ƿ�᰸
    [conclude_date] smalldatetime null ,-- �᰸ʱ��
 	[registrar_id] int not null references [user]([id]),
    [date_time] smalldatetime not null,
	[last_modify_time] smalldatetime null,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

create table [instruction]
(
	[id] int not null identity(1,1) primary key,
	[case_id] int not null references [case]([id]),
	[title] varchar(100) not null,
	[content] varchar(1000) not null,
    [attachment_file_name] varchar(100),
    [attachment_file_data] varbinary(max),
	[department_id] int not null references [department]([id]),
 	[issuer_id] int not null references [user]([id]),
 	[date_time] datetime not null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

create table [forum_block]
(
	[id] int not null identity(1,1) primary key,
	[name] varchar(100) not null,
 	[last_publisher_id] int null references [user]([id]),
 	[last_publish_time] datetime null,
 	[admin_id] int not null references [user]([id]),
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [forum_block] on;

insert into [forum_block]([id], [name], [admin_id], [list_order])
values (1, '��������', 1, 1);

insert into [forum_block]([id], [name], [admin_id], [list_order])
values (2, 'ר��Ӧ��', 1, 2);

insert into [forum_block]([id], [name], [admin_id], [list_order])
values (3, '���Լ���', 1, 3);

set IDENTITY_INSERT [forum_block] off;

-- abstract
create table [forum_post]
(
	[id] int not null identity(1,1) primary key,
	[block_id] int not null references [forum_block]([id]),
	[content] varchar(2000) not null,
 	[publisher_id] int not null references [user]([id]),
	[publish_time] datetime not null,
	[list_order] smallint null default 0,
    [attachment_file_name] varchar(100),
    [attachment_file_data] varbinary(max)
);

-- derived from [forum_post]
create table [forum_topic]
(
	[id] int not null primary key references [forum_post]([id]),
	[title] varchar(100) not null,
	[last_publisher_id] int null references [user]([id]),
	[last_publish_time] datetime not null
)

-- derived from [forum_post]
create table [forum_response]
(
	[id] int not null primary key references [forum_post]([id]),
	[topic_id] int not null references [forum_topic]([id])
);

create table [message]
(
    [id] int not null identity(1,1) primary key ,
	[sender_id] int not null references [user]([id]),
	[receiver_id] int not null references [user]([id]),
	[subject] varchar(200) null,
	[content] varchar(200) null,
    [attachment_file_name] varchar(100),
    [attachment_file_data] varbinary(max),
    [send_date_time] smalldatetime not null,
    [receive_date_time] smalldatetime not null,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

create table [publication_type]
(
	[id] int not null identity(1,1) primary key ,
	[name] varchar(20) not null,
	[deactivated] bit null default 0 -- 0 = false, 1 = true
);

set IDENTITY_INSERT [publication_type] on;

insert into [publication_type]
([id],[name],[deactivated])
values(1,'��������', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(2,'����֪ͨ', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(3,'���Ͻ���', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(4,'��������', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(5,'���ɷ���', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(6,'���Ž���', 0);

insert into [publication_type]
([id],[name],[deactivated])
values(7,'�Ⲻ��̬', 0);

set IDENTITY_INSERT [publication_type] off;

create table [publication]
(
    [id] int not null identity(1,1) primary key,
	[title] varchar(100) not  null,
    [type_id] int not null references [publication_type]([id]),
    [content] varchar(max) not null,
    [attachment_file_name] varchar(100),
    [attachment_file_data] varbinary(max),
	[department_id] int not null references [department]([id]),
	[publisher_id] int not null references [user]([id]),
    [date_time] smalldatetime not null,
	[list_order] smallint null default 0,
	[deactivated] bit null default 0 -- 0 = false, 1 = true

);	
	
