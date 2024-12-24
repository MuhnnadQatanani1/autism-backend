using Autsim.Models;
using Autsim.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using Autsim.Controllers;
using Autsim.Controllers;

namespace Autsim.Services
{
    public class PecsCardService
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PecsCardService(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<PecsCardDto> AddPecsCardAsync(PecsCardCreateDto pecsCardCreateDto)
        {
            if (pecsCardCreateDto == null)
                throw new ArgumentNullException(nameof(pecsCardCreateDto), "PecsCard data is required.");

            byte[] imageData;

            // التعامل مع الصورة كما هو
            if (pecsCardCreateDto.Images != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await pecsCardCreateDto.Images.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            else
            {
                throw new ArgumentException("Image is required.", nameof(pecsCardCreateDto.Images));
            }

            // توليد الصوت باستخدام Python Helper
            MemoryStream audioStream = await PythonHelper.GenerateAudioAsync(pecsCardCreateDto.Name, "ar");
            byte[] audioData;

            using (var memoryStream = new MemoryStream())
            {
                await audioStream.CopyToAsync(memoryStream);
                audioData = memoryStream.ToArray();
            }

            // إنشاء نموذج PecsCard
            var pecsCard = new PecsCard
            {
                Name = pecsCardCreateDto.Name,
                CreationTime = DateTime.UtcNow,
                Image = new PecsImage
                {
                    ImageData = imageData // تخزين الصورة كما هو
                },
                AudioData = audioData // تخزين الصوت
            };

            _context.PecsCards.Add(pecsCard);
            await _context.SaveChangesAsync();

            return new PecsCardDto
            {
                Id = pecsCard.Id,
                Name = pecsCard.Name,
                IFormFile = Convert.ToBase64String(imageData) // الحفاظ على إعادة الصورة بنفس الطريقة
            };
        }


        public async Task<(IEnumerable<PecsCardDto> cards, int totalCount)> GetAllPecsCardsAsync(int? pecsCardId = null, int pageNumber = 1, int pageSize = 10)
        {
            var allCards = await _context.PecsCards
                .Include(pc => pc.Image)
                .Include(pc => pc.ExpectedEntries)
                .ToListAsync();

            List<PecsCardDto> expectedCards = new List<PecsCardDto>();
            List<PecsCardDto> remainingCards = new List<PecsCardDto>();

            if (pecsCardId.HasValue)
            {
                var specifiedCard = allCards.FirstOrDefault(pc => pc.Id == pecsCardId.Value);

                if (specifiedCard != null)
                {
                    // Populate expectedCards using Key from ExpectedEntries
                    expectedCards = specifiedCard.ExpectedEntries
                        .OrderByDescending(e => e.Value)
                        .Select(e => new PecsCardDto
                        {
                            Id = e.Key,
                            Name = allCards.First(pc => pc.Id == e.Key).Name,
                            IFormFile = Convert.ToBase64String(allCards.First(pc => pc.Id == e.Key).Image?.ImageData)
                        })
                        .ToList();
                }
            }

            // Get remaining cards excluding expected cards and specified pecsCardId
            remainingCards = allCards
                .Where(pc => !expectedCards.Any(ec => ec.Id == pc.Id) && (pecsCardId == null || pc.Id != pecsCardId.Value))
                .Select(pc => new PecsCardDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IFormFile = pc.Image?.ImageData != null ? Convert.ToBase64String(pc.Image.ImageData) : null
                })
                .ToList();

            // Combine expected and remaining cards
            var allCombinedCards = expectedCards.Concat(remainingCards).ToList();

            // Apply pagination
            var paginatedCards = allCombinedCards
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = allCombinedCards.Count();

            return (paginatedCards, totalCount);
        }


        //Retrieves a single PecsCard by its ID.
        public async Task<PecsCardDto> GetPecsCardByIdAsync(int id)
        {
            var pecsCard = await _context.PecsCards
                .Include(pc => pc.Image)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (pecsCard == null)
                return null;

            return new PecsCardDto
            {
                Id = pecsCard.Id,
                Name = pecsCard.Name,
                IFormFile = pecsCard.Image?.ImageData != null ? Convert.ToBase64String(pecsCard.Image.ImageData) : null
            };
        }

        //Updates an existing PecsCard and its associated image.
        public async Task<PecsCardDto> UpdatePecsCardAsync(int id, PecsCardCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "PecsCard data is required.");

            var pecsCard = await _context.PecsCards
                .Include(pc => pc.Image)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (pecsCard == null)
                return null;

            pecsCard.Name = dto.Name;

            if (dto.Images != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.Images.CopyToAsync(memoryStream);
                    pecsCard.Image.ImageData = memoryStream.ToArray();
                }
            }
            else
            {
                throw new ArgumentException("Image is required for update.", nameof(dto.Images));
            }

            await _context.SaveChangesAsync();

            return new PecsCardDto
            {
                Id = pecsCard.Id,
                Name = pecsCard.Name,
                IFormFile = Convert.ToBase64String(pecsCard.Image.ImageData)
            };
        }

        //Deletes a PecsCard and its associated image.
        public async Task<bool> DeletePecsCardAsync(int id)
        {
            var pecsCard = await _context.PecsCards
                .Include(pc => pc.Image)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (pecsCard == null)
                return false;

            _context.PecsCards.Remove(pecsCard);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<PecsCardDto>> SearchPecsCardsByNameAsync(string name)
        {
            var pecsCards = await _context.PecsCards
                .Include(pc => pc.Image)
                .Where(pc => pc.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return pecsCards.Select(pc => new PecsCardDto
            {
                Id = pc.Id,
                Name = pc.Name,
                IFormFile = pc.Image?.ImageData != null ? Convert.ToBase64String(pc.Image.ImageData) : null
            }).ToList();
        }

        public async Task<List<PecsCardDto>> GetPecsCardsByIdsAsync(List<int> pecsCardIds)
        {
            var pecsCards = await _context.PecsCards
                .Include(pc => pc.Image)
                .Where(pc => pecsCardIds.Contains(pc.Id))
                .ToListAsync();

            return pecsCards.Select(pecsCard => new PecsCardDto
            {
                Id = pecsCard.Id,
                Name = pecsCard.Name,
                IFormFile = pecsCard.Image?.ImageData != null ? Convert.ToBase64String(pecsCard.Image.ImageData) : null
            }).ToList();
        }

        public async Task<List<PecsCard>> ReadPecsCardNamesAsync(List<int> selectedPecsCardIds)
        {
            var pecsCards = await _context.PecsCards
                .Include(pc => pc.ExpectedEntries)
                .Where(pc => selectedPecsCardIds.Contains(pc.Id))
                .ToListAsync();

            for (int i = 0; i < selectedPecsCardIds.Count - 1; i++)
            {
                var currentCardId = selectedPecsCardIds[i];
                var nextCardId = selectedPecsCardIds[i + 1];

                var currentCard = pecsCards.FirstOrDefault(pc => pc.Id == currentCardId);
                if (currentCard == null) continue;

                var existingEntry = currentCard.ExpectedEntries.FirstOrDefault(e => e.Key == nextCardId);

                if (existingEntry != null)
                {
                    existingEntry.Value++;
                }
                else
                {
                    currentCard.ExpectedEntries.Add(new PecsCardExpectedEntry
                    {
                        PecsCardId = currentCardId,
                        Key = nextCardId,
                        Value = 1
                    });
                }
            }

            await _context.SaveChangesAsync();


            return pecsCards;
        }
    }
}
