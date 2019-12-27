# Simple Cloudflare DDNS [![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/Saxarok/CloudFlareDDNS/blob/master/LICENSE)

A script designed to dynamically update Cloudflare DNS records. 

Our goals:
* Simple. This tool is intended to be easily configurable, so anyone could use it. It uses a straight forward config format and auto-generates an example configuration file on start.
* Reliable. Very resistant to heavy system load and always works like intended. We made sure that everything will work correctly in every case possible.
* Open-source. Our view is that software with open source code is awesome! This tool is licensed under the terms of the MIT license, so other developers could fork it or contribute to this repository.

The main use-case is in a situation where a host machine has a dynamic IP, so this tool could come in handy to make the IP address "pseudo-static".

## Getting started
1. Download the latest build from https://github.com/KrykiZZ/CloudFlareDDNS/releases/latest
2. Run it with ```dotnet CloudFlareDDNS.dll``` to generate config files
3. Get your global api key from https://dash.cloudflare.com/profile/api-tokens
4. Get your zone id from the domain page
5. Rename and modify example config file (located at domains/example.com.json)
6. Run the tool again

## Dependencies
You'll need [.NET Core Runtime](https://dotnet.microsoft.com/download) and the following libraries:
 - [Newtonsoft.Json 12.0.2](https://www.nuget.org/packages/Newtonsoft.Json/12.0.2)
 
## Features
  - Lightweight
  - Simple configuration
  - Smart update (your record will only be updated if needed)
  - Multiple domain/account support  
  - Full Logging
  
## License
The CloudFlareDDNS is licensed under the terms of the MIT license and is available for free.
