using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Extensions.Password;
using URLS.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Helpers;

namespace URLS.Application.Seeder
{
    public abstract class BaseSeederService : ISeederService
    {
        protected readonly URLSDbContext _db;

        public BaseSeederService(URLSDbContext db)
        {
            _db = db;
        }

        public virtual async Task SeedSystemAsync()
        {
            if (!await _db.Apps.AnyAsync())
            {
                var apps = GetApps();
                await _db.Apps.AddRangeAsync(apps);
                await _db.SaveChangesAsync();
            }

            if (!await _db.Settings.AnyAsync())
            {
                var setting = GetSetting();
                await _db.Settings.AddAsync(setting);
                await _db.SaveChangesAsync();
            }

            if (!await _db.UserGroupRoles.AnyAsync())
            {
                var userGroupRoles = GetUserGroupRoles();
                await _db.UserGroupRoles.AddRangeAsync(userGroupRoles);
                await _db.SaveChangesAsync();
            }

            if (!await _db.Roles.AnyAsync())
            {
                var roles = GetRoles();
                await _db.Roles.AddRangeAsync(roles);
                await _db.SaveChangesAsync();
            }

            if (!await _db.Users.AnyAsync())
            {
                var admin = GetAdmin();
                await _db.Users.AddAsync(admin);
                await _db.SaveChangesAsync();
                await SetupUserRole(admin);
            }
        }

        private User GetAdmin()
        {
            var admin = new User("Адмін", "Адмінович", "Адміненко", "admin@mail.com", Generator.GetUsername());
            admin.PasswordHash = Defaults.Password.GeneratePasswordHash();
            admin.LockoutEnabled = false;
            admin.AccessFailedCount = 0;
            admin.JoinAt = DateTime.Now;
            admin.NotificationSettings = new NotificationSettings
            {
                AcceptedInGroup = true,
                ChangePassword = true,
                NewLogin = true,
                Logout = true,
                NewPost = true,
                Welcome = true
            };
            admin.PrepareToCreate();
            return admin;
        }

        private List<Role> GetRoles()
        {
            var listRoles = new List<Role>();

            var role1 = new Role(Roles.Admin);
            role1.Label = Roles.AdminUa;
            role1.Color = Roles.AdminColor;
            role1.CanDelete = false;
            role1.PrepareToCreate();
            listRoles.Add(role1);

            var role2 = new Role(Roles.Moderator);
            role2.Label = Roles.ModeratorUa;
            role2.Color = Roles.ModeratorColor;
            role2.CanDelete = false;
            role2.PrepareToCreate();
            listRoles.Add(role2);

            var role3 = new Role(Roles.Developer);
            role3.Label = Roles.DeveloperUa;
            role3.Color = Roles.DeveloperColor;
            role3.CanDelete = false;
            role3.PrepareToCreate();
            listRoles.Add(role3);

            var role4 = new Role(Roles.Teacher);
            role4.Label = Roles.TeacherUa;
            role4.Color = Roles.TeacherColor;
            role4.CanDelete = false;
            role4.PrepareToCreate();
            listRoles.Add(role4);

            var role5 = new Role(Roles.Student);
            role5.Label = Roles.StudentUa;
            role5.Color = Roles.StudentColor;
            role5.CanDelete = false;
            role5.PrepareToCreate();
            listRoles.Add(role5);

            return listRoles;
        }

