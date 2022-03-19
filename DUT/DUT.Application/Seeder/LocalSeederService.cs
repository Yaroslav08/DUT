using DUT.Application.Extensions;
using DUT.Application.Helpers;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Seeder
{
    public class LocalSeederService : ISeederService
    {
        private readonly DUTDbContext _db;
        public LocalSeederService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task SeedSystemAsync()
        {
            int count = 0;

            #region Apps
            if (!await _db.Apps.AnyAsync())
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

                await _db.Apps.AddRangeAsync(listApps);
                count++;
            }
            #endregion

            #region Roles
            if (!await _db.Roles.AnyAsync())
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

                await _db.Roles.AddRangeAsync(listRoles);
                count++;
            }
            #endregion

            #region Setting
            if (!await _db.Settings.AnyAsync())
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

                await _db.Settings.AddAsync(newSetting);

                count++;
            }
            #endregion

            #region UserGroupRole
            if (!await _db.UserGroupRoles.AnyAsync())
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

                await _db.UserGroupRoles.AddRangeAsync(listUserGroupRole);
                count++;
            }
            #endregion

            if (count > 0)
                await _db.SaveChangesAsync();

            #region Users
            if (!await _db.Users.AnyAsync())
            {
                var newUser1 = new User("Ярослав", "Юрійович", "Мудрик", "yaroslav.mudryk@gmail.com", "Yarik08");
                newUser1.PasswordHash = Defaults.Password.GeneratePasswordHash();
                newUser1.LockoutEnabled = false;
                newUser1.PrepareToCreate();
                await _db.Users.AddAsync(newUser1);
                await _db.SaveChangesAsync();

                var notify = NotificationsHelper.GetWelcomeNotification();
                notify.UserId = newUser1.Id;
                await _db.Notifications.AddAsync(notify);
                await _db.SaveChangesAsync();

                var userRole = new UserRole
                {
                    UserId = newUser1.Id,
                    RoleId = (await _db.Roles.Where(x => x.Name == Roles.Admin).Select(s => new Role { Id = s.Id }).FirstOrDefaultAsync()).Id,
                };
                await _db.UserRoles.AddAsync(userRole);
                await _db.SaveChangesAsync();
            }
            #endregion

            #region Claims

            if (!await _db.Claims.AnyAsync())
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

                await _db.Claims.AddRangeAsync(claims);
                await _db.SaveChangesAsync();

                #endregion


                #region Set roles to admin

                var adminRole = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == Roles.Admin);

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

                await _db.RoleClaims.AddRangeAsync(adminRoleClaims);
                await _db.SaveChangesAsync();

                #endregion
            }

            #endregion

            #region Entities

            #region University

            int universityId = 0;

            if (!await _db.Universities.AnyAsync())
            {
                var newUniversity = new University
                {
                    Name = "Державний університет телекомунікацій",
                    NameEng = "State University of Telecommunications",
                    ShortName = "ДУТ",
                    ShortNameEng = "SUT"
                };
                newUniversity.PrepareToCreate();

                await _db.Universities.AddAsync(newUniversity);
                await _db.SaveChangesAsync();
                universityId = newUniversity.Id;
            }

            #endregion


            #region Faculties

            List<Faculty> facuties = new List<Faculty>();

            if (!await _db.Faculties.AnyAsync())
            {
                var listFaculties = new List<Faculty>();

                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-науковий інститут захисту інформації"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-Науковий Інститут Інформаційних Технологій"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-науковий інститут Телекомунікацій"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-науковий інститут менеджменту та підприємництва"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-науковий інститут заочного та дистанційного навчання"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Навчально-науковий інститут гуманітарних та природничих дисциплін"
                });
                listFaculties.Add(new Faculty
                {
                    Name = "Аспірантура"
                });

                listFaculties.ForEach(x =>
                {
                    x.UniversityId = universityId;
                    x.PrepareToCreate();
                });

                await _db.Faculties.AddRangeAsync(listFaculties.ToArray());
                await _db.SaveChangesAsync();
                facuties.AddRange(listFaculties);
            }

            #endregion


            #region Specialties

            if (!await _db.Specialties.AnyAsync())
            {
                var listSpecialties = new List<Specialty>();

                listSpecialties.Add(new Specialty
                {
                    Name = "Інженерія програмного забезпечення",
                    Code = "121"
                });
                listSpecialties.Add(new Specialty
                {
                    Name = "Комп'ютерні науки",
                    Code = "122"
                });
                listSpecialties.Add(new Specialty
                {
                    Name = "Комп'ютерна інженерія",
                    Code = "123"
                });

                var facultyInfoId = GetFacultyIdByName("Навчально-Науковий Інститут Інформаційних Технологій");

                listSpecialties.ForEach(x =>
                {
                    x.PrepareToCreate();
                    x.FacultyId = facultyInfoId;
                });

                await _db.Specialties.AddRangeAsync(listSpecialties.ToArray());
                await _db.SaveChangesAsync();
            }

            #endregion

            int GetFacultyIdByName(string name)
            {
                return facuties.FirstOrDefault(x => x.Name == name).Id;
            }

            #endregion
        }
    }
}
