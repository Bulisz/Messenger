using Messenger.Backend.Abstactions;
using Messenger.Backend.DataBase;
using Messenger.Backend.Models;
using Messenger.Backend.Models.MessageDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Backend.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task JoinToGroupAsync(string userName, string groupName)
    {
        Group? group = await _context.Groups.Include(g => g.Users)
                                            .FirstOrDefaultAsync(g => g.Name == groupName);
        ApplicationUser? user = await _userManager.FindByNameAsync(userName);

        if (group is null)
        {
            group = new Group() { Name = groupName };
            _context.Groups.Add(group);
            if(user is not null)
            {
                group.Users = new List<ApplicationUser>() { user };
            }
            await _context.SaveChangesAsync();
        }
        else
        {
            if (group.Users is null)
            {
                if (user is not null)
                {
                    group.Users = new List<ApplicationUser>() { user };
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                if (user is not null)
                {
                    group.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    public async Task SaveMessageAsync(SaveMessageDTO saveMessageDTO)
    {
        Group? group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == saveMessageDTO.GroupName);
        ApplicationUser? user = await _userManager.FindByNameAsync(saveMessageDTO.UserName);
        if (user is not null && group is not null)
        {
            Message message = new()
            {
                User = user,
                Group = group,
                Text = saveMessageDTO.Text,
                CreatedAt = DateTime.Now,
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
