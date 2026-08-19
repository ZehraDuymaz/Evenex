using HashidsNet;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IHashids _hashids;

    public UsersController(IHashids hashids)
    {
        _hashids = hashids;
    }

    // sending to the frontend
    [HttpGet("test-encode/{realId}")]
    public IActionResult GetEncodedId(int realId)
    {
        // Turns 10 into something like "a1B2c3D4"
        string eid = _hashids.Encode(realId); 
        
        return Ok(new { EncryptedId = eid });
    }

    // receive from frontend
    [HttpGet("{eid}")]
    public IActionResult GetUserByEid(string eid)
    {
        // Turns "a1B2c3D4" back into 10
        var decodedArray = _hashids.Decode(eid);
        
        if (decodedArray.Length == 0)
        {
            return BadRequest("Nice try! Being nasty eh? ");
        }

        int realId = decodedArray[0];

        // for searching use realID
        return Ok(new { Message = $"Veritabanındaki gerçek id: {realId}!" });
    }
}