        private Setting GetSetting()
        {
            var now = DateTime.Now;

            var newSetting = new Setting
            {
                MaxCourseInUniversity = 6,
                FirtsSemesterStart = new DateTime(now.Year, 9, 1),
                FirtsSemesterEnd = new DateTime(now.Year, 12, 31),
                SecondSemesterStart = new DateTime(now.Year + 1, 1, 1),
                SecondSemesterEnd = new DateTime(now.Year + 1, 6, 30),
                Holidays = new List<Holiday>()
                    {
                        new Holiday { Id = 1, Name = "Новий Рік", Date = new DateTime(2022, 1, 1) },
                        new Holiday { Id = 2, Name = "Різдво Христове", Date = new DateTime(2022, 1, 7) },
                        new Holiday { Id = 3, Name = "Міжнародний жіночий день", Date = new DateTime(2022, 3, 8) },
                        new Holiday { Id = 4, Name = "Великдень", Date = new DateTime(2022, 4, 24) },
                        new Holiday { Id = 5, Name = "День праці", Date = new DateTime(2022, 5, 1) },
                        new Holiday { Id = 6, Name = "День пам'яті", Date = new DateTime(2022, 5, 9) },
                        new Holiday { Id = 7, Name = "Трійця", Date = new DateTime(2022, 6,12) },
                        new Holiday { Id = 8, Name = "День Конституції України", Date = new DateTime(2022, 6, 28) },
                        new Holiday { Id = 9, Name = "День Незалежності України", Date = new DateTime(2022, 8, 24) },
                        new Holiday { Id = 10, Name = "День Захисники України", Date = new DateTime(2022, 10, 14) },
                        new Holiday { Id = 11, Name = "Різдво Христове за Григоріанським календарем", Date = new DateTime(2022, 12, 25) },

                    }.AsEnumerable(),
                LessonTimes = new List<LessonTime>
                    {
                        new LessonTime
                        {
                            Id = 1,
                            Start = new DateTime(2022, 01, 01, 8,0, 0),
                            End = new DateTime(2022, 01, 01, 9, 35, 0),
                            Description = "Перша зміна",
                            Number = 1.ToString()
                        },
                        new LessonTime
                        {
                            Id = 2,
                            Start = new DateTime(2022, 01, 01, 9,45,0),
                            End = new DateTime(2022, 01, 01, 11, 20, 0),
                            Description = "Перша зміна",
                            Number = 2.ToString()
                        },
                        new LessonTime
                        {
                            Id = 3,
                            Start = new DateTime(2022, 01, 01, 11,45, 0),
                            End = new DateTime(2022, 01, 01, 13, 20, 0),
                            Description = "Перша зміна",
                            Number = 3.ToString()
                        },
                        new LessonTime
                        {
                            Id = 4,
                            Start = new DateTime(2022, 01, 01, 13,30,0),
                            End = new DateTime(2022, 01, 01, 15, 05, 0),
                            Description = "Друга зміна",
                            Number = 4.ToString()
                        },
                        new LessonTime
                        {
                            Id = 5,
                            Start = new DateTime(2022, 01, 01, 15,15, 0),
                            End = new DateTime(2022, 01, 01, 16, 50, 0),
                            Description = "Друга зміна",
                            Number = 5.ToString()
                        },
                        new LessonTime
                        {
                            Id = 6,
                            Start = new DateTime(2022, 01, 01, 17,0, 0),
                            End = new DateTime(2022, 01, 01, 18, 35, 0),
                            Description = "Друга зміна",
                            Number = 6.ToString()
                        }
                    }.AsEnumerable()
            };
            newSetting.PrepareToCreate();

            return newSetting;
        }

        private List<App> GetApps()
        {
            var listApps = new List<App>();

            var app1 = new App
            {
                Name = "Веб застосунок",
                ShortName = "Web",
                AppId = "x3Vw-QFdk-Pt0B-7jjd",
                AppSecret = "DlST6311QjEfEoUX0JJAsjQGmHrBlUl8qjhXMCaJFJrlTiv0Fn0PcWqOPCEfsrEZuSfvMj",
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddYears(10),
                Description = "Веб застосунок для роботи з системою УСДН",
                IsActive = true,
                Image = ""
            };
            app1.PrepareToCreate();
            listApps.Add(app1);

            var app2 = new App
            {
                Name = "Android застосунок",
                ShortName = "Android",
                AppId = "F7Gv-2mu8-rH7Z-U4pt",
                AppSecret = "bUqNPgQhJoj8oHyIsz2cwICuxTCGo0oBLDaBODiQ0pOLnLn6H99IEJipavIwHhbEzf5Y4s",
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddYears(10),
                Description = "Android застосунок для роботи з системою УСДН",
                IsActive = true,
                Image = ""
            };
            app2.PrepareToCreate();
            listApps.Add(app2);

            var app3 = new App
            {
                Name = "IOS застосунок",
                ShortName = "IOS",
                AppId = "5BoG-NGOS-Ofqu-6mmS",
                AppSecret = "WI6IygntT3a7ur39Ndv6w6bsvWYYz31PMg6HdKfKzz3Wsi8q4L5aA65hwfzZZqcxOkahf6",
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddYears(10),
                Description = "IOS застосунок для роботи з системою УСДН",
                IsActive = true,
                Image = ""
            };
            app3.PrepareToCreate();
            listApps.Add(app3);

            var app4 = new App
            {
                Name = "Desktop застосунок",
                ShortName = "Desktop",
                AppId = "T2Ke-RJUB-lV0u-BHpB",
                AppSecret = "cc8Qa7fILdAmmxiZszfaxzMUrJSPZKGagVxOqvhmyWwxznGHKyNpGLBOzpOBxdoOHyxcRY",
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddYears(10),
                Description = "Desktop застосунок для роботи з системою УСДН",
                IsActive = true,
                Image = ""
            };
            app4.PrepareToCreate();
            listApps.Add(app4);

            return listApps;
        }

