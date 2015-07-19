using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoolpyCould
{
    public class Errors
    {
        static Errors err;
        public static Errors GetError()
        {
            if (err == null)
            {
                err = new Errors();
            }
            return err;
        }

        #region 管理API
        //系统没有对应服务接口
        public void p417(HttpContext context)
        {
            context.Response.Status = "417 Expectation Failed";
            context.Response.StatusCode = 417;
        }
        //系统没有对应服务接口
        public void p404(HttpContext context)
        {
            context.Response.Status = "404 Not Found";
            context.Response.StatusCode = 404;
        }
        //请求的头文件必须项ApiKey验证没有通过
        public void p412(HttpContext context)
        {
            context.Response.Status = "412 Precondition Failed";
            context.Response.StatusCode = 412;
        }
        //用户请求过快，系统默认是10秒内不可重复操作
        public void p406(HttpContext context)
        {
            context.Response.Status = "406 Not Acceptable";
            context.Response.StatusCode = 406;
        }
      
        //请求中没有发现提交内容
        public void p204(HttpContext context)
        {
            context.Response.Status = "204 No Content";
            context.Response.StatusCode = 204;
        }
       
        //添加或修改时数据出错
        public static readonly string e7001 = "{Error:7001}";
        //请求路径中的设备ID出错
        public static readonly string e7002 = "{Error:7002}";
        //添加设备时系统出错
        public static readonly string e7003 = "{Error:7003}";
        //查看修改删除数据时没有发现设备ID对应在的记录
        public static readonly string e7004 = "{Error:7004}";
        //修改设备时系统出错
        public static readonly string e7005 = "{Error:7005}";
        //连接数据库失败（确保数据库已经运行，并检查web.config中的连接字符串是否有误
        public static readonly string e7006 = "{Error:7006}";
        //系统不支持的传感器类型
        public static readonly string e7007 = "{Error:7007}";
        //数据结点key已在存在不再被添加
        public static readonly string e7008 = "{Error:7008}";

        //系统未定义功能错误
        public static readonly string e7009 = "{Error:7009}";
        #endregion
    }

}