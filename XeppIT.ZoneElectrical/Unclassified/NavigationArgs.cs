using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace XeppIT.ZoneElectrical.Unclassified
{
    public class NavigationArgs
    {
        private readonly NavigationManager _navigationManager;
        public object Context { get; private set; }
        public string ReturnUrl { get; private set; }

        public NavigationArgs(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateTo(string destinationUrl, string returnUrl = null, object context = null)
        {
            Context = context;
            ReturnUrl = returnUrl;
            _navigationManager.NavigateTo(destinationUrl);
        }
    }
}
