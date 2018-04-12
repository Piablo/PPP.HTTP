using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.DataProvider.Menu;
using DisplayModels;
using DisplayModels.Menu;
using Microsoft.AspNetCore.Mvc;

namespace PPP2.HTTP.Controllers
{
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [Route("api/test")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [Route("api/Menu/GetPizza")]
        public BaseResponse GetPizzas([FromBody] MenuRequestModel lm)
        {
            BaseResponse br = new BaseResponse();
            try
            {
                var data = new MenuDataProvider();
                var retval = data.GetData(lm);
                br.Result = Newtonsoft.Json.JsonConvert.SerializeObject(retval);
                br.ErrorCode = 0;
                br.ErrorMessage = "";
            }
            catch(Exception ex)
            {
                br.ErrorCode = -1;
                br.ErrorMessage = ex.ToString();
            }
            return br;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
