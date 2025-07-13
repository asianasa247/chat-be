global using ChatappLC.Domain.Entities;
global using MongoDB.Bson;
global using MongoDB.Driver;
global using ChatappLC.Infrastructure.MongoDb;
global using Microsoft.AspNetCore.Http;

global using ChatappLC.Application.Interfaces.Chat;
global using ChatappLC.Application.Interfaces.User;
global using ChatappLC.Infrastructure.Repositories.Room;
global using ChatappLC.Application.DTOs;
global using ChatappLC.Application.DTOs.Chat;
global using ChatappLC.Application.Common;
global using ChatappLC.Infrastructure.Repositories.Message;

global using CrossCuttingConcerns.TimeHelper;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.AspNetCore.Authorization;

global using ChatappLC.Infrastructure.Services.Chat;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using ChatappLC.Infrastructure.ServicesPlugin;

global using ChatappLC.Application.DTOs.Auth;
global using ChatappLC.Application.DTOs.User;
global using ChatappLC.Application.Interfaces.Auth;
global using ChatappLC.CrossCuttingConcerns.MethodCommon;
global using ChatappLC.Infrastructure.ServicesPlugin;






