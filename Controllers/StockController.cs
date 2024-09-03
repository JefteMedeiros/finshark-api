using System;
using finshark_api.Dtos.Stock;
using finshark_api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("/api/stock")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public StockController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _context.Stock.ToListAsync();

        var stockDto = stocks.Select(stock => stock.ToStockDto());

        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID([FromRoute] int id)
    {
        var stock = await _context.Stock.FindAsync(id);

        if (stock == null)
        {
            return NotFound();
        }

        return Ok(stock.ToStockDto());
    }

    [HttpPost]
    public async Task<IActionResult>Create([FromBody] CreateStockRequestDto stockDto)
    {
        var stockModel = stockDto.ToStockFromRequestDTO();
        
        await _context.Stock.AddAsync(stockModel);
        await _context.SaveChangesAsync();

        // It runs getbyid passing the stock id as a parameter and then transforms it into a stock dto
        return CreatedAtAction(nameof (GetByID), new { id =  stockModel.Id }, stockModel.ToStockDto());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
    {
        var stockModel = await _context.Stock.FirstOrDefaultAsync(stock => stock.Id == id);

        if (stockModel == null)
        {
            return NotFound();
        }

        stockModel.CompanyName = updateDto.CompanyName;
        stockModel.Symbol = updateDto.Symbol;
        stockModel.MarketCap = updateDto.MarketCap;
        stockModel.Purchase = updateDto.Purchase;
        stockModel.Industry = updateDto.Industry;
        stockModel.LastDiv = updateDto.LastDiv;

        await _context.SaveChangesAsync();

        return Ok(stockModel.ToStockDto());
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var stockModel = await _context.Stock.FirstOrDefaultAsync(stock => stock.Id == id);

        if (stockModel == null)
        {
            return NotFound();
        }

        _context.Stock.Remove(stockModel);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

