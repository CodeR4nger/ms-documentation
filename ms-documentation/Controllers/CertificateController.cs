using Microsoft.AspNetCore.Mvc;
using ms_documentation.Models;
using ms_documentation.Services;

namespace ms_documentation.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class CertificateController(IAlumnoService alumnoService) : ControllerBase
{
    readonly IAlumnoService _alumnoService = alumnoService;

    [HttpGet("{type}/{id}")]
    public async Task<IActionResult> Get(int id,string type)
    {
        if (id <= 0)
            return NotFound();
        Alumno? alumno = await _alumnoService.GetAlumnoFromId(id);
        if (alumno == null)
            return NotFound();

        var file = CertificateService.Generate(type, alumno);
        if (file == null)
            return BadRequest($"Invalid file type: {type}");
        return File(file, $"application/{type}", $"certificado.{type}");
    }
}