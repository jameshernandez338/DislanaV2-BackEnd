// Common global usings for the API project to reduce repetitive using directives
global using System;
global using System.IO;
global using System.Text;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi;
global using Serilog;

global using Dislana.Api.Middlewares;
// Application implementation namespaces (concrete services)
global using Dislana.Application.Auth;
global using Dislana.Application.Order;
global using Dislana.Application.Payment;
global using Dislana.Application.Product;
global using Dislana.Application.Quote;
global using Dislana.Application.Stock;
global using Dislana.Application.Transaction;
global using Dislana.Application.Secrets;
// Application service and interface namespaces used by Program.cs
global using Dislana.Application.Auth.Interfaces;
global using Dislana.Application.Order.Interfaces;
global using Dislana.Application.Payment.Interfaces;
global using Dislana.Application.Product.Interfaces;
global using Dislana.Application.Quote.Interfaces;
global using Dislana.Application.Stock.Interfaces;
global using Dislana.Application.Transaction.Interfaces;

// Domain interfaces
global using Dislana.Domain.Auth.Interfaces;
global using Dislana.Domain.Order.Interfaces;
global using Dislana.Domain.Payment.Interfaces;
global using Dislana.Domain.Product.Interfaces;
global using Dislana.Domain.Quote.Interfaces;
global using Dislana.Domain.Stock.Interfaces;
global using Dislana.Domain.Transaction.Interfaces;

// Infrastructure types used in DI registration
global using Dislana.Infrastructure.Configuration;
global using Dislana.Infrastructure.Auth;
global using Dislana.Infrastructure.Persistence.Dapper;
global using Dislana.Infrastructure.Persistence.Repositories.Auth;
global using Dislana.Infrastructure.Persistence.Repositories.Order;
global using Dislana.Infrastructure.Persistence.Repositories.Payment;
global using Dislana.Infrastructure.Persistence.Repositories.Product;
global using Dislana.Infrastructure.Persistence.Repositories.Quote;
global using Dislana.Infrastructure.Persistence.Repositories.Stock;
global using Dislana.Infrastructure.Persistence.Repositories.Transaction;
