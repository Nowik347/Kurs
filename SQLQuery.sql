CREATE DATABASE RepairShop;
GO
USE RepairShop;
GO
CREATE TABLE [dbo].[Roles] 
(
    [ROLE_ID]   INT            NOT NULL,
    [ROLE_NAME] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([ROLE_ID] ASC)
);
GO
INSERT INTO Roles VALUES (N'0', N'Оператор'), (N'1', N'Пользователь'), (N'2', N'Администратор');
GO
CREATE TABLE [dbo].[States] 
(
    [STATUS_ID]   INT           NOT NULL,
    [STATUS_NAME] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([STATUS_ID] ASC)
);
GO
INSERT INTO States VALUES (N'0', N'Открыта'), (N'1', N'В работе'), (N'2', N'Выполнена'), (N'3', N'Закрыта');
GO
CREATE TABLE [dbo].[Users] 
(
    [USER_ID]  INT            IDENTITY (1, 1) NOT NULL,
    [NAME]     NVARCHAR (255) NOT NULL,
    [SURNAME]  NVARCHAR (255) NULL,
    [EMAIL]    NVARCHAR (255) NOT NULL,
    [LOGIN]    NVARCHAR (255) NOT NULL,
    [PASSWORD] NVARCHAR (255) NOT NULL,
    [ROLE]     INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([USER_ID] ASC),
    CONSTRAINT [FK_Users_ToRoles] FOREIGN KEY ([ROLE]) REFERENCES [dbo].[Roles] ([ROLE_ID])
);
GO
INSERT INTO Users VALUES (N'Админ', null, N'admin@mail.ru', N'Admin', N'admin', 2);
GO
CREATE TABLE [dbo].[Requests] 
(
    [REQUEST_ID]          INT            IDENTITY (1, 1) NOT NULL,
    [USER_ID]             INT            NOT NULL,
    [CREATION_DATE]       DATE           NOT NULL,
    [LAST_CHANGE_DATE]    DATE           NULL,
    [TROUBLE_DEVICES]     NVARCHAR (255) NOT NULL,
    [PROBLEM_DESCRIPTION] NVARCHAR (255) NOT NULL,
    [STATUS]              INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([REQUEST_ID] ASC),
    CONSTRAINT [FK_Requests_ToUsers] FOREIGN KEY ([USER_ID]) REFERENCES [dbo].[Users] ([USER_ID]),
    CONSTRAINT [FK_Requests_ToStates] FOREIGN KEY ([STATUS]) REFERENCES [dbo].[States] ([STATUS_ID])
);