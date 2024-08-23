using NCS.DSS.Customer.ReferenceData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace NCS.DSS.Customer.Helpers
{
    //JSON converter for prioritygroups
    public class PriorityGroupConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize(reader, objectType);
            }
            if (reader.Value != null)
            {
                var parseArray = JArray.Parse(reader.Value.ToString());
                return parseArray.ToObject<List<PriorityCustomer>>();
            }
            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
