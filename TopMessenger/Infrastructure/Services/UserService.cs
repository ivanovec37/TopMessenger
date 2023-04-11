using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopMessenger.Data;
using TopMessenger.Models;
using TopMessenger.ModelShells;

namespace TopMessenger.Infrastructure.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        public UserService()
        {
            _context = new AppDbContext();
        }

        public async Task<int> AddNewUser(User user)
        {
            var list = new ObservableCollection<FriendList>();
            list.Add(new FriendList() {  Name = "FirstList" });
            user.FriendLists = list;
            var res = _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return res.Id;
        }
       /// <summary>
       /// Временный метод для заполнения фото пользователей. Не использовать!!!!
       /// </summary>
       /// <param name="user">Пользователь чей FriendList мы берем для заполнения фото у друзей</param>
       /// <returns>успешная ли операция</returns>
        public async Task<bool> AddPhotoSource(User user)
        {
            var fl = await GetFriendList(user);
            foreach (var item in fl)
            {
                item.Photo = "C:\\Users\\feday\\Pictures\\2.jpg";
            }
            //int res = await _context.SaveChangesAsync();
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include("FriendLists").Include("Messages").FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<bool> AddNewFriend(User mainUser, User friend)
        {
            var userTemp = _context.Users.Include("FriendLists").FirstOrDefault(u => u.Id == mainUser.Id);
            var friendTemp = _context.Users.Include("FriendLists").FirstOrDefault(u => u.Id == friend.Id);
            userTemp.FriendLists.FirstOrDefault().Friends.Add(friendTemp);
            var res = await _context.SaveChangesAsync();
            if (res > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<ObservableCollection<User>> GetFriendList(User user)
        {
            var userId = user.Id;
            var userFriendListId = user.FriendLists.FirstOrDefault().Id;
            ObservableCollection<User> res = null;

            await Task.Run(() =>
            {
               res = new ObservableCollection<User>(_context.Users.Include("FriendLists").Include("Messages").
                   Where(u => u.FriendLists.FirstOrDefault().Id == userFriendListId)
                   .Select(u => u)
                   .ToList());

            });

            return res;
        }
        public async Task<bool> SendMessage(Message message) //Sender = Id// Receiver = Id
        {
            (await _context.Users.FirstOrDefaultAsync(u => u.Id == message.Sender.Id)).Messages.Add(message);
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return true; 
        }
        public async Task<ObservableCollection<Message>> GetMessageWithUser(User mainUser,User friend )
        {
            var res = new ObservableCollection<Message>(await _context.Messages.Where(m => m.Receiver.Id == friend.Id && m.Sender.Id == mainUser.Id 
            || m.Receiver.Id == mainUser.Id && m.Sender.Id == friend.Id)
                .Select(s => s).ToListAsync());
            foreach (var item in res)
            {
                if (item.Sender == mainUser)
                {
                    item.Sender.IsMain = true;
                }
                else if (item.Receiver == mainUser)
                {
                    item.Receiver.IsMain = true;
                }
            }
            return res;

        }

        public async Task<ObservableCollection<UserAddFriendShell>> GetAllUsersForAddFriend(User mainUser)
        {
            var res = new List<UserAddFriendShell>();
            var allUsers = await _context.Users.Include("Messages").Include("FriendLists").ToListAsync();

            allUsers.ForEach(u => res.Add(new UserAddFriendShell
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Photo = u.Photo,
                IsFriend = mainUser.FriendLists.Any(fl => fl.Friends.Any(f => f.Id == u.Id))
            }));

            return new ObservableCollection<UserAddFriendShell>(res);

        }

    }
}
