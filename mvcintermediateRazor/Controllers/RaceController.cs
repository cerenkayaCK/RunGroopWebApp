using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;

namespace mvcintermediateRazor.Controllers
{

    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _photoService = photoService;
            _raceRepository = raceRepository;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);

                var race = new Race()
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State
                    }
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(raceVM);


        }

        public async Task<IActionResult> Edit(int id)
        {

            var race = await _raceRepository.GetByIdAsync(id);
            var raceViewModel = new EditRaceViewModel()
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory

            };


            return View(raceViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club.");
                return View("Error", raceViewModel);
            }

            var RaceNoTrancking = await _raceRepository.GetByIdAsyncNoTracking(id);

            if (RaceNoTrancking != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(RaceNoTrancking.Image);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "could not delete photo");
                    return View(raceViewModel);

                }

                var photoResult = await _photoService.AddPhotoAsync(raceViewModel.Image);
                var race = new Race
                {
                    Id = id,
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = raceViewModel.AddressId,
                    Address = raceViewModel.Address

                };

                _raceRepository.Update(race);

                return RedirectToAction("Index");

            }
            else
            {
                return View(raceViewModel);
            }

        }
    }
}
