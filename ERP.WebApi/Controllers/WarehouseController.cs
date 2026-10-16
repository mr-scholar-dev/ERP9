using ERP.Application.Interfaces;
using ERP.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehouseController(IWarehouseService service)
    {
        _service = service;
    }

    // GET api/Warehouse
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return StatusCode(result.Status, result);
    }

    // GET api/Warehouse/{pageNumber}/{pageSize}
    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    public async Task<IActionResult> GetPaginationAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _service.GetPaginationAsync(pageNumber, pageSize);
        return StatusCode(result.Status, result);
    }

    // GET api/Warehouse/{id}
    [HttpGet("{id:int}", Name = "GetWarehouseById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.Status, result);
    }

    // POST api/Warehouse
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] WarehouseCreateDto warehouse)
    {
        var result = await _service.AddAsync(warehouse);
        if (!result.IsSuccess)
            return StatusCode(result.Status, result);

        return CreatedAtRoute("GetWarehouseById", new { id = result.Data!.IdWarehouse }, result);
    }

    // PUT api/Warehouse/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] WarehouseUpdateDto warehouse)
    {
        var result = await _service.UpdateAsync(id, warehouse);
        return StatusCode(result.Status, result);
    }

    // DELETE api/Warehouse/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _service.DeleteAsync(id);
        return StatusCode(result.Status, result);
    }
}
