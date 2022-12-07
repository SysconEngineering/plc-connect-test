using PLC_Connect_Test.Model.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PLC_Connect_Test.Framework.Database.Controller
{
    public partial class PlcController
    {
        public List<TbPlcInfo> GetPlcInfos()
        {
            var result = new List<TbPlcInfo>();

            try
            {
                using var context = new PlcDbContext();
                var query = from plc in context.PlcInfos
                            select plc;
                result = query.ToList();
            }
            catch (Exception ex)
            {
                //Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                //Get the top stack frame
                var frame = st.GetFrame(0);
                //Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
            }
            return result;
        }

        public List<TbPlcInfoDtl> GetPlcInfoDtl(int idx)
        {
            var result = new List<TbPlcInfoDtl>();

            try
            {
                using var context = new PlcDbContext();
                var query = from dtls in context.PlcInfoDtls
                            where dtls.PlcInfoIdx == idx
                            select dtls;

                result = query.ToList();

            }
            catch (Exception ex)
            {
                //Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                //Get the top stack frame
                var frame = st.GetFrame(0);
                //Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
            }

            return result;
        }
    }
}
