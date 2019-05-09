# pdfservice
Azure-based service converting text into printable form (PDF)

## Utilities

### HTMLCleanup.exe

#### Usage

HTMLCleanup.exe *URL*

Configuration file *Config.xml* must be located nearby the utility executable.

To regenerate XML parsing module __config.cs__ from __Config.xsd__ open Visual Studio
Developer Command Prompt and execute __xsd config.xsd /classes /namespace:HTMLCleanup.Config__.

Template configuration file is created by HTMLCleanup utility when *URL* is not provided.
