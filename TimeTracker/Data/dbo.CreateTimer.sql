-- Script Date: 12/23/2017 9:30 AM  - ErikEJ.SqlCeScripting version 3.5.2.74
DROP TABLE IF EXISTS [Timer];
CREATE TABLE [Timer] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT
, [Name] TEXT NULL UNIQUE
, [Elapsed] TEXT DEFAULT "00:00:00" NULL
);

INSERT INTO [Timer] VALUES(null,"One","00:01:00");
INSERT INTO [Timer] VALUES(null,"Two","00:02:00");
INSERT INTO [Timer] VALUES(null,"Three","00:03:00");
INSERT INTO [Timer] VALUES(null,"Four","00:04:00");
INSERT INTO [Timer] VALUES(null,"Five","00:05:00");
INSERT INTO [Timer] VALUES(null,"Six","00:06:00");
INSERT INTO [Timer] VALUES(null,"Seven","00:07:00");
