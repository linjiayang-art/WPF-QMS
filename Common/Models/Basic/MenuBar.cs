namespace SicoreQMS.Common.Models.Basic
{
    public class MenuBar
    {
        private string _icon;

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _nameSpace;

        /// <summary>
        /// 菜单命名空间
        /// </summary>
        public string NameSpace
        {
            get { return _nameSpace; }
            set { _nameSpace = value; }
        }

        public MenuBar()
        {

        }
    }
}
