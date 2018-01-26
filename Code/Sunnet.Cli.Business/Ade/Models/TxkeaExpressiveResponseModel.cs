using Sunnet.Cli.Core.Ade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaExpressiveResponseModel
    {
        public TxkeaExpressiveResponseModel()
        {            
        }

        public TxkeaExpressiveResponseModel(bool isDeleted)
        {
            Text = string.Empty;
            Mandatory = true;
            Type = TypedResponseType.Radionbox;
            Buttons = 1;
            IsDeleted = isDeleted;

            Options = new List<TxkeaExpressiveResponseOptionModel>();
            for (var i = 0; i < Buttons; i++)
                Options.Add(new TxkeaExpressiveResponseOptionModel());
        }

        public int ID { get; set; }

        public int ItemId { get; set; }

        public string Text { get; set; }

        public bool Mandatory { get; set; }

        [Description("Response type")]
        public TypedResponseType Type { get; set; }

        /// <summary>
        /// number of check boxes or number of radio buttons 
        /// </summary>
        [Description("Number of Radio buttons")]
        public int Buttons { get; set; }

        public bool IsDeleted { get; set; }

        public List<TxkeaExpressiveResponseOptionModel> Options { get; set; }
    }
}
