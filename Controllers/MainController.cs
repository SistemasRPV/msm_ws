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
    public class MainController : ControllerBase
    {
        #region [Constructor & variables]
        private MainRepository _repository;

        public MainController(MainRepository repository)
        {
            _repository = repository;
        }
        #endregion
        
        #region [Auth]
        [AllowAnonymous]
        [HttpPost("Auth")]
        public async Task<ActionResult<AuthDto>> Authenticate([FromBody]LoginDto auth)
        {
            return Ok(await _repository.Authenticate(auth));
        }
        #endregion
        
        #region [Getters]
        [HttpGet("GetFotoByOrdenAndCentro")]
        public async Task<OkObjectResult> GetFotoByOrdenAndCentroleteFoto(int idOrden, int idCentro)
        {
            return Ok(await _repository.GetFotoByOrdenAndCentro( idOrden, idCentro));
        }
        
        [HttpGet("GetTipoByIdTipo")]
        public async Task<OkObjectResult> GetTipoByIdTipo(int idTipo)
        {
            return Ok(await _repository.GetTipoByIdTipo(idTipo));
        }
        
        [HttpGet("GetTipos")]
        public async Task<OkObjectResult> GetTipos()
        {
            return Ok(await _repository.GetTipos());
        }
        
        [HttpGet("GetMateriales")]
        public async Task<OkObjectResult> GetMateriales()
        {
            return Ok(await _repository.GetMateriales());
        }

        [HttpGet("GetOrdenesByCentro")]
        public async Task<OkObjectResult> GetOrdenesByCentro(int idCentro)
        {
            return Ok(await _repository.GetOrdenesByCentro(idCentro));
        }
        
        [HttpGet("GetCentros")]
        public async Task<OkObjectResult> GetCentros(int idUser)
        {
            return Ok(await _repository.GetCentros(idUser));
        }
        
        [HttpGet("GetTodosCentros")]
        public async Task<OkObjectResult> GetTodosCentros(int idUser)
        {
            return Ok(await _repository.GetTodosCentros(idUser));
        }

        [HttpGet("GetVisita")]
        public async Task<OkObjectResult> GetVisita(int idUsuario, string idPersona, int idCentro)
        {
            return Ok(await _repository.GetVisita(idUsuario, idPersona, idCentro));
        }
        
        [HttpGet("GetContactosCentroByIdCentro")]
        public async Task<OkObjectResult> GetContactosCentroByIdCentro(int idCentro)
        {
            return Ok(await _repository.GetContactosCentroByIdCentro(idCentro));
        }
        #endregion
        
        #region [Setters]
        // [HttpPost("SetFotos")]
        // public async Task<ActionResult<FotoDto>> SetFotos([FromBody]FotoDto[] fotos)
        // {
        //     return Ok(await _repository.SetFotos(fotos));
        // }
        
        [HttpPost("DeleteFoto")]
        public async Task<OkObjectResult> DeleteFoto(int idFoto)
        {
            return Ok(await _repository.DeleteFoto(idFoto));
        }

        [HttpPost("SetContacto")]
        public async Task<ActionResult<ContactoDto>> SetContacto([FromBody]ContactoDto contacto)
        {
            return Ok(await _repository.SetContacto(contacto));
        }
        
        [HttpPost("SetOrden")]
        public async Task<ActionResult<ReporteOrdenDto>> SetOrden([FromBody]OrdenFotoDto orden)
        {
            return Ok(await _repository.SetOrden(orden));
        }
        
        [HttpPost("SetNewOrden")]
        public async Task<ActionResult<string>> SetNewOrden([FromBody]OrdenMsmDto orden)
        {
            return Ok(await _repository.SetNewOrden(orden));
        }
        
        #endregion
    }
}
