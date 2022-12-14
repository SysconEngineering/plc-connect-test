using PLC_Connect_Test.Interface;
using System.Net;
using System.Threading;

namespace PLC_Connect_Test.OuterPLC
{
    public class OuterBase : IOuterPlc
    {
        protected string HOST = "";
        protected int PORT = 0;

        public int plc_type = 0;
        protected enum PLCProtocol { PT_A = 0, PT_QnA = 1 }; // A호환
        protected Thread ServerConnect_Checkthread;
        protected Thread DataRequestThread; // 데이터 요구 쓰레드
        protected Thread DataRecvThread;  //데이터 수신
        protected Thread DataWriteThread; // 데이터 쓰기
        protected bool bConnectCheck = false;
        protected System.Net.Sockets.Socket clientsock_data = null;
        protected System.Net.Sockets.Socket dataSock = null;
        protected IPEndPoint ipep;

        public bool bConnected = false;
        public int socket_state = 0;
        public bool bSockclose = false;
        protected string strSocketid = "";

        public OuterBase(string host, int port)
        {
            this.HOST = host;
            this.PORT = port;
        }

        virtual public void AddPlc(string name, int plc_type)
        {
        }

        virtual public void Init()
        {

        }

        virtual public void Processiong()
        {
            //throw new NotImplementedException();
        }

        virtual public void Start()
        {
            //throw new NotImplementedException();
        }

        virtual public void Stop()
        {
            //throw new NotImplementedException();
        }

        virtual public void Write(string plc_name, string address, string data)
        {

        }

        public int GetPlcType()
        {
            return this.plc_type;
        }
    }
}
