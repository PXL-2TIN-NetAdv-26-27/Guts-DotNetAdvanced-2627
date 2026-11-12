using Microsoft.AspNetCore.Mvc;
using BethanysPieShop.Models;
using BethanysPieShop.ApiModels;

namespace BethanysPieShop.Controllers.Api
{
    [Route("api/[controller]")]
    //[Route("api/pies")] //maybe it is better to use a literal string instead of [controller] because "PiesApi" (the name of the controller) is not a good RESTfull name
    [ApiController]
    public class PiesApiController : ControllerBase
    {
        private readonly IPieRepository _pieRepository;

        public PiesApiController(IPieRepository repository)
        {
            _pieRepository = repository;
        }

        /// <summary>
        /// Gets a list of all the Pies 
        /// </summary>
        /// <returns>A list of Pies</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Pie>> GetAllPies()
        {
            return Ok(_pieRepository.AllPies);
        }

        /// <summary>
        /// Gets a specific pie
        /// </summary>
        /// <param name="id">Id of the Pie to get</param>
        /// <returns>A Pie</returns>
        [HttpGet("{id}")]
        public ActionResult<Pie> GetPie(int id)
        {
            var pie = _pieRepository.GetPieById(id);

            if (pie == null)
            {
                return NotFound();
            }

            return Ok(pie);
        }

        /// <summary>
        /// Post a new pie
        /// </summary>
        /// <param name="pie"></param>
        /// <returns>A new Pie</returns>
        /// <response code="201">Returns the newly created item</response>
        [HttpPost]
        public ActionResult<Pie> PostPie(CreatePieModel pieVM)
        {
            Pie newPie = CreatePieFromModel(pieVM);

            // newPie will get a new Id from the database! 
            _pieRepository.AddPie(newPie);

            return CreatedAtAction("GetPie", new { id = newPie.PieId }, newPie);
        }

        /// <summary>
        /// Deletes a Pie by id
        /// </summary>
        /// <param name="id">Id of the Pie to delete</param>
        [HttpDelete("{id}")]
        public IActionResult DeletePie(int id)
        {
            Pie? pieToDelete = _pieRepository.GetPieById(id);
            if (pieToDelete is null)
            {
                return NotFound();
            }
            _pieRepository.DeletePie(id);
            return NoContent();
        }

        /// <summary>
        /// Search for pies
        /// </summary>
        /// <param name="searchQuery">Query to use in the search for pies</param>
        [HttpPost("search")]
        public IActionResult SearchPies([FromBody] string searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery))
            {
                IEnumerable<Pie> pies = _pieRepository.SearchPies(searchQuery);
                return Ok(pies);
            }

            // Just return all pies
            return Ok(_pieRepository.AllPies.ToList());
        }


        // Note:
        // Kinda dumb helper function
        // we should use a mapper for this.
        // Check out AutoMapper or Mapperly! 
        private Pie CreatePieFromModel(CreatePieModel createPieModel)
        {
            Pie pie = new Pie();
            pie.Name = createPieModel.Name;
            pie.ShortDescription = createPieModel.ShortDescription;
            pie.LongDescription = createPieModel.LongDescription;
            pie.AllergyInformation = createPieModel.AllergyInformation;
            pie.Price = createPieModel.Price;
            pie.ImageUrl = createPieModel.ImageUrl;
            pie.ImageThumbnailUrl = createPieModel.ImageThumbnailUrl;    
            pie.IsPieOfTheWeek = createPieModel.IsPieOfTheWeek;
            pie.InStock = createPieModel.InStock;
            pie.CategoryId = createPieModel.CategoryId;
            return pie;
        }
    }
}
