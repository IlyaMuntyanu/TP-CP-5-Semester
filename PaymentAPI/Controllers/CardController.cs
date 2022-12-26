using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CardController: ControllerBase
{

    private readonly ApiDbContext _db;

    public CardController(ApiDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<ActionResult> OpenCard()
    {
        var random = new Random();
        var cardNumber = random.NextInt64(9000000000000000, 9999999999999999);
        var cvc = random.Next(100, 999);
        
        var currentDate = DateTime.Now;
        var validThrough = new DateOnly(currentDate.Year + 6, currentDate.Month, currentDate.Day);
        var cardStatus = await _db.CardStatus.FirstAsync(cs => cs.Name == "Открыта");

        var card = new Card
        {
            CardNumber = cardNumber,
            Cvc = cvc,
            ValidThrough = validThrough,
            CardStatus = cardStatus
        };

        await _db.Cards.AddAsync(card);
        await _db.SaveChangesAsync();

        return Created("/Card", card);
    }
}