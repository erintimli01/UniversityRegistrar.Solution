using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;
using System.Collections.Generic;
using System.Linq;

namespace UniversityRegistrar.Controllers
{
  public class CoursesController : Controller
  {
    private readonly UniversityRegistrarContext _db;

    public CoursesController(UniversityRegistrarContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Course> model = _db.Courses
                              .Include(course => course.Student)
                              .ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "CourseName");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Course course)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "CourseName");
        return View(course);
      }
      else
      {
        _db.Courses.Add(course);
        _db.SaveChanges();
        return RedirectToAction("Index");
      }
    }

    public ActionResult Details(int id)
    {
      Course thisCourse = _db.Courses
        .Include(course => course.JoinStudent)
        .ThenInclude(join => join.Student)
        .Include(course => course.JoinDepartment)
        .ThenInclude(join => join.Department)
        .FirstOrDefault(course => course.CourseId == id);
      return View(thisCourse);
    }

    public ActionResult Edit(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "StudentName");
      return View(thisCourse);
    }

    public ActionResult IsComplete(int id, bool isComplete)
    {
      Course course = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      if (course == null)
      {
        return NotFound();
      }
      course.IsComplete = isComplete;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult Edit(Course course)
    {
      _db.Courses.Update(course);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      return View(thisCourse);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      _db.Courses.Remove(thisCourse);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddDepartment(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(courses => courses.CourseId == id);
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "CourseName");
      return View(thisCourse);
    }

    [HttpPost]
    public ActionResult AddDepartment(Course course, int departmentId)
    {
#nullable enable
      CourseDepartment? joinEntity = _db.CourseDepartments.FirstOrDefault(join => (join.DepartmentId == departmentId && join.CourseId == course.CourseId));
#nullable disable
      if (joinEntity == null && departmentId != 0)
      {
        _db.CourseDepartments.Add(new CourseDepartment() { DepartmentId = departmentId, CourseId = course.CourseId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = course.CourseId });
    }

    public ActionResult AddStudent(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(courses => courses.CourseId == id);
      ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "StudentName");
      return View(thisCourse);
    }

    [HttpPost]
    public ActionResult AddStudent(Course course, int studentId)
    {
      #nullable enable
      StudentCourse? joinEntity = _db.StudentCourses.FirstOrDefault(join => (join.StudentId == studentId && join.CourseId == course.CourseId));
      #nullable disable
      if (joinEntity == null && studentId != 0)
      {
        _db.StudentCourses.Add(new StudentCourse() { StudentId = studentId, CourseId = course.CourseId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = course.CourseId });
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      CourseDepartment joinEntry = _db.CourseDepartments.FirstOrDefault(entry => entry.CourseDepartmentId == joinId);
      _db.CourseDepartments.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}