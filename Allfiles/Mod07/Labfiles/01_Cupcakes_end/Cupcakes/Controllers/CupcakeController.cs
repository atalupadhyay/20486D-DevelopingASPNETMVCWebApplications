﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Cupcakes.Models;
using Cupcakes.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cupcakes.Controllers
{
    public class CupcakeController : Controller
    {
        private IHostingEnvironment _environment;
        private ICupcakeRepository _repository;

        public CupcakeController(ICupcakeRepository repository, IHostingEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View(_repository.GetCupcakes());
        }

        public IActionResult Details(int id)
        {
            var cupcake = _repository.GetCupcakeById(id);
            if (cupcake == null)
            {
                return NotFound();
            }
            return View(cupcake);
        }
		
        [HttpGet, ActionName("Create")]
        public IActionResult Create()
        {
            PopulateBakeriesDropDownList();
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CupcakeId,BakeryId,CupcakeType,Description,GlutenFree,Price,PhotoAvatar")] Cupcake cupcake)
        {
            if (ModelState.IsValid)
            {
                _repository.CreateCupcake(cupcake);
                return RedirectToAction(nameof(Index));
            }
            PopulateBakeriesDropDownList(cupcake.BakeryId);
            return View(cupcake);
        }

        [HttpGet, ActionName("Edit")]
        public IActionResult Edit(int id)
        {
            Cupcake cupcake = _repository.GetCupcakeById(id);
            if (cupcake == null)
            {
                return NotFound();
            }
            PopulateBakeriesDropDownList(cupcake.BakeryId);
            return View(cupcake);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            var cupcakeToUpdate = _repository.GetCupcakeById(id);
            if (await TryUpdateModelAsync<Cupcake>(
                cupcakeToUpdate,
                "",
                c => c.BakeryId, c => c.CupcakeType, c => c.Description, c => c.GlutenFree, c => c.Price))
            {
                _repository.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            PopulateBakeriesDropDownList(cupcakeToUpdate.BakeryId);
            return View(cupcakeToUpdate);
        }

        [HttpGet, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            var cupcake = _repository.GetCupcakeById(id);
            if (cupcake == null)
            {
                return NotFound();
            }
            return View(cupcake);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteCupcake(id);
            return RedirectToAction(nameof(Index));
        }
		
		
		private void PopulateBakeriesDropDownList(object selectedbakery = null)
        {
            var bakeries = _repository.PopulateBakeriesDropDownList();
            ViewBag.BakeryID = new SelectList(bakeries.AsNoTracking(), "BakeryId", "BakeryName", selectedbakery);
        }
		
        public IActionResult GetImage(int id)
        {
            Cupcake requestedcupcake = _repository.GetCupcakeById(id);
            if (requestedcupcake != null)
            {
                string webRootpath = _environment.WebRootPath;
                string folderPath = "\\images\\";
                string fullPath = webRootpath + folderPath + requestedcupcake.ImageName;
                if (System.IO.File.Exists(fullPath))
                {
                    FileStream fileOnDisk = new FileStream(fullPath, FileMode.Open);
                    byte[] fileBytes;
                    using (BinaryReader br = new BinaryReader(fileOnDisk))
                    {
                        fileBytes = br.ReadBytes((int)fileOnDisk.Length);
                    }
                    return File(fileBytes, requestedcupcake.ImageMimeType);
                }
                else
                {
                    if (requestedcupcake.PhotoFile.Length > 0)
                    {
                        return File(requestedcupcake.PhotoFile, requestedcupcake.ImageMimeType);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}