using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Basic
{
    public class SelectBasic:BindableBase
    {
        public string Label { get; set; }
        public string Value { get ; set; }
    }

    public class CheckBasic:BindableBase {

        private bool _isCheck;
        public string Label { get; set; }
        public bool IsCheck
        {
            get => _isCheck;
            set => SetProperty(ref _isCheck, value);
        }
    }

}
