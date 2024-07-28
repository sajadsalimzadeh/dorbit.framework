using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Services;
using Dorbit.Framework.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Framework.Hubs;

public abstract class NotificationHub : Hub
{
    public const string GroupUserOnline = "User-Online";
    public const string OnOnlineUserUpdated = nameof(OnOnlineUserUpdated);
    
    protected HubService HubService;

    protected NotificationHub(HubService hubService)
    {
        HubService = hubService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            var userResolver = httpContext.RequestServices.GetService<IUserResolver>();
            var userId = (Guid)userResolver.User.Id;
            HubService.Add(userId, Context.ConnectionId);
        }

        await updateOnlineUsers();
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        HubService.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task updateOnlineUsers()
    {
        await Clients.Group(GroupUserOnline).SendCoreAsync(OnOnlineUserUpdated, [HubService.GetAllUserId()]);
    }
}