using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using GogoKit;

namespace MVCWebApplication.Models
{
    public class EventInfo
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }
        public int Tickets { get; set; }
        public string MinPrice { get; set; }
    }

    public class MinPriceEvents
    {
        public int EID { get; set; }
        public decimal MinPriceTicket { get; set; }
        public int EventCount { get; set; }
    }

    public class EventSearch
    {
        //Parameter : categoryId
        //Methos    : This method uses categoryId parameter to pull Events that fall under the given category
        //            using Viagogo API
        public async System.Threading.Tasks.Task<List<GogoKit.Models.Response.Event>> SearchEventsAsync(int categoryId)
        {
            var api = new ViagogoClient(new ProductHeaderValue("AwesomeApp", "1.0"),
                               "TaRJnBcw1ZvYOXENCtj5", "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");
            var categoryResults = await api.Categories.GetAsync(categoryId);
            var eventResults = await api.Events.GetAllByCategoryAsync(categoryResults.Id.Value);

            return eventResults.ToList();
        }
    }
}