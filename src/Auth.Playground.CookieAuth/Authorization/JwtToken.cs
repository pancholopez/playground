﻿using System.Text.Json.Serialization;

namespace Auth.Playground.CookieAuth.Authorization;

public class JwtToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }
}