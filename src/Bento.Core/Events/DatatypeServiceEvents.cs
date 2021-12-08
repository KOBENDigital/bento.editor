using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using Umbraco.Core.Services.Implement;

namespace Bento.Core.Events
{
    class DatatypeServiceEvents : IComponent
    {
        public void Initialize()
        {
            DataTypeService.Saving += DataTypeService_Saving;
        }

        private void DataTypeService_Saving(Umbraco.Core.Services.IDataTypeService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IDataType> e)
        {
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
