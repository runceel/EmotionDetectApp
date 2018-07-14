using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevConsultHack.Client.ViewModels
{
	public class AppNavigationPageViewModel : ViewModelBase
	{
        public AppNavigationPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }
	}
}
