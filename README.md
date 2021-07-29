# WClipboard
[![Build Status](https://wibrenwiersma.visualstudio.com/WClipboard/_apis/build/status/Classic%20WClipboard-CI?branchName=master)](https://wibrenwiersma.visualstudio.com/WClipboard/_build/latest?definitionId=5&branchName=master)

WClipboard is an intelligent, free to use, opensource clipboard manager.
It stores your clipboard history and simplifies your workflow.

WClipboard requires [.NET 5.0 x64 .NET Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime). We only support a 64 bits Windows 10 machine version 1903 or later (Api version 10.0.18362.0).
## Installation
Go to the latest release. There you find in the assets two options to install WClipboard:
1. Download and run the Setup MSI installer.
2. Or download the WClipboard.zip and unpack it wherever you like. Running WClipboard.exe for the first time will setup some required things on your machine.

When an error appears that you don't have the correct .NET runtime installed, make sure you have [.NET 5.0 x64 .NET Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime) installed. (This download page is more helpfull than the one where the error box points you to.)

### After installation:
- We recommend to setup WClipboard to start when Windows starts in the WClipboard settings menu. WClipboard will only be able to monitor your clipboard when running.
- Take a look at the other settings also.

## License
WClipboard is licensed under GNU GPL v3 License

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

For more detail see [License.txt](LICENSE.txt)

We do not require this or any license for "plugins" (external dlls) created for WClipboard