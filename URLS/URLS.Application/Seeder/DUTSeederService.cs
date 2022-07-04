using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Seeder
{
    public class DUTSeederService : BaseSeederService
    {
        public DUTSeederService(URLSDbContext db) : base(db) { }

        public override async Task SeedSystemAsync()
        {
            await base.SeedSystemAsync();

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
                    Code = "121",
                    Invite = Generator.CreateGroupInviteCode()
                });
                listSpecialties.Add(new Specialty
                {
                    Name = "Комп'ютерні науки",
                    Code = "122",
                    Invite = Generator.CreateGroupInviteCode()
                });
                listSpecialties.Add(new Specialty
                {
                    Name = "Комп'ютерна інженерія",
                    Code = "123",
                    Invite = Generator.CreateGroupInviteCode()
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

        }
    }
}