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
        var result = await _service.ObterTodos();
        return result.IsSuccess
            ? Ok(new { result.Value, result.IsSuccess, result.Error })
            : BadRequest(new { result.Value, result.IsSuccess, result.Error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var result = await _service.ObterPorId(id);
        return result.IsSuccess
            ? Ok(new { result.Value, result.IsSuccess, result.Error })
            : NotFound(new { result.Value, result.IsSuccess, result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Inserir(ClienteRequest request)
    {
        var result = await _service.Inserir(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(ObterPorId), new { id = result.Value }, new { result.Value, result.IsSuccess, result.Error })
            : BadRequest(new { result.Value, result.IsSuccess, result.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ClienteRequest request)
    {
        var result = await _service.Atualizar(id, request);
        return result.IsSuccess
            ? Ok(new { result.IsSuccess, Error = (string?)null })
            : NotFound(new { result.IsSuccess, result.Error });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var result = await _service.Deletar(id);
        return result.IsSuccess
            ? Ok(new { result.IsSuccess, Error = (string?)null })
            : NotFound(new { result.IsSuccess, result.Error });
    }
}
