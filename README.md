# Simple Cloudflare DDNS

A script for dynamically updating Cloudflare DNS records. 
If you have a dynamic IP and you're hosting servers behind it, it's an easy way to make it "pseudo-static".

## Getting started
1. Download from https://github.com/KrykiZZ/CloudFlareDDNS/releases
2. Retrieve your global api key from https://dash.cloudflare.com/profile/api-tokens.
3. Retrieve zone id from your domain page.
4. Modify config.json.
5. Run with ```dotnet CloudFlareDDNS.dll```

## Dependencies
You'll need a [.NET Core Runtime](https://dotnet.microsoft.com/download) and the following libraries:
 - [Newtonsoft.Json 12.0.2](https://www.nuget.org/packages/Newtonsoft.Json/12.0.2)
 
## Features
  - Logging
  - Lightweight
  - Simple configuration
  - Smart update (your record will only be updated if needed)