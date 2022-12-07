using System;

namespace PLC_Connect_Test.Pattern
{
    // type parameter는 레퍼런스 타입이고(class) 디폴트 생성자를 가져야 함(new())
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
        // public static의 객체반환 함수
        public static T Instance { get { return _instance.Value; } }
    }
}
