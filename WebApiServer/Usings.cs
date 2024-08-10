// system
global using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
global using JsonSerializer = System.Text.Json.JsonSerializer;
global using System.Reflection;
global using System.Net;
global using System.Text.Json.Serialization;
global using System.Diagnostics;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Cryptography;
global using System.ComponentModel;
global using HttpResponseHeaders = System.Net.Http.Headers.HttpResponseHeaders;
global using System.Web;
global using System.Data.Common;
global using System.IO.Compression;
global using static System.Net.Mime.MediaTypeNames;
global using System.Text.Json;

// microsoft
global using ILogger = Microsoft.Extensions.Logging.ILogger;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Net.Http.Headers;
global using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.OpenApi.Models;

// thirdy
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Swashbuckle.AspNetCore.Annotations;
global using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
global using Npgsql;
global using SearchAThing.Ext;

// examplewebapp
global using static ExampleWebApp.Backend.WebApi.Constants;
global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Types;
global using ExampleWebApp.Backend.Data;
global using ExampleWebApp.Backend.Data.Types;
