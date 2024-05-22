using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class TestCount
    {

        public TestCount()
        {

        }
        public string Id { get; set; }

        public string ProdNo { get; set; }
        public string TestNo { get; set; }

        public string TestLot { get; set; }
        public string One { get; set; }
        public string Two { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }

        public string StatusDesc { get; set; }

        public string StatusDescColer { get; set; }
        public string Remark { get; set; }
        static void SetProperty(object obj, string propertyName, object value)
        {
            // 获取obj对象的类型
            Type type = obj.GetType();
            // 根据属性名获取PropertyInfo对象
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            // 判断属性是否存在
            if (propertyInfo != null)
            {
                // 设置属性值
                propertyInfo.SetValue(obj, value, null);
            }

        }

    }

    public interface ITestCount
    {
        string TestItem { get; set; }
        string TestYield { get; set; }
    }
}
