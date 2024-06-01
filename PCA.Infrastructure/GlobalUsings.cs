global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net.Http;
global using System.Net.Http.Headers;
global using System.Net.WebSockets;
global using System.Reflection;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

global using PCA.Core.Entities;
global using PCA.Core.Extensions;
global using PCA.Core.Interfaces;
global using PCA.Core.Interfaces.Entities;
global using PCA.Core.Interfaces.Repositories;
global using PCA.Core.Interfaces.Services;
global using PCA.Core.Models;
global using PCA.Core.Utils;
global using PCA.Infrastructure.Context;
global using PCA.Infrastructure.Services.HttpClients;
global using PCA.Infrastructure.Services.WebSocket;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using AutoMapper;