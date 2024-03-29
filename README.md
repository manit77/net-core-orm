# .NET Core Custom ORM Mapper

## Description

This application makes it easy to autogenerate your own custom code or text file from a database. Use it to autogenerate your custom code such as .cs, .ts, .js, .txt, .html, .xml files etc. It uses Razor pages as template files and currently supports SQL Server and MySQL database. Written in C# .NET Core 8.0.

### Examples included:

- SQL Server --> C# classes
- SQL Server --> TypeScript files
- MySQL --> TypeScript files

### Usage

- Download the source code and net-core-utils project
- Open net-core-orm.sln
- Compile the source as Release
- create your own custom Razor templates in the Views directory, samples provided
- create config.json file in the Release folder, see sample_config.json
- execute the net-core-orm.exe
- enter the configname from you config file
- enjoy your code being autogenerated :)