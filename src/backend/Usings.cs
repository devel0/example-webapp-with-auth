// system
global using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
global using static System.Net.Mime.MediaTypeNames;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO.Compression;
global using System.Linq.Expressions;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Web;

// microsoft
global using ILogger = Microsoft.Extensions.Logging.ILogger;
global using Index = Microsoft.EntityFrameworkCore.IndexAttribute;
global using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;
global using Microsoft.OpenApi.Models;

// thirdy
global using SmtpClient = MailKit.Net.Smtp.SmtpClient;
global using CodeCasing;
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
global using static ExampleWebApp.Backend.WebApi.Constants;
global using static ExampleWebApp.Backend.WebApi.Toolkit;
global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Data;
global using ExampleWebApp.Backend.WebApi.Services;
global using ExampleWebApp.Backend.WebApi.Services.Abstractions;
global using ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth;
global using ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.Data;
global using ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;
global using ExampleWebApp.Backend.WebApi.Services.Auth;
global using ExampleWebApp.Backend.WebApi.Types;