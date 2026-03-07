# Google Maps API Setup Guide

## Overview
The LocationService uses Google Maps Geocoding API for address geocoding and reverse geocoding. This guide will help you set up the API key.

## Features
- **Geocoding**: Convert addresses to latitude/longitude coordinates
- **Reverse Geocoding**: Convert coordinates to addresses
- **Fallback Mode**: Works with mock data when API key is not configured (development mode)
- **Automatic Parsing**: Extracts city, state, country, and pincode from responses

## Setup Instructions

### 1. Get a Google Maps API Key

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the **Geocoding API**:
   - Navigate to "APIs & Services" > "Library"
   - Search for "Geocoding API"
   - Click "Enable"
4. Create credentials:
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "API Key"
   - Copy the generated API key

### 2. Secure Your API Key (Important!)

For production, restrict your API key:
1. In the credentials page, click on your API key
2. Under "Application restrictions":
   - Select "HTTP referrers" for web apps
   - Or "IP addresses" for server apps
3. Under "API restrictions":
   - Select "Restrict key"
   - Choose "Geocoding API"

### 3. Configure the Application

Add your API key to `appsettings.json`:

```json
{
  "GoogleMapsSettings": {
    "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY_HERE",
    "BaseUrl": "https://maps.googleapis.com/maps/api"
  }
}
```

**For development**, you can also use `appsettings.Development.json`:

```json
{
  "GoogleMapsSettings": {
    "ApiKey": "YOUR_DEV_API_KEY"
  }
}
```

### 4. Environment Variables (Recommended for Production)

Instead of storing the API key in files, use environment variables:

**Windows:**
```bash
setx GOOGLE_MAPS_API_KEY "your-api-key-here"
```

**Linux/Mac:**
```bash
export GOOGLE_MAPS_API_KEY="your-api-key-here"
```

Then update `appsettings.json`:
```json
{
  "GoogleMapsSettings": {
    "ApiKey": "${GOOGLE_MAPS_API_KEY}",
    "BaseUrl": "https://maps.googleapis.com/maps/api"
  }
}
```

## Free Tier Information

Google Maps offers a **$200 monthly credit** which includes:
- **40,000 geocoding requests** per month (free)
- After that: $5 per 1,000 requests

For most applications, the free tier is sufficient.

## Development Mode (No API Key)

If no API key is configured, the service automatically uses **mock data**:
- Returns Bangalore, India coordinates by default
- Useful for development and testing
- No external API calls made

## Alternative: Free OpenStreetMap (Nominatim)

If you prefer a completely free solution without rate limits, you can use Nominatim:

1. Replace the API calls in `LocationService.cs` with Nominatim endpoints:
   - Geocoding: `https://nominatim.openstreetmap.org/search?q={address}&format=json`
   - Reverse: `https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json`

2. Add a User-Agent header to comply with Nominatim usage policy:
   ```csharp
   _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");
   ```

**Note**: Nominatim has usage limits (1 request/second) but no API key required.

## Testing the Service

```csharp
// Example usage
var result = await _locationService.GeocodeAddressAsync("1600 Amphitheatre Parkway, Mountain View, CA");
if (result.IsSuccess)
{
    Console.WriteLine($"Latitude: {result.Latitude}");
    Console.WriteLine($"Longitude: {result.Longitude}");
    Console.WriteLine($"City: {result.City}");
    Console.WriteLine($"State: {result.State}");
}

// Reverse geocoding
var reverseResult = await _locationService.ReverseGeocodeAsync(37.4224764, -122.0842499);
if (reverseResult.IsSuccess)
{
    Console.WriteLine($"Address: {reverseResult.FormattedAddress}");
}
```

## Troubleshooting

### "REQUEST_DENIED" Error
- Check if Geocoding API is enabled in Google Cloud Console
- Verify API key is correct
- Ensure API key restrictions allow your server/domain

### "OVER_QUERY_LIMIT" Error
- You've exceeded the free tier limit
- Consider caching results
- Implement rate limiting in your application

### Empty Results
- The service returns mock data if API key is not set
- Check `result.IsSuccess` and `result.ErrorMessage` for details

## Best Practices

1. **Cache Results**: Store geocoded addresses to avoid repeated API calls
2. **Rate Limiting**: Implement request throttling in high-traffic scenarios
3. **Error Handling**: Always check `IsSuccess` before using results
4. **Batch Processing**: For bulk operations, consider using batch geocoding endpoints
5. **Monitor Usage**: Check Google Cloud Console regularly for API usage

## Security Notes

⚠️ **Never commit your API key to version control!**
- Add `appsettings.json` to `.gitignore` (if not already)
- Use `appsettings.example.json` for templates
- Use environment variables or Azure Key Vault for production
