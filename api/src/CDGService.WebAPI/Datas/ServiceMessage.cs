using CDGService.Data.Enums;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Datas
{

    public class ServiceMessage<T>
    {
        public bool Success { get; set; }

        public string Error { get; set; }
        public CodeType code { get; set; }
        public T Result { get; set; }

        public int DataCount { get; set; }
        public ServiceMessage(T t)
        {
            this.Success = true;
            this.code = CodeType.Success;
            this.Result = t;
        }
        public ServiceMessage(T t, int dataCount)
        {
            this.Success = true;
            if (dataCount == 0)
                this.code = CodeType.NullData;
            else
                this.code = CodeType.Success;
            this.DataCount = dataCount;
            this.Result = t;

        }

        public ServiceMessage(Exception ey)
        {
            this.Success = false;
            this.code = CodeType.SystemException;
            this.Error = ey?.Message;
        }
        //public ServiceMessage(Exception ey,)
        //{
        //    this.Success = false;
        //    this.code = CodeType.SystemException;
        //    this.Error = ey?.Message;
        //}
        public ServiceMessage()
        {
            this.Result = default(T);
        }



    }
    public class MsgModel
    {
        public string  msg { get; set; }
    }

    //public class PageCount
    //{
    //    public int Count { get; set; }
    //}


    public class PageData<T>
    {
        public T Result { get; set; }
        public int Count { get; set; }
        public PageData(T t, int count)
        {
            this.Count = count;
            this.Result = t;
        }
    }
}
