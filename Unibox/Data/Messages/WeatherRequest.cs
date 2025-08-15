using NetMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Data.Messages
{
    public class WeatherRequest : IRequest<WeatherResponse>
    {
        public string City { get; set; }
        public DateTime Date { get; set; }
    }
}