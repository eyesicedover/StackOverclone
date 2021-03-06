﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StackOverflow.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace StackOverflow.Controllers
{
   
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController (UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            //var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var currentUser = await _userManager.FindByIdAsync(userId);
            //return View(_db.Questions.Where(x => x.User.Id == currentUser.Id));

            return View(_db.Questions.Include(question => question.Answers));
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            question.User = currentUser;
            _db.Questions.Add(question);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

     
        public IActionResult Details(int id)
        {
            //var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var currentUser = await _userManager.FindByIdAsync(userId);
            ViewBag.UserId = null;
            var data = _db.Questions.Include(question => question.Answers).Include(question => question.User).FirstOrDefault(x => x.QuestionId == id);
            data.Answers = data.Answers.OrderByDescending(x => x.Best).ThenByDescending(x => x.VoteTally).ToList();
            return View("Details", data);
        }

        [Authorize]
        public async Task<IActionResult> DetailsAuthenticated(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            ViewBag.UserId = userId;
            var data = _db.Questions.Include(question => question.Answers).Include(question => question.User).FirstOrDefault(x => x.QuestionId == id);
            data.Answers = data.Answers.OrderByDescending(x => x.Best).ThenByDescending(x => x.VoteTally).ToList();
            return View("Details", data);
        }
    }
}
