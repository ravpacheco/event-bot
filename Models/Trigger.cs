using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace event_bot.Models
{
    [DataContract]
    public class Trigger : Document
    {
        public const string MIME_TYPE = "application/vnd.c4d.trigger+json";

        public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

        public Trigger() 
            : base(MediaType)
        {
            Payload = new JsonDocument();
        }

        [DataMember]
        public string StateId { get; set; }
        
        [DataMember]
        public JsonDocument Payload { get; set; }
    }

    public static class TriggerHelper
    {
        public static Trigger WithPayload(Trigger trigger, JsonDocument payload)
        {
            var triggerResult = new Trigger
            {
                StateId = trigger.StateId,
                Payload = payload
            };
            return triggerResult;
        }
    }
}
