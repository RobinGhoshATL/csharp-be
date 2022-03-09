using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Service.Control.DataBaseConnection.Data;
using System.Service.Control.DataBaseConnection.Models;
using System.Threading.Tasks;

namespace System.Service.Control.Controller
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DataBaseConnectionController : ControllerBase
    {
        #region "Variables"
        DatabaseConnectRepository _respository;
        #endregion "Variables"

        #region "Constructor"

        public DataBaseConnectionController(DatabaseConnectRepository repository) {
            _respository = repository;
        }

        #endregion "Constructor"

        [Route("getProductListing")]
        [HttpGet]
        public async Task<List<DataBaseConnectionModel>> GetProductsListing() {
           var result =  _respository.GetProductsListing();
            return await result;
        }
    }
}
