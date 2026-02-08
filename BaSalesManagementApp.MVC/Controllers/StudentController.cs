using BaSalesManagementApp.Dtos.StudentDTOs;
using BaSalesManagementApp.MVC.Models.StudentVMs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public StudentController(IStudentService studentService, IStringLocalizer<Resource> stringLocalizer)
        {
            _studentService = studentService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _studentService.GetAllAsync();

            var studentListVMs = result.Data.Adapt<List<StudentListVM>>();

            var studentOptions = new List<SelectListItem>();
            foreach (var student in studentListVMs)
            {
                studentOptions.Add(new SelectListItem
                {
                    Value = student.Id.ToString(),
                    Text = student.Name
                });
            }

            ViewBag.StudentOptions = studentOptions;

            if (result.IsSuccess)
            {
                TempData["Message"] = _stringLocalizer[Messages.STUDENT_LISTED_SUCCESS];
            }
            else
            {
                TempData["Message"] = result.Message;
            }
            //Console.WriteLine(result.Message);
            //TempData["Message"] = result.Message;
            return View(studentListVMs);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateVM studentCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(studentCreateVM);
            }

            var result = await _studentService.AddAsync(studentCreateVM.Adapt<StudentCreateDTO>());
            if (!result.IsSuccess)
            {
                TempData["Message"] = result.Message;
                // Console.WriteLine(result.Message);
                return View(studentCreateVM);
            }
            TempData["Message"] = _stringLocalizer[Messages.STUDENT_ADDED_SUCCESS];
            //Console.WriteLine(result.Message);

            //TempData["Message"] = result.Message;

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(Guid studentId)
        {
            var result = await _studentService.DeleteAsync(studentId);
            //if (!result.IsSuccess)
            //{
            //    await Console.Out.WriteLineAsync(result.Message);

            //}
            if (!result.IsSuccess)
            {
                TempData["Message"] = result.Message;
            }
            else
            {
                TempData["Message"] = _stringLocalizer[Messages.STUDENT_DELETED_SUCCESS];
            }

            await Console.Out.WriteLineAsync(result.Message);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
