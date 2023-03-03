using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace AlloyDemo.Models.Loaders
{
    public class TokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken  { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn  { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType  { get; set; }
    }
}