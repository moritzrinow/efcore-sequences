namespace EntityFrameworkCore.Extensions.Sequences.Test.Controllers
{
  using System.Linq;
  using System.Threading.Tasks;
  using EntityFrameworkCore.Extensions.Sequences.Test.Database;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;

  [ApiController]
  public class SequenceController : ControllerBase
  {
    private readonly SequenceContext context;

    public SequenceController(SequenceContext context)
    {
      this.context = context;
    }

    [HttpGet]
    [Route("sequences/{name}")]
    public async Task<IActionResult> GetSequence([FromRoute] string name)
    {
      var sequence = await this.context.Sequences().SingleOrDefaultAsync(p => p.Name == name);

      if (sequence == null)
      {
        return this.NotFound();
      }

      return this.Ok(sequence);
    }

    [HttpGet]
    [Route("sequences/{name}/next")]
    public async Task<IActionResult> GetNextSequenceValue([FromRoute] string name)
    {
      var value = await this.context.NextSequenceValueAsync<long>(name, this.HttpContext.RequestAborted);

      return this.Ok(value);
    }

    [HttpGet]
    [Route("sequences/selectnext")]
    public async Task<IActionResult> SelectNextSequenceValue()
    {
      var values = await this.context.Sequences()
                             .Select(p => new
                             {
                               Name = p.Name,
                               NextValue = EF.Functions.NextValLong(p.Schema + "." + p.Name)
                             })
                             .ToListAsync(this.HttpContext.RequestAborted);

      return this.Ok(values);
    }

    [HttpGet]
    [Route("sequences")]
    public async Task<IActionResult> GetSequences()
    {
      var sequences = await this.context.Sequences().ToListAsync(this.HttpContext.RequestAborted);

      return this.Ok(sequences);
    }

    [HttpGet]
    [Route("sequencenames")]
    public async Task<IActionResult> GetSequenceNames()
    {
      var names = await this.context.Sequences().Select(p => p.Name).ToListAsync(this.HttpContext.RequestAborted);

      return this.Ok(names);
    }

    [HttpPost]
    [Route("sequences")]
    public async Task<IActionResult> CreateSequence([FromBody] DbSequence<long> sequence)
    {
      var res = await this.context.CreateSequenceAsync(sequence, this.HttpContext.RequestAborted);

      return this.Ok();
    }

    [HttpPut]
    [Route("sequences/{name}")]
    public async Task<IActionResult> UpdateSequence([FromRoute] string name, [FromBody] DbSequenceUpdate<long> update)
    {
      var res = await this.context.UpdateSequenceAsync(name, update, this.HttpContext.RequestAborted);

      return this.Ok();
    }

    [HttpDelete]
    [Route("sequences/{name}")]
    public async Task<IActionResult> DropSequence([FromRoute] string name)
    {
      var res = await this.context.DropSequenceAsync(name, this.HttpContext.RequestAborted);

      if (res)
      {
        return this.Ok();
      }
      else
      {
        return this.NotFound();
      }
    }

    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test()
    {
      var query = this.context.Sequences().Select(p => p.NextVal());
      
      var sequences = await this.context.Sequences()
                                .Select(p => new
                                {
                                  Name = p.Name,
                                  Value = EF.Functions.NextValLong(p.Schema + "." + p.Name),
                                }).ToListAsync(this.HttpContext.RequestAborted);

      return this.Ok(sequences);
    }
  }
}