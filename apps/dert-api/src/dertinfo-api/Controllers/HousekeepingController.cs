using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Connection;
using DertInfo.Repository;
using DertInfo.Services;
using DertInfo.Services.Entity.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Housekeeping controller is for firing immutable functions that will ensure that the data is in a consistant state
/// </summary>
namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class HousekeepingController : AuthController
    {
        private readonly IImageService _imageService;
        private readonly IBlobStorageRepository _blobStorageRepository;
        private readonly IStorageAccountConnection _storageAccountConnection;
        private readonly DertInfoContext _context;

        public HousekeepingController
            (
                IDertInfoUser user,
                IImageService imageService,
                IBlobStorageRepository blobStorageRepository,
                IStorageAccountConnection storageAccountConnection,
                DertInfoContext context

            ) : base(user)
        {
            this._imageService = imageService;
            this._blobStorageRepository = blobStorageRepository;
            this._storageAccountConnection = storageAccountConnection;
            this._context = context;
        }

        [HttpPost]
        [Route("audit/images")]
        public async Task<IActionResult> Post()
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            ImageHousekeepingReport imageHousekeepingReport = new ImageHousekeepingReport();

            if (this._user.AuthId == "auth0|58bc7cb0a0ba8d24312f92cd")
            {
                var allDbImages = await this._imageService.ListAll();

                // Check Table Reference Images - those images that are defined in the table but do not have matching files
                foreach (var dbImage in allDbImages)
                {
                    if (dbImage.BlobName != null && dbImage.BlobName.Length > 0)
                    {
                        var exists = await this._blobStorageRepository.TestExists(_storageAccountConnection.getImagesStorageConnectionString(), dbImage.Container, dbImage.BlobPath, dbImage.BlobName);

                        if (!exists)
                        {
                            imageHousekeepingReport.DoesNotExistCount++;
                        }
                    }
                    else
                    {
                        imageHousekeepingReport.NotMigratedCount++;
                    }

                }
            }

            return Ok(imageHousekeepingReport);

        }

        [HttpPost]
        [Route("data/competitions/markpopulated")]
        public IActionResult MarkOldCompetitionsPopulated()
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            int countMarkedPopulated = 0;

            if (this._user.AuthId == "auth0|58bc7cb0a0ba8d24312f92cd")
            {

                var competitions = this._context.Competitions.Where(c => c.EventId < 23); //DERT 2019 - Current

                foreach (var competition in competitions)
                {
                    competition.ResultsPublished = true;
                    competition.HasBeenPopulated = true;
                    competition.DatePopulated = competition.DateModified;
                }

                this._context.SaveChanges();

                countMarkedPopulated = competitions.Count();
            }

            return Ok("Competitions Updated: " + countMarkedPopulated.ToString());
        }
    }

    public class ImageHousekeepingReport
    {
        public int NotMigratedCount { get; set; }
        public int DoesNotExistCount { get; set; }
        public int Missing100x100 { get; set; }
        public int Missing480x360 { get; set; }
    }
}
