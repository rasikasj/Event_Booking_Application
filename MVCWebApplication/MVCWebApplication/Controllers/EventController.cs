using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MVCWebApplication.Models;

namespace MVCWebApplication.Controllers
{
    public class EventController : Controller
    {
        // GET: Categories
        public async System.Threading.Tasks.Task<ActionResult> Categories(string category)
        {
            List<CategoryInfo> categories = new List<CategoryInfo>();
            var categoryResults = new List<GogoKit.Models.Response.SearchResult>();
            CategoryInfo cInfo = new CategoryInfo();
            CategorySearch c = new CategorySearch();

            if (!String.IsNullOrEmpty(category))
            {
                categoryResults = await c.SearchCategoryAsync(category);
            }

            foreach (GogoKit.Models.Response.SearchResult s in categoryResults)
            {
                if (s.CategoryLink != null)
                {
                    cInfo = new CategoryInfo();
                    cInfo.ID = Convert.ToInt32(s.CategoryLink.HRef.Split('/').Last());
                    cInfo.Category = s.Title;
                    categories.Add(cInfo);
                }
            }

            return View(categories);
        }

        // GET: Events
        public async System.Threading.Tasks.Task<ActionResult> Index(int id)
        {
            List<EventInfo> events = new List<EventInfo>();
            var eventResults = new List<GogoKit.Models.Response.Event>();
            Dictionary<string, MinPriceEvents> minPrice = new Dictionary<string, MinPriceEvents>();
            EventInfo eInfo = new EventInfo();
            EventSearch s = new EventSearch();

            if (id != 0)
            {
                eventResults = await s.SearchEventsAsync(id);
            }

            var filteredResults = eventResults.AsEnumerable().Where(x => x.MinTicketPrice != null).OrderBy(x => x.Venue.Country.Name).ToList();

            foreach (GogoKit.Models.Response.Event e in filteredResults)
            {
                MinPriceTracker(minPrice, e);

                eInfo = new EventInfo();
                eInfo.ID = e.Id.Value;
                eInfo.Artist = e.Name;
                eInfo.EventDate = DateTime.Parse(e.StartDate.Value.ToString());
                eInfo.Venue = e.Venue.Name;
                eInfo.Location = e.Venue.City + ", " + e.Venue.Country.Name;
                eInfo.Tickets = e.NumberOfTickets.Value;
                eInfo.MinPrice = e.MinTicketPrice == null ? "N/A" : e.MinTicketPrice.Display;

                events.Add(eInfo);
            }

            MarkMinPricedEvents(events, minPrice);

            return View(events);
        }

        // GET: Tickets
        public async System.Threading.Tasks.Task<ActionResult> Tickets(int id)
        {
            List<TicketInfo> tickets = new List<TicketInfo>();
            var ticketsResults = new List<GogoKit.Models.Response.Listing>();
            TicketInfo t = new TicketInfo();
            TicketSearch s = new TicketSearch();

            if (id != 0)
            {
                ticketsResults = await s.SearchTicketsAsync(id);
            }

            StringBuilder seat = new StringBuilder();

            foreach (GogoKit.Models.Response.Listing l in ticketsResults)
            {
                seat = new StringBuilder();
                t = new TicketInfo();
                GetSeatingDetails(seat, l);

                t.ID = l.Id.Value;
                t.TicketType = l.TicketType.Type;
                t.NumOfTickets = l.NumberOfTickets.Value;
                t.Seats = string.IsNullOrEmpty(seat.ToString()) ? "Seat details not available" : seat.ToString();
                t.Price = l.EstimatedTicketPrice.Display;

                tickets.Add(t);
            }

            return View(tickets);
        }

        //This method marks min price for the countries that are hosting more than one event.
        private static void MarkMinPricedEvents(List<EventInfo> events, Dictionary<string, MinPriceEvents> minPrice)
        {
            foreach (KeyValuePair<string, MinPriceEvents> kvp in minPrice)
            {
                if (kvp.Value.EventCount > 1)
                {
                    var e = events.Find(r => r.ID == kvp.Value.EID);
                    e.MinPrice = e.MinPrice + " **";
                }
            }
        }

        //This methos keeps track of numbers of events that are happening in a country along with eventid
        //that has min priced tickets
        private static void MinPriceTracker(Dictionary<string, MinPriceEvents> minPrice, GogoKit.Models.Response.Event e)
        {
            if (minPrice.ContainsKey(e.Venue.Country.Name) && e.MinTicketPrice != null)
            {
                if (minPrice[e.Venue.Country.Name].MinPriceTicket > e.MinTicketPrice.Amount.Value)
                {
                    minPrice[e.Venue.Country.Name].MinPriceTicket = e.MinTicketPrice.Amount.Value;
                    minPrice[e.Venue.Country.Name].EID = e.Id.Value;
                }
                minPrice[e.Venue.Country.Name].EventCount = minPrice[e.Venue.Country.Name].EventCount + 1;
            }
            else
            {
                MinPriceEvents mpe = new MinPriceEvents();
                mpe.EID = e.Id.Value;
                mpe.MinPriceTicket = e.MinTicketPrice.Amount.Value;
                mpe.EventCount = 1;
                minPrice.Add(e.Venue.Country.Name, mpe);
            }
        }

        //This method puts seating information together
        private static void GetSeatingDetails(StringBuilder seat, GogoKit.Models.Response.Listing l)
        {
            if (!string.IsNullOrEmpty(l.Seating.Section))
            {
                seat.Append("Section: " + l.Seating.Section);
            }

            if (!string.IsNullOrEmpty(l.Seating.Row))
            {
                seat.Append(" || Row: " + l.Seating.Section);
            }

            if (!string.IsNullOrEmpty(l.Seating.SeatFrom))
            {
                seat.Append(" || Seats from " + l.Seating.SeatFrom);
            }

            if (!string.IsNullOrEmpty(l.Seating.SeatTo))
            {
                seat.Append(" to " + l.Seating.SeatTo);
            }
        }
    }
}