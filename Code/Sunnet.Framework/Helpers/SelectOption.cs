using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Helpers
{
    /// <summary>
    /// 下拉框 Default 值
    /// </summary>
    public sealed class SelectOptions
    {
        public SelectOptions(bool insertDefault, Object val, string text)
        {
            _insertDefault = insertDefault;
            _valu = val;
            _text = text;
        }

        private bool _insertDefault;
        /// <summary>
        /// 是否添加Default 值
        /// </summary>
        public bool InsertDefault { get {return _insertDefault;}}

        private Object _valu;
        /// <summary>
        /// 下拉框选项的值
        /// </summary>
        public Object Valu { get { return _valu; } }

        private string _text;
        /// <summary>
        /// 下拉框选项目的数据
        /// </summary>
        public string Text { get { return _text; } }
    }
}
