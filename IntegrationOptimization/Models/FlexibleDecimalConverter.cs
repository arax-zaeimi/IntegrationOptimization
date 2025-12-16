using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace IntegrationOptimization.Models;

public class FlexibleDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
            {
                return decimalValue;
            }
            return 0; // or throw an exception based on your requirements
        }
        
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetDecimal(out var decimalValue))
            {
                return decimalValue;
            }
            
            // If decimal fails, try double and convert
            if (reader.TryGetDouble(out var doubleValue))
            {
                return (decimal)doubleValue;
            }
        }
        
        return 0; // or throw an exception based on your requirements
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}