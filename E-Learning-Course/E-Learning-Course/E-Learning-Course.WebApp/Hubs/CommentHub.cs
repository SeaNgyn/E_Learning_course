using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.SignalR;

public class CommentHub : Hub
{
    public async Task SendComment(List<CommentList>  comments)
    {
        await Clients.All.SendAsync ("ReceiveComment", comments);
    }
}