using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boutique.Models;

namespace Boutique.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Staff
        public ActionResult Index()
        {
            ViewBag.Job_title = new SelectList(_db.Job_title, "Id", "Name");
            return View();
        }
        public ActionResult GetData()
        {
            var result = _db.Staffs
                .Include(s => s.Job_title1).Select(s => new {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Password = s.Password,
                    Avatar = s.Avatar,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    CMT = s.CMT,
                    Phone = s.Phone,
                    Job_title1 = s.Job_title1 != null ? s.Job_title1.Name : ""
                })
                .ToList();
            return Json(new { Data = result, TotalItems = result.Count }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Create(Staff model)
        {
            if(model.Id != 0)
            {
                var staff = _db.Staffs.Find(model.Id);
                staff.FullName = model.FullName;
                staff.Email = model.Email;
                staff.Password = model.Password;
                staff.Address = model.Address;
                if(model.DateOfBirth != null)
                {
                    staff.DateOfBirth = model.DateOfBirth;
                }
                staff.CMT = model.CMT;
                staff.Phone = model.Phone;
                staff.Job_title = model.Job_title;
                _db.Entry(staff).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
               
                    _db.Staffs.Add(model);
                    try
                    {
                        _db.SaveChanges();
                        return Json(new { success = true });

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                        return Json(new { success = false, errors = errors });
                    }
            }
          
        }
        [HttpGet]
        public ActionResult GetById(int Id)
        {
            var s = _db.Staffs.Find(Id);
            var staff = new
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                Password = s.Password,
                Avatar = s.Avatar,
                Address = s.Address,
                DateOfBirth = s.DateOfBirth,
                CMT = s.CMT,
                Phone = s.Phone,
                Job_title = s.Job_title != null ? s.Job_title : 0
            };
            return Json(new { data = staff }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(Staff request)
        {
            var staff = _db.Staffs.Find(request.Id);
            _db.Entry(staff).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var staff = _db.Staffs.Find(Id);
            _db.Staffs.Remove(staff);
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}