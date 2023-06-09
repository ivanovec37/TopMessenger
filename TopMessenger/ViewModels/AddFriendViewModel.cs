﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TopMessenger.Infrastructure.Services;
using TopMessenger.ModelShells;
using TopMessenger.ViewModels.Commands;

namespace TopMessenger.ViewModels
{
    public class AddFriendViewModel : BaseViewModel
    {
		#region Properties
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; UpdateValue<string>(ref name, value); }
		}

		private string lastName;

		public string LastName
		{
			get { return lastName; }
			set { lastName = value; UpdateValue<string>(ref lastName, value); }
		}

		private ObservableCollection<UserAddFriendShell> allUsers;

		public ObservableCollection<UserAddFriendShell> AllUsers
		{
			get { return allUsers; }
			set { UpdateValue(ref allUsers, value); }
		}

		private readonly UserService _userService;

		#endregion

		#region Commands
		public ActionCommand CloseWindowCommand => new ActionCommand(x => Application.Current.Windows[Application.Current.Windows.Count -1].Close());
		public ActionCommand AddFriendCommand => new ActionCommand(x => MessageBox.Show("Check"));
        #endregion


        public AddFriendViewModel()
        {
			_userService = new UserService();
			LoadInfo().GetAwaiter();
        }


		private async Task LoadInfo()
		{
			AllUsers = await _userService.GetAllUsersForAddFriend(await _userService.GetUser(30));

		}
    }
}
