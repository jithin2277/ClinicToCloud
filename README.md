# ClinicToCloud

## Installation

Clone the repository in your local machine

Go to the repository location and run the following commands

```bash
cd PatientsAPI
dotnet build -c Release  .\PatientsApp.Api.csproj

cd .\bin\Release\netcoreapp2.1
dotnet PatientsApp.Api.dll
```

## Usage

Access the API on http://localhost:5000/api/v1/patients

## Technology Stack

* ASP.NET Core 2.1
* EF Core 2.1.14
* AutoMapper

## Development Steps

* Started working on the Data project and the repository file first
* Then worked on the API project
* Used AutoMapper to Map Entity object to the Model object 
* I wish I had time for TDD
* Wrote Unit tests after I was done working on both projects
* Wish had time for Docker testing also
