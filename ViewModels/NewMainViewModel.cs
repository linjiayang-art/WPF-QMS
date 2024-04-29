using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SicoreQMS.Common.Interface;
using SicoreQMS.Common.Models.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels
{
    public class NewMainViewModel: NavigationViewModel
    {
        private bool isTopDrawerOpen;
        private readonly IRegionManager manger;

        public bool IsTopDrawerOpen
        {
            get { return isTopDrawerOpen; }
            set { isTopDrawerOpen = value; RaisePropertyChanged(); }
        }



        public DelegateCommand<NavigationItem> NavigateCommand { get; set;}


        public NewMainViewModel(IRegionManager manger,INavigationMenuService navigationService)
        {
            this.manger = manger;
            NavigationService = navigationService;
            NavigateCommand = new DelegateCommand<NavigationItem>(Navigat);
        }

        private void Navigat(NavigationItem item)
        {
           if(item==null)  return;
           if(item.Name.Equals("首页"))
            {
                IsTopDrawerOpen = true;
                return;
            }
           IsTopDrawerOpen= false;
        }

        public INavigationMenuService NavigationService { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigationService.InitMenus();
            NavigatePage("IndexView");
            base.OnNavigatedTo(navigationContext);
            
        }
       
        private void NavigatePage(string pageName)
        {
            this.manger.Regions["MainViewRegion"].RequestNavigate(pageName, back =>
            {
                if (!(bool)back.Result)
                {
                    System.Diagnostics.Debug.WriteLine(back.Error.Message);
                }
            });
        }


      
    }
}
