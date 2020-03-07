using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientsApp.Api.Models;
using PatientsApp.Data;
using PatientsApp.Data.Entities;

namespace PatientsApp.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<PatientsDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "PatientsDb"));

            services.AddScoped<IPatientRepository, PatientRepository>();

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                AddTestData(serviceScope.ServiceProvider.GetService<IPatientRepository>());
            }

            app.UseMvc();
        }

        private void AddTestData(IPatientRepository patientRepository)
        {
            var patients = new List<PatientEntity>();
            for (int i = 0; i < 100; i++)
            {
                var patientEntity = new PatientEntity()
                {
                    CreatedAt = DateTime.UtcNow.AddDays(i*-1),
                    DOB = $"1/1/2000",
                    Email = $"patient{i}@clinictocloud.com",
                    FirstName = $"First Name {i}",
                    LastName = $"Last Name {i}",
                    Gender = "Male",
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Phone = $"99999999{i}",
                    UpdatedAt = DateTime.UtcNow.AddDays(i*-1)
                };
                patients.Add(patientEntity);
            }

            patientRepository.AddPatients(patients);
        }
    }
}
