using DUT.Application.Extensions;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;

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
                role1.PrepareToCreate();
                listRoles.Add(role1);

                var role2 = new Role(Roles.Moderator);
                role2.PrepareToCreate();
                listRoles.Add(role2);

                var role3 = new Role(Roles.Teacher);
                role3.PrepareToCreate();
                listRoles.Add(role3);

                var role4 = new Role(Roles.Student);
                role4.PrepareToCreate();
                listRoles.Add(role4);

                db.Roles.AddRange(listRoles);
                count++;
            }
            #endregion

            if (count > 0)
                db.SaveChanges();

            db.Dispose();
        }
    }
}
