global using System.Net;
global using System.Reflection;
global using System.Text;

global using AutoMapper;

global using System.Text.Json;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using PCA.Configurations.AutoMapper;
global using PCA.Core.Entities;
global using PCA.Core.Extensions;
global using PCA.Core.Interfaces;
global using PCA.Core.Interfaces.Repositories;
global using PCA.Core.Interfaces.Services;
global using PCA.Core.Models;
global using PCA.Infrastructure.Context;
global using PCA.Infrastructure.Repositories;
global using PCA.Infrastructure.Services;
global using PCA.Infrastructure.Services.HttpClients;
global using PCA.Infrastructure.Services.WebSocket;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using JsonSerializerDefaults = System.Text.Json.JsonSerializerDefaults;
global using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

