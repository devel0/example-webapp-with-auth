// system
global using static System.Net.Mime.MediaTypeNames;
global using HttpResponseHeaders = System.Net.Http.Headers.HttpResponseHeaders;
global using JsonSerializer = System.Text.Json.JsonSerializer;
global using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Data.Common;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO.Compression;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Web;

// microsoft
global using ILogger = Microsoft.Extensions.Logging.ILogger;
global using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;
global using Microsoft.OpenApi.Models;

// thirdy
global using SmtpClient = MailKit.Net.Smtp.SmtpClient;
global using MimeKit;
global using Npgsql;
global using SearchAThing.Ext;
global using Serilog;
global using Serilog.Templates;
global using Serilog.Templates.Themes;
global using Swashbuckle.AspNetCore.Annotations;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

// project
global using static ExampleWebApp.Backend.Abstractions.Constants;
global using static ExampleWebApp.Backend.WebApi.Constants;
global using static ExampleWebApp.Backend.WebApi.Types.Toolkit;
global using ExampleWebApp.Backend.Abstractions;
global using ExampleWebApp.Backend.Abstractions.Types;
global using ExampleWebApp.Backend.Data;
global using ExampleWebApp.Backend.Data.Types;
global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Types;