using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using GogoKit;

namespace MVCWebApplication.Models
{
    public class CategoryInfo
    {
        public int ID { get; set; }
        public string Category { get; set; }
    }

    public class CategorySearch
    {
        //Parameter : category
        //Methos    : This method uses category parameter to pull catgories that match from Viagogo API
        public async System.Threading.Tasks.Task<List<GogoKit.Models.Response.SearchResult>> SearchCategoryAsync(string category)
        {
            var api = new ViagogoClient(new ProductHeaderValue("AwesomeApp", "1.0"),
                               "TaRJnBcw1ZvYOXENCtj5", "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");
            var searchResults = await api.Search.GetAllAsync(category);

            return searchResults.ToList();
        }
    }
}