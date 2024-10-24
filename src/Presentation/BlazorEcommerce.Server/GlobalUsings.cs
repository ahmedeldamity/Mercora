﻿global using Asp.Versioning;
global using BlazorEcommerce.Application.Dtos;
global using BlazorEcommerce.Application.Interfaces.Repositories;
global using BlazorEcommerce.Application.Interfaces.Services;
global using BlazorEcommerce.Application.MappingProfıles;
global using BlazorEcommerce.Application.Models;
global using BlazorEcommerce.Application.Specifications.ProductSpecifications;
global using BlazorEcommerce.Domain.Entities.IdentityEntities;
global using BlazorEcommerce.Domain.Entities.OrderEntities;
global using BlazorEcommerce.Domain.Entities.ProductEntities;
global using BlazorEcommerce.Domain.ErrorHandling;
global using BlazorEcommerce.Infrastructure.Services;
global using BlazorEcommerce.Infrastructure.Utility;
global using BlazorEcommerce.Persistence;
global using BlazorEcommerce.Persistence.Store;
global using BlazorEcommerce.Server;
global using BlazorEcommerce.Server.Errors;
global using BlazorEcommerce.Server.Extensions;
global using BlazorEcommerce.Server.Health;
global using BlazorEcommerce.Server.Helpers;
global using BlazorEcommerce.Server.Middlewares;
global using BlazorEcommerce.Server.ServicesExtension;
global using BlazorEcommerce.Shared.Account;
global using BlazorEcommerce.Shared.Cart;
global using BlazorEcommerce.Shared.Checkout;
global using BlazorEcommerce.Shared.Response;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Hangfire;
global using HangfireBasicAuthenticationFilter;
global using HealthChecks.UI.Client;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Serilog;
global using System.Net;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Threading.RateLimiting;
global using BlazorEcommerce.Shared.Models;