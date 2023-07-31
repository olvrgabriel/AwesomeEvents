using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _devEventsDbContext;
        private readonly IMapper _mapper;
        public DevEventsController(
            DevEventsDbContext devEventsDbContext,
            IMapper mapper)
        {
            _devEventsDbContext = devEventsDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _devEventsDbContext.DevEvents.Include(de => de.Speakers).Where(d => !d.IsDeleted).ToList();

            var viewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);

            return Ok(viewModel);
        }

        /// <summary>
        /// Obter evento específico
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Dados do evento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetById(Guid id)
        {
            var devEvent = _devEventsDbContext.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound("Objeto não encontrado");

            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);


            return Ok(viewModel);
        }

        /// <summary>
        /// Cadastra um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string", ","description"."string", ","startDate":2023-02-27T17:59:14.141Z"," ","endDate":2023-02-27T17:59:14.141Z")
        /// </remarks>
        /// <param name="input">Dados do evento</param>
        /// <returns>Objeto recém criado</returns>
        /// <response code="201">Sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEventInputModel input)
        {
            var devEvent = _mapper.Map<DevEvent>(input);

            _devEventsDbContext.DevEvents.Add(devEvent);
            _devEventsDbContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id}, devEvent);
        }

        /// <summary>
        /// Atualiza um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string", ","description"."string", ","startDate":2023-02-27T17:59:14.141Z"," ","endDate":2023-02-27T17:59:14.141Z")
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="input">Dados do evento</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(Guid id, DevEventInputModel input)
        {
            var devEvent = _devEventsDbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound("Objeto não encontrado");

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _devEventsDbContext.DevEvents.Update(devEvent);
            _devEventsDbContext.SaveChanges();
            return NoContent();

        }

        /// <summary>
        /// Cadastrar palestrante
        /// </summary>
        /// <remarks>
        /// {"name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string"}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="input">Dados do palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostSpekaer(Guid id, DevEventSpeakerInputModel input)
        {
            var speaker = _mapper.Map<DevEventSpeaker>(input);

            speaker.DevEventId = id;

            var devEvent = _devEventsDbContext.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
                return NotFound("Objeto não encontrado");

            _devEventsDbContext.DevEventSpeakers.Add(speaker);
            _devEventsDbContext.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _devEventsDbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound("Objeto não encontrado");

            devEvent.Delete();

            _devEventsDbContext.SaveChanges();

            return NoContent();
        }

        
    }
}
