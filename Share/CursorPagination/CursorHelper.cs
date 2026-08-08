using System;
using System.Text;
using System.Text.Json;

namespace vsa_w_controller_csharp.Share.CursorPagination;

public static class CursorHelper
{
    public static string EncodeCursor<T>(T rawCursor) // from json to cursor
    {
        if(rawCursor == null) return null;
        
        var jsonCursor = JsonSerializer.Serialize(rawCursor);

        var cursorByte = System.Text.Encoding.UTF8.GetBytes(jsonCursor);
        var cursorEncoded = Convert.ToBase64String(cursorByte);

        return cursorEncoded;
    }

    public static T Decode<T>(string cursor) where T : class //from cursor to json
    {
        if(string.IsNullOrEmpty(cursor)) return null;
        
        try
        {
            var toByte = Convert.FromBase64String(cursor);
            var toString = Encoding.UTF8.GetString(toByte);

            var toJson = JsonSerializer.Deserialize<T>(toString);

            return toJson;
        }
        catch
        {
            return null;
        }
    }
}
