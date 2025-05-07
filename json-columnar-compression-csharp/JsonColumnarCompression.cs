using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace json_columnar_compression
{
    public static class JsonColumnarCompression
    {



        public static JsonNode? CompressJsonToColumnar(JsonNode? jsonData)
        {

            if (jsonData is null) return null;

            if (jsonData is JsonArray jsonArray)
            {
                if (jsonArray.Count == 0 || jsonArray[0] is not JsonObject) return jsonData;

                var keys = jsonArray[0]!.AsObject().Select(kvp => kvp.Key).ToList();
                var resultArray = new JsonArray();

                foreach (var key in keys)
                {
                    var columnArray = new JsonArray();

                    foreach (var item in jsonArray)
                    {
                        if (item is JsonObject obj)
                        {
                            if (obj[key] is JsonArray innerArray)
                            {
                                columnArray.Add(innerArray.Count == 0 ? null : CompressJsonToColumnar(innerArray));
                            }
                            else
                            {
                                columnArray.Add(obj[key]?.DeepClone());
                            }
                        }
                        else
                        {
                            columnArray.Add(null);
                        }
                    }

                    resultArray.Add(new JsonArray { key, columnArray });
                }

                return resultArray;
            }
            else if (jsonData is JsonObject jsonObject)
            {
                var newObj = new JsonObject();

                foreach (var kvp in jsonObject)
                {
                    if (kvp.Value is JsonArray arr)
                    {
                        newObj[kvp.Key] = CompressJsonToColumnar(arr);
                    }
                    else if (kvp.Value is JsonObject obj)
                    {
                        newObj[kvp.Key] = CompressJsonToColumnar(obj);
                    }
                    else
                    {
                        newObj[kvp.Key] = kvp.Value?.DeepClone();
                    }
                }

                return newObj;
            }

            return jsonData;
        }





























        public static JsonNode? DecompressColumnarToJson(JsonNode? objectData)
        {
            if (objectData is null)
                return null;


            if (objectData is JsonArray arrayData)
            {
                // Check if first element is also an array: it should be in [key, values] format
                if (arrayData.Count == 0 || arrayData[0] is not JsonArray)
                    return objectData;

                // Extract keys and value arrays
                var keys = arrayData.Select(pair => pair![0]!.GetValue<string>()).ToList();
                var values = arrayData.Select(pair => pair![1] as JsonArray).ToList();

                int rowCount = values.FirstOrDefault()?.Count ?? 0;
                var resultArray = new JsonArray();

                for (int i = 0; i < rowCount; i++)
                {
                    var obj = new JsonObject();
                    for (int j = 0; j < keys.Count; j++)
                    {
                        var cell = values[j]?[i];

                        if (cell is JsonArray innerArray)
                        {
                            obj[keys[j]] = DecompressColumnarToJson(innerArray);
                        }
                        else if (cell is JsonObject innerObj)
                        {
                            obj[keys[j]] = DecompressColumnarToJson(innerObj);
                        }
                        else
                        {
                            obj[keys[j]] = cell?.DeepClone();
                        }
                    }

                    resultArray.Add(obj);
                }

                return resultArray;
            }
            else if (objectData is JsonObject jsonObject)
            {
                var result = new JsonObject();
                foreach (var kvp in jsonObject)
                {
                    if (kvp.Value is JsonArray arr)
                    {
                        result[kvp.Key] = DecompressColumnarToJson(arr);
                    }
                    else if (kvp.Value is JsonObject obj)
                    {
                        result[kvp.Key] = DecompressColumnarToJson(obj);
                    }
                    else
                    {
                        result[kvp.Key] = kvp.Value?.DeepClone();
                    }
                }

                return result;
            }

            return objectData;
        }








    }
}
