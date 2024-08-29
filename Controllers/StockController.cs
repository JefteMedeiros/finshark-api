using System;
using finshark_api.Dtos.Stock;
using finshark_api.Mappers;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult GetAll()
    {
        var stocks = _context.Stock.Select(stock => stock.ToStockDto());

        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public IActionResult GetByID([FromRoute] int id)
    {
        var stock = _context.Stock.Find(id);

        if (stock == null)
        {
            return NotFound();
        }

        return Ok(stock.ToStockDto());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateStockRequestDto stockDto)
    {
        var stockModel = stockDto.ToStockFromRequestDTO();
        _context.Stock.Add(stockModel);
        _context.SaveChanges();

        // It runs getbyid passing the stock id as a parameter and then transforms it into a stock dto
        return CreatedAtAction(nameof (GetByID), new { id =  stockModel.Id }, stockModel.ToStockDto());
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
    {
        var stockModel = _context.Stock.FirstOrDefault(stock => stock.Id == id);

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

        _context.SaveChanges();

        return Ok(stockModel.ToStockDto());
    }

    [HttpDelete]
    public IActionResult Delete([FromRoute] int id)
    {
        var stockModel = _context.Stock.FirstOrDefault(stock => stock.Id == id);

        if (stockModel == null)
        {
            return NotFound();
        }

        _context.Stock.Remove(stockModel);

        _context.SaveChanges();

        return NoContent();
    }
}

