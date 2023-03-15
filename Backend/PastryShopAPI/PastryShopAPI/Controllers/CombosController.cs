using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PastryShopAPI.Exceptions;
using PastryShopAPI.Models;
using PastryShopAPI.Models.Combos;
using PastryShopAPI.Models.Security;
using PastryShopAPI.Services;
using PastryShopAPI.Services.Combos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastryShopAPI.Controllers
{
    [Route("api/[controller]")]
    public class CombosController : Controller
    {
        private ICombosService _combosService;
        private IFileService _fileService;

        public CombosController(ICombosService combosService, IFileService fileService)
        {
            _combosService = combosService;
            _fileService = fileService;
        }

        // ==== COMBO ENDPOINTS ====

        // Create Combo
        [HttpPost("Form")]
        public async Task<ActionResult<ComboModel>> CreateComboFormAsync([FromForm] ComboFormModel newCombo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var file = newCombo.Image;
                string imagePath = _fileService.UploadFile(file);
                newCombo.ImagePath = imagePath;


                var combo = await _combosService.CreateComboAsync(newCombo);
                return Created($"/api/combos/{combo.Id}", combo);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something unexpected happened.");
            }
        }

        // Get combos (without products. Just combo data)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComboModel>>> GetCombosAsync()
        {
            try
            {
                var combos = await _combosService.GetCombosAsync();
                return Ok(combos);
            }
            catch (InvalidOperationItemException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something unexpected happened.");
            }
        }

        // Get a particular combo
        // api/teams/2
        [HttpGet("{comboId:long}")]
        public async Task<ActionResult<ComboModel>> GetComboAsync(long comboId)// public async Task<ActionResult<CategoryWithProductModel>> GetCategoryAsync(long teamId)
        {
            try
            {
                var combo = await _combosService.GetComboAsync(comboId);
                return Ok(combo);
            }
            catch (NotFoundItemException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something unexpected happened.");
            }
        }



        // ==== COMBOS - PRODUCTS  (MANY to MANY) ====

        // Create/Link a Product with a Combo

        /*[HttpPost("UserRole")]*/
        [HttpPost("ProductsCombos")]
        public async Task<ActionResult<UserManagerResponse>> CreateProductComboAsync([FromBody] Product_ComboModel product_comboModel)
        {
            if (ModelState.IsValid)
            {
                // CALCULATE TOTAL PRICE:
                // Add the new products price to the combo it is being linked to
               
                //newCombo.Price = totalPrice;
                var result = await _combosService.CreateProductComboAsync(product_comboModel);

                if (true)//result.IsSuccess
                {
                    var totalPrice = await _combosService.CalculateComboPrice(product_comboModel.ComboId);// calculate combo price
                    var combo = await _combosService.GetComboAsync(product_comboModel.ComboId);
                    combo.Price = totalPrice;
                    await _combosService.UpdateComboAsync(combo);// Edit combo with  new price (hacerlo todo de una en el service o nox)

                    return Ok(result);
                }

                //return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
        }

        /*[HttpPost("ProductsCombosAttach")]
        public async Task<ActionResult<Product_ComboModel>> AttachProductComboAsync([FromBody] Product_ComboModel product_comboModel)
        {
            var response = await CreateProductComboAsync(product_comboModel);
            return product_comboModel;
        }*/

            // Get combo products


            [HttpGet("Products")]
        // Seguramente necesite usar el entity y model de "Product_Combos", para obtener todos los productos correspondientes a un comboId.
        //  Idea1: 
        //  obtener todos los productId que esten con el comboId y guardar esos productId en una lista o algo, y finalmente obtener todos los productos con esos productId's
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllComboProductsAsync(long comboId)
        {
            try
            {
                var products = await _combosService.GetAllComboProductsAsync(comboId); // Gets All products with belonging to a comboId. 
                return Ok(products);
            }
            catch (NotFoundItemException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something unexpected happened.");
            }
        }
        


    }
}
