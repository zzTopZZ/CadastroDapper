using Asp.Versioning;
using Cadastro.API.DTOs;
using Cadastro.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cadastro.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;

    public ClienteController(IClienteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var clientes = await _service.ObterTodos();
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _service.ObterPorId(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Inserir(ClienteRequest request)
    {
        var id = await _service.Inserir(request);
        return CreatedAtAction(nameof(ObterPorId), new { id }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ClienteRequest request)
    {
        await _service.Atualizar(id, request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _service.Deletar(id);
        return NoContent();
    }
}
