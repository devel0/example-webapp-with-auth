// system
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Web;

// microsoft
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;

// other
global using Npgsql;
global using SearchAThing.Ext;
global using Xunit;
global using Xunit.Abstractions;

// project
global using static ExampleWebApp.Backend.Test.Constants;
global using static ExampleWebApp.Backend.Test.Toolkit;
global using static ExampleWebApp.Backend.WebApi.Constants;
global using static ExampleWebApp.Backend.WebApi.Toolkit;
global using static ExampleWebApp.Backend.WebApi.Types.AppConfig.DatabaseConfig.SeedConfig;
global using ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;
global using ExampleWebApp.Backend.WebApi.Services.Auth;
global using ExampleWebApp.Backend.WebApi.Services.Data;
global using ExampleWebApp.Backend.WebApi.Services;
global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Types;