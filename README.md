
# Outdated: please use [Gujian 3 Manager](https://github.com/Kaplas80/GuJian3Manager) for more features and improvements


# Gujian3TextEditor
Gujian3TextEditor is a tool used for editing binary json (string only) in Gujian 3.




## Installation

- Donwload [lastest release](https://github.com/eprilx/Gujian3TextEditor/releases).
- Run Gujian3TextEditor.exe in the cmd

## Building from source
- **[Install .NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)**
- ``git clone --recurse-submodules https://github.com/eprilx/Gujian3TextEditor.git``
- ``dotnet build``

## Usage
❄ **[Install/Download Gujian 3 text mod by alanm](https://zenhax.com/viewtopic.php?f=12&t=14879&p=67510#p67511)**

Use this tool to extract/pack text.bin and text1302142.bin
```
Usage: Gujian3TextEditor [OPTIONS]
Options:
  -e, --extract              Extract String
  -p, --pack                 Pack String
```
❄ Extract String
```
Usage: Gujian3TextEditor --extract [OPTIONS]
Options:
  -a, --all                  (optional) Extract all strings
  -i, --input=VALUE          (required) Decrypted text buffer path
  -o, --output=VALUE         (optional) Output text file path
```
**Example:**
`Gujian3TextEditor -e -i gujian3_text.bin`

❄ Pack String
```
Usage: Gujian3TextEditor --pack [OPTIONS]
Options:
  -i, --input=VALUE          (required) Decrypted text buffer path
  -t, --text=VALUE           (required) Text file path
  -o, --output=VALUE         (optional) Output file path
```
**Example:**
`Gujian3TextEditor -p -i gujian3_text.bin -t gujian3_text.bin.txt`

## Notes
- Make sure that **Gujian 3 text mod** is installed to display these text in-game.
- Some string can't be edited, because it's not in-game text. So **edit carefully** !

## Special Thanks
- [alanm](https://zenhax.com/memberlist.php?mode=viewprofile&u=8736) (Gujian 3 Text Mod)
- [Kaplas](https://zenhax.com/memberlist.php?mode=viewprofile&u=5785) (find where the decrypted size is stored, [Gujian3Manager](https://github.com/Kaplas80/GuJian3Manager)
- [Rick Gibbed](https://github.com/gibbed) ([Gibbed.IO](https://github.com/gibbed/Gibbed.IO) library)

## License
[MIT](LICENSE)