        private List<UserGroupRole> GetUserGroupRoles()
        {
            var listUserGroupRole = new List<UserGroupRole>();

            var groupRole1 = new UserGroupRole
            {
                Name = UserGroupRoles.Names.ClassTeacher,
                NameEng = UserGroupRoles.Names.ClassTeacherEng,
                Description = UserGroupRoles.Descriptions.ClassTeacherDes,
                DescriptionEng = UserGroupRoles.Descriptions.ClassTeacherEngDes,
                Color = UserGroupRoles.Colors.Red,
                Permissions = new UserGroupPermission
                {
                    CanCreateInviteCode = true,
                    CanRemoveInviteCode = true,
                    CanWriteComment = true,
                    CanUpdateImage = true,
                    CanRemovePost = true,
                    CanRemoveComment = true,
                    CanCreatePost = true,
                    CanEditAllPosts = true,
                    CanEditPost = true,
                    CanOpenCloseComment = true,
                    CanRemoveAllComments = true,
                    CanRemoveAllPosts = true,
                    CanUpdateInviteCode = true
                },
                CanEdit = false,
                UniqId = UserGroupRoles.UniqIds.ClassTeacher
            };
            groupRole1.PrepareToCreate();
            listUserGroupRole.Add(groupRole1);

            var groupRole2 = new UserGroupRole
            {
                Name = UserGroupRoles.Names.Headmaster,
                NameEng = UserGroupRoles.Names.HeadmasterEng,
                Description = UserGroupRoles.Descriptions.HeadmasterDes,
                DescriptionEng = UserGroupRoles.Descriptions.HeadmasterEngDes,
                Color = UserGroupRoles.Colors.Green,
                Permissions = new UserGroupPermission
                {
                    CanCreateInviteCode = true,
                    CanRemoveInviteCode = true,
                    CanWriteComment = true,
                    CanUpdateImage = true,
                    CanRemovePost = true,
                    CanRemoveComment = true,
                    CanCreatePost = true,
                    CanEditAllPosts = false,
                    CanEditPost = true,
                    CanOpenCloseComment = true,
                    CanRemoveAllComments = false,
                    CanRemoveAllPosts = false,
                    CanUpdateInviteCode = true
                },
                CanEdit = false,
                UniqId = UserGroupRoles.UniqIds.Headmaster
            };
            groupRole2.PrepareToCreate();
            listUserGroupRole.Add(groupRole2);

            var groupRole3 = new UserGroupRole
            {
                Name = UserGroupRoles.Names.Student,
                NameEng = UserGroupRoles.Names.StudentEng,
                Description = UserGroupRoles.Descriptions.StudentDes,
                DescriptionEng = UserGroupRoles.Descriptions.StudentEngDes,
                Color = UserGroupRoles.Colors.Blue,
                Permissions = new UserGroupPermission
                {
                    CanCreateInviteCode = false,
                    CanRemoveInviteCode = false,
                    CanWriteComment = true,
                    CanUpdateImage = false,
                    CanRemovePost = false,
                    CanRemoveComment = true,
                    CanCreatePost = false,
                    CanEditAllPosts = false,
                    CanEditPost = false,
                    CanOpenCloseComment = false,
                    CanRemoveAllComments = false,
                    CanRemoveAllPosts = false,
                    CanUpdateInviteCode = false
                },
                CanEdit = false,
                UniqId = UserGroupRoles.UniqIds.Student
            };
            groupRole3.PrepareToCreate();
            listUserGroupRole.Add(groupRole3);

            return listUserGroupRole;
        }

        private async Task SetupUserRole(User user)
        {
            var welcomeNotification = NotificationsHelper.GetWelcomeNotification();
            welcomeNotification.UserId = user.Id;
            welcomeNotification.PrepareToCreate();


            var adminRole = await _db.Roles.AsNoTracking().Select(s => new Role { Id = s.Id }).FirstOrDefaultAsync(s => s.Name == Roles.Admin);

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            };
            userRole.PrepareToCreate();

            await _db.Notifications.AddAsync(welcomeNotification);
            await _db.UserRoles.AddAsync(userRole);
            await _db.SaveChangesAsync();
        }
    }
}