using Prism.Events;
using SicoreQMS.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Extensions
{
    public static class DialogExtensions
    {

        public static void ResgiterMessage(this IEventAggregator aggregator,Action<string> action)
        {
            aggregator.GetEvent<MessageEvent>().Subscribe(action);
        }
 

        public static void SendMessage(this IEventAggregator aggregator, string message, string filterName = "Main")
        {
            aggregator.GetEvent<MessageEvent>().Publish(message);
        }
    }
}
