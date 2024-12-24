using Microsoft.AspNetCore.Mvc;
using Autsim.Models.DTOs;
using Autsim.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Autsim.Models;
using System.Security.Claims;
using Autsim.Controllers;

namespace Autsim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PecsCardController : ControllerBase
    {
        private readonly PecsCardService _pecsCardService;
        private readonly ILogger<PecsCardController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;



        public PecsCardController(PecsCardService pecsCardService, UserManager<ApplicationUser> userManager, ILogger<PecsCardController> logger)
        {
            _pecsCardService = pecsCardService;

            _userManager = userManager;
            _logger = logger;

        }

        //Adds a new PecsCard.
        [HttpPost("AddPecs")]
        public async Task<IActionResult> AddPecsCard([FromForm] PecsCardCreateDto pecsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pecsDto == null)
            {
                return BadRequest("PecsCard data is required.");
            }

            try
            {
                var result = await _pecsCardService.AddPecsCardAsync(pecsDto);
                // Return a 201 Created status code with location header
                return CreatedAtAction(nameof(GetPecsCardById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input data.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error adding PecsCard.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("getAllPecsCards")]
        public async Task<IActionResult> GetAllPecsCards([FromForm] int? pecsCardId = null, [FromForm] int pageNumber = 1, [FromForm] int pageSize = 10)
        {
            var (cards, totalCount) = await _pecsCardService.GetAllPecsCardsAsync(pecsCardId, pageNumber, pageSize);

            // Return the response with total count and paginated cards
            return Ok(new { TotalCount = totalCount, Cards = cards });
        }
        //Retrieves a specific PecsCard by ID.

        [HttpGet("getpecsById/{id}")]
        public async Task<IActionResult> GetPecsCardById(int id)
        {
            try
            {
                var card = await _pecsCardService.GetPecsCardByIdAsync(id);
                if (card == null)
                    return NotFound(new { message = $"PecsCard with id {id} not found." });

                return Ok(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving PecsCard with id {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Updates an existing PecsCard.
        [HttpPut("Updatepecs/{id}")]
        public async Task<IActionResult> UpdatePecsCard(int id, [FromForm] PecsCardCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCard = await _pecsCardService.UpdatePecsCardAsync(id, dto);
                if (updatedCard == null)
                    return NotFound(new { message = $"PecsCard with id {id} not found." });

                return Ok(updatedCard);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input data.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating PecsCard with id {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete pecs/{id}")]
        public async Task<IActionResult> DeletePecsCard(int id)
        {
            try
            {
                var isDeleted = await _pecsCardService.DeletePecsCardAsync(id);
                if (!isDeleted)
                    return NotFound(new { message = $"PecsCard with id {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting PecsCard with id {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("searchPecs")]
        public async Task<IActionResult> SearchPecsCardsByName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Search term cannot be empty.");
                }

                var pecsCards = await _pecsCardService.SearchPecsCardsByNameAsync(name);

                if (pecsCards == null || !pecsCards.Any())
                {
                    return NotFound($"No PECS cards found matching the name '{name}'.");
                }

                return Ok(pecsCards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PECS cards by name.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("pecs/read-names")]
        public async Task<IActionResult> ReadPecsCardNames([FromQuery] List<int> selectedPecsCardIds)
        {
            try
            {
                if (selectedPecsCardIds == null || !selectedPecsCardIds.Any())
                {
                    return BadRequest("No card IDs provided.");
                }

                // استدعاء الخدمة لجلب البطاقات المحددة
                var pecsCards = await _pecsCardService.ReadPecsCardNamesAsync(selectedPecsCardIds);

                if (!pecsCards.Any())
                {
                    return NotFound("No matching PECS cards found.");
                }

                // دمج الملفات الصوتية المحفوظة في قاعدة البيانات
                using (var memoryStream = new MemoryStream())
                {
                    foreach (var card in pecsCards)
                    {
                        memoryStream.Write(card.AudioData, 0, card.AudioData.Length);
                    }

                    memoryStream.Position = 0;

                    // إعادة الملف الصوتي المدمج
                    return File(memoryStream.ToArray(), "audio/wav", "merged_audio.wav");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading and merging audio files.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}