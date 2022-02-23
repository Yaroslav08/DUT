using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Lesson;
using DUT.Application.ViewModels.Subject;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SubjectsController : ApiBaseController
    {
        private readonly ISubjectService _subjectService;
        private readonly ILessonService _lessonService;
        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateModel model)
        {
            return JsonResult(await _subjectService.CreateSubjectAsync(model));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects(int offset = 0, int count = 20)
        {
            return JsonResult(await _subjectService.SearchSubjectsAsync(new SearchSubjectOptions
            {
                Offset = offset,
                Count = count
            }));
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplateSubjects(int offset, int count)
        {
            return JsonResult(await _subjectService.SearchSubjectsAsync(new SearchSubjectOptions
            {
                IsTemplate = true,
                Offset = offset,
                Count = count
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return JsonResult(await _subjectService.GetSubjectByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectEditModel model)
        {
            model.Id = id;
            return JsonResult(await _subjectService.UpdateSubjectAsync(model));
        }


        #region Lessons

        [HttpGet("{subjectId}/lessons")]
        public async Task<IActionResult> GetSubjectLessons(int subjectId, DateTime? from = null, DateTime? to = null)
        {
            return JsonResult(await _lessonService.GetLessonsBySubjectIdAsync(subjectId, from, to));
        }

        [HttpGet("{subjectId}/lessons/{lessonId}")]
        public async Task<IActionResult> GetLessonById(int subjectId, long lessonId)
        {
            return JsonResult(await _lessonService.GetLessonByIdAsync(lessonId));
        }

        [HttpPost("{subjectId}/lessons")]
        public async Task<IActionResult> CreateLesson(int subjectId, [FromBody] LessonCreateModel model)
        {
            model.SubjectId = subjectId;
            return JsonResult(await _lessonService.CreateLessonAsync(model));
        }

        [HttpPut("{subjectId}/lessons/{lessonId}")]
        public async Task<IActionResult> UpdateLesson(int subjectId, long lessonId, [FromBody]LessonEditModel model)
        {
            model.Id = lessonId;
            model.SubjectId = subjectId;
            return JsonResult(await _lessonService.UpdateLessonAsync(model));
        }

        [HttpDelete("{subjectId}/lessons/{lessonId}")]
        public async Task<IActionResult> RemoveLesson(int subjectId, long lessonId)
        {
            return JsonResult(await _lessonService.RemoveLessonAsync(lessonId));
        }

        #endregion
    }
}
