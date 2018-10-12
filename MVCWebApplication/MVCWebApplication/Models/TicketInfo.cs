using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GogoKit;

namespace MVCWebApplication.Models
{
    public class TicketInfo
    {
        public int ID { get; set; }
        public string TicketType { get; set; }
        public int NumOfTickets { get; set; }
        public string Seats { get; set; }
        public string Price { get; set; }
    }

    public class TicketSearch
    {
        //Parameter : eventId
        //Methos    : This method uses eventId parameter to pull Tickets for selected event
        //            using Viagogo API
        public async Task<List<GogoKit.Models.Response.Listing>> SearchTicketsAsync(int eventId)
        {
            var api = new ViagogoClient(new ProductHeaderValue("AwesomeApp", "1.0"),
                               "TaRJnBcw1ZvYOXENCtj5", "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");

            var ticketResult = await api.Listings.GetAllByEventAsync(eventId);

            return ticketResult.ToList();
        }
    }
}