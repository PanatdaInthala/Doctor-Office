using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using DoctorOffice.Models;

namespace DoctorOffice.Controllers
{
    public class PatientController : Controller
    {

        [HttpGet("/patients")]
        public ActionResult Index()
        {
          List<Patient> allPatients = Patient.GetAll();
        return View(allPatients);
        }
        [HttpGet("/patients/new")]
        public ActionResult CreateForm()
        {
          return View();
        }
        [HttpPost("/patients")]
        public ActionResult Create()
        {
          Patient newPatient = new Patient(Request.Form["patient-name"], Request.Form["patient-birthdate"]);
          newPatient.Save();
          return RedirectToAction("Success", "Home");
        }
    }
}
