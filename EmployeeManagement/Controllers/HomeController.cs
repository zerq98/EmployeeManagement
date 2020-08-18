using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataProtector _protector;

        public HomeController(IEmployeeRepository employeeRepository,
                              IWebHostEnvironment environment,
                              ILogger<HomeController> logger,
                              IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            _environment = environment;
            _logger = logger;
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllEmployees()
                                           .Select(e =>
                                           {
                                                e.EncryptedId = _protector.Protect(e.Id.ToString());
                                                return e;
                                           }));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(string id)
        {
            int employeeId = Convert.ToInt32(_protector.Unprotect(id));

            var model = _employeeRepository.GetEmployee(employeeId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                newEmployee = _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                ExistingPhotoPath = model.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        var path = Path.Combine(_environment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(path);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}