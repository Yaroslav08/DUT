using DUT.Application.Extensions;
using DUT.Application.Helpers;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application
{
    public class DataSeeder
    {
        public static void SeedSystem(DUTDbContext db)
        {
            int count = 0;

            #region Apps
            if (!db.Apps.Any())
            {
                var listApps = new List<App>();

                var app1 = new App
                {
                    Name = "DUT Web Application",
                    ShortName = "DUT Web",
                    AppId = "x3Vw-QFdk-Pt0B-7jjd",
                    AppSecret = "DlST6311QjEfEoUX0JJAsjQGmHrBlUl8qjhXMCaJFJrlTiv0Fn0PcWqOPCEfsrEZuSfvMj",
                    ActiveFrom = DateTime.Now,
                    ActiveTo = DateTime.Now.AddYears(10),
                    Description = "Веб застосунок для роботи з системою СДН ДУТ",
                    IsActive = true,
                    Image = "",
                };
                app1.PrepareToCreate();
                listApps.Add(app1);

                var app2 = new App
                {
                    Name = "DUT Android Application",
                    ShortName = "DUT And",
                    AppId = "F7Gv-2mu8-rH7Z-U4pt",
                    AppSecret = "bUqNPgQhJoj8oHyIsz2cwICuxTCGo0oBLDaBODiQ0pOLnLn6H99IEJipavIwHhbEzf5Y4s",
                    ActiveFrom = DateTime.Now,
                    ActiveTo = DateTime.Now.AddYears(10),
                    Description = "Android застосунок для роботи з системою СДН ДУТ",
                    IsActive = true,
                    Image = "",
                };
                app2.PrepareToCreate();
                listApps.Add(app2);

                var app3 = new App
                {
                    Name = "DUT IOS Application",
                    ShortName = "DUT IOS",
                    AppId = "5BoG-NGOS-Ofqu-6mmS",
                    AppSecret = "WI6IygntT3a7ur39Ndv6w6bsvWYYz31PMg6HdKfKzz3Wsi8q4L5aA65hwfzZZqcxOkahf6",
                    ActiveFrom = DateTime.Now,
                    ActiveTo = DateTime.Now.AddYears(10),
                    Description = "IOS застосунок для роботи з системою СДН ДУТ",
                    IsActive = true,
                    Image = "",
                };
                app3.PrepareToCreate();
                listApps.Add(app3);

                var app4 = new App
                {
                    Name = "DUT Desktop Application",
                    ShortName = "DUT Desktop",
                    AppId = "T2Ke-RJUB-lV0u-BHpB",
                    AppSecret = "cc8Qa7fILdAmmxiZszfaxzMUrJSPZKGagVxOqvhmyWwxznGHKyNpGLBOzpOBxdoOHyxcRY",
                    ActiveFrom = DateTime.Now,
                    ActiveTo = DateTime.Now.AddYears(10),
                    Description = "Десктопний застосунок для роботи з системою СДН ДУТ",
                    IsActive = true,
                    Image = "",
                };
                app4.PrepareToCreate();
                listApps.Add(app4);

                db.Apps.AddRange(listApps);
                count++;
            }
            #endregion

            #region Roles
            if (!db.Roles.Any())
            {
                var listRoles = new List<Role>();

                var role1 = new Role(Roles.Admin);
                role1.CanDelete = false;
                role1.PrepareToCreate();
                listRoles.Add(role1);

                var role2 = new Role(Roles.Moderator);
                role2.CanDelete = false;
                role2.PrepareToCreate();
                listRoles.Add(role2);

                var role3 = new Role(Roles.Developer);
                role3.CanDelete = false;
                role3.PrepareToCreate();
                listRoles.Add(role3);

                var role4 = new Role(Roles.Teacher);
                role4.CanDelete = false;
                role4.PrepareToCreate();
                listRoles.Add(role4);

                var role5 = new Role(Roles.Student);
                role5.CanDelete = false;
                role5.PrepareToCreate();
                listRoles.Add(role5);

                db.Roles.AddRange(listRoles);
                count++;
            }
            #endregion

            #region Setting
            if (!db.Settings.Any())
            {
                var now = DateTime.Now;

                var newSetting = new Setting
                {
                    MaxCourseInUniversity = 6,
                    FirtsSemesterStart = new DateTime(now.Year, 9, 1),
                    FirtsSemesterEnd = new DateTime(now.Year, 12, 31),
                    SecondSemesterStart = new DateTime(now.Year + 1, 1, 1),
                    SecondSemesterEnd = new DateTime(now.Year + 1, 6, 30)
                };
                newSetting.PrepareToCreate();

                db.Settings.Add(newSetting);

                count++;
            }
            #endregion

            #region UserGroupRole
            if (!db.UserGroupRoles.Any())
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

                db.UserGroupRoles.AddRange(listUserGroupRole);
                count++;
            }
            #endregion

            if (count > 0)
                db.SaveChanges();

            #region Users
            if (!db.Users.Any())
            {
                var newUser1 = new User("Ярослав", "Юрійович", "Мудрик", "yaroslav.mudryk@gmail.com", "Yarik08");
                newUser1.PasswordHash = Defaults.Password.GeneratePasswordHash();
                newUser1.LockoutEnabled = false;
                newUser1.PrepareToCreate();
                db.Users.Add(newUser1);
                db.SaveChanges();

                var notify = NotificationsHelper.GetWelcomeNotification();
                notify.UserId = newUser1.Id;
                db.Notifications.Add(notify);
                db.SaveChanges();

                var userRole = new UserRole
                {
                    UserId = newUser1.Id,
                    RoleId = db.Roles.Where(x => x.Name == Roles.Admin).Select(s => new Role { Id = s.Id }).FirstOrDefault().Id,
                };
                db.UserRoles.Add(userRole);
                db.SaveChanges();
            }
            #endregion

            ConfigNewClaims(db);

            db.Dispose();
        }

        private static bool ConfigNewClaims(DUTDbContext _db)
        {
            if (!_db.Claims.Any())
            {
                var claims = new List<Claim>();

                #region Permission

                claims.Add(new Claim
                {
                    Type = PermissionClaims.Permissions,
                    Value = Permissions.All,
                    DisplayName = "Створення, перегляд, редагування, видалення, ролей та дозволів"
                });

                #endregion


                #region University

                claims.Add(new Claim
                {
                    DisplayName = "Створення університету",
                    Type = PermissionClaims.University,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування університету",
                    Type = PermissionClaims.University,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Видалення університету",
                    Type = PermissionClaims.University,
                    Value = Permissions.CanRemove
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про університет",
                    Type = PermissionClaims.University,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з університетом",
                    Type = PermissionClaims.University,
                    Value = Permissions.All
                });

                #endregion


                #region Faculty

                claims.Add(new Claim
                {
                    DisplayName = "Створення інституту",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування інституту",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Видалення інституту",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.CanRemove
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про інститут",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про всі інститути",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.CanViewAll
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з інститутом",
                    Type = PermissionClaims.Faculties,
                    Value = Permissions.All
                });

                #endregion


                #region Specialty

                claims.Add(new Claim
                {
                    DisplayName = "Створення спеціальності",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування спеціальності",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Видалення спеціальності",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.CanRemove
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про спеціальність",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про всі спеціальності",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.CanViewAll
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з спеціальністю",
                    Type = PermissionClaims.Specialties,
                    Value = Permissions.All
                });

                #endregion


                #region Group

                claims.Add(new Claim
                {
                    DisplayName = "Створення групи",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування групи",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Видалення групи",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.CanRemove
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про групу",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про всі групи",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.CanViewAll
                });
                claims.Add(new Claim
                {
                    DisplayName = "Пошук груп",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.Search
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з групами",
                    Type = PermissionClaims.Groups,
                    Value = Permissions.All
                });

                #endregion


                #region User

                claims.Add(new Claim
                {
                    DisplayName = "Створення користувача",
                    Type = PermissionClaims.Users,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування користувача",
                    Type = PermissionClaims.Users,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Видалення користувача",
                    Type = PermissionClaims.Users,
                    Value = Permissions.CanRemove
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про користувача",
                    Type = PermissionClaims.Users,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд інформації про всіх користувачів",
                    Type = PermissionClaims.Users,
                    Value = Permissions.CanViewAll
                });
                claims.Add(new Claim
                {
                    DisplayName = "Пошук користувачів",
                    Type = PermissionClaims.Users,
                    Value = Permissions.Search
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з користувачами",
                    Type = PermissionClaims.Users,
                    Value = Permissions.All
                });

                #endregion


                #region Settings

                claims.Add(new Claim
                {
                    DisplayName = "Створення налаштувань по замовченню",
                    Type = PermissionClaims.Settings,
                    Value = Permissions.CanCreate
                });
                claims.Add(new Claim
                {
                    DisplayName = "Редагування налаштувань по замовченню",
                    Type = PermissionClaims.Settings,
                    Value = Permissions.CanEdit
                });
                claims.Add(new Claim
                {
                    DisplayName = "Перегляд налаштувань по замовченню",
                    Type = PermissionClaims.Settings,
                    Value = Permissions.CanView
                });
                claims.Add(new Claim
                {
                    DisplayName = "Усі дії пов'язані з налаштуваннями по замовченню",
                    Type = PermissionClaims.Settings,
                    Value = Permissions.All
                });

                #endregion


                #region Save Claims

                claims.ForEach(x =>
                {
                    x.PrepareToCreate();
                });

                _db.Claims.AddRange(claims);
                _db.SaveChanges();

                #endregion


                #region Set roles to admin

                var adminRole = _db.Roles.AsNoTracking().FirstOrDefault(s => s.Name == Roles.Admin);


                var adminRoleClaims = new List<RoleClaim>();

                foreach (var roleClaim in claims)
                {
                    adminRoleClaims.Add(new RoleClaim
                    {
                        RoleId = adminRole.Id,
                        ClaimId = roleClaim.Id
                    });
                }

                adminRoleClaims.ForEach(x =>
                {
                    x.PrepareToCreate();
                });

                _db.RoleClaims.AddRange(adminRoleClaims);
                _db.SaveChanges();

                #endregion
            }
            return true;
        }
    }
}
