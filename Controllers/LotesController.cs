using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using msm_ws.Data.Dto;
using msm_ws.Repositories;

namespace msm_ws.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LotesController : ControllerBase
    {
        #region [Constructor & variables]
        private LotesRepository _repository;

        public LotesController(LotesRepository repository)
        {
            _repository = repository;
        }
        #endregion
        
        #region [Getters]
        [HttpGet("GetLotes01")]
        public async Task<OkObjectResult> GetLotes01([FromHeader] string authorization, string tipoCliente)
        {
            return Ok(await _repository.GetLotes01(authorization, tipoCliente));
        }
        
        [HttpGet("GetLotes02")]
        public async Task<OkObjectResult> GetLotes02([FromHeader] string authorization, string centro)
        {
            return Ok(await _repository.GetLotes02(authorization, centro));
        }
        
        [HttpGet("GetLotes03")]
        public async Task<OkObjectResult> GetLotes03([FromHeader] string authorization, string centro)
        {
            return Ok(await _repository.GetLotes03(authorization, centro));
        }
      
        #endregion
        
        #region [Setters]
        [HttpPost("setLote01")]
        public async Task<ActionResult<string>> setLote01([FromHeader] string authorization, [FromBody]Lote01Dto lote)
        {
            return Ok(await _repository.SetLote01(authorization, lote));
        }
        
        [HttpPost("setLote02")]
        public async Task<ActionResult<string>> setLote02([FromHeader] string authorization, [FromBody]Lote02Dto lote)
        {
            return Ok(await _repository.SetLote02(authorization, lote));
        }
        
        [HttpPost("setLote03")]
        public async Task<ActionResult<string>> setLote03([FromHeader] string authorization, [FromBody]Lote03Dto[] lote)
        {
            return Ok(await _repository.SetLote03(authorization, lote));
        }
        #endregion
    }
}
