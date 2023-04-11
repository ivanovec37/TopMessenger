using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopMessenger.Infrastructure.Services;
using TopMessenger.Models;
using TopMessenger.ViewModels.Commands;

namespace TopMessenger.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        public ActionCommand WindowMinimizeCommand => new ActionCommand(x => WindowMinimize());
        public ActionCommand WindowMaximizeCommand => new ActionCommand(x => WindowMaximize());
        public ActionCommand AppCloseCommand => new ActionCommand(x => Application.Current.Shutdown());
        public ActionCommand SendMessageCommand => new ActionCommand(x => MessageBox.Show(NewText));
        #endregion

        #region Property
        private string newText;
        public string NewText
        {
            get => newText;
            set
            {
                newText = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<User> friends;

        public ObservableCollection<User> Friends
        {
            get { return friends; }
            set 
            { 
                friends = value;
                OnPropertyChanged();
            }
        }
        private event Action Chat;
        private User selectedFriend;

        public User SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                Chat(); // uncomment 
                OnPropertyChanged();
            }
        }
        private User mainUserTemp;
        private ObservableCollection<Message> chatWithUser;
        public  ObservableCollection<Message> ChatWithUser
        {
            get => chatWithUser;
            set
            {
                chatWithUser = value;
                OnPropertyChanged();
            }
        }
        #endregion
        private UserService userService;
        public MainViewModel()
        {
            Chat += ChatWU;
            LoadMeth().GetAwaiter();
        }
        private async void ChatWU()//Убрали параметр
        {
            ChatWithUser = await userService.GetMessageWithUser(mainUserTemp, SelectedFriend); //Передаем SelectedFriend
        }
        private async Task LoadMeth()
        {
            userService = new UserService();
            mainUserTemp = await userService.GetUser(29);
            #region
            //ChatWithUser = await userService.GetMessageWithUser(await userService.GetUser(29), await userService.GetUser(30));
            //var user1 = new User
            //{
            //    FirstName = "first"
            //};
            //var user2 = new User
            //{
            //    FirstName = "Second"
            //};

            //var user3 = new User
            //{
            //    FirstName = "third"
            //};


            //await userService.AddPhotoSource(await userService.GetUser(29));
            #endregion
            ObservableCollection<User> tempUsers = await userService.GetFriendList(await userService.GetUser(29));
            foreach (var item in tempUsers)
            {
                var tempMessages = await userService.GetMessageWithUser(mainUserTemp , item);
                item.LastMessage = tempMessages.LastOrDefault()?.Text;
            }
            Friends = tempUsers;
            #region
            //Message mes = new Message
            //{
            //    Text = "AsdasdasdTexstadsdqw12131231231rewfvasad",
            //    DateOfSend = DateTime.Now,
            //    Sender = await userService.GetUser(30),
            //    Receiver = await userService.GetUser(29)
            //};
            //await userService.SendMessage(mes);
            //Message mes2 = new Message
            //{
            //    Text = "AsdasdasdTexstasdasd",
            //    DateOfSend = DateTime.Now,
            //    Sender = await userService.GetUser(30),
            //    Receiver = await userService.GetUser(29)
            //};
            //await userService.SendMessage(mes2);
            //Message mes3 = new Message
            //{
            //    Text = "AsdasdasdTexst3",
            //    DateOfSend = DateTime.Now,
            //    Sender = await userService.GetUser(29),
            //    Receiver = await userService.GetUser(30)
            //};
            //await userService.SendMessage(mes3);

            //var res = await userService.GetMessageWithUser(await userService.GetUser(29), await userService.GetUser(30));
            //string text = "";
            //foreach (var item in res)
            //{
            //    text += item.Text + "\n";
            //}
            //MessageBox.Show(text);
            //await userService.AddNewUser(user3);
            ////await userService.AddNewUser(user2);
            //await userService.AddNewFriend(await userService.GetUser(29), await userService.GetUser(31));
            //    await userService.GetUser(30));
            #endregion
        }

        private void WindowMaximize()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void WindowMinimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }



        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
