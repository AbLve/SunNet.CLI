--Clean up and init users Data
use [cliengage-db-dev]
truncate table [ApplicantContacts]
truncate table [ApplicantEmails]
truncate table [Applicants]
truncate table [dbo].[Auditors]
truncate table [dbo].classes
truncate table [dbo].classrooms
truncate table [dbo].Communities
truncate table [dbo].[CommunitySpecialists]
truncate table [CommunityUsers]
update BasicCommunities set Status = 1
truncate table [dbo].[CoordCoachEquipments]
truncate table [dbo].[CoordCoachs]
truncate table [Permission_AssignedPackages]
--select * from [dbo].[Permission_Authority]  ----???????????????????chinese!!!
update [Permission_Authority] set Descriptions= Authority
truncate table [dbo].Permission_UserRole
truncate table [dbo].[Permission_UserAuthorities]
truncate table [dbo].[Principals]
truncate table [dbo].schools
truncate table [dbo].[SchoolSpecialists]
truncate table [dbo].[StateWides]
truncate table [dbo].[StudentClassRelations]
truncate table [dbo].[Students]
truncate table [dbo].[TeacherClassRelations]
truncate table [dbo].[TeacherEquiementRelations]
truncate table [dbo].[Teachers]
truncate table [dbo].[TeacherTransactions]
truncate table [dbo].[UserCertificateRelations]
truncate table [dbo].[UserCommunityRelations]
truncate table [dbo].[UserPDRelations]
truncate table [dbo].[VideoCodings]
truncate table [dbo].[Users]  


--init users
GO


INSERT [dbo].[Users] ( [Role], [GoogleId], [FirstName], [MiddleName], [LastName], [PreviousLastName], [Status], [StatusDate], [PrimaryEmailAddress], [SecondaryEmailAddress], [PrimaryPhoneNumber], [PrimaryNumberType], [SecondaryPhoneNumber], [SecondaryNumberType], [FaxNumber], [CreatedOn], [UpdatedOn], [IsDeleted]) VALUES (1, N'clisunnet@gmail.com', N'TestOnly', N'', N'No Access Manager', N'', 1, CAST(0x0000A39301089084 AS DateTime), N'team@sunnet.us', N'', N'(111)111-1111', 1, N'', 0, N'', CAST(0x0000A3A600F3922A AS DateTime), CAST(0x0000A3A600F3922A AS DateTime), 0)
INSERT [dbo].[Users] ( [Role], [GoogleId], [FirstName], [MiddleName], [LastName], [PreviousLastName], [Status], [StatusDate], [PrimaryEmailAddress], [SecondaryEmailAddress], [PrimaryPhoneNumber], [PrimaryNumberType], [SecondaryPhoneNumber], [SecondaryNumberType], [FaxNumber], [CreatedOn], [UpdatedOn], [IsDeleted]) VALUES (1, N'whuang3', N'David', N'', N'Huang', N'', 1, CAST(0x0000A3A4016FE540 AS DateTime), N'david@sunnet.us', N'', N'(111)111-1111', 1, N'', 0, N'', CAST(0x0000A3A600F53DE4 AS DateTime), CAST(0x0000A3A600F53DE4 AS DateTime), 0)

--Init Permission_UserRole
insert into Permission_UserRole(UserId,RoleId)
select a.ID as 'UserId',b.ID as 'RoleId' from Users a
inner join 
Permission_Roles b 
on a.Role=b.UserType
and b.IsDefault=1


--Final init [Permission_UserAuthorities]
---http://mainsite/Permission/page/AddUserAuthority


--Or init [Permission_UserAuthorities] 
GO
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 1, 128, CAST(0x0000A3AB007B01BE AS DateTime), CAST(0x0000A3AB007B01BE AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 2, 128, CAST(0x0000A3AB007B01BE AS DateTime), CAST(0x0000A3AB007B01BE AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 3, 128, CAST(0x0000A3AB007B01BE AS DateTime), CAST(0x0000A3AB007B01BE AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 4, 1, CAST(0x0000A3AB007B01BE AS DateTime), CAST(0x0000A3AB007B01BE AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 5, 39, CAST(0x0000A3AB007B01BE AS DateTime), CAST(0x0000A3AB007B01BE AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 6, 39, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 7, 39, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 8, 295, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 9, 551, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 11, 128, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 10, 13, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 14, 1031, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 15, 1031, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 16, 1031, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 17, 1031, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 18, 14343, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 19, 7, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 20, 7, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 12, 8271, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (1, 13, 17423, CAST(0x0000A3AB007B01C2 AS DateTime), CAST(0x0000A3AB007B01C2 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 1, 128, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 2, 128, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 3, 128, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 4, 1, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 5, 39, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 6, 39, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 7, 39, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 8, 295, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 9, 551, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 11, 128, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 10, 13, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 14, 1031, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 15, 1031, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 16, 1031, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 17, 1031, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 18, 14343, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 19, 7, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 20, 7, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 12, 8271, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))
INSERT [dbo].[Permission_UserAuthorities] ([UserId], [PageId], [Authority], [CreatedOn], [UpdatedOn]) VALUES (2, 13, 17423, CAST(0x0000A3AB007B0229 AS DateTime), CAST(0x0000A3AB007B0229 AS DateTime))

