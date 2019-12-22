/*
 * 作者：变形精怪 手机/微信17723896676 QQ/邮箱453008453
 * 创建时间：2018-10-13 1:07:12
 *
 * 说明：略...
 */
using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    //abp默认提供的CombboxItemDto只是一个辅助类，我们可以定义一个更符合我们要求的辅助类
    //其实用他的也可以，只是不是很方便



    /// <summary>
    /// Easyui的Combobox下拉框控件对应数据的模型，它是一个通用模型
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class GeneralTreeComboboxDto<TId>
    {
        public TId Value { get; set; }
        public string Text { get; set; }

        //虽然子类可以继承这个进行自定义数据扩展，但是那是编译时/设计时扩展，前端无法随时扩展自定义数据，因此这里ExtData功能是有必要的
        public dynamic ExtData { get; private set; }

        [JsonIgnore]
        public string ExtDataString
        {
            get
            {
                if (ExtData == null)
                    return null;
                return JsonConvert.SerializeObject(ExtData);
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    ExtData = JsonConvert.DeserializeObject<dynamic>(value);
                else
                    ExtData = null;
            }
        }

        public GeneralTreeComboboxDto()
        {

        }
        public GeneralTreeComboboxDto(TId id, string text, string extData = "")
        {
            Value = id;
            Text = text;
            ExtDataString = extData;
        }
    }
}
