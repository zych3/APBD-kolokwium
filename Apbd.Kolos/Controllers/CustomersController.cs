using Apbd.Kolos.DTOs;
using Apbd.Kolos.Exceptions;
using Apbd.Kolos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apbd.Kolos.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomersController(IDbService db) : ControllerBase
{
    [HttpGet("{id:int}/purchases")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var customer = await db.GetByIdAsync(id);
            return Ok(customer);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCustomerDto dto)
    {
        try
        {
            await db.CreateAsync(dto);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (TooManyTicketsException e)
        {
            return BadRequest(e.Message);
        }
        catch (AlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